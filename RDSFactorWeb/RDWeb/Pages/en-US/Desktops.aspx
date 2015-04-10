<?xml version="1.0" encoding="UTF-8"?>
<?xml-stylesheet type="text/xsl" href="../Site.xsl"?>
<?xml-stylesheet type="text/css" href="../RenderFail.css"?>
<% @Page Language="C#" Debug="false" ResponseEncoding="utf-8" ContentType="text/xml" %>
<% @Import Namespace="System.Globalization " %>
<% @Import Namespace="System.Web.Configuration" %>
<% @Import Namespace="System.Security" %>
<% @Import Namespace="Microsoft.TerminalServices.Publishing.Portal.FormAuthentication" %>
<% @Import Namespace="Microsoft.TerminalServices.Publishing.Portal" %>
<script runat="server">

    //
    // Customizable Text
    //
    string L_CompanyName_Text = "Work Resources";

    //
    // Localizable Text
    //
    const string L_RemoteAppProgramsLabel_Text = "RemoteApp and Desktops";
    const string L_DesktopTab_Text = "Connect to a remote PC";
    const string L_SignOutTab_Text = "Sign out";
    const string L_HelpTab_Text = "Help";
    const string L_DesktopHeader_Text = "Remote Desktop Services Remote Desktop Web Connection";
    const string L_DesktopDesc_Text = "Enter the name of the remote computer that you want to connect to, specify options, and then click Connect.";
    const string L_Options_Text = "<u>O</u>ptions";
    const string L_ConnectionOptionsHeader_Text = "Connection options";
    const string L_RemoteDesktopSize_Text = "<u>R</u>emote desktop size:";
    const string L_FullScreenLabel_Text = "Full screen";
    const string L_800x600Label_Text = "800 x 600 pixels";
    const string L_1024x768Label_Text = "1024 x 768 pixels";
    const string L_1280x1024Label_Text = "1280 x 1024 pixels";
    const string L_1600x1200Label_Text = "1600 x 1200 pixels";
    const string L_ConnectLabel_Text = "Co<u>n</u>nect";
    const string L_MachineNameAccessKey_Text = "c";
    const string L_MachineName_Text = "<u>C</u>onnect to:";
    const string L_ResolutionAccessKey_Text = "r";
    const string L_Resolution_Text="<u>R</u>emote desktop size:";
    const string L_OptionsAccessKey_Text = "o";
    const string L_ConnectAccessKey_Text = "n";
    const string L_DevAndRes_Text = "Devices and resources";
    const string L_DevAndResDesc_Text = "Select the devices and resources that you want to use in your remote session.";
    const string L_PrinterLabel_Text = "Prin<u>t</u>ers";
    const string L_ClipboardLabel_Text = "C<u>l</u>ipboard";
    const string L_DrivesLabel_Text = "Dr<u>i</u>ves";
    const string L_PNPLabel_Text = "Supported Plug and Pl<u>a</u>y devices";
    const string L_SerialLabel_Text = "S<u>e</u>rial ports";
    const string L_PrinterRedirectionAccessKey_Text = "t";
    const string L_ClipboardAccessKey_Text = "l";
    const string L_DrivesAccessKey_Text = "i";
    const string L_PNPAccessKey_Text="a";
    const string L_SerialAccessKey_Text="e";
    const string L_AdditionalOptions_Text = "Additional options";
    const string L_SoundLabel_Text = "Remote computer <u>s</u>ound:";
    const string L_SoundToComputerLabel_Text = "Bring to this computer";
    const string L_SoundDisabledLabel_Text = "Do not play";
    const string L_SoundAtServerLabel_Text = "Leave at remote computer";
    const string L_KeyLabel_Text = "Apply <u>k</u>eyboard shortcuts:";
    const string L_KeyAtServerLabel_Text = "On the remote computer";
    const string L_KeyAtComputerLabel_Text = "On the local computer";
    const string L_KeyAtServerFullScreenLabel_Text = "In Full screen mode only";
    const string L_PerformanceLabel_Text = "<u>P</u>erformance:";
    const string L_ModemLabel_Text = "Modem (56 Kbps)";
    const string L_LowSpeedBroadbandLabel_Text = "Low-speed broadband (256 Kbps - 2 Mbps)";
    const string L_SatelliteLabel_Text = "Satellite (2 Mbps - 16 Mbps with high latency)";
    const string L_HighSpeedBroadbandLabel_Text = "High-speed broadband (2 Mbps - 10 Mbps)";
    const string L_WANLabel_Text = "WAN (10 Mbps or higher with high latency)";
    const string L_LANLabel_Text = "LAN (10 Mbps or higher)";
    const string L_AutoDetectBandWidth_Text = "Detect connection quality automatically";        
    const string L_KeyboardAccessKey_Text = "k";
    const string L_PerformanceAccessKey_Text = "p";
    const string L_SoundAccessKey_Text = "s";
    const string L_PrivateDesc_Text = "By selecting this option you can save your credentials so that they can be used in the future when connecting to these programs. Before you select this option, please ensure that saving your credentials is in compliance with your organization's security policy.";
    const string L_HideInfoLabel_Text = "Hide additional information...";
    const string L_MoreInfoLabel_Text = "More information...";
    const string L_PrivateLabel_Text = "I am using a private computer that complies with my organization's security policy.";
    const string L_InvalidMachineName_Text = "You must enter a remote computer name.";
    const string L_RenderFailTitle_Text = "Error: Unable to display RD Web Access";
    const string L_RenderFailP1_Text = "An unexpected error has occurred that is preventing this page from being displayed correctly.";
    const string L_RenderFailP2_Text = "Viewing this page in Internet Explorer with the Enhanced Security Configuration enabled can cause such an error.";
    const string L_RenderFailP3_Text = "Please try loading this page without the Enhanced Security Configuration enabled. If this error continues to be displayed, please contact your administrator.";

    //
    // Page Variables
    //
    public string sHelpSourceServer, sLocalHelp;
    public bool bPrivateMode = false;
    public int SessionTimeoutInMinutes = 0;
    public string DefaultTSGateway = "";
    public string GatewayCredentialsSource = "";
    public string xPrinterRedirection;
    public string xClipboard;
    public string xDriveRedirection;
    public string xPnPRedirection;
    public string xPortRedirection;
    public string strDomainUserName = "";
    public AuthenticationMode eAuthenticationMode = AuthenticationMode.None;
    public Uri baseUrl;

    public WorkspaceInfo objWorkspaceInfo = null;

    protected void Page_PreInit(object sender, EventArgs e)
    {
        // Deny requests with "additional path information"
        if (Request.PathInfo.Length != 0)
        {
            Response.StatusCode = 404;
            Response.End();
        }
        
        // gives us https://<hostname>[:port]/rdweb/pages/<lang>/
        baseUrl = new Uri(new Uri(PageContentsHelper.GetBaseUri(Context), Request.FilePath), ".");

        AuthenticationSection objAuthenticationSection = ConfigurationManager.GetSection("system.web/authentication") as AuthenticationSection;
        if ( objAuthenticationSection  != null )
        {
            eAuthenticationMode = objAuthenticationSection.Mode;
        }

        if ( eAuthenticationMode == AuthenticationMode.Forms )
        {
            if ( HttpContext.Current.User.Identity.IsAuthenticated == false )
            {
                Response.Redirect("login.aspx?ReturnUrl=desktops.aspx");
            }
            TSFormAuthTicketInfo objTSFormAuthTicketInfo = new TSFormAuthTicketInfo(HttpContext.Current);            
            bPrivateMode = objTSFormAuthTicketInfo.PrivateMode;
            strDomainUserName = objTSFormAuthTicketInfo.DomainUserName;

            if ( bPrivateMode == true )
            {
                try
                {
                    string strPrivateModeSessionTimeoutInMinutes = ConfigurationManager.AppSettings["PrivateModeSessionTimeoutInMinutes"].ToString();
                    SessionTimeoutInMinutes = Int32.Parse(strPrivateModeSessionTimeoutInMinutes);
                }
                catch (Exception objException)
                {
                    Console.WriteLine("\nException : " + objException.Message);
                    SessionTimeoutInMinutes = 240;
                }
            }
            else
            {
                try
                {
                    string strPublicModeSessionTimeoutInMinutes = ConfigurationManager.AppSettings["PublicModeSessionTimeoutInMinutes"].ToString();
                    SessionTimeoutInMinutes = Int32.Parse(strPublicModeSessionTimeoutInMinutes);
                }
                catch (Exception objException)
                {
                    Console.WriteLine("\nException : " + objException.Message);
                    SessionTimeoutInMinutes = 20;
                }
            }
        }

        objWorkspaceInfo = PageContentsHelper.GetWorkspaceInfo();
        if ( objWorkspaceInfo != null )
        {
            string strWorkspaceName = objWorkspaceInfo.WorkspaceName;
            if ( String.IsNullOrEmpty(strWorkspaceName ) == false )
            {
                L_CompanyName_Text = strWorkspaceName;
            }
        }

        sLocalHelp = ConfigurationManager.AppSettings["LocalHelp"];

        if ((sLocalHelp != null) && (sLocalHelp == "true"))
            sHelpSourceServer = "./rap-help.htm";
        else
            sHelpSourceServer = "http://go.microsoft.com/fwlink/?LinkId=141038";

    }

    protected void Page_Init(object sender, EventArgs e)
    {
        DefaultTSGateway = ConfigurationManager.AppSettings["DefaultTSGateway"].ToString();
        GatewayCredentialsSource = ConfigurationManager.AppSettings["GatewayCredentialsSource"].ToString();

        xPrinterRedirection = ConfigurationManager.AppSettings["xPrinterRedirection"].ToString();
        xClipboard = ConfigurationManager.AppSettings["xClipboard"].ToString();
        xDriveRedirection = ConfigurationManager.AppSettings["xDriveRedirection"].ToString();
        xPnPRedirection = ConfigurationManager.AppSettings["xPnPRedirection"].ToString();
        xPortRedirection = ConfigurationManager.AppSettings["xPortRedirection"].ToString();

        Response.Cache.SetCacheability(HttpCacheability.NoCache);
    }
    private string IsChecked(string xArg)
    {
        return (xArg == "true") ? "checked='checked'" : "";
    }

</script>

<RDWAPage 
    helpurl="<%=sHelpSourceServer%>" 
    domainuser="<%=SecurityElement.Escape(strDomainUserName)%>" 
    workspacename="<%=SecurityElement.Escape(L_CompanyName_Text)%>" 
    baseurl="<%=SecurityElement.Escape(baseUrl.AbsoluteUri)%>"
    >
    <RenderFailureMessage>
        <html xmlns="http://www.w3.org/1999/xhtml">
            <head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
                <title><%=L_RenderFailTitle_Text%></title>
            </head>
            <body>
                <h1><%=L_RenderFailTitle_Text%></h1>
                <p><%=L_RenderFailP1_Text%></p>
                <p><%=L_RenderFailP2_Text%></p>
                <p><%=L_RenderFailP3_Text%></p>
            </body>
        </html> 
    </RenderFailureMessage>
    <HeaderJS>
        bFormAuthenticationMode = false;
    <%  if ( eAuthenticationMode == AuthenticationMode.Forms ) { %>
        bFormAuthenticationMode = true;
    <% } %>
        iSessionTimeout = parseInt("<%=SessionTimeoutInMinutes%>");
    </HeaderJS>
    <BodyAttr onload="onAuthenticatedPageload(event)" onunload="onPageUnload(event)" onmousedown="onUserActivity(event)" onmousewheel="onUserActivity(event)" onscroll="onUserActivity(event)" onkeydown="onUserActivity(event)"/>
    <NavBar
    <% if ( eAuthenticationMode == AuthenticationMode.Forms ) { %>
        showsignout="true"
    <% } %>
        activetab="PORTAL_REMOTE_DESKTOPS"
    >
        <Tab id="PORTAL_REMOTE_PROGRAMS" href="Default.aspx"><%=L_RemoteAppProgramsLabel_Text%></Tab>
        <Tab id="PORTAL_REMOTE_DESKTOPS" href="Desktops.aspx"><%=L_DesktopTab_Text%></Tab>
    </NavBar>
    <HTMLMainContent>


        <table border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td width="30">&#160;</td>
                       
            
            <td>

                <table border="0" cellpadding="0" cellspacing="0">

                    <tr height="20">
                        <td></td>
                    </tr>
                    <tr>
                        <td><%=L_DesktopDesc_Text%></td>
                    </tr>

                    <tr height="20">
                        <td></td>
                    </tr>
                    <tr>
                        <td><b><%=L_ConnectionOptionsHeader_Text%></b></td>
                    </tr>

                    <tr height="7">
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                        <table cellpadding="0" cellspacing="0" border="0">
                            <tr>
                            <td align="right" valign="middle" style="width:160px;padding-bottom:3px;">
                                <label accesskey="<%=L_MachineNameAccessKey_Text %>" for="MachineName"><%=L_MachineName_Text %></label>
                            </td>
                            <td width="7"></td>
                            <td valign="top" style="padding-bottom:4px;padding-right:4px;" colspan="2">
                                &#160;<input name="MachineName" maxlength="255" id="MachineName" class="textInputField" type="text"
                                    onfocus="updateConnectButtonState(this);" onblur="updateConnectButtonState(this);"
                                    onkeyup="onConnectToKeyUp(this);" onpropertychange="onConnectToPropertyChange(this);"/>
                            </td>
                            </tr>

                            <tr id="trErrorInvalidMachine" style="display:none" >
                            <td align="right" valign="top">&#160;</td>
                            <td width="7"></td>
                            <td height="20">&#160;<span class="wrng"><%=L_InvalidMachineName_Text%></span></td>
                            </tr>

                            <tr>

                            <td align="right" valign="top">
                                <label accesskey="<%=L_ResolutionAccessKey_Text %>" for="comboResolution"><%=L_Resolution_Text %></label>
                            </td>
                            <td width="7"></td>
                            <td valign="top" style="padding-bottom: 4px;">
                                &#160;<select class="topspace" id="comboResolution" style="width: 270px" name="comboResolution">
                                        <option value="0" selected="selected"><%=L_FullScreenLabel_Text %></option>
                                        <option value="1"><%=L_800x600Label_Text %></option>
                                        <option value="2"><%=L_1024x768Label_Text %></option>
                                        <option value="3"><%=L_1280x1024Label_Text %></option>
                                        <option value="4"><%=L_1600x1200Label_Text %></option>
                                    </select>
                            </td>
                            </tr>
                            <tr>
                            <td align="right" valign="top">&#160;</td>
                            <td width="7"></td>
                            <td valign="top" style="padding-top: 4px; padding-bottom: 10px;">
                                &#160;<button type="button" id="ButtonOptions" name="ButtonOptions" class="formButton" onclick="jscript:hideshowOptions();" accesskey="<%=L_OptionsAccessKey_Text %>"><%=L_Options_Text %> &gt;&gt;</button>
                                &#160;<button type="button" id="ButtonConnect" name="ButtonConnect" disabled="disabled" class="formButton" onclick="BtnConnect()" accesskey="<%=L_ConnectAccessKey_Text %>"><%=L_ConnectLabel_Text %></button>
                            </td>
                            </tr>
                        </table>
                        </td>
                    </tr>

                    <tr height="20">
                        <td></td>
                    </tr>

                <!-- ******************** Hidden Row for 'Additional Options' ******************** -->
                <tr>
                    <td>
                    <table cellpadding="0" cellspacing="0" border="0"  id="opt_panel" style="visibility: hidden;">

                        <tr>
                            <td><b><%=L_DevAndRes_Text%></b></td>
                        </tr>

                        <tr height="7">
                            <td></td>
                        </tr>
                        <tr>
                            <td><%=L_DevAndResDesc_Text %></td>
                        </tr>

                        <tr height="7">
                            <td></td>
                        </tr>
                        <tr>
                            <td>

                                <table cellpadding="0" cellspacing="0" border="0">
                                    <tr>
                                        <td width="167"></td>
                                        <td>

                                        <table cellpadding="0" cellspacing="0" border="0">
                                            <tr>
                                            <td><input type="checkbox" value="OFF" id="xPrinterRedirection" name="xPrinterRedirection" <%=IsChecked(xPrinterRedirection)%>/></td>
                                            <td><label accesskey="<%=L_PrinterRedirectionAccessKey_Text %>" for="xPrinterRedirection"><%=L_PrinterLabel_Text %></label></td>
                                            <td width="7"></td>
                                            <td><input type="checkbox" value="OFF" id="xClipboard" name="xClipboard" <%=IsChecked(xClipboard)%>/></td>
                                            <td><label accesskey="<%=L_ClipboardAccessKey_Text %>" for="xClipboard"><%=L_ClipboardLabel_Text %></label></td>
                                            </tr>

                                            <tr>
                                            <td><input type="checkbox" value="OFF" id="xDriveRedirection" name="xDriveRedirection" <%=IsChecked(xDriveRedirection)%> /></td>
                                            <td><label id="xDriveRedirection_Label" accesskey="<%=L_DrivesAccessKey_Text %>" for="xDriveRedirection"><%=L_DrivesLabel_Text %> </label></td>
                                            <td width="7"></td>
                                            <td><input type="checkbox" value="OFF" id="xPnPRedirection" name="xPnPRedirection" <%=IsChecked(xPnPRedirection)%> /></td>
                                            <td><label id="xPnPRedirection_Label" accesskey="<%=L_PNPAccessKey_Text %>" for="xPnPRedirection"><%=L_PNPLabel_Text %></label></td>
                                            </tr>

                                            <tr>
                                            <td><input type="checkbox" value="OFF" id="xPortRedirection" name="xPortRedirection" <%=IsChecked(xPortRedirection)%> /></td>
                                            <td><label id="xPortRedirection_Label" accesskey="<%=L_SerialAccessKey_Text %>" for="xPortRedirection"><%=L_SerialLabel_Text %></label></td>
                                            <td width="7"></td>
                                            <td></td>
                                            <td></td>
                                            </tr>
                                        </table>

                                        </td>
                                    </tr>
                                </table>

                            </td>
                        </tr>

                        <tr height="20">
                        <td></td>
                        </tr>
                        <tr>

                        <td><b><%=L_AdditionalOptions_Text%></b></td>
                        </tr>

                        <tr height="7">
                        <td></td>
                        </tr>
                        <tr>
                            <td>

                                <table cellpadding="0" cellspacing="0" border="0">
                                    <tr>
                                        <td align="right" valign="top" width="160">
                                        <label accesskey="<%=L_SoundAccessKey_Text %>" for="comboAudio"><%=L_SoundLabel_Text %></label>
                                        </td>
                                        <td width="7"></td>
                                        <td valign="top" style="padding-bottom: 4px;" width="300">
                                        &#160;<select class="topspace" id="comboAudio" style="width: 270px" name="comboAudio">
                                                <option value="0" selected="selected"><%=L_SoundToComputerLabel_Text %></option>
                                                <option value="1"><%=L_SoundAtServerLabel_Text %></option>
                                                <option value="2"><%=L_SoundDisabledLabel_Text %></option>
                                                </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" valign="top">
                                            <label accesskey="<%=L_KeyboardAccessKey_Text %>" for="comboKeyboard"><%=L_KeyLabel_Text %></label>
                                        </td>
                                        <td width="7"></td>
                                        <td valign="top" style="padding-bottom: 4px;">
                                        &#160;<select class="topspace" id="comboKeyboard" style="width: 270px" name="comboKeyboard">
                                                <option value="0"><%=L_KeyAtComputerLabel_Text %></option>
                                                <option value="1"><%=L_KeyAtServerLabel_Text %></option>
                                                <option value="2" selected="selected"><%=L_KeyAtServerFullScreenLabel_Text %></option>
                                                </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" valign="top">
                                        <label accesskey="<%=L_PerformanceAccessKey_Text %>" for="comboPerfomance"><%=L_PerformanceLabel_Text %></label>
                                        </td>
                                        <td width="7"></td>
                                        <td valign="top" style="padding-bottom: 4px;">
                                        &#160;<select class="topspace" id="comboPerfomance" style="width: 270px" name="comboPerfomance"
                                                onchange="jscript:setPerf();">
                                                <option value="1"><%=L_ModemLabel_Text%></option>
                                                <option value="2"><%=L_LowSpeedBroadbandLabel_Text%></option>
                                                <option value="3"><%=L_SatelliteLabel_Text%></option>
                                                <option value="4"><%=L_HighSpeedBroadbandLabel_Text%></option>
                                                <option value="5"><%=L_WANLabel_Text%></option>
                                                <option value="6"><%=L_LANLabel_Text%></option>
                                                <option value="7" selected="selected"><%=L_AutoDetectBandWidth_Text%></option>
                                                </select>
                                        </td>
                                    </tr>
                                </table>

                            </td>
                        </tr>

                    </table>
                    </td>
                </tr>

              </table>

            <div style="display:none;height:0px; width:0px; background-color:Transparent;">
            <script type="text/vbscript" language="vbscript">
            <!--
                Function TSMsgBox(sMsg, iButtons, sTitle)
                    TSMsgBox = MsgBox(sMsg, iButtons, sTitle)
                End Function
            -->
            </script>
            <span id="DefaultTSGateway" style="display:none"><%=SecurityElement.Escape(DefaultTSGateway) %></span>
            <span id="GatewayCredentialsSource" style="display:none"><%=SecurityElement.Escape(GatewayCredentialsSource) %></span>
            <script type="text/javascript" language="javascript">
            <![CDATA[
                var DefaultTSGateway = document.getElementById("DefaultTSGateway").innerHTML;
                var GatewayCredentialsSource = document.getElementById("GatewayCredentialsSource").innerHTML;
                
                document.getElementById("MachineName").focus()
                window.onerror = fnErrTrap;
                var vbCritical = 16;
                var vbInformation = 64;
                var L_sTitle_Text = "Remote Desktop Connection";

                function updateConnectButtonState( objMachineName )
                {
                    if ( objMachineName.value.length == 0 )
                    {
                        document.getElementById("ButtonConnect").disabled = true;
                    }
                    else if ( objMachineName.value.length > 0 )
                    {
                        hideElement(document.getElementById("trErrorInvalidMachine"));
                        document.getElementById("ButtonConnect").disabled = false;
                    }
                }

                function onConnectToFocus( objMachineName )
                {
                    updateConnectButtonState( objMachineName );
                }

                function onConnectToBlur( objMachineName )
                {
                    updateConnectButtonState( objMachineName );
                }

                function onConnectToPropertyChange( objMachineName )
                {
                    // make sure it it's the value that changed
                    if ( window.event.propertyName == "value" )
                    {
                        updateConnectButtonState( objMachineName );
                    }
                }

                function TestMN(s)
                {
                    //-- Admins can set up their own validation here --//
                    var r, re;

                    //
                    // Match between 1 and 256 occurences of these characters: "\f\n\r\t'space'"*+,;=?|¦"
                    // starting from a word boundary (/b).
                    // See: http://msdn.microsoft.com/en-us/library/1400241x(VS.85).aspx
                    //
                    re = new RegExp("[\b\f\n\r\t\x20\x22\x2A-\x2C\x3B-\x3F\x7C\xA6]{1,256}");
                    r = s.match(re);
                    var retval = (r == null)? false : true;
                    return retval;
                }

                function TestTxt( objMachineName )
                {
                    var retval = false;
                    if ( TestMN(objMachineName.value) )
                    {
                        document.getElementById("ButtonConnect").disabled = true;
                        showElement(document.getElementById("trErrorInvalidMachine"));
                        retval = true;
                    }
                    return retval;
                }

                function onConnectToKeyUp( objMachineName )
                {
                    if ( window.event.keyCode == 13 )
                    {
                        if ( objMachineName.value.length > 0 )
                        {
                            BtnConnect();
                        }
                        else
                        {
                            showElement(document.getElementById("trErrorInvalidMachine"));
                        }
                    }
                }

                function fnErrTrap(sMsg,sUrl,sLine)
                {
                    var retval;
                    var L_errMsg_Text = "To use this Web site, your computer must be running the Remote Desktop Connection (RDC) client. \n\nTo continue, install the latest RDC client and the most recent updates from the Microsoft Update Web site, and then try again.";
                    if (sMsg.indexOf('is undefined') != -1)
                    {
                        retval = TSMsgBox(L_errMsg_Text, vbInformation, L_sTitle_Text);
                    }
                    else if (sMsg.indexOf('is null or not an object') != -1)
                    {
                        retval = TSMsgBox(L_errMsg_Text, vbInformation, L_sTitle_Text);
                    }
                    else if (sMsg.indexOf('canceled by the user') != -1)
                    {
                        return true;
                    }
                    else
                    {
                        var L_errMsg2_Text = "An application error was caught:\n\nError:%ErrorMessage%\nURL:%URL%\nLine:%ErrorLineNumber%" ; // {Placeholder="%ErrorMessage%","%URL%","%ErrorLineNumber%"}
                        var errMsg2 = L_errMsg2_Text
                        errMsg2 = errMsg2.replace("%ErrorMessage%",sMsg);
                        errMsg2 = errMsg2.replace("%ErrorNumber%",sURL);
                        errMsg2 = errMsg2.replace("%ErrorLineNumber%",sLine);

                        retval = TSMsgBox(L_errMsg2_Text, vbInformation, L_sTitle_Text);
                    }
                    return true;
                }

                var WebAccessControlPresent = IsWebAccessControlPresent();

                try
                {
                    document.write("<object type=\"application/x-oleobject\"");
                    document.write("id=\"MsRdpClient\" name=\"MsRdpClient\"");
                    document.write("onerror=\"OnControlLoadError\"");
                    document.write("height=\"0\" width=\"0\"");
                    if ( WebAccessControlPresent ) {
                        document.write("classid=\"CLSID:6A5B0C7C-5CCB-4F10-A043-B8DE007E1952\">");
                    }
                    else {
                        document.write("classid=\"CLSID:4eb89ff4-7f78-4a0f-8b8d-2bf02e94e4b2\">");
                    }
                    document.write("</object>");
                }
                catch(e)
                {
                    throw e;
                }

                var MsRdpClientShell;
                var MsRdpClient = document.getElementById("MsRdpClient");

                if ( WebAccessControlPresent ) {
                    MsRdpClientShell = MsRdpClient;
                }
                else {
                    MsRdpClientShell = MsRdpClient.MsRdpClientShell;
                }

                function OnControlLoadError()
                {
                    var L_errMsgLoad_Text = "A problem was detected while loading the ActiveX Control.";
                    var retval = TSMsgBox(L_errMsgLoad_Text, vbInformation, L_sTitle_Text);
                    return true;
                }

                function IsWebAccessControlPresent()
                {
                    var retval = false;
                    try {
                        var WebAccessControl = new ActiveXObject("MsRdpWebAccess.MsRdpClientShell");
                        if ( WebAccessControl ) {
                            retval = true;
                        }
                    }
                    catch(e) {
                        retval = false;
                    }
                    return retval;
                }
            ]]>
            </script>
        </div>

        <script type="text/javascript" language="javascript">MsRdpClient.MsRdpClientShell</script>
        <script type="text/javascript" language="javascript">
        <![CDATA[

            function CheckDisableRedirections()
            {
                var fRedirsAllowed = false;

                try
                {
                    fRedirsAllowed = MsRdpClient.GetSecuredRedirsEnabled();
                }
                catch(e)
                {
                    // If the client doesnt have the new SecuredRedirections check, fall-back...
                    fRedirsAllowed = MsRdpClient.SecuredSettingsEnabled;
                }

                if (!fRedirsAllowed)
                {
                    document.getElementById("xDriveRedirection").checked = false;
                    document.getElementById("xPnPRedirection").checked = false;
                    document.getElementById("xPortRedirection").checked = false;

                    document.getElementById("xDriveRedirection").disabled = true;
                    document.getElementById("xPnPRedirection").disabled = true;
                    document.getElementById("xPortRedirection").disabled = true;

                    document.getElementById("xDriveRedirection_Label").disabled = true;
                    document.getElementById("xPnPRedirection_Label").disabled = true;
                    document.getElementById("xPortRedirection_Label").disabled = true;
                }
            }

            function hideshowOptions()
            {
                if (document.getElementById("opt_panel").style.visibility == "hidden") {
                    document.getElementById("opt_panel").style.visibility = "visible";
                    document.getElementById("ButtonOptions").innerHTML = "<%= L_Options_Text%>" +" &lt;&lt;";
                }
                else {
                    document.getElementById("opt_panel").style.visibility = "hidden";
                    document.getElementById("ButtonOptions").innerHTML = "<%= L_Options_Text%>" +" &gt;&gt;";
                }
            }

            var objPerformanceOptions = new Array(8);

            function setPerf()
            {
                var iIndex;
                
                for (iIndex = 0; iIndex < objPerformanceOptions.length; iIndex++)
                {
                    objPerformanceOptions[iIndex] = 0;
                }

                //
                // Based on the choosen Performance option
                // from the Experience tab, turn ON specific options.
                //
                switch(parseInt(document.getElementById("comboPerfomance").value)) {
                    case 1:     // Modem
                        objPerformanceOptions[6] = 1;
                        break;
                    case 2:     // Low-speed broadband
                        objPerformanceOptions[5] = 1;
                        objPerformanceOptions[6] = 1;
                        break;
                    case 3:     // Satellite
                    case 4:     // High-speed broadband
                        objPerformanceOptions[2] = 1;
                        objPerformanceOptions[5] = 1;
                        objPerformanceOptions[6] = 1;
                        break;
                    case 5:     // WAN
                    case 6:     // LAN
                        objPerformanceOptions[0] = 1;
                        objPerformanceOptions[1] = 1;
                        objPerformanceOptions[2] = 1;
                        objPerformanceOptions[3] = 1;
                        objPerformanceOptions[4] = 1;
                        objPerformanceOptions[5] = 1;
                        objPerformanceOptions[6] = 1;
                        break;
                    case 7:
                        objPerformanceOptions[5] = 1;
                        objPerformanceOptions[6] = 1;
                        objPerformanceOptions[7] = 1;
                    default:
                        break;
                }
            }

            function setRes()
            {
                var retval;
                var iRes = GetParam("comboResolution", false, "0");
                var RDPstr1 = "screen mode id:i:2\n";
                var RDPstr2 = "Desktop Size ID:i:" + iRes + "\n";

                if (iRes == "0") {
                    retval =  RDPstr1;
                }
                else {
                    retval =  RDPstr2;
                }
                return retval;
            }           
            
            function flipBit (iVal)
            {
                return (iVal == 1) ? 0:1;
            }

            function BtnConnect()
            {
                var iConnectionType = 0;
                var RDPstr = "full address:s:" + GetParam("MachineName", true, "") + "\n";
                RDPstr += "authentication level:i:2\n";
                RDPstr += "gatewayhostname:s:" +  DefaultTSGateway + "\n";
                RDPstr += "gatewaycredentialssource:i:" +  GatewayCredentialsSource + "\n";
                if ((DefaultTSGateway != null) && (DefaultTSGateway.length > 0)) {
                    RDPstr += "gatewayusagemethod:i:2\n";
                    RDPstr += "gatewayprofileusagemethod:i:1\n";
                }
                else {
                    RDPstr += "gatewayusagemethod:i:2\n";
                    RDPstr += "gatewayprofileusagemethod:i:0\n";
                }

                setPerf();
                RDPstr += "disable wallpaper:i:" + flipBit(objPerformanceOptions[0]).toString() + "\n";
                RDPstr += "allow font smoothing:i:" + objPerformanceOptions[1].toString() + "\n";
                RDPstr += "allow desktop composition:i:" + objPerformanceOptions[2].toString() + "\n";
                RDPstr += "disable full window drag:i:" + flipBit(objPerformanceOptions[3]).toString() + "\n";
                RDPstr += "disable menu anims:i:" + flipBit(objPerformanceOptions[4]).toString() + "\n";
                RDPstr += "disable themes:i:" + flipBit(objPerformanceOptions[5]).toString() + "\n";
                RDPstr += "bitmapcachepersistenable:i:" + objPerformanceOptions[6].toString() + "\n";

                RDPstr += "bandwidthautodetect:i:1\n";
                RDPstr += "networkautodetect:i:" + objPerformanceOptions[7].toString() + "\n";

                iConnectionType = document.getElementById("comboPerfomance").value;

                if (iConnectionType == 7) {
                    //
                    // default to lowspeed-broadband
                    //
                    iConnectionType = 2;
                }

                RDPstr += "connection type:i:" + iConnectionType + "\n";

                RDPstr += setRes();
                RDPstr += "displayconnectionbar:i:1\n";
                RDPstr += "session bpp:i:32\n";

                RDPstr += "redirectclipboard:i:" + GetParam("xClipboard", false, "0") + "\n";
                if (GetParam("xDriveRedirection", false, "0") == 1)
                {
                    RDPstr += "redirectdrives:i:1\n";
                    RDPstr += "drivestoredirect:s:*\n";
                }
                else
                {
                    RDPstr += "redirectdrives:i:0\n";
                }
                RDPstr += "redirectcomports:i:" + GetParam("xPortRedirection", false, "0") + "\n";
                RDPstr += "devicestoredirect:s:" + ((document.getElementById("xPnPRedirection").checked) ? "*" : "") + "\n";
                RDPstr += "keyboardhook:i:" + parseInt(document.getElementById("comboKeyboard").value) + "\n";
                RDPstr += "audiomode:i:" + parseInt(document.getElementById("comboAudio").value) + "\n";

                RDPstr += "redirectprinters:i:" + GetParam("xPrinterRedirection", false, "0") + "\n";
                RDPstr += "redirectsmartcards:i:1\n";
                RDPstr += "compression:i:1\n";
                RDPstr += "autoreconnection enabled:i:1\n";
                RDPstr += getUserNameRdpProperty();
                RDPstr += "";

    <%
                if ( eAuthenticationMode == AuthenticationMode.Forms )
                {
    %>
                    MsRdpClientShell.PublicMode = <%=(!bPrivateMode).ToString().ToLower(CultureInfo.InvariantCulture)%>;
    <%
                }
                else
                {
    %>
                    MsRdpClientShell.PublicMode = !(document.getElementById("PublicCheckbox").checked);
    <%
                }
    %>

                MsRdpClientShell.RdpFileContents = RDPstr;
                if (TestTxt(document.getElementById("MachineName"))== false)
                {
                    try
                    {
                        MsRdpClientShell.Launch();
                    }
                    catch(e)
                    {
                        if (e.number==-2147467259)
                        {
                            var L_ErrMsgLaunch_Text = "The security settings of your browser are preventing this Remote Desktop Services website from launching the remote program. Please add this Remote Desktop Services website to the Trusted sites or Local intranet zone of your browser and try again.";
                            var retval = TSMsgBox(L_ErrMsgLaunch_Text, vbInformation, L_sTitle_Text);
                        }
                        else
                        {
                            throw e
                        }
                    }
                }
                else
                {
                    document.getElementById("MachineName").focus();
                }
            }

            function GetParam(sParam, bReqd, vDefault)
            {
                var obj = document.getElementById(sParam);
                if(obj != null)
                {
                    switch(obj.tagName)
                    {
                        case "SELECT":
                            return obj.selectedIndex;
                            break;
                        case "INPUT":
                            if (obj.type == "checkbox") return ((obj.checked) ? 1 : 0);
                            if (obj.type == "hidden") return obj.value;
                            if (obj.type == "text") return obj.value;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if ((bReqd) && ((vDefault == "") || (vDefault == null) || (obj == null)))
                    {
                        var L_ErrMsgInvalid_Text = "%ParameterName% is not a valid or available parameter name.";  // {Placeholder="%ParameterName%"}
                        var errMsgInvalid = sParam;
                        errMsgInvalid = errMsgInvalid.replace("%ParameterName%", sParam);
                        var retval = TSMsgBox(errMsgInvalid, vbInformation, L_sTitle_Text);
                        return null;
                    }
                    else
                    {
                        return vDefault;
                    }
                }
            }
        ]]>
        </script>

            </td>
            </tr>
        </table>

    </HTMLMainContent>
    <ExtraRows>
        <!-- Row 7.1 - Empty -->
        <tr>
            <td height="40">
            </td>
        </tr>


<%
    if ( eAuthenticationMode == AuthenticationMode.Windows )
    {
%>
    <!-- Row 7.2 - Checkbox -->
    <tr>
        <td height="30">

        <table border="0" cellpadding="0" cellspacing="0">
            <tr>
            <td width="30">&#160;</td>
            <td>

                <table width="100%" cellspacing="0" cellpadding="0" border="0" id='contentPublicCheckbox' class='tswa_PublicCheckboxLess' >
                    <tr>
                    <td valign="top" width="30">
                        <input id='PublicCheckbox' type="checkbox" value="ON" />
                    </td>
                    <td id='SecurityText2' valign="top" style="padding-top:2px"><%=L_PrivateLabel_Text %>
                        <span id='p2M' onclick="toggle(this);">(<a href="javascript:toggle(this)"><%=L_MoreInfoLabel_Text %></a>)</span>
                        <span id='privateMore' style="display:none"><br /><%=L_PrivateDesc_Text %>
                        <span id='p2L' onclick="toggle(this);">(<a href="javascript:toggle(this)"><%=L_HideInfoLabel_Text %></a>)</span>
                        </span>
                    </td>
                    </tr>
                </table>

                <!--[if IE 6]>
                <script type="text/javascript" language="javascript">
                    document.styleSheets[0].addRule('.tswa_PublicCheckboxMore', 'position:relative');
                    document.styleSheets[0].addRule('.tswa_PublicCheckboxLess', 'position:relative');
                </script>
                <![endif]-->
                <!--[if IE 7]>
                <script type="text/javascript" language="javascript">
                    document.styleSheets[0].addRule('.tswa_PublicCheckboxMore', 'position:relative');
                    document.styleSheets[0].addRule('.tswa_PublicCheckboxLess', 'position:relative');
                </script>
                <![endif]-->

                <script type="text/javascript" language="javascript">
                    function toggle(e)
                    {
                      	if (e.id == "p2M")
                      	{
                            p2M.style.display = "none";
                            p2L.style.display = "";
                            privateMore.style.display = "";
                            contentPublicCheckbox.className = "tswa_PublicCheckboxMore";
                        }
                      	else if (e.id == "p2L")
                      	{
                            p2M.style.display = "";
                            p2L.style.display = "none";
                            privateMore.style.display = "none";
                            contentPublicCheckbox.className = "tswa_PublicCheckboxLess";
                      	}
                    }

                    CheckDisableRedirections();
                    document.getElementById("MachineName").focus();
                </script>

            </td>
            </tr>
        </table>
        </td>
    </tr>
<%
	}
%>
    </ExtraRows>
</RDWAPage>


