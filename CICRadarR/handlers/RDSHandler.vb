Imports System.DirectoryServices

Public Class RDSHandler

    Private Shared userSessions As New Hashtable
    Private Shared sessionTimestamps As New Hashtable

    Private Shared userSidTokens As New Hashtable
    Private Shared tokenTimestamps As New Hashtable

    Private mPacket As RADIUSPacket

    Private packetUsername As String
    Private packetPassword As String
    Private packetSessionId As String
    Private packetChallangeCode As String

    ' RDS specific values 
    Private mIsAppLaunchRequest As Boolean
    Private mIsGatewayRequest As Boolean
    Private mIsSMSRequest As Boolean
    Private mIsEmailRequest As Boolean

    Private mHasState As Boolean
    Private mHasProxyState As Boolean
    Private mProxyState As RADIUSAttribute
    Private mState As RADIUSAttribute

    Private TSGWLaunchIdTimeStampHash As New Hashtable
    Private TSGWFirstLoginHash As New Hashtable ' Ensure that only one sms is send even if radius need to re-authenticate.
    Private TSGWFirstLoginTimeStampHash As New Hashtable ' Ensure that only one sms is send even if radius need to re-authenticate.

    Public Sub New(packet As RADIUSPacket)
        mPacket = packet
    End Sub

    Public Sub ProcessRequest()
        ExtractAttributes()

        If ValidPacket() = False Then
            Exit Sub
        End If

        If mIsAppLaunchRequest Then
            ProcessAppLaunchRequest()
        ElseIf mIsGatewayRequest Then
            ProcessGatewayRequest()
        ElseIf mHasState Then
            ProcessChallengeResponse()
        Else
            ProcessAccessRequest()
        End If

    End Sub

    Public Sub ProcessAppLaunchRequest()
        Console.WriteLine("ProcessAppLaunchRequest")

        Dim sessionId = userSessions(packetUsername)
        Dim sessionTimestamp = sessionTimestamps(packetUsername)

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
        Console.WriteLine("Process Gateway Request")

        Dim sessionId = userSessions(packetUsername)
        Dim sessionTimestamp = sessionTimestamps(packetUsername)
        Dim attributes As New RADIUSAttributes

        If sessionId = Nothing Or sessionTimestamp = Nothing Then
            Console.WriteLine("No user session... User must re-authenticate")
            mPacket.RejectAccessRequest()
            Exit Sub
        End If

        If mHasProxyState Then
            attributes.Add(mProxyState)
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
            Console.WriteLine("Authentication failed. Sending reject.")
            mPacket.RejectAccessRequest()
        End Try
    End Sub

    Private Sub Accept()
        Dim sGUID As String = System.Guid.NewGuid.ToString()
        userSessions(packetUsername) = sGUID
        sessionTimestamps(packetUsername) = Now

        Dim attributes As New RADIUSAttributes
        Dim guidAttribute As New RADIUSAttribute(RadiusAttributeType.ReplyMessage, sGUID)

        attributes.Add(guidAttribute)
        mPacket.AcceptAccessRequest(attributes)
    End Sub

    Private Sub ProcessChallengeResponse()
        Console.WriteLine("ProcessChallengeResponse")
        Dim sid = EncDec.Encrypt(packetUsername & "_" & packetChallangeCode, CICRadarR.encCode)
        Dim mStateStr = mState.ToString
        If sid = mState.ToString Then
            Accept()
        Else
            mPacket.RejectAccessRequest()
        End If
    End Sub

    Private Sub TwoFactorChallenge()
        Dim code = CICRadarR.GenerateCode
        Dim sid = EncDec.Encrypt(packetUsername & "_" & code, CICRadarR.encCode) 'generate unique code
        Console.WriteLine("Access Challange Code: " & code)

        userSidTokens(packetUsername) = sid
        tokenTimestamps(packetUsername) = Now

        If mIsSMSRequest Then
            Console.WriteLine("SMS: ")
        ElseIf mIsEmailRequest Then
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

        Console.WriteLine("Authenticating: LDAPPAth: " & "LDAP://" & ldapDomain & ", Username: " & packetUsername)
        Dim dirEntry As New DirectoryEntry("LDAP://" & ldapDomain, packetUsername, password)

        Dim obj As Object = dirEntry.NativeObject
        Dim search As New DirectorySearcher(dirEntry)

        If InStr(packetUsername, "@") > 0 Then
            search.Filter = "(userPrincipalName=" + packetUsername + ")"
        Else
            search.Filter = "(SAMAccountName=" + Split(packetUsername, "\")(1) + ")"
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
            Console.WriteLine("Unable to find correct phone number for user " & packetUsername)
        End If
        Return mobile
    End Function

    Private Function LdapGetEmail(result As SearchResult) As String
        Dim email = result.Properties(CICRadarR.ADMailField)(0)

        If InStr(email, "@") = 0 Then
            Console.WriteLine("Unable to find correct email for user " & packetUsername)
        End If
        Return email
    End Function

    Private Function ValidPacket()
        If packetUsername Is Nothing Then
            Console.WriteLine("Not a valid radius packet.. No username present.. Drop!")
            Return False
        End If
        Return True
    End Function

    Private Sub ExtractAttributes()
        mHasState = mPacket.Attributes.AttributeExists(RadiusAttributeType.State)
        mHasProxyState = mPacket.Attributes.AttributeExists(RadiusAttributeType.ProxyState)

        If mHasState Then
            mState = mPacket.Attributes.GetFirstAttribute(RadiusAttributeType.State)
            Console.WriteLine("State:" & mState.ToString)
        End If
        If mHasProxyState Then
            mProxyState = mPacket.Attributes.GetFirstAttribute(RadiusAttributeType.ProxyState)
            Console.WriteLine("ProxyState:" & mProxyState.ToString)
        End If

        packetUsername = mPacket.UserName.ToLower
        packetPassword = mPacket.UserPassword

        ' When the packet is an AppLaunchRequest the password attribute contains the session id!
        packetSessionId = packetPassword

        ' When the packet is an Challange-Response the password attr. contains the token
        packetChallangeCode = packetPassword

        For Each atts As RADIUSAttribute In mPacket.Attributes.GetAllAttributes(RadiusAttributeType.VendorSpecific)
            Dim value As String = atts.GetVendorSpecific.VendorValue.ToString

            Select Case UCase(value)
                Case "LAUNCH"
                    mIsAppLaunchRequest = True
                Case "TSGATEWAY"
                    mIsGatewayRequest = True
                Case "SMS"
                    mIsSMSRequest = True
                Case "EMAIL"
                    mIsEmailRequest = True
            End Select
        Next

    End Sub

End Class

