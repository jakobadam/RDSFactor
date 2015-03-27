<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CICRadiusRConfig
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CICRadiusRConfig))
        Dim ListViewItem1 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem("Setup", 0)
        Dim ListViewItem2 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem("Active Directory", 1)
        Dim ListViewItem3 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem("SMS Setup", 2)
        Dim ListViewItem4 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem("Mail Setup", 3)
        Dim ListViewItem5 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem("Radius Clients", 4)
        Me.btnRestart = New System.Windows.Forms.Button()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.lvConfig = New System.Windows.Forms.ListView()
        Me.MenuBar = New System.Windows.Forms.MenuStrip()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveConfigurationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RestartRadiusServerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TestModemConfigurationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TestMailConfigurationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusBar = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.MenuLabel = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.PanelSetup = New System.Windows.Forms.Panel()
        Me.TextBox3 = New System.Windows.Forms.TextBox()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.rbCitrixNetscaler = New System.Windows.Forms.RadioButton()
        Me.rbRDGateway = New System.Windows.Forms.RadioButton()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.ckEnableMail = New System.Windows.Forms.CheckBox()
        Me.ckEnableSMS = New System.Windows.Forms.CheckBox()
        Me.ckEnableOTP = New System.Windows.Forms.CheckBox()
        Me.ckDebug = New System.Windows.Forms.CheckBox()
        Me.PanelActiveDirectory = New System.Windows.Forms.Panel()
        Me.txtADMailField = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.TextBox5 = New System.Windows.Forms.TextBox()
        Me.TextBox4 = New System.Windows.Forms.TextBox()
        Me.txtADField = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtLdapDomain = New System.Windows.Forms.TextBox()
        Me.txtNetBios = New System.Windows.Forms.TextBox()
        Me.lblLdapDomain = New System.Windows.Forms.Label()
        Me.lblNetBios = New System.Windows.Forms.Label()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.PanelSMSSetup = New System.Windows.Forms.Panel()
        Me.TextBox9 = New System.Windows.Forms.TextBox()
        Me.TextBox8 = New System.Windows.Forms.TextBox()
        Me.rbLocalSMS = New System.Windows.Forms.RadioButton()
        Me.rbOnlineSMS = New System.Windows.Forms.RadioButton()
        Me.TextBox7 = New System.Windows.Forms.TextBox()
        Me.btnTestModem = New System.Windows.Forms.Button()
        Me.txtSMSC = New System.Windows.Forms.TextBox()
        Me.txtComPort = New System.Windows.Forms.TextBox()
        Me.lblSMSC = New System.Windows.Forms.Label()
        Me.lblComPort = New System.Windows.Forms.Label()
        Me.txtProvider = New System.Windows.Forms.TextBox()
        Me.lblProvider = New System.Windows.Forms.Label()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.PanelRadiusClients = New System.Windows.Forms.Panel()
        Me.TextBox10 = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtSecret = New System.Windows.Forms.TextBox()
        Me.lblSecret = New System.Windows.Forms.Label()
        Me.btnRemoveClient = New System.Windows.Forms.Button()
        Me.ListClients = New System.Windows.Forms.ListBox()
        Me.btnAddClient = New System.Windows.Forms.Button()
        Me.txtClient = New System.Windows.Forms.TextBox()
        Me.lblClient = New System.Windows.Forms.Label()
        Me.Panel7 = New System.Windows.Forms.Panel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.PanelMailSetup = New System.Windows.Forms.Panel()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.TextBox13 = New System.Windows.Forms.TextBox()
        Me.txtSenderEmail = New System.Windows.Forms.TextBox()
        Me.txtMailServer = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.MenuBar.SuspendLayout()
        Me.StatusBar.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.PanelSetup.SuspendLayout()
        Me.PanelActiveDirectory.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.PanelSMSSetup.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.PanelRadiusClients.SuspendLayout()
        Me.Panel7.SuspendLayout()
        Me.PanelMailSetup.SuspendLayout()
        Me.Panel6.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnRestart
        '
        Me.btnRestart.Location = New System.Drawing.Point(15, 475)
        Me.btnRestart.Name = "btnRestart"
        Me.btnRestart.Size = New System.Drawing.Size(86, 23)
        Me.btnRestart.TabIndex = 16
        Me.btnRestart.Text = "Restart Radius"
        Me.btnRestart.UseVisualStyleBackColor = True
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "gear.png")
        Me.ImageList1.Images.SetKeyName(1, "branch_element.png")
        Me.ImageList1.Images.SetKeyName(2, "mobilephone3.png")
        Me.ImageList1.Images.SetKeyName(3, "mail.png")
        Me.ImageList1.Images.SetKeyName(4, "server_id_card.png")
        Me.ImageList1.Images.SetKeyName(5, "mobilephone3_gray.png")
        Me.ImageList1.Images.SetKeyName(6, "mail_gray.png")
        '
        'lvConfig
        '
        Me.lvConfig.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.lvConfig.Items.AddRange(New System.Windows.Forms.ListViewItem() {ListViewItem1, ListViewItem2, ListViewItem3, ListViewItem4, ListViewItem5})
        Me.lvConfig.LargeImageList = Me.ImageList1
        Me.lvConfig.Location = New System.Drawing.Point(8, 13)
        Me.lvConfig.MultiSelect = False
        Me.lvConfig.Name = "lvConfig"
        Me.lvConfig.Size = New System.Drawing.Size(97, 500)
        Me.lvConfig.TabIndex = 28
        Me.lvConfig.UseCompatibleStateImageBehavior = False
        '
        'MenuBar
        '
        Me.MenuBar.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem1, Me.FileToolStripMenuItem, Me.HelpToolStripMenuItem})
        Me.MenuBar.Location = New System.Drawing.Point(0, 0)
        Me.MenuBar.Name = "MenuBar"
        Me.MenuBar.Size = New System.Drawing.Size(1775, 24)
        Me.MenuBar.TabIndex = 29
        Me.MenuBar.Text = "MenuBar"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SaveConfigurationToolStripMenuItem, Me.ExitToolStripMenuItem})
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(35, 20)
        Me.ToolStripMenuItem1.Text = "File"
        '
        'SaveConfigurationToolStripMenuItem
        '
        Me.SaveConfigurationToolStripMenuItem.Name = "SaveConfigurationToolStripMenuItem"
        Me.SaveConfigurationToolStripMenuItem.Size = New System.Drawing.Size(164, 22)
        Me.SaveConfigurationToolStripMenuItem.Text = "Save configuration"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(164, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RestartRadiusServerToolStripMenuItem, Me.TestModemConfigurationToolStripMenuItem, Me.TestMailConfigurationToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.FileToolStripMenuItem.Text = "Tools"
        '
        'RestartRadiusServerToolStripMenuItem
        '
        Me.RestartRadiusServerToolStripMenuItem.Name = "RestartRadiusServerToolStripMenuItem"
        Me.RestartRadiusServerToolStripMenuItem.Size = New System.Drawing.Size(198, 22)
        Me.RestartRadiusServerToolStripMenuItem.Text = "Restart radius server"
        '
        'TestModemConfigurationToolStripMenuItem
        '
        Me.TestModemConfigurationToolStripMenuItem.Name = "TestModemConfigurationToolStripMenuItem"
        Me.TestModemConfigurationToolStripMenuItem.Size = New System.Drawing.Size(198, 22)
        Me.TestModemConfigurationToolStripMenuItem.Text = "Test modem configuration"
        '
        'TestMailConfigurationToolStripMenuItem
        '
        Me.TestMailConfigurationToolStripMenuItem.Name = "TestMailConfigurationToolStripMenuItem"
        Me.TestMailConfigurationToolStripMenuItem.Size = New System.Drawing.Size(198, 22)
        Me.TestMailConfigurationToolStripMenuItem.Text = "Test mail configuration"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutToolStripMenuItem})
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(40, 20)
        Me.HelpToolStripMenuItem.Text = "Help"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(103, 22)
        Me.AboutToolStripMenuItem.Text = "About"
        '
        'StatusBar
        '
        Me.StatusBar.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1})
        Me.StatusBar.Location = New System.Drawing.Point(0, 851)
        Me.StatusBar.Name = "StatusBar"
        Me.StatusBar.Size = New System.Drawing.Size(1775, 22)
        Me.StatusBar.SizingGrip = False
        Me.StatusBar.TabIndex = 30
        Me.StatusBar.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(45, 17)
        Me.ToolStripStatusLabel1.Text = "Status: "
        '
        'Panel1
        '
        Me.Panel1.AutoScroll = True
        Me.Panel1.BackColor = System.Drawing.SystemColors.ButtonHighlight
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel1.Controls.Add(Me.lvConfig)
        Me.Panel1.Location = New System.Drawing.Point(5, 27)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(125, 357)
        Me.Panel1.TabIndex = 31
        '
        'MenuLabel
        '
        Me.MenuLabel.AutoSize = True
        Me.MenuLabel.BackColor = System.Drawing.Color.SteelBlue
        Me.MenuLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MenuLabel.ForeColor = System.Drawing.Color.White
        Me.MenuLabel.Location = New System.Drawing.Point(12, 1)
        Me.MenuLabel.Name = "MenuLabel"
        Me.MenuLabel.Size = New System.Drawing.Size(73, 25)
        Me.MenuLabel.TabIndex = 32
        Me.MenuLabel.Text = "Setup"
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.SteelBlue
        Me.Panel2.Controls.Add(Me.MenuLabel)
        Me.Panel2.Location = New System.Drawing.Point(-2, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(474, 32)
        Me.Panel2.TabIndex = 33
        '
        'PanelSetup
        '
        Me.PanelSetup.BackColor = System.Drawing.Color.White
        Me.PanelSetup.Controls.Add(Me.TextBox3)
        Me.PanelSetup.Controls.Add(Me.TextBox2)
        Me.PanelSetup.Controls.Add(Me.rbCitrixNetscaler)
        Me.PanelSetup.Controls.Add(Me.rbRDGateway)
        Me.PanelSetup.Controls.Add(Me.TextBox1)
        Me.PanelSetup.Controls.Add(Me.Label6)
        Me.PanelSetup.Controls.Add(Me.ckEnableMail)
        Me.PanelSetup.Controls.Add(Me.ckEnableSMS)
        Me.PanelSetup.Controls.Add(Me.Panel2)
        Me.PanelSetup.Controls.Add(Me.ckEnableOTP)
        Me.PanelSetup.Controls.Add(Me.ckDebug)
        Me.PanelSetup.Location = New System.Drawing.Point(136, 27)
        Me.PanelSetup.Name = "PanelSetup"
        Me.PanelSetup.Size = New System.Drawing.Size(474, 357)
        Me.PanelSetup.TabIndex = 34
        '
        'TextBox3
        '
        Me.TextBox3.BackColor = System.Drawing.Color.White
        Me.TextBox3.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox3.Location = New System.Drawing.Point(25, 249)
        Me.TextBox3.Multiline = True
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.ReadOnly = True
        Me.TextBox3.Size = New System.Drawing.Size(426, 32)
        Me.TextBox3.TabIndex = 41
        Me.TextBox3.Text = "To troubleshoot configuration errors enable debugging. This will record addition " & _
    "information to the logs."
        '
        'TextBox2
        '
        Me.TextBox2.BackColor = System.Drawing.Color.White
        Me.TextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox2.Location = New System.Drawing.Point(25, 127)
        Me.TextBox2.Multiline = True
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.ReadOnly = True
        Me.TextBox2.Size = New System.Drawing.Size(426, 31)
        Me.TextBox2.TabIndex = 40
        Me.TextBox2.Text = "Two factor authentication can be enable by sending a one-time password as an emai" & _
    "l or SMS message."
        '
        'rbCitrixNetscaler
        '
        Me.rbCitrixNetscaler.AutoSize = True
        Me.rbCitrixNetscaler.Location = New System.Drawing.Point(25, 95)
        Me.rbCitrixNetscaler.Name = "rbCitrixNetscaler"
        Me.rbCitrixNetscaler.Size = New System.Drawing.Size(95, 17)
        Me.rbCitrixNetscaler.TabIndex = 39
        Me.rbCitrixNetscaler.Text = "Citrix Netscaler"
        Me.rbCitrixNetscaler.UseVisualStyleBackColor = True
        '
        'rbRDGateway
        '
        Me.rbRDGateway.AutoSize = True
        Me.rbRDGateway.Checked = True
        Me.rbRDGateway.Location = New System.Drawing.Point(25, 77)
        Me.rbRDGateway.Name = "rbRDGateway"
        Me.rbRDGateway.Size = New System.Drawing.Size(150, 17)
        Me.rbRDGateway.TabIndex = 38
        Me.rbRDGateway.TabStop = True
        Me.rbRDGateway.Text = "Remote Desktop Gateway"
        Me.rbRDGateway.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.BackColor = System.Drawing.Color.White
        Me.TextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox1.Location = New System.Drawing.Point(25, 40)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ReadOnly = True
        Me.TextBox1.Size = New System.Drawing.Size(427, 56)
        Me.TextBox1.TabIndex = 37
        Me.TextBox1.Text = "This page configures the CIC Radar to support either Remote Desktop Gateway or Ci" & _
    "trix Netscaler.  "
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(-2, 51)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(0, 13)
        Me.Label6.TabIndex = 36
        '
        'ckEnableMail
        '
        Me.ckEnableMail.AutoSize = True
        Me.ckEnableMail.Location = New System.Drawing.Point(47, 212)
        Me.ckEnableMail.Name = "ckEnableMail"
        Me.ckEnableMail.Size = New System.Drawing.Size(79, 17)
        Me.ckEnableMail.TabIndex = 35
        Me.ckEnableMail.Text = "Mail Token"
        Me.ckEnableMail.UseVisualStyleBackColor = True
        '
        'ckEnableSMS
        '
        Me.ckEnableSMS.AutoSize = True
        Me.ckEnableSMS.Location = New System.Drawing.Point(47, 190)
        Me.ckEnableSMS.Name = "ckEnableSMS"
        Me.ckEnableSMS.Size = New System.Drawing.Size(83, 17)
        Me.ckEnableSMS.TabIndex = 34
        Me.ckEnableSMS.Text = "SMS Token"
        Me.ckEnableSMS.UseVisualStyleBackColor = True
        '
        'ckEnableOTP
        '
        Me.ckEnableOTP.AutoSize = True
        Me.ckEnableOTP.Location = New System.Drawing.Point(25, 164)
        Me.ckEnableOTP.Name = "ckEnableOTP"
        Me.ckEnableOTP.Size = New System.Drawing.Size(187, 17)
        Me.ckEnableOTP.TabIndex = 10
        Me.ckEnableOTP.Text = "Enable Two Factor Authentication"
        Me.ckEnableOTP.UseVisualStyleBackColor = True
        '
        'ckDebug
        '
        Me.ckDebug.AutoSize = True
        Me.ckDebug.Location = New System.Drawing.Point(25, 287)
        Me.ckDebug.Name = "ckDebug"
        Me.ckDebug.Size = New System.Drawing.Size(149, 17)
        Me.ckDebug.TabIndex = 8
        Me.ckDebug.Text = "Enable Debug Information"
        Me.ckDebug.UseVisualStyleBackColor = True
        '
        'PanelActiveDirectory
        '
        Me.PanelActiveDirectory.BackColor = System.Drawing.Color.White
        Me.PanelActiveDirectory.Controls.Add(Me.txtADMailField)
        Me.PanelActiveDirectory.Controls.Add(Me.Label8)
        Me.PanelActiveDirectory.Controls.Add(Me.TextBox5)
        Me.PanelActiveDirectory.Controls.Add(Me.TextBox4)
        Me.PanelActiveDirectory.Controls.Add(Me.txtADField)
        Me.PanelActiveDirectory.Controls.Add(Me.Label1)
        Me.PanelActiveDirectory.Controls.Add(Me.txtLdapDomain)
        Me.PanelActiveDirectory.Controls.Add(Me.txtNetBios)
        Me.PanelActiveDirectory.Controls.Add(Me.lblLdapDomain)
        Me.PanelActiveDirectory.Controls.Add(Me.lblNetBios)
        Me.PanelActiveDirectory.Controls.Add(Me.Panel4)
        Me.PanelActiveDirectory.Location = New System.Drawing.Point(136, 403)
        Me.PanelActiveDirectory.Name = "PanelActiveDirectory"
        Me.PanelActiveDirectory.Size = New System.Drawing.Size(474, 357)
        Me.PanelActiveDirectory.TabIndex = 35
        Me.PanelActiveDirectory.Visible = False
        '
        'txtADMailField
        '
        Me.txtADMailField.Location = New System.Drawing.Point(115, 208)
        Me.txtADMailField.Name = "txtADMailField"
        Me.txtADMailField.Size = New System.Drawing.Size(152, 20)
        Me.txtADMailField.TabIndex = 43
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(25, 214)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(51, 13)
        Me.Label8.TabIndex = 42
        Me.Label8.Text = "Mail Field"
        '
        'TextBox5
        '
        Me.TextBox5.BackColor = System.Drawing.Color.White
        Me.TextBox5.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox5.Location = New System.Drawing.Point(25, 137)
        Me.TextBox5.Multiline = True
        Me.TextBox5.Name = "TextBox5"
        Me.TextBox5.ReadOnly = True
        Me.TextBox5.Size = New System.Drawing.Size(426, 33)
        Me.TextBox5.TabIndex = 41
        Me.TextBox5.Text = "Different field in Active Directory can be used for email and phone number. Use A" & _
    "dsiedit to find the desired field name if deviating for the default configuratio" & _
    "n."
        '
        'TextBox4
        '
        Me.TextBox4.BackColor = System.Drawing.Color.White
        Me.TextBox4.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox4.Location = New System.Drawing.Point(25, 40)
        Me.TextBox4.Multiline = True
        Me.TextBox4.Name = "TextBox4"
        Me.TextBox4.ReadOnly = True
        Me.TextBox4.Size = New System.Drawing.Size(426, 34)
        Me.TextBox4.TabIndex = 40
        Me.TextBox4.Text = "To authenticate users specify the FQDN and Netbios name of the domain where the u" & _
    "sers reside."
        '
        'txtADField
        '
        Me.txtADField.Location = New System.Drawing.Point(115, 179)
        Me.txtADField.Name = "txtADField"
        Me.txtADField.Size = New System.Drawing.Size(152, 20)
        Me.txtADField.TabIndex = 34
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(25, 185)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(63, 13)
        Me.Label1.TabIndex = 39
        Me.Label1.Text = "Phone Field"
        '
        'txtLdapDomain
        '
        Me.txtLdapDomain.Location = New System.Drawing.Point(115, 77)
        Me.txtLdapDomain.Name = "txtLdapDomain"
        Me.txtLdapDomain.Size = New System.Drawing.Size(152, 20)
        Me.txtLdapDomain.TabIndex = 36
        '
        'txtNetBios
        '
        Me.txtNetBios.Location = New System.Drawing.Point(115, 103)
        Me.txtNetBios.Name = "txtNetBios"
        Me.txtNetBios.Size = New System.Drawing.Size(152, 20)
        Me.txtNetBios.TabIndex = 35
        '
        'lblLdapDomain
        '
        Me.lblLdapDomain.AutoSize = True
        Me.lblLdapDomain.Location = New System.Drawing.Point(25, 80)
        Me.lblLdapDomain.Name = "lblLdapDomain"
        Me.lblLdapDomain.Size = New System.Drawing.Size(74, 13)
        Me.lblLdapDomain.TabIndex = 38
        Me.lblLdapDomain.Text = "Domain Name"
        '
        'lblNetBios
        '
        Me.lblNetBios.AutoSize = True
        Me.lblNetBios.Location = New System.Drawing.Point(25, 106)
        Me.lblNetBios.Name = "lblNetBios"
        Me.lblNetBios.Size = New System.Drawing.Size(83, 13)
        Me.lblNetBios.TabIndex = 37
        Me.lblNetBios.Text = "NetBios Domain"
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.SteelBlue
        Me.Panel4.Controls.Add(Me.Label3)
        Me.Panel4.Location = New System.Drawing.Point(-2, 0)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(474, 32)
        Me.Panel4.TabIndex = 33
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.SteelBlue
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(12, 1)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(179, 25)
        Me.Label3.TabIndex = 32
        Me.Label3.Text = "Active Directory"
        '
        'PanelSMSSetup
        '
        Me.PanelSMSSetup.BackColor = System.Drawing.Color.White
        Me.PanelSMSSetup.Controls.Add(Me.TextBox9)
        Me.PanelSMSSetup.Controls.Add(Me.TextBox8)
        Me.PanelSMSSetup.Controls.Add(Me.rbLocalSMS)
        Me.PanelSMSSetup.Controls.Add(Me.rbOnlineSMS)
        Me.PanelSMSSetup.Controls.Add(Me.TextBox7)
        Me.PanelSMSSetup.Controls.Add(Me.btnTestModem)
        Me.PanelSMSSetup.Controls.Add(Me.txtSMSC)
        Me.PanelSMSSetup.Controls.Add(Me.txtComPort)
        Me.PanelSMSSetup.Controls.Add(Me.lblSMSC)
        Me.PanelSMSSetup.Controls.Add(Me.lblComPort)
        Me.PanelSMSSetup.Controls.Add(Me.txtProvider)
        Me.PanelSMSSetup.Controls.Add(Me.lblProvider)
        Me.PanelSMSSetup.Controls.Add(Me.Panel5)
        Me.PanelSMSSetup.Location = New System.Drawing.Point(666, 29)
        Me.PanelSMSSetup.Name = "PanelSMSSetup"
        Me.PanelSMSSetup.Size = New System.Drawing.Size(474, 357)
        Me.PanelSMSSetup.TabIndex = 36
        Me.PanelSMSSetup.Visible = False
        '
        'TextBox9
        '
        Me.TextBox9.BackColor = System.Drawing.Color.White
        Me.TextBox9.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox9.Location = New System.Drawing.Point(25, 225)
        Me.TextBox9.Multiline = True
        Me.TextBox9.Name = "TextBox9"
        Me.TextBox9.ReadOnly = True
        Me.TextBox9.Size = New System.Drawing.Size(423, 29)
        Me.TextBox9.TabIndex = 46
        Me.TextBox9.Text = "If using a locally attached modem be sure to get the correct SMSC number for your" & _
    " Telco. See http://smsclist.com/downloads/default.txt."
        '
        'TextBox8
        '
        Me.TextBox8.BackColor = System.Drawing.Color.White
        Me.TextBox8.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox8.Location = New System.Drawing.Point(25, 114)
        Me.TextBox8.Multiline = True
        Me.TextBox8.Name = "TextBox8"
        Me.TextBox8.ReadOnly = True
        Me.TextBox8.Size = New System.Drawing.Size(423, 39)
        Me.TextBox8.TabIndex = 45
        Me.TextBox8.Text = "If using an online provider be sure to replace the parameter for phone number and" & _
    " message with ***NUMBER*** and ***TEXTMESSAGE***."
        '
        'rbLocalSMS
        '
        Me.rbLocalSMS.AutoSize = True
        Me.rbLocalSMS.Location = New System.Drawing.Point(25, 84)
        Me.rbLocalSMS.Name = "rbLocalSMS"
        Me.rbLocalSMS.Size = New System.Drawing.Size(115, 17)
        Me.rbLocalSMS.TabIndex = 44
        Me.rbLocalSMS.Text = "Local SMS Modem"
        Me.rbLocalSMS.UseVisualStyleBackColor = True
        '
        'rbOnlineSMS
        '
        Me.rbOnlineSMS.AutoSize = True
        Me.rbOnlineSMS.Checked = True
        Me.rbOnlineSMS.Location = New System.Drawing.Point(25, 65)
        Me.rbOnlineSMS.Name = "rbOnlineSMS"
        Me.rbOnlineSMS.Size = New System.Drawing.Size(123, 17)
        Me.rbOnlineSMS.TabIndex = 43
        Me.rbOnlineSMS.TabStop = True
        Me.rbOnlineSMS.Text = "Online SMS Provider"
        Me.rbOnlineSMS.UseVisualStyleBackColor = True
        '
        'TextBox7
        '
        Me.TextBox7.BackColor = System.Drawing.Color.White
        Me.TextBox7.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox7.Location = New System.Drawing.Point(25, 40)
        Me.TextBox7.Multiline = True
        Me.TextBox7.Name = "TextBox7"
        Me.TextBox7.ReadOnly = True
        Me.TextBox7.Size = New System.Drawing.Size(423, 24)
        Me.TextBox7.TabIndex = 42
        Me.TextBox7.Text = "SMS token can be send by either an online SMS provider or a locally attached mode" & _
    "m.  "
        '
        'btnTestModem
        '
        Me.btnTestModem.BackColor = System.Drawing.Color.SteelBlue
        Me.btnTestModem.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnTestModem.ForeColor = System.Drawing.Color.White
        Me.btnTestModem.Location = New System.Drawing.Point(25, 320)
        Me.btnTestModem.Name = "btnTestModem"
        Me.btnTestModem.Size = New System.Drawing.Size(120, 23)
        Me.btnTestModem.TabIndex = 37
        Me.btnTestModem.Text = "Test SMS Setup"
        Me.btnTestModem.UseVisualStyleBackColor = False
        '
        'txtSMSC
        '
        Me.txtSMSC.Location = New System.Drawing.Point(143, 287)
        Me.txtSMSC.Name = "txtSMSC"
        Me.txtSMSC.Size = New System.Drawing.Size(130, 20)
        Me.txtSMSC.TabIndex = 36
        '
        'txtComPort
        '
        Me.txtComPort.Location = New System.Drawing.Point(143, 260)
        Me.txtComPort.Name = "txtComPort"
        Me.txtComPort.Size = New System.Drawing.Size(130, 20)
        Me.txtComPort.TabIndex = 35
        '
        'lblSMSC
        '
        Me.lblSMSC.AutoSize = True
        Me.lblSMSC.Location = New System.Drawing.Point(25, 287)
        Me.lblSMSC.Name = "lblSMSC"
        Me.lblSMSC.Size = New System.Drawing.Size(37, 13)
        Me.lblSMSC.TabIndex = 40
        Me.lblSMSC.Text = "SMSC"
        '
        'lblComPort
        '
        Me.lblComPort.AutoSize = True
        Me.lblComPort.Location = New System.Drawing.Point(25, 264)
        Me.lblComPort.Name = "lblComPort"
        Me.lblComPort.Size = New System.Drawing.Size(53, 13)
        Me.lblComPort.TabIndex = 39
        Me.lblComPort.Text = "COM Port"
        '
        'txtProvider
        '
        Me.txtProvider.AccessibleDescription = ""
        Me.txtProvider.Location = New System.Drawing.Point(143, 152)
        Me.txtProvider.Multiline = True
        Me.txtProvider.Name = "txtProvider"
        Me.txtProvider.Size = New System.Drawing.Size(305, 65)
        Me.txtProvider.TabIndex = 34
        Me.txtProvider.Tag = ""
        '
        'lblProvider
        '
        Me.lblProvider.AutoSize = True
        Me.lblProvider.Location = New System.Drawing.Point(25, 152)
        Me.lblProvider.Name = "lblProvider"
        Me.lblProvider.Size = New System.Drawing.Size(105, 13)
        Me.lblProvider.TabIndex = 38
        Me.lblProvider.Text = "Online SMS Provider"
        '
        'Panel5
        '
        Me.Panel5.BackColor = System.Drawing.Color.SteelBlue
        Me.Panel5.Controls.Add(Me.Label7)
        Me.Panel5.Location = New System.Drawing.Point(-2, 0)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(474, 32)
        Me.Panel5.TabIndex = 33
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.SteelBlue
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.ForeColor = System.Drawing.Color.White
        Me.Label7.Location = New System.Drawing.Point(12, 1)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(129, 25)
        Me.Label7.TabIndex = 32
        Me.Label7.Text = "SMS Setup"
        '
        'PanelRadiusClients
        '
        Me.PanelRadiusClients.BackColor = System.Drawing.Color.White
        Me.PanelRadiusClients.Controls.Add(Me.TextBox10)
        Me.PanelRadiusClients.Controls.Add(Me.Label2)
        Me.PanelRadiusClients.Controls.Add(Me.txtSecret)
        Me.PanelRadiusClients.Controls.Add(Me.lblSecret)
        Me.PanelRadiusClients.Controls.Add(Me.btnRemoveClient)
        Me.PanelRadiusClients.Controls.Add(Me.ListClients)
        Me.PanelRadiusClients.Controls.Add(Me.btnAddClient)
        Me.PanelRadiusClients.Controls.Add(Me.txtClient)
        Me.PanelRadiusClients.Controls.Add(Me.lblClient)
        Me.PanelRadiusClients.Controls.Add(Me.Panel7)
        Me.PanelRadiusClients.Location = New System.Drawing.Point(1164, 31)
        Me.PanelRadiusClients.Name = "PanelRadiusClients"
        Me.PanelRadiusClients.Size = New System.Drawing.Size(474, 357)
        Me.PanelRadiusClients.TabIndex = 37
        Me.PanelRadiusClients.Visible = False
        '
        'TextBox10
        '
        Me.TextBox10.BackColor = System.Drawing.Color.White
        Me.TextBox10.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox10.Location = New System.Drawing.Point(25, 40)
        Me.TextBox10.Multiline = True
        Me.TextBox10.Name = "TextBox10"
        Me.TextBox10.ReadOnly = True
        Me.TextBox10.Size = New System.Drawing.Size(439, 20)
        Me.TextBox10.TabIndex = 42
        Me.TextBox10.Text = "This page configures the radius clients allowed to authenticate against this radi" & _
    "us server."
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(25, 173)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(89, 13)
        Me.Label2.TabIndex = 41
        Me.Label2.Text = "Current Client List"
        '
        'txtSecret
        '
        Me.txtSecret.Location = New System.Drawing.Point(132, 93)
        Me.txtSecret.Name = "txtSecret"
        Me.txtSecret.Size = New System.Drawing.Size(100, 20)
        Me.txtSecret.TabIndex = 35
        '
        'lblSecret
        '
        Me.lblSecret.AutoSize = True
        Me.lblSecret.Location = New System.Drawing.Point(25, 98)
        Me.lblSecret.Name = "lblSecret"
        Me.lblSecret.Size = New System.Drawing.Size(74, 13)
        Me.lblSecret.TabIndex = 40
        Me.lblSecret.Text = "Radius Secret"
        '
        'btnRemoveClient
        '
        Me.btnRemoveClient.BackColor = System.Drawing.Color.SteelBlue
        Me.btnRemoveClient.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRemoveClient.ForeColor = System.Drawing.Color.White
        Me.btnRemoveClient.Location = New System.Drawing.Point(132, 279)
        Me.btnRemoveClient.Name = "btnRemoveClient"
        Me.btnRemoveClient.Size = New System.Drawing.Size(100, 23)
        Me.btnRemoveClient.TabIndex = 38
        Me.btnRemoveClient.Text = "Remove Client"
        Me.btnRemoveClient.UseVisualStyleBackColor = False
        '
        'ListClients
        '
        Me.ListClients.FormattingEnabled = True
        Me.ListClients.Location = New System.Drawing.Point(132, 173)
        Me.ListClients.Name = "ListClients"
        Me.ListClients.Size = New System.Drawing.Size(306, 95)
        Me.ListClients.TabIndex = 37
        '
        'btnAddClient
        '
        Me.btnAddClient.BackColor = System.Drawing.Color.SteelBlue
        Me.btnAddClient.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAddClient.ForeColor = System.Drawing.Color.White
        Me.btnAddClient.Location = New System.Drawing.Point(132, 121)
        Me.btnAddClient.Name = "btnAddClient"
        Me.btnAddClient.Size = New System.Drawing.Size(100, 23)
        Me.btnAddClient.TabIndex = 36
        Me.btnAddClient.Text = "Add Client"
        Me.btnAddClient.UseVisualStyleBackColor = False
        '
        'txtClient
        '
        Me.txtClient.Location = New System.Drawing.Point(132, 66)
        Me.txtClient.Name = "txtClient"
        Me.txtClient.Size = New System.Drawing.Size(100, 20)
        Me.txtClient.TabIndex = 34
        '
        'lblClient
        '
        Me.lblClient.AutoSize = True
        Me.lblClient.Location = New System.Drawing.Point(25, 70)
        Me.lblClient.Name = "lblClient"
        Me.lblClient.Size = New System.Drawing.Size(94, 13)
        Me.lblClient.TabIndex = 39
        Me.lblClient.Text = "New Radius Client"
        '
        'Panel7
        '
        Me.Panel7.BackColor = System.Drawing.Color.SteelBlue
        Me.Panel7.Controls.Add(Me.Label4)
        Me.Panel7.Location = New System.Drawing.Point(-2, 0)
        Me.Panel7.Name = "Panel7"
        Me.Panel7.Size = New System.Drawing.Size(474, 32)
        Me.Panel7.TabIndex = 33
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.SteelBlue
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(12, 1)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(165, 25)
        Me.Label4.TabIndex = 32
        Me.Label4.Text = "Radius Clients"
        '
        'PanelMailSetup
        '
        Me.PanelMailSetup.BackColor = System.Drawing.Color.White
        Me.PanelMailSetup.Controls.Add(Me.Button1)
        Me.PanelMailSetup.Controls.Add(Me.TextBox13)
        Me.PanelMailSetup.Controls.Add(Me.txtSenderEmail)
        Me.PanelMailSetup.Controls.Add(Me.txtMailServer)
        Me.PanelMailSetup.Controls.Add(Me.Label10)
        Me.PanelMailSetup.Controls.Add(Me.Label9)
        Me.PanelMailSetup.Controls.Add(Me.Panel6)
        Me.PanelMailSetup.Location = New System.Drawing.Point(683, 428)
        Me.PanelMailSetup.Name = "PanelMailSetup"
        Me.PanelMailSetup.Size = New System.Drawing.Size(474, 357)
        Me.PanelMailSetup.TabIndex = 38
        Me.PanelMailSetup.Visible = False
        '
        'Button1
        '
        Me.Button1.BackColor = System.Drawing.Color.SteelBlue
        Me.Button1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.ForeColor = System.Drawing.Color.White
        Me.Button1.Location = New System.Drawing.Point(25, 136)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(120, 23)
        Me.Button1.TabIndex = 42
        Me.Button1.Text = "Test Email Setup"
        Me.Button1.UseVisualStyleBackColor = False
        '
        'TextBox13
        '
        Me.TextBox13.BackColor = System.Drawing.Color.White
        Me.TextBox13.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox13.Location = New System.Drawing.Point(25, 40)
        Me.TextBox13.Multiline = True
        Me.TextBox13.Name = "TextBox13"
        Me.TextBox13.ReadOnly = True
        Me.TextBox13.Size = New System.Drawing.Size(426, 34)
        Me.TextBox13.TabIndex = 41
        Me.TextBox13.Text = "Sending the access token as an email requires a mail server and an email address " & _
    "from which the mail is send. Be sure to allow the radius server to relay through" & _
    " your mail server."
        '
        'txtSenderEmail
        '
        Me.txtSenderEmail.Location = New System.Drawing.Point(115, 103)
        Me.txtSenderEmail.Name = "txtSenderEmail"
        Me.txtSenderEmail.Size = New System.Drawing.Size(152, 20)
        Me.txtSenderEmail.TabIndex = 37
        '
        'txtMailServer
        '
        Me.txtMailServer.Location = New System.Drawing.Point(115, 77)
        Me.txtMailServer.Name = "txtMailServer"
        Me.txtMailServer.Size = New System.Drawing.Size(152, 20)
        Me.txtMailServer.TabIndex = 36
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(25, 105)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(61, 13)
        Me.Label10.TabIndex = 35
        Me.Label10.Text = "Reply email"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(25, 78)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(60, 13)
        Me.Label9.TabIndex = 34
        Me.Label9.Text = "Mail Server"
        '
        'Panel6
        '
        Me.Panel6.BackColor = System.Drawing.Color.SteelBlue
        Me.Panel6.Controls.Add(Me.Label5)
        Me.Panel6.Location = New System.Drawing.Point(-2, 0)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(474, 32)
        Me.Panel6.TabIndex = 33
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.SteelBlue
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.White
        Me.Label5.Location = New System.Drawing.Point(12, 1)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(124, 25)
        Me.Label5.TabIndex = 32
        Me.Label5.Text = "Mail Setup"
        '
        'CICRadiusRConfig
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1775, 873)
        Me.Controls.Add(Me.PanelMailSetup)
        Me.Controls.Add(Me.PanelRadiusClients)
        Me.Controls.Add(Me.PanelSMSSetup)
        Me.Controls.Add(Me.PanelActiveDirectory)
        Me.Controls.Add(Me.PanelSetup)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.StatusBar)
        Me.Controls.Add(Me.btnRestart)
        Me.Controls.Add(Me.MenuBar)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuBar
        Me.MaximizeBox = False
        Me.Name = "CICRadiusRConfig"
        Me.Text = "Radius Configuration"
        Me.MenuBar.ResumeLayout(False)
        Me.MenuBar.PerformLayout()
        Me.StatusBar.ResumeLayout(False)
        Me.StatusBar.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.PanelSetup.ResumeLayout(False)
        Me.PanelSetup.PerformLayout()
        Me.PanelActiveDirectory.ResumeLayout(False)
        Me.PanelActiveDirectory.PerformLayout()
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.PanelSMSSetup.ResumeLayout(False)
        Me.PanelSMSSetup.PerformLayout()
        Me.Panel5.ResumeLayout(False)
        Me.Panel5.PerformLayout()
        Me.PanelRadiusClients.ResumeLayout(False)
        Me.PanelRadiusClients.PerformLayout()
        Me.Panel7.ResumeLayout(False)
        Me.Panel7.PerformLayout()
        Me.PanelMailSetup.ResumeLayout(False)
        Me.PanelMailSetup.PerformLayout()
        Me.Panel6.ResumeLayout(False)
        Me.Panel6.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnRestart As System.Windows.Forms.Button
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents lvConfig As System.Windows.Forms.ListView
    Friend WithEvents MenuBar As System.Windows.Forms.MenuStrip
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveConfigurationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RestartRadiusServerToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TestModemConfigurationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TestMailConfigurationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusBar As System.Windows.Forms.StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents MenuLabel As System.Windows.Forms.Label
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents PanelSetup As System.Windows.Forms.Panel
    Friend WithEvents ckEnableOTP As System.Windows.Forms.CheckBox
    Friend WithEvents ckDebug As System.Windows.Forms.CheckBox
    Friend WithEvents PanelActiveDirectory As System.Windows.Forms.Panel
    Friend WithEvents txtADField As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtLdapDomain As System.Windows.Forms.TextBox
    Friend WithEvents txtNetBios As System.Windows.Forms.TextBox
    Friend WithEvents lblLdapDomain As System.Windows.Forms.Label
    Friend WithEvents lblNetBios As System.Windows.Forms.Label
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents PanelSMSSetup As System.Windows.Forms.Panel
    Friend WithEvents btnTestModem As System.Windows.Forms.Button
    Friend WithEvents txtSMSC As System.Windows.Forms.TextBox
    Friend WithEvents txtComPort As System.Windows.Forms.TextBox
    Friend WithEvents lblSMSC As System.Windows.Forms.Label
    Friend WithEvents lblComPort As System.Windows.Forms.Label
    Friend WithEvents txtProvider As System.Windows.Forms.TextBox
    Friend WithEvents lblProvider As System.Windows.Forms.Label
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents PanelRadiusClients As System.Windows.Forms.Panel
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtSecret As System.Windows.Forms.TextBox
    Friend WithEvents lblSecret As System.Windows.Forms.Label
    Friend WithEvents btnRemoveClient As System.Windows.Forms.Button
    Friend WithEvents ListClients As System.Windows.Forms.ListBox
    Friend WithEvents btnAddClient As System.Windows.Forms.Button
    Friend WithEvents txtClient As System.Windows.Forms.TextBox
    Friend WithEvents lblClient As System.Windows.Forms.Label
    Friend WithEvents Panel7 As System.Windows.Forms.Panel
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents PanelMailSetup As System.Windows.Forms.Panel
    Friend WithEvents Panel6 As System.Windows.Forms.Panel
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents ckEnableMail As System.Windows.Forms.CheckBox
    Friend WithEvents ckEnableSMS As System.Windows.Forms.CheckBox
    Friend WithEvents rbCitrixNetscaler As System.Windows.Forms.RadioButton
    Friend WithEvents rbRDGateway As System.Windows.Forms.RadioButton
    Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox3 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox4 As System.Windows.Forms.TextBox
    Friend WithEvents txtADMailField As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents TextBox5 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox7 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox8 As System.Windows.Forms.TextBox
    Friend WithEvents rbLocalSMS As System.Windows.Forms.RadioButton
    Friend WithEvents rbOnlineSMS As System.Windows.Forms.RadioButton
    Friend WithEvents TextBox9 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox10 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox13 As System.Windows.Forms.TextBox
    Friend WithEvents txtSenderEmail As System.Windows.Forms.TextBox
    Friend WithEvents txtMailServer As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button

End Class
