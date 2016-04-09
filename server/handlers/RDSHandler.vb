Imports System.DirectoryServices
Imports System.Web.Helpers
Imports RADAR

Public Class RDSHandler

    ' User -> Token that proves user has authenticated, but not yet proved
    ' herself with the 2. factor
    Private Shared authTokens As New Hashtable

    Private Shared userSessions As New Hashtable
    Private Shared sessionTimestamps As New Hashtable
    Private Shared encryptedChallengeResults As New Hashtable
    Private Shared userLaunchTimestamps As New Hashtable

    Private mPacket As RADIUSPacket
    Private mUsername As String
    Private mPassword As String

    ' RDS specific values 
    Private mIsAppLaunchRequest As Boolean
    Private mIsGatewayRequest As Boolean
    Private mUseSMSFactor As Boolean
    Private mUseEmailFactor As Boolean

    Public Sub New(packet As RADIUSPacket)
        mPacket = packet

        mUsername = mPacket.UserName
        mPassword = mPacket.UserPassword

        CleanUsername()

        For Each atts As RADIUSAttribute In mPacket.Attributes.GetAllAttributes(RadiusAttributeType.VendorSpecific)
            Dim value As String = atts.GetVendorSpecific.VendorValue.ToString

            Select Case UCase(value)
                Case "LAUNCH"
                    mIsAppLaunchRequest = True
                Case "TSGATEWAY"
                    mIsGatewayRequest = True
                Case "SMS"
                    mUseSMSFactor = True
                Case "EMAIL"
                    mUseEmailFactor = True
            End Select
        Next
    End Sub

    Private Sub CleanUsername()
        ' RD Gateway sends EXAMPLE\username
        ' RD Web sends example\username or - TODO - even example.com\username
        If Not mUsername = Nothing Then
            mUsername = mUsername.ToLower
        End If
    End Sub

    Public Sub ProcessRequest()
        If mIsAppLaunchRequest Then
            ProcessAppLaunchRequest()
        ElseIf mIsGatewayRequest Then
            ProcessGatewayRequest()
        Else
            ProcessAccessRequest()
        End If
    End Sub

    ' Process the RDS specific App Launch request.
    ' These requests are sent when an app is clicked in RD Web.
    '
    ' It's checked whether the session is still valid. In which case, a
    ' window is opened for the user, where we allow the user to connect
    ' through the gateway, an Accept-Access is returned and the RD Web
    ' launches the RDP client.
    '
    ' NOTE: Requests contain the session GUID in the password attribute
    ' of the packet.
    Public Sub ProcessAppLaunchRequest()
        RDSFactor.LogDebug(mPacket, "AppLaunchRequest")

        ' When the packet is an AppLaunchRequest the password attribute contains the session id!
        Dim packetSessionId = mPassword
        Dim storedSessionId = userSessions(mUsername)

        If storedSessionId = Nothing Then
            RDSFactor.LogDebug(mPacket, "User has no session. MUST re-authenticate!")
            mPacket.RejectAccessRequest()
            Exit Sub
        End If

        If Not storedSessionId = packetSessionId Then
            RDSFactor.LogDebug(mPacket, "Stored session id didn't match packet session id!")
            mPacket.RejectAccessRequest()
            Exit Sub
        End If

        If HasValidSession(mUsername) Then
            RDSFactor.LogDebug(mPacket, "Opening window")
            ' Pro-long user session
            sessionTimestamps(mUsername) = Now
            ' Open gateway connection window
            userLaunchTimestamps(mUsername) = Now
            mPacket.AcceptAccessRequest()
            Exit Sub
        Else
            RDSFactor.LogDebug(mPacket, "Session timed out -- User MUST re-authenticate")
            userSessions.Remove(mUsername)
            sessionTimestamps.Remove(mUsername)
            mPacket.RejectAccessRequest()
        End If
    End Sub

    Public Shared Function HasValidLaunchWindow(username) As Boolean
        Dim timestamp = userLaunchTimestamps(username)

        Dim secondsSinceLaunch = DateDiff(DateInterval.Second, timestamp, Now)
        If secondsSinceLaunch < RDSFactor.LaunchTimeOut Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function HasValidSession(username) As Boolean
        Dim id = userSessions(username)
        Dim timestamp = sessionTimestamps(username)

        Dim minsSinceLastActivity = DateDiff(DateInterval.Minute, timestamp, Now)
        If minsSinceLastActivity < RDSFactor.SessionTimeOut Then
            Return True
        Else
            Return False
        End If
    End Function

    ' Process the request from the Network Policy Server in the RDS Gateway.
    ' These are sent when an RDP client tries to connect through the Gateway.
    '
    ' Accept-Access is returned when the user has a
    ' * valid session; and a
    ' * valid app launch window
    '
    ' The launch window is closed after this request.
    Public Sub ProcessGatewayRequest()
        RDSFactor.LogDebug(mPacket, "Gateway Request")

        Dim sessionId = userSessions(mUsername)
        Dim launchTimestamp = userLaunchTimestamps(mUsername)
        Dim attributes As New RADIUSAttributes

        If sessionId = Nothing Or launchTimestamp = Nothing Then
            RDSFactor.LogDebug(mPacket, "User has no launch window. User must re-authenticate")
            mPacket.RejectAccessRequest()
            Exit Sub
        End If

        Dim hasProxyState = mPacket.Attributes.AttributeExists(RadiusAttributeType.ProxyState)
        If hasProxyState Then
            Dim proxyState = mPacket.Attributes.GetFirstAttribute(RadiusAttributeType.ProxyState)
            attributes.Add(proxyState)
        End If

        If HasValidLaunchWindow(mUsername) Then
            RDSFactor.LogDebug(mPacket, "Opening gateway launch window")
            mPacket.AcceptAccessRequest(attributes)
        Else
            RDSFactor.LogDebug(mPacket, "Gateway launch window has timed out!")
            mPacket.RejectAccessRequest()
        End If

        RDSFactor.LogDebug(mPacket, "Removing gateway launch window")
        userLaunchTimestamps.Remove(mUsername)
    End Sub

    Public Sub ProcessAccessRequest()
        Dim hasState = mPacket.Attributes.AttributeExists(RadiusAttributeType.State)
        If hasState Then
            ' An Access-Request with a state is pr. definition a challenge response.
            ProcessChallengeResponse()
            Exit Sub
        End If

        RDSFactor.LogDebug(mPacket, "AccessRequest")
        Try
            Dim ldapResult = Authenticate()

            If RDSFactor.EnableOTP Then
                TwoFactorChallenge(ldapResult)
                Exit Sub
            Else
                Accept()
            End If
        Catch ex As Exception
            RDSFactor.LogDebug(mPacket, "Authentication failed. Sending reject. Error: " & ex.Message)
            mPacket.RejectAccessRequest(ex.Message)
        End Try
    End Sub

    Private Sub Accept()
        RDSFactor.LogDebug(mPacket, "AcceptAccessRequest")
        Dim sGUID As String = System.Guid.NewGuid.ToString()
        userSessions(mUsername) = sGUID
        sessionTimestamps(mUsername) = Now

        Dim attributes As New RADIUSAttributes
        Dim guidAttribute As New RADIUSAttribute(RadiusAttributeType.ReplyMessage, sGUID)

        attributes.Add(guidAttribute)
        mPacket.AcceptAccessRequest(attributes)
    End Sub

    Private Sub ProcessChallengeResponse()
        Dim authToken = mPacket.Attributes.GetFirstAttribute(RadiusAttributeType.State).ToString
        If Not authToken = authTokens(mUsername) Then
            Throw New Exception("User is trying to respond to challenge without valid auth token")
        End If

        ' When the packet is an Challenge-Response the password attr. contains the encrypted result
        Dim userEncryptedResult = mPassword
        Dim localEncryptedResult = encryptedChallengeResults(mUsername)

        If localEncryptedResult = userEncryptedResult Then
            RDSFactor.LogDebug(mPacket, "ChallengeResponse Success")
            encryptedChallengeResults.Remove(mUsername)
            authTokens.Remove(mUsername)
            Accept()
        Else
            RDSFactor.LogDebug(mPacket, "Wrong challenge code!")
            mPacket.RejectAccessRequest()
        End If
    End Sub

    Private Sub TwoFactorChallenge(ldapResult As SearchResult)
        Dim challengeCode = RDSFactor.GenerateCode
        Dim authToken = System.Guid.NewGuid.ToString
        Dim clientIP = mPacket.EndPoint.Address.ToString
        Dim sharedSecret = RDSFactor.secrets(clientIP)

        RDSFactor.LogDebug(mPacket, "Access Challenge Code: " & challengeCode)

        If sharedSecret = Nothing Then
            Throw New Exception("No shared secret for client:" & clientIP)
        End If

        authTokens(mUsername) = authToken
        Dim encryptedChallengeResult = Crypto.SHA256(mUsername & challengeCode & sharedSecret)
        encryptedChallengeResults(mUsername) = encryptedChallengeResult

        If mUseSMSFactor Then
            Dim mobile = LdapGetNumber(ldapResult)
            RDSFactor.SendSMS(mobile, challengeCode)
        End If

        If mUseEmailFactor Then
            Dim email = LdapGetEmail(ldapResult)
            RDSFactor.SendEmail(email, challengeCode)
        End If

        Dim attributes As New RADIUSAttributes

        Dim replyMessageAttr As New RADIUSAttribute(RadiusAttributeType.ReplyMessage, "SMS Token")
        Dim stateAttr As New RADIUSAttribute(RadiusAttributeType.State, authToken)

        attributes.Add(replyMessageAttr)
        attributes.Add(stateAttr)

        mPacket.SendAccessChallange(attributes)
    End Sub

    Private Function Authenticate() As System.DirectoryServices.SearchResult
        Dim password As String = mPacket.UserPassword
        Dim ldapDomain As String = RDSFactor.LDAPDomain

        RDSFactor.LogDebug(mPacket, "Authenticating with LDAP: " & "LDAP://" & ldapDomain)
        Dim dirEntry As New DirectoryEntry("LDAP://" & ldapDomain, mUsername, password)

        Dim obj As Object = dirEntry.NativeObject
        Dim search As New DirectorySearcher(dirEntry)

        If InStr(mUsername, "@") > 0 Then
            search.Filter = "(userPrincipalName=" + mUsername + ")"
        Else
            search.Filter = "(SAMAccountName=" + Split(mUsername, "\")(1) + ")"
        End If

        search.PropertiesToLoad.Add("distinguishedName")
        If RDSFactor.EnableOTP = True Then
            search.PropertiesToLoad.Add(RDSFactor.ADMobileField)
            search.PropertiesToLoad.Add(RDSFactor.ADMailField)
        End If

        Dim result = search.FindOne()

        If IsDBNull(result) Then
            RDSFactor.LogDebug(mPacket, "Failed to authenticate with Active Directory")
            Throw New MissingUser
        End If

        Return result
    End Function

    Private Function LdapGetNumber(result As SearchResult) As String
        If Not result.Properties.Contains(RDSFactor.ADMobileField) Then
            Throw New MissingLdapField(RDSFactor.ADMobileField, mUsername)
        End If
        Dim mobile = result.Properties(RDSFactor.ADMobileField)(0)
        mobile = Replace(mobile, "+", "")
        If mobile.Trim.Length = 0 Then
            RDSFactor.LogDebug(mPacket, "Unable to find correct phone number for user " & mUsername)
            Throw New MissingNumber(mUsername)
        End If
        Return mobile
    End Function

    Private Function LdapGetEmail(result As SearchResult) As String
        If Not result.Properties.Contains(RDSFactor.ADMailField) Then
            Throw New MissingLdapField(RDSFactor.ADMailField, mUsername)
        End If
        Dim email = result.Properties(RDSFactor.ADMailField)(0)
        If InStr(email, "@") = 0 Then
            RDSFactor.LogDebug(mPacket, "Unable to find correct email for user " & mUsername)
            Throw New MissingEmail(mUsername)
        End If
        Return email
    End Function

    Public Shared Sub Cleanup()
        RDSFactor.LogDebug("TimerCleanUp")

        Dim users = New ArrayList(userSessions.Keys)
        For Each username In users
            If Not HasValidSession(username) Then
                userSessions.Remove(username)
                sessionTimestamps.Remove(username)
                userLaunchTimestamps.Remove(username)
                encryptedChallengeResults.Remove(username)
                authTokens.Remove(username)
            End If
        Next
    End Sub

End Class

