Imports System.DirectoryServices

Public Class RDSHandler

    Private Shared userSessions As New Hashtable
    Private Shared sessionTimestamps As New Hashtable

    Private Shared userSidTokens As New Hashtable
    Private Shared tokenTimestamps As New Hashtable

    Private mPacket As RADIUSPacket
    Private mUsername As String
    Private mPassword As String

    ' RDS specific values 
    Private mIsAppLaunchRequest As Boolean
    Private mIsGatewayRequest As Boolean
    Private mUseSMSFactor As Boolean
    Private mUseEmailFactor As Boolean

    Private TSGWLaunchIdTimeStampHash As New Hashtable
    Private TSGWFirstLoginHash As New Hashtable ' Ensure that only one sms is send even if radius need to re-authenticate.
    Private TSGWFirstLoginTimeStampHash As New Hashtable ' Ensure that only one sms is send even if radius need to re-authenticate.

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

    Public Sub ProcessAppLaunchRequest()
        Console.WriteLine("ProcessAppLaunchRequest")

        ' When the packet is an AppLaunchRequest the password attribute contains the session id!
        Dim packetSessionId = mPassword

        Dim sessionId = userSessions(mUsername)
        Dim sessionTimestamp = sessionTimestamps(mUsername)

        If sessionId = Nothing Or sessionTimestamp = Nothing Then
            Console.WriteLine("Rejecting Access-Request to open app")
            mPacket.RejectAccessRequest()
            Exit Sub
        End If

        Dim tValid = DateDiff(DateInterval.Minute, sessionTimestamp, Now)
        If tValid < CICRadarR.SessionTimeOut Then
            If packetSessionId = sessionId Then
                Console.WriteLine("Accepting Request to open app")
                ' Pro-long open window
                sessionTimestamps(sessionId) = Now
                mPacket.AcceptAccessRequest()
                Exit Sub
            End If
        End If

        Console.WriteLine("Token timed out")
        mPacket.RejectAccessRequest()
    End Sub

    Public Sub ProcessGatewayRequest()
        Console.WriteLine("Gateway Request for user: " & mUsername)

        Dim sessionId = userSessions(mUsername)
        Dim sessionTimestamp = sessionTimestamps(mUsername)
        Dim attributes As New RADIUSAttributes

        If sessionId = Nothing Or sessionTimestamp = Nothing Then
            Console.WriteLine("No user session... User must re-authenticate")
            mPacket.RejectAccessRequest()
            Exit Sub
        End If

        Dim hasProxyState = mPacket.Attributes.AttributeExists(RadiusAttributeType.ProxyState)
        If hasProxyState Then
            Dim proxyState = mPacket.Attributes.GetFirstAttribute(RadiusAttributeType.ProxyState)
            attributes.Add(proxyState)
        End If

        Dim tValid = DateDiff(DateInterval.Minute, sessionTimestamp, Now)
        If tValid < CICRadarR.SessionTimeOut Then
            Console.WriteLine("Accepting Reuqest to open app")
            sessionTimestamps(sessionId) = Now
            mPacket.AcceptAccessRequest(attributes)
            Exit Sub
        Else
            Console.WriteLine("Session IDs did not match")
        End If

    End Sub

    Public Sub ProcessAccessRequest()
        Dim hasState = mPacket.Attributes.AttributeExists(RadiusAttributeType.State)
        If hasState Then
            ' An access-request with a state is pr. definition a challange response.
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
            Console.WriteLine("SMS: ")
        ElseIf mUseEmailFactor Then
            Console.WriteLine("Email: ")
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
        Console.WriteLine("Passowrd: " & password)
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

