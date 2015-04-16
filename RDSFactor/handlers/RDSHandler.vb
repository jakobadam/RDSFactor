Imports System.DirectoryServices
Imports RADAR

Public Class RDSHandler

    Private Shared userSessions As New Hashtable
    Private Shared sessionTimestamps As New Hashtable

    Private Shared userSidTokens As New Hashtable
    Private Shared tokenTimestamps As New Hashtable

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
        RDSFactor.AccessLog(mPacket, "AppLaunchRequest")

        ' When the packet is an AppLaunchRequest the password attribute contains the session id!
        Dim packetSessionId = mPassword
        Dim storedSessionId = userSessions(mUsername)
        Dim sessionTimestamp = sessionTimestamps(mUsername)

        If storedSessionId = Nothing Or sessionTimestamp = Nothing Then
            RDSFactor.AccessLog(mPacket, "User has no session. MUST re-authenticate!")
            mPacket.RejectAccessRequest()
            Exit Sub
        End If

        If packetSessionId = storedSessionId Then
            Dim minsSinceLastActivity = DateDiff(DateInterval.Minute, sessionTimestamp, Now)
            If minsSinceLastActivity < RDSFactor.SessionTimeOut Then
                RDSFactor.AccessLog(mPacket, "Opening window")
                ' Pro-long session
                sessionTimestamps(storedSessionId) = Now
                ' Open launch window
                userLaunchTimestamps(mUsername) = Now
                mPacket.AcceptAccessRequest()
                Exit Sub
            Else
                RDSFactor.AccessLog(mPacket, "Session timed out -- User MUST re-authenticate")
                userSessions.Remove(mUsername)
                sessionTimestamps.Remove(mUsername)
            End If
        Else
            RDSFactor.AccessLog(mPacket, "Stored session id didn't match packet session id!")
        End If

        mPacket.RejectAccessRequest()
    End Sub

    ' Process the request from the Network Policy Server in the RDS Gateway.
    ' These are sent when an RDP client tries to connect through the Gateway.
    '
    ' Accept-Access is returned when the user has a
    ' * valid session; and a
    ' * valid app launch window
    '
    ' The launch window is closed after this request.
    '
    ' TODO: Fix race-condition RD Web vs. Gateway. Don't start RDP client in RD Web 
    ' before ensuring App Launch request was successful 
    Public Sub ProcessGatewayRequest()
        RDSFactor.AccessLog(mPacket, "Gateway Request")

        Dim sessionId = userSessions(mUsername)
        Dim launchTimestamp = userLaunchTimestamps(mUsername)
        Dim attributes As New RADIUSAttributes

        If sessionId = Nothing Or launchTimestamp = Nothing Then
            RDSFactor.AccessLog(mPacket, "User's has no launch window. User must re-authenticate")
            mPacket.RejectAccessRequest()
            Exit Sub
        End If

        Dim hasProxyState = mPacket.Attributes.AttributeExists(RadiusAttributeType.ProxyState)
        If hasProxyState Then
            Dim proxyState = mPacket.Attributes.GetFirstAttribute(RadiusAttributeType.ProxyState)
            attributes.Add(proxyState)
        End If

        Dim secondsSinceLaunch = DateDiff(DateInterval.Second, launchTimestamp, Now)
        If secondsSinceLaunch < RDSFactor.LaunchTimeOut Then
            RDSFactor.AccessLog(mPacket, "Opening gateway connection window")
            mPacket.AcceptAccessRequest(attributes)
        Else
            RDSFactor.AccessLog(mPacket, "Gateway connection window has timed out!")
        End If

        RDSFactor.AccessLog(mPacket, "Removing gateway connection window")
        ' close window
        userLaunchTimestamps.Remove(mUsername)
    End Sub

    Public Sub ProcessAccessRequest()
        Dim hasState = mPacket.Attributes.AttributeExists(RadiusAttributeType.State)
        If hasState Then
            ' An Access-Request with a state is pr. definition a challange response.
            ProcessChallengeResponse()
            Exit Sub
        End If

        RDSFactor.AccessLog(mPacket, "AccessRequest")
        Try
            Dim ldapResult = Authenticate()

            If RDSFactor.EnableOTP Then
                TwoFactorChallenge()
                Exit Sub
            Else
                Accept()
            End If
        Catch ex As Exception
            RDSFactor.AccessLog(mPacket, "Authentication failed. Sending reject. Error: " & ex.Message)
            mPacket.RejectAccessRequest()
        End Try
    End Sub

    Private Sub Accept()
        RDSFactor.AccessLog(mPacket, "AcceptAccessRequest")
        Dim sGUID As String = System.Guid.NewGuid.ToString()
        userSessions(mUsername) = sGUID
        sessionTimestamps(mUsername) = Now

        Dim attributes As New RADIUSAttributes
        Dim guidAttribute As New RADIUSAttribute(RadiusAttributeType.ReplyMessage, sGUID)

        attributes.Add(guidAttribute)
        mPacket.AcceptAccessRequest(attributes)
    End Sub

    Private Sub ProcessChallengeResponse()
        RDSFactor.AccessLog(mPacket, "ChallengeResponse")

        ' When the packet is an Challange-Response the password attr. contains the token
        Dim challangeCode = mPassword
        Dim state = mPacket.Attributes.GetFirstAttribute(RadiusAttributeType.State)

        Dim sid = EncDec.Encrypt(mUsername & "_" & challangeCode, RDSFactor.encCode)
        If sid = state.ToString Then
            Accept()
        Else
            mPacket.RejectAccessRequest()
        End If
    End Sub

    Private Sub TwoFactorChallenge()
        Dim code = RDSFactor.GenerateCode
        Dim sid = EncDec.Encrypt(mUsername & "_" & code, RDSFactor.encCode) 'generate unique code
        RDSFactor.AccessLog(mPacket, "Access Challange Code: " & code)

        userSidTokens(mUsername) = sid
        tokenTimestamps(mUsername) = Now

        If mUseSMSFactor Then
            RDSFactor.AccessLog(mPacket, "TODO: Send SMS")
        End If

        If mUseEmailFactor Then
            RDSFactor.AccessLog(mPacket, "TODO: Send Email")
        End If

        Dim attributes As New RADIUSAttributes

        Dim attr As New RADIUSAttribute(RadiusAttributeType.ReplyMessage, "SMS Token")
        Dim state As New RADIUSAttribute(RadiusAttributeType.State, sid)

        attributes.Add(attr)
        attributes.Add(state)

        mPacket.SendAccessChallange(attributes)
    End Sub

    Private Function Authenticate() As System.DirectoryServices.SearchResult
        Dim password As String = mPacket.UserPassword
        Dim ldapDomain As String = RDSFactor.LDAPDomain

        RDSFactor.AccessLog(mPacket, "Authenticating with LDAP: " & "LDAP://" & ldapDomain)
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
            search.PropertiesToLoad.Add(RDSFactor.ADField)
            search.PropertiesToLoad.Add(RDSFactor.ADMailField)
        End If

        Dim result = search.FindOne()

        If IsDBNull(result) Then
            RDSFactor.AccessLog(mPacket, "Failed to authenticate with Active Directory")
            Throw New MissingUser
        End If

        Return result
    End Function

    Private Function LdapGetNumber(result As SearchResult) As String
        Dim mobile = result.Properties(RDSFactor.ADField)(0)
        mobile = Replace(mobile, "+", "")
        If mobile.Trim.Length = 0 Then
            RDSFactor.AccessLog(mPacket, "Unable to find correct phone number for user " & mUsername)
        End If
        Return mobile
    End Function

    Private Function LdapGetEmail(result As SearchResult) As String
        Dim email = result.Properties(RDSFactor.ADMailField)(0)

        If InStr(email, "@") = 0 Then
            RDSFactor.AccessLog(mPacket, "Unable to find correct email for user " & mUsername)
        End If
        Return email
    End Function

End Class

