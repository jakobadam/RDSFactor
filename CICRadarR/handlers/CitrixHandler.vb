Imports System.DirectoryServices
Imports RADAR

' TODO: I don't use this! It's a leftover, moved out of the way
' from the CICRadarR.vb
'
' Look in RDSHandler how this should be refactored.
Public Class CitrixHandler

    Public Sub New(packet As RADIUSPacket)

    End Sub

    Private Sub ProcessPacketCSG(ByVal server As RADIUSServer, ByVal packet As RADIUSPacket)

        ' Let's take a look at just authentication requests, 
        ' and drop other requests silently ...

        If packet.Code <> RadiusPacketCode.AccessRequest Then
            CICRadarR.AccessLog("Not a valid radius packet.. Drop!")
            Exit Sub
        End If



        ' Let's see if we have a username present ...
        Dim username As RADIUSAttribute = packet.Attributes.GetFirstAttribute(RadiusAttributeType.UserName)
        Dim pass As RADIUSAttribute = packet.Attributes.GetFirstAttribute(RadiusAttributeType.UserPassword)
        Dim sid As String = ""
        Dim mobile As String = ""
        Dim smsCode As String = ""
        Dim UserEmail As String = ""


        ' If an attribute of a particular type is not found, the function
        ' will return Nothing.
        If username Is Nothing Then
            ' Technically, this case is against RFC, so ... drop.
            CICRadarR.AccessLog("Not a valid radius packet.. No username pressent.. Drop!")
            Exit Sub
        End If

        CICRadarR.AccessLog("Processing packet for user: " & username.ToString)

        'If packetHash.ContainsKey(username.GetString & "_" & pass.GetString) Then
        '    Exit Sub
        'End If



        Dim existState As Boolean = packet.Attributes.AttributeExists(RadiusAttributeType.State)
        CICRadarR.AccessLog("Packet contains a state attribute? State=" & existState.ToString)
        If existState = True Then  ' Ok we have at packet with the State attribute set. Check if we can identify the authtentication packet.
            Dim state As String = packet.Attributes.GetFirstAttribute(RadiusAttributeType.State).ToString
            CICRadarR.AccessLog("Packet contains a state attribute State=" & state)
            Dim UserDomain As String = ""
            'lets see if user login using upd or UPN name
            Dim sUserName As String = username.ToString
            Dim sPassword As String = packet.UserPassword

            CICRadarR.AccessLog("SMSToken supplied by user: " & sUserName)

            sid = ""
            If InStr(sUserName, "@") > 0 Then 'UPN
                UserDomain = sUserName
            Else 'UPD
                'read domain from Hashtable 
                UserDomain = CICRadarR.NetBiosDomain & "\" & sUserName
            End If

            sid = EncDec.Encrypt(UserDomain & "_" & packet.UserPassword, CICRadarR.encCode)
            CICRadarR.AccessLog("Checking for userHash " & sid)
            If sid = state Then
                packet.AcceptAccessRequest()
            Else
                packet.RejectAccessRequest()
            End If
        Else ' process the first login

            '  packetHash.Add(username.GetString & "_" & pass.GetString, 0)

            '  Console.WriteLine(username.GetString & " is trying to log in ... ")
            ' Note that an attribute can represent a string, number, IP, etc.
            ' RADAR will not guess that automatically, so use the appropriate
            ' function according to the attribute you're trying to read. Otherwise,
            ' the Value property is just a bunch of bytes as received in the 
            ' RADIUS packet.


            'Now lets get some information from ad if password is valid
            Dim success As Boolean = False
            Dim UserDomain As String = ""
            'lets see if user login using upd or UPN name
            Dim sUserName As String = username.ToString
            Dim sPassword As String = packet.UserPassword
            If InStr(sUserName, "@") > 0 Then 'UPN
                UserDomain = sUserName
            Else 'UPD
                'read domain from Hashtable 
                UserDomain = CICRadarR.NetBiosDomain & "\" & sUserName
            End If

            CICRadarR.AccessLog("User " & UserDomain & " is trying to log in ...")



            Try
                Dim dirEntry As New DirectoryEntry("LDAP://" & CICRadarR.LDAPDomain, UserDomain, sPassword)

                Dim obj As Object = dirEntry.NativeObject
                Dim search As New DirectorySearcher(dirEntry)

                If InStr(sUserName, "@") > 0 Then
                    search.Filter = "(userPrincipalName=" + sUserName + ")"
                Else
                    search.Filter = "(SAMAccountName=" + sUserName + ")"
                End If
                'Load the Properties we need from AD
                search.PropertiesToLoad.Add("distinguishedName")
                'search.PropertiesToLoad.Add("primaryTelexNumber")
                If CICRadarR.EnableOTP = True Then
                    If CICRadarR.EnableEmail = True Then
                        search.PropertiesToLoad.Add(CICRadarR.ADMailField)
                    End If
                    If CICRadarR.EnableSMS = True Then
                        search.PropertiesToLoad.Add(CICRadarR.ADField)
                    End If

                End If
                ' Time to find out if user entered the correct username and pasword 
                CICRadarR.AccessLog("Trying to authenticate user agains Active Directory using te following parameters: " & "LDAPPAth: " & "LDAP://" & CICRadarR.LDAPDomain & ", Username: " & UserDomain & ", Password: " & sPassword)

                Dim result As SearchResult = search.FindOne()
                'Get the setting form AD. Yes we uses the field primaryTelexNumber, for who the f... still users telex. (I bet half the people reading this code don't even know what a telex is!)
                'Dim code As String = DirectCast(result.Properties("primaryTelexNumber")(0), String)
                'Dim aCode As String() = code.Split("/")

                'Dim userLdap As String = "LDAP://" & LDAPPath & "/" & result.Properties("distinguishedName")(0)
                'Dim userEntry As New DirectoryEntry(userLdap, UserDomain, sPassword)
                If CICRadarR.EnableOTP = True Then
                    smsCode = CICRadarR.GenerateCode()

                    ' REMEMBER to put at check for empty phone string
                    If CICRadarR.EnableEmail = True Then
                        Try
                            UserEmail = DirectCast(result.Properties(CICRadarR.ADMailField)(0), String)

                            If UserEmail.Trim.Length = 0 Or InStr(UserEmail, "@") = 0 Then
                                success = False
                                CICRadarR.AccessLog("Unable to find correct email for user " & UserDomain)
                            Else
                                success = True
                            End If
                        Catch
                            CICRadarR.AccessLog("Unable to find correct email for user " & UserDomain)
                            success = False
                        End Try
                    End If
                    If CICRadarR.EnableSMS = True Then
                        Try
                            mobile = DirectCast(result.Properties(CICRadarR.ADField)(0), String)
                            mobile = Replace(mobile, "+", "")
                            If mobile.Trim.Length = 0 Then
                                success = False
                                CICRadarR.AccessLog("Unable to find correct phone number for user " & UserDomain)
                            Else
                                success = True
                            End If
                        Catch
                            CICRadarR.AccessLog("Unable to find correct phone number for user " & UserDomain)
                            success = False
                        End Try

                    End If

                    sid = EncDec.Encrypt(UserDomain & "_" & smsCode, CICRadarR.encCode) 'generate unique code
                End If
                ' sid = UserDomain & "_" & smsCode
                'userEntry.Properties("primaryTelexNumber").Value = aCode(0) & "/" & smsCode & "/" & aCode(2) & "/" & aCode(3)
                'userEntry.CommitChanges()
                'userEntry.Dispose()
                If 1 = 1 Then ' check if smscode is disabled for the user (Need to write this code)
                    'If userHash.ContainsKey(sid) Then
                    '    userHash(sid) = sPassword
                    '    If DEBUG = True Then
                    '        CICRadarR.AccessLog("Updating userHash " & sid)
                    '    End If
                    'Else
                    '    userHash.Add(sid, sPassword)
                    '    If DEBUG = True Then
                    '        CICRadarR.AccessLog("Adding userHash " & sid)
                    '    End If
                    'End If
                    ' new code stored in AD now send it to the users phone
                    ' Console.WriteLine(smsCode)

                    success = True
                Else
                    success = False
                End If
            Catch
                CICRadarR.AccessLog("Failed to authenticate user agains Active Directory using the following parameters: " & "LDAPPAth: " & "LDAP://" & CICRadarR.LDAPDomain & ", Username: " & UserDomain & ", Password: " & sPassword)
                success = False
            End Try


            Dim attributes As New RADIUSAttributes
            If success Then ' Yay! Someone guess the password ...

                CICRadarR.AccessLog("User " & UserDomain & " authenticated agains Active Directory")
                If CICRadarR.EnableOTP = True Then
                    Dim attr As New RADIUSAttribute(RadiusAttributeType.ReplyMessage, "SMS Token")
                    attributes.Add(attr)
                    Dim state As New RADIUSAttribute(RadiusAttributeType.State, sid)
                    attributes.Add(state)
                    '  Console.WriteLine("len " & packet.Authenticator.Length.ToString)
                    server.SendAsResponse( _
                        New RADIUSPacket(RadiusPacketCode.AccessChallenge, _
                                         packet.Identifier, attributes, _
                                         packet.EndPoint), _
                        packet.Authenticator)
                    If CICRadarR.EnableSMS = True Then
                        CICRadarR.AccessLog("Sending access token: " & smsCode & " to phonenumber " & mobile)
                        Call CICRadarR.SendSMS(mobile, smsCode)
                    End If
                    If CICRadarR.EnableEmail = True Then
                        CICRadarR.AccessLog("Sending access token: " & smsCode & " to email " & UserEmail)
                        Call CICRadarR.SendEmail(UserEmail, smsCode)
                    End If
                Else
                    CICRadarR.AccessLog("One time Password not enabled, so we let the user in")
                    packet.AcceptAccessRequest()
                End If
                ' packetHash.Remove(username.GetString & "_" & pass.GetString)
            Else ' Wrong username / password ...

                CICRadarR.AccessLog("User " & UserDomain & " failed to authenticate against Active Directory")
                Dim pk As New RADIUSPacket(RadiusPacketCode.AccessReject, packet.Identifier, Nothing, packet.EndPoint)
                server.SendAsResponse(pk, packet.Authenticator)
                ' FYI ... if no additional attributes need to be added
                ' to the response, you can sepcify Nothing instead of
                ' creating an empty RADIUSAttributes object.
                '   packetHash.Remove(username.GetString & "_" & pass.GetString)
            End If



        End If
    End Sub


End Class
