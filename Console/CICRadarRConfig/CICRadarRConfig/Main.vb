Imports CICRadarRConfig.SMS
Imports System.ServiceProcess
Imports System.IO
Imports System.Net.Mail
Public Class CICRadiusRConfig
    Private ConfigFile As New IniFile
    Private encCode As String = "gewsyy#sjs2!"
    Private Sub CICRadiusRConfig_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Height = 440
        Me.Width = 620


        PanelSetup.Location = New Point(136, 27)
        PanelSetup.Height = 357
        PanelSetup.Width = 470

        PanelActiveDirectory.Location = New Point(136, 27)
        PanelActiveDirectory.Height = 357
        PanelActiveDirectory.Width = 470

        PanelRadiusClients.Location = New Point(136, 27)
        PanelRadiusClients.Height = 357
        PanelRadiusClients.Width = 470

        PanelSMSSetup.Location = New Point(136, 27)
        PanelSMSSetup.Height = 357
        PanelSMSSetup.Width = 470

        PanelMailSetup.Location = New Point(136, 27)
        PanelMailSetup.Height = 357
        PanelMailSetup.Width = 470
        Try
            ConfigFile.Load(Application.StartupPath & "\CICRadarR.ini")
            txtNetBios.Text = ConfigFile.GetKeyValue("CICRadarR", "NetBiosDomain")
            txtLdapDomain.Text = ConfigFile.GetKeyValue("CICRadarR", "LDAPDomain")
            txtProvider.Text = ConfigFile.GetKeyValue("CICRadarR", "Provider")
            txtADField.Text = ConfigFile.GetKeyValue("CICRadarR", "ADField")
            txtADMailField.Text = ConfigFile.GetKeyValue("CICRadarR", "ADMailField")
            Dim ModemType As String
            ModemType = ConfigFile.GetKeyValue("CICRadarR", "USELOCALMODEM")
            If ModemType = "1" Then
                rbLocalSMS.Checked = True
            Else
                rbLocalSMS.Checked = False
            End If

            Dim EnableOTP As String
            EnableOTP = ConfigFile.GetKeyValue("CICRadarR", "EnableOTP")
            If EnableOTP = "1" Then
                ckEnableOTP.Checked = True
                Dim EnableSMS As String
                Dim EnableEmail As String
                EnableSMS = ConfigFile.GetKeyValue("CICRadarR", "EnableSMS")
                EnableEmail = ConfigFile.GetKeyValue("CICRadarR", "EnableEmail")
                If EnableEmail = "1" Then
                    ckEnableMail.Checked = True
                Else
                    TestMailConfigurationToolStripMenuItem.Enabled = False
                    lvConfig.Items.Item(3).ImageIndex = 6
                    ckEnableMail.Checked = False

                End If

                If EnableSMS = "1" Then
                    ckEnableSMS.Checked = True
                Else
                    lvConfig.Items.Item(2).ImageIndex = 5
                    ckEnableSMS.Checked = False
                    TestModemConfigurationToolStripMenuItem.Enabled = False
                End If
            Else
                lvConfig.Items.Item(2).ImageIndex = 5
                lvConfig.Items.Item(3).ImageIndex = 6
                TestMailConfigurationToolStripMenuItem.Enabled = False
                TestModemConfigurationToolStripMenuItem.Enabled = False
                ckEnableSMS.Checked = False
                ckEnableMail.Checked = False
                ckEnableOTP.Checked = False
            End If

            txtMailServer.Text = ConfigFile.GetKeyValue("CICRadarR", "MailServer")
            txtSenderEmail.Text = ConfigFile.GetKeyValue("CICRadarR", "SenderEmail")

            txtComPort.Text = ConfigFile.GetKeyValue("CICRadarR", "COMPORT")
            txtSMSC.Text = ConfigFile.GetKeyValue("CICRadarR", "SMSC")
            Dim Debug As String
            Debug = ConfigFile.GetKeyValue("CICRadarR", "Debug")
            If Debug = "1" Then
                ckDebug.Checked = True
            Else
                ckDebug.Checked = False
            End If

            Dim RDGateway As String
            RDGateway = ConfigFile.GetKeyValue("CICRadarR", "TSGW")
            If RDGateway = "1" Then
                rbRDGateway.Checked = True
                rbCitrixNetscaler.Checked = False
            Else
                rbRDGateway.Checked = False
                rbCitrixNetscaler.Checked = True
            End If

            Dim ClientList() As String
            ClientList = Split(ConfigFile.GetKeyValue("CICRadarR", "ClientList"), ",")

            For i As Integer = 0 To ClientList.Length - 1
                ListClients.Items.Add(ClientList(i) & " ( " & ConfigFile.GetKeyValue("Clients", ClientList(i)) & " )")
            Next




        Catch
        End Try

        Dim tt As New ToolTip()

        tt.ShowAlways = True

        tt.SetToolTip(txtADField, "Type the Active Directory field where phonenumbers are stored." & vbCrLf & "Ex: mobile or telephoneNumber" & vbCrLf & "Use Adsiedit.msc to find the correct field.")
        tt.SetToolTip(txtSMSC, "See http://smsclist.com/downloads/default.txt for your provider" & vbCrLf & "Ex: +4540390999")
        tt.SetToolTip(txtComPort, "Ex: com1")
        tt.SetToolTip(txtProvider, "Type the https address of your SMS provider." & vbCrLf & "Replace the message field of the url with ***TEXTMESSAGE***" & vbCrLf & "Replace the recipient field of the url with ***NUMBER***" & vbCrLf & "Ex: https://www.cpsms.dk/sms/?username=myuser&password=mypassword&recipient=***NUMBER***&message=&from=CPSMS")
        tt.SetToolTip(txtNetBios, "Set NetBios Domain name" & vbCrLf & "Ex: MYDOMAIN")
        tt.SetToolTip(txtLdapDomain, "Set LDAP Domain" & vbCrLf & "Ex: test.lan")
        tt.SetToolTip(ckEnableOTP, "Enable SMS Magic.")

        ToolStripStatusLabel1.Text = "Status: " & "Configuration loaded"
    End Sub

   






   


    Private Sub btnRemoveClient_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If ListClients.SelectedIndex > -1 Then

            ListClients.Items.RemoveAt(ListClients.SelectedIndex)

        End If
    End Sub

    Private Sub btnAddClient_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Not ListClients.Items.Contains(txtClient.Text & " ( " & EncDec.Encrypt(txtSecret.Text, encCode) & " )") Then
            ListClients.Items.Add(txtClient.Text & " ( " & EncDec.Encrypt(txtSecret.Text, encCode) & " )")
        End If
    End Sub


    Private Sub btnRestart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRestart.Click
        Dim ok As MsgBoxResult
        btnRestart.Enabled = False
        ok = MsgBox("Restart CICRadiusR Service?", MsgBoxStyle.YesNo, "Restart Service")
        If ok = vbYes Then
            ToolStripStatusLabel1.Text = "Status: " & "Radius server restarting..."
            Dim controller As New ServiceController("CICRadiusR")
            controller.Stop()
            controller.WaitForStatus(ServiceControllerStatus.Stopped)
            ToolStripStatusLabel1.Text = "Status: " & "Radius server stopped"
            controller.Start()
            controller.WaitForStatus(ServiceControllerStatus.Running)
            ToolStripStatusLabel1.Text = "Status: " & "Radius server started"
            MsgBox("CICRadiusR Service restarted", MsgBoxStyle.Information, "Information")
        End If
        btnRestart.Enabled = True
    End Sub


    Private Sub lvConfig_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles lvConfig.SelectedIndexChanged
        Dim ele As ListViewItem
        If lvConfig.SelectedIndices.Count > 0 Then
            Select lvConfig.SelectedItems(0).Text.ToUpper

                Case "SETUP"
                    PanelSetup.Visible = True
                    PanelActiveDirectory.Visible = False
                    PanelSMSSetup.Visible = False
                    PanelMailSetup.Visible = False
                    PanelRadiusClients.Visible = False
                Case "ACTIVE DIRECTORY"
                    PanelSetup.Visible = False
                    PanelActiveDirectory.Visible = True
                    PanelSMSSetup.Visible = False
                    PanelMailSetup.Visible = False
                    PanelRadiusClients.Visible = False
                Case "SMS SETUP"
                    If ckEnableSMS.Checked = True Then
                        PanelSetup.Visible = False
                        PanelActiveDirectory.Visible = False
                        PanelSMSSetup.Visible = True
                        PanelMailSetup.Visible = False
                        PanelRadiusClients.Visible = False
                    End If
                Case "MAIL SETUP"
                    If ckEnableMail.Checked = True Then
                        PanelSetup.Visible = False
                        PanelActiveDirectory.Visible = False
                        PanelSMSSetup.Visible = False
                        PanelMailSetup.Visible = True
                        PanelRadiusClients.Visible = False
                    End If
                Case "RADIUS CLIENTS"
                    PanelSetup.Visible = False
                    PanelActiveDirectory.Visible = False
                    PanelSMSSetup.Visible = False
                    PanelMailSetup.Visible = False
                    PanelRadiusClients.Visible = True
                    Panel1.AutoScrollPosition = New Point(0, 130)
            End Select

        End If
    End Sub

    Private Sub rbRDGateway_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbRDGateway.CheckedChanged
        If rbRDGateway.Checked = True Then
            rbCitrixNetscaler.Checked = False
        End If
    End Sub

    Private Sub rbCitrixNetscaler_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbCitrixNetscaler.CheckedChanged
        If rbCitrixNetscaler.Checked = True Then
            rbRDGateway.Checked = False
        End If
    End Sub


 

    Private Sub rbOnlineSMS_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbOnlineSMS.CheckedChanged
        If rbOnlineSMS.Checked = True Then
            rbLocalSMS.Checked = False
            txtComPort.Enabled = False
            txtSMSC.Enabled = False
            txtProvider.Enabled = True
            txtSMSC.BackColor = Color.White
            txtComPort.BackColor = Color.White
        End If
    End Sub

    Private Sub rbLocalSMS_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbLocalSMS.CheckedChanged
        If rbLocalSMS.Checked = True Then
            rbOnlineSMS.Checked = False
            txtComPort.Enabled = True
            txtSMSC.Enabled = True
            txtProvider.Enabled = False
            txtProvider.BackColor = Color.White
        End If
    End Sub

   
    
    Private Sub btnTestModem_Click(sender As System.Object, e As System.EventArgs) Handles btnTestModem.Click
        Call TestModem()

    End Sub

 
    Private Sub ckEnableOTP_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles ckEnableOTP.CheckedChanged
        If ckEnableOTP.Checked = True Then
            ckEnableMail.Enabled = True
            ckEnableSMS.Enabled = True
            'If ckEnableSMS.Checked = True Then
            '    lvConfig.Items.Item(2).ImageIndex = 2
            'Else
            '    lvConfig.Items.Item(2).ImageIndex = 5
            'End If
            'If ckEnableMail.Checked = True Then
            '    lvConfig.Items.Item(3).ImageIndex = 3
            'Else
            '    lvConfig.Items.Item(3).ImageIndex = 6
            'End If

            'txtADField.Enabled = True
            'txtComPort.Enabled = True
            'txtSMSC.Enabled = True
            'If rbLocalSMS.Checked = True Then
            '    txtComPort.Enabled = True
            '    txtSMSC.Enabled = True
            '    txtProvider.Enabled = False

            'Else
            '    txtComPort.Enabled = False
            '    txtSMSC.Enabled = False
            '    txtProvider.Enabled = True

            'End If
            'btnTestModem.Enabled = True
        Else
            ckEnableMail.Enabled = False
            ckEnableSMS.Enabled = False
            lvConfig.Items.Item(2).ImageIndex = 5
            lvConfig.Items.Item(3).ImageIndex = 6
            'txtADField.Enabled = False
            'txtComPort.Enabled = False
            'txtSMSC.Enabled = False
            'txtProvider.Enabled = False
            'btnTestModem.Enabled = False
        End If
    End Sub

    Private Sub ckEnableSMS_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles ckEnableSMS.CheckedChanged
        If ckEnableSMS.Checked = True Then
            lvConfig.Items.Item(2).ImageIndex = 2
            TestModemConfigurationToolStripMenuItem.Enabled = True
        Else
            TestModemConfigurationToolStripMenuItem.Enabled = False
            lvConfig.Items.Item(2).ImageIndex = 5
        End If
    End Sub

    Private Sub ckEnableMail_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles ckEnableMail.CheckedChanged
        If ckEnableMail.Checked = True Then
            lvConfig.Items.Item(3).ImageIndex = 3
            TestMailConfigurationToolStripMenuItem.Enabled = True
        Else
            lvConfig.Items.Item(3).ImageIndex = 6
            TestMailConfigurationToolStripMenuItem.Enabled = False
        End If
    End Sub

    Private Sub SaveConfigurationToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles SaveConfigurationToolStripMenuItem.Click
        Call save()
    End Sub

    Private Sub RestartRadiusServerToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles RestartRadiusServerToolStripMenuItem.Click
        Dim ok As MsgBoxResult
        RestartRadiusServerToolStripMenuItem.Enabled = False
        ok = MsgBox("Restart CICRadiusR Service?", MsgBoxStyle.YesNo + MsgBoxStyle.Information, "Restart Service")
        If ok = vbYes Then

            Dim controller As New ServiceController("CICRadiusR")
            controller.Stop()
            controller.WaitForStatus(ServiceControllerStatus.Stopped)
            controller.Start()
            controller.WaitForStatus(ServiceControllerStatus.Running)
            MsgBox("CICRadiusR Service restarted", MsgBoxStyle.Information, "Information")
        End If
        RestartRadiusServerToolStripMenuItem.Enabled = True
    End Sub

    Sub Save()
        ConfigFile.SetKeyValue("CICRadarR", "NetBiosDomain", txtNetBios.Text)
        ConfigFile.SetKeyValue("CICRadarR", "LDAPDomain", txtLdapDomain.Text)
        ConfigFile.SetKeyValue("CICRadarR", "Provider", txtProvider.Text)
        ConfigFile.SetKeyValue("CICRadarR", "ADField", txtADField.Text)
        ConfigFile.SetKeyValue("CICRadarR", "ADMailField", txtADMailField.Text)

        ConfigFile.SetKeyValue("CICRadarR", "SenderEmail", txtSenderEmail.Text)
        ConfigFile.SetKeyValue("CICRadarR", "MailServer", txtMailServer.Text)

        If rbLocalSMS.Checked = True Then
            ConfigFile.SetKeyValue("CICRadarR", "USELOCALMODEM", "1")
        Else
            ConfigFile.SetKeyValue("CICRadarR", "USELOCALMODEM", "0")
        End If

        ConfigFile.SetKeyValue("CICRadarR", "COMPORT", txtComPort.Text)
        ConfigFile.SetKeyValue("CICRadarR", "SMSC", txtSMSC.Text)

        If ckDebug.Checked = True Then
            ConfigFile.SetKeyValue("CICRadarR", "Debug", "1")
        Else
            ConfigFile.SetKeyValue("CICRadarR", "Debug", "0")
        End If

        If ckEnableOTP.Checked = True Then
            ConfigFile.SetKeyValue("CICRadarR", "EnableOTP", "1")
        Else
            ConfigFile.SetKeyValue("CICRadarR", "EnableOTP", "0")
        End If

        If rbRDGateway.Checked = True Then
            ConfigFile.SetKeyValue("CICRadarR", "TSGW", "1")
        Else
            ConfigFile.SetKeyValue("CICRadarR", "TSGW", "0")
        End If

        If ckEnableMail.Checked = True Then
            ConfigFile.SetKeyValue("CICRadarR", "EnableEmail", "1")
        Else
            ConfigFile.SetKeyValue("CICRadarR", "EnableEmail", "0")
        End If

        If ckEnableSMS.Checked = True Then
            ConfigFile.SetKeyValue("CICRadarR", "EnableSMS", "1")
        Else
            ConfigFile.SetKeyValue("CICRadarR", "EnableSMS", "0")
        End If
        ConfigFile.RemoveSection("Clients")
        Dim ClientList As String = ""
        For i As Integer = 0 To ListClients.Items.Count - 1
            Dim Client As String
            Dim Secret As String
            Client = Split(ListClients.Items(i), " ( ")(0)
            Secret = Replace(Split(ListClients.Items(i), " ( ")(1), " )", "")
            ConfigFile.SetKeyValue("Clients", Client, Secret)

            If i = ListClients.Items.Count - 1 Then
                ClientList = ClientList & Client
            Else
                ClientList = ClientList & Client & ","
            End If

        Next
        ConfigFile.SetKeyValue("CICRadarR", "ClientList", ClientList)


        ConfigFile.Save(Application.StartupPath & "\CICRadarR.ini")
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ExitToolStripMenuItem.Click

        Select Case MsgBox("Save configuration before exit?", MsgBoxStyle.YesNoCancel + MsgBoxStyle.Information, "Configuration")
            Case vbYes
                Call Save()
                ToolStripStatusLabel1.Text = "Status: " & "Configuration saved"
                MsgBox("Configuration saved", vbOKOnly + MsgBoxStyle.Information, "Configuration")
                End
            Case vbNo
                End
            Case vbCancel
        End Select
       
    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        MsgBox("Configuration tool for CICRadar." & vbCrLf & vbCrLf & "Version 1.1", MsgBoxStyle.OkOnly + MsgBoxStyle.Information, "About")
    End Sub

    Sub TestModem()
        Dim number As String
        number = InputBox("Type the phone number to send the test sms to" & vbCrLf & vbCrLf & "Ex: +4512345678", "Phone Number", "", Me.Left + 150, Me.Top + 100)
        If rbLocalSMS.Checked = True Then
            Dim testsms As New SmsClass(txtComPort.Text)
            testsms.Opens()
            testsms.sendSms(number, "Test SMS Service", txtSMSC.Text)
            testsms.Closes()
        Else
            Dim baseurl As String = txtProvider.Text.Split("?")(0)
            Dim client As New System.Net.WebClient()
            ' Add a user agent header in case the requested URI contains a query.

            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR1.0.3705;)")

            Dim parameters As String = txtProvider.Text.Split("?")(1)
            Dim pary As String() = parameters.Split("&")

            For i As Integer = 0 To pary.Length - 1
                If pary(i).IndexOf("***TEXTMESSAGE***") > 0 Then
                    Dim qpar As String() = pary(i).Split("=")
                    client.QueryString.Add(qpar(0), "Test SMS Service")
                ElseIf pary(i).IndexOf("***NUMBER***") > 0 Then
                    Dim qpar As String() = pary(i).Split("=")
                    client.QueryString.Add(qpar(0), number)
                Else

                    Dim qpar As String() = pary(i).Split("=")
                    client.QueryString.Add(qpar(0), qpar(1))
                End If
            Next


            Dim data As Stream = client.OpenRead(baseurl)
            Dim reader As New StreamReader(data)
            Dim s As String = reader.ReadToEnd()
            data.Close()
            reader.Close()

        End If
    End Sub

    Sub TestEmail()
        Dim email As String
        email = InputBox("Type the email address to send the test email to" & vbCrLf & vbCrLf & "Ex: test@my.mail.com", "Email", "", Me.Left + 150, Me.Top + 100)
        Dim mail As New MailMessage()
        mail.To.Add(email)
        mail.From = New MailAddress(txtSenderEmail.Text)
        mail.Subject = "Test mail from CICRadar"
        mail.Body = "Just a test."
        mail.IsBodyHtml = False
        Dim smtp As New SmtpClient(txtMailServer.Text)


        Try
            smtp.Send(mail)
           
            ToolStripStatusLabel1.Text = "Status: Mail send"
        Catch e As InvalidCastException

            ToolStripStatusLabel1.Text = "Status: Failed to send mail"
        End Try


    End Sub

    Private Sub TestModemConfigurationToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles TestModemConfigurationToolStripMenuItem.Click
        Call TestModem()
    End Sub

    Private Sub TestMailConfigurationToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles TestMailConfigurationToolStripMenuItem.Click
        Call TestEmail()
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Call TestEmail()
    End Sub

   
    Private Sub btnRemoveClient_Click(sender As System.Object, e As System.EventArgs) Handles btnRemoveClient.Click
        If ListClients.SelectedIndex > -1 Then

            ListClients.Items.RemoveAt(ListClients.SelectedIndex)

        End If
    End Sub

    Private Sub btnAddClient_Click(sender As System.Object, e As System.EventArgs) Handles btnAddClient.Click
        If Not ListClients.Items.Contains(txtClient.Text & " ( " & EncDec.Encrypt(txtSecret.Text, encCode) & " )") Then
            ListClients.Items.Add(txtClient.Text & " ( " & EncDec.Encrypt(txtSecret.Text, encCode) & " )")
        End If
    End Sub
End Class
