Imports System.DirectoryServices

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
        Console.WriteLine("ProcessAppLaunchRequest")

        ' When the packet is an AppLaunchRequest the password attribute contains the session id!
        Dim packetSessionId = mPassword
        Dim storedSessionId = userSessions(mUsername)
        Dim sessionTimestamp = sessionTimestamps(mUsername)

        If storedSessionId = Nothing Or sessionTimestamp = Nothing Then
            Console.WriteLine("User has no session. MUST re-authenticate!")
            mPacket.RejectAccessRequest()
            Exit Sub
        End If

        If packetSessionId = storedSessionId Then
            Dim minsSinceLastActivity = DateDiff(DateInterval.Minute, sessionTimestamp, Now)
            If minsSinceLastActivity < CICRadarR.SessionTimeOut Then
                Console.WriteLine("Opening window for: " & mUsername)
                ' Pro-long session
                sessionTimestamps(storedSessionId) = Now
                ' Open launch window
                userLaunchTimestamps(mUsername) = Now
                mPacket.AcceptAccessRequest()
                Exit Sub
            Else
                Console.WriteLine("Session timed out -- User MUST re-authenticate")
                userSessions.Remove(mUsername)
                sessionTimestamps.Remove(mUsername)
            End If
        Else
            Console.WriteLine("Stored session id didn't match packet session id!")
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
    Public Sub ProcessGatewayRequest()
        Console.WriteLine("Gateway Request for user: " & mUsername)

        Dim sessionId = userSessions(mUsername)
        Dim launchTimestamp = userLaunchTimestamps(mUsername)
        Dim attributes As New RADIUSAttributes

        If sessionId = Nothing Or launchTimestamp = Nothing Then
            Console.WriteLine("User's has no lauch window. User must re-authenticate")
            mPacket.RejectAccessRequest()
            Exit Sub
        End If

        Dim hasProxyState = mPacket.Attributes.AttributeExists(RadiusAttributeType.ProxyState)
        If hasProxyState Then
            Dim proxyState = mPacket.Attributes.GetFirstAttribute(RadiusAttributeType.ProxyState)
            attributes.Add(proxyState)
        End If

        Dim secondsSinceLaunch = DateDiff(DateInterval.Second, launchTimestamp, Now)
        If secondsSinceLaunch < CICRadarR.LaunchTimeOut Then
            Console.WriteLine("Allowing access through gateway for user: " & mUsername & " -- closing window")
            mPacket.AcceptAccessRequest(attributes)
        Else
            Console.WriteLine("Launch window has closed!")
        End If

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

        Console.WriteLine("ProcessAccessRequest")
        Try
            Dim ldapResult = Authenticate()

            If CICRadarR.EnableOTP Then
                TwoFactorChallenge()
                Exit Sub
            Else
                Accept()
            End If
        Catch ex As Exception
            Console.WriteLine("Authentication failed. Sending reject. Error: " & ex.Message)
            mPacket.RejectAccessRequest()
        End Try
    End Sub

    Private Sub Accept()
        Console.WriteLine("Accept")
        Dim sGUID As String = System.Guid.NewGuid.ToString()
        userSessions(mUsername) = sGUID
        sessionTimestamps(mUsername) = Now

        Dim attributes As New RADIUSAttributes
        Dim guidAttribute As New RADIUSAttribute(RadiusAttributeType.ReplyMessage, sGUID)

        attributes.Add(guidAttribute)
        mPacket.AcceptAccessRequest(attributes)
    End Sub

    Private Sub ProcessChallengeResponse()
        Console.WriteLine("ProcessChallengeResponse")

        ' When the packet is an Challange-Response the password attr. contains the token
        Dim challangeCode = mPassword
        Dim state = mPacket.Attributes.GetFirstAttribute(RadiusAttributeType.State)

        Dim sid = EncDec.Encrypt(mUsername & "_" & challangeCode, CICRadarR.encCode)
        If sid = state.ToString Then
            Accept()
        Else
            mPacket.RejectAccessRequest()
        End If
    End Sub

    Private Sub TwoFactorChallenge()
        Dim code = CICRadarR.GenerateCode
        Dim sid = EncDec.Encrypt(mUsername & "_" & code, CICRadarR.encCode) 'generate unique code
        Console.WriteLine("Access Challange Code: " & code)

        userSidTokens(mUsername) = sid
        tokenTimestamps(mUsername) = Now

        If mUseSMSFactor Then
            Console.WriteLine("TODO: Send SMS")
        End If

        If mUseEmailFactor Then
            Console.WriteLine("TODO: Send Email")
        End If

        Dim attributes As New RADIUSAttributes

        Dim attr As New RADIUSAttribute(RadiusAttributeType.ReplyMessage, "SMS Token")
        Dim state As New RADIUSAttribute(RadiusAttributeType.State, sid)

        attributes.Add(attr)
        attributes.Add(state)

        mPacket.SendAccessChallenge(attributes)
    End Sub

    Private Function Authenticate() As System.DirectoryServices.SearchResult
        Dim password As String = mPacket.UserPassword
        Dim ldapDomain As String = CICRadarR.LDAPDomain

        Console.WriteLine("Authenticating: LDAPPAth: " & "LDAP://" & ldapDomain & ", Username: " & mUsername)
        Dim dirEntry As New DirectoryEntry("LDAP://" & ldapDomain, mUsername, password)

        Dim obj As Object = dirEntry.NativeObject
        Dim search As New DirectorySearcher(dirEntry)

        If InStr(mUsername, "@") > 0 Then
            search.Filter = "(userPrincipalName=" + mUsername + ")"
        Else
            search.Filter = "(SAMAccountName=" + Split(mUsername, "\")(1) + ")"
        End If

        search.PropertiesToLoad.Add("distinguishedName")
        If CICRadarR.EnableOTP = True Then
            search.PropertiesToLoad.Add(CICRadarR.ADField)
            search.PropertiesToLoad.Add(CICRadarR.ADMailField)
        End If

        Dim result = search.FindOne()

        If IsDBNull(result) Then
            Console.WriteLine("Failed to authenticate with Active Directory")
            Throw New MissingUser
        End If

        Return result
    End Function

    Private Function LdapGetNumber(result As SearchResult) As String
        Dim mobile = result.Properties(CICRadarR.ADField)(0)
        mobile = Replace(mobile, "+", "")
        If mobile.Trim.Length = 0 Then
            Console.WriteLine("Unable to find correct phone number for user " & mUsername)
        End If
        Return mobile
    End Function

    Private Function LdapGetEmail(result As SearchResult) As String
        Dim email = result.Properties(CICRadarR.ADMailField)(0)

        If InStr(email, "@") = 0 Then
            Console.WriteLine("Unable to find correct email for user " & mUsername)
        End If
        Return email
    End Function

End Class

