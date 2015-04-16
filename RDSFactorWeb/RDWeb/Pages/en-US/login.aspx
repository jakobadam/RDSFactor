<?xml version="1.0" encoding="UTF-8"?>
<?xml-stylesheet type="text/xsl" href="../Site.xsl"?>
<?xml-stylesheet type="text/css" href="../RenderFail.css"?>
<% @Page Language="C#" Debug="false" ResponseEncoding="utf-8" ContentType="text/xml" %>
<% @Import Namespace="System " %>
<% @Import Namespace="System.Security" %>
<% @Import Namespace="Microsoft.TerminalServices.Publishing.Portal.FormAuthentication" %>
<% @Import Namespace="Microsoft.TerminalServices.Publishing.Portal" %>

<script language="C#" runat=server>

    //
    // Customizable Text
    //
    string L_CompanyName_Text = "Work Resources";
    
    //
    // Localizable Text
    //
    const string L_DomainUserNameLabel_Text = "Domain\\user name:";
    const string L_PasswordLabel_Text = "Password:";
    const string L_PasswordExpiredChangeBeginning_Text = "Your password is expired. Click ";
    const string L_PasswordExpiredChangeLink_Text = "here";
    const string L_PasswordExpiredChangeEnding_Text = " to change it.";
    const string L_PasswordExpiredNoChange_Text = "Your password is expired. Please contact your administrator for assistance.";
    const string L_ExistingWorkspaceLabel_Text = "Another user of your computer is currently using this connection.  This user must disconnect before you can log on.";
    const string L_DisconnectedWorkspaceLabel_Text = "Another user of your computer has disconnected from this connection.  Please type your user name and password again.";
    const string L_LogonFailureLabel_Text = "The user name or password that you entered is not valid. Try typing it again.";
    const string L_LogonSMSFailureLabel_Text = "The token code that you entered is not valid. Try again.";
    const string L_LogonRadiusFailureLabel_Text = "The radius server did not respond. Check radius configuration or give it another try.";
    const string L_DomainNameMissingLabel_Text = "You must enter a valid domain name.";
    const string L_AuthorizationFailureLabel_Text = "You aren’t authorized to log on to this connection.  Contact your system administrator for authorization.";
    const string L_ServerConfigChangedLabel_Text = "Your RD Web Access session expired due to configuration changes on the remote computer.  Please sign in again.";
    const string L_SecurityLabel_Text = "Security";
    const string L_ShowExplanationLabel_Text = "show explanation";
    const string L_HideExplanationLabel_Text = "hide explanation";
    const string L_PublicLabel_Text = "This is a public or shared computer";
    const string L_PublicExplanationLabel_Text = "Select this option if you use RD Web Access on a public computer.  Be sure to log off when you have finished using RD Web Access and close all windows to end your session.";
    const string L_PrivateLabel_Text = "This is a private computer";
    const string L_PrivateExplanationLabel_Text = "Select this option if you are the only person who uses this computer.  Your server will allow a longer period of inactivity before logging you off.";
    const string L_PrivateWarningLabel_Text = "Warning:  By selecting this option, you confirm that this computer complies with your organization's security policy.";
    const string L_PrivateWarningLabelNoAx_Text = "Warning:  By logging in to this web page, you confirm that this computer complies with your organization's security policy.";
    const string L_SignInLabel_Text = "Sign in";
    const string L_TSWATimeoutLabel_Text = "To protect against unauthorized access, your RD Web Access session will automatically time out after a period of inactivity.  If your session ends, refresh your browser and sign in again.";
    const string L_RenderFailTitle_Text = "Error: Unable to display RD Web Access";
    const string L_RenderFailP1_Text = "An unexpected error has occurred that is preventing this page from being displayed correctly.";
    const string L_RenderFailP2_Text = "Viewing this page in Internet Explorer with the Enhanced Security Configuration enabled can cause such an error.";
    const string L_RenderFailP3_Text = "Please try loading this page without the Enhanced Security Configuration enabled. If this error continues to be displayed, please contact your administrator.";

    //
    // Page Variables
    //
    public string strErrorMessageRowStyle;
    public string strDeliveryStyle;
    public bool bFailedLogon = false, bFailedAuthorization = false, bServerConfigChanged = false, bWorkspaceInUse = false, bWorkspaceDisconnected = false, bPasswordExpired = false, bPasswordExpiredNoChange = false, bFailedSMSLogon = false, bFailedRadiusLogon = false, bOTP = false;
    public string strWorkSpaceID = "";
    public string strRDPCertificates = "";
    public string strRedirectorName = "";
    public string strReturnUrl = "";
    public string strReturnUrlPage = "";
    public string strPasswordExpiredQueryString = "";
    public string sHelpSourceServer, sLocalHelp;
    public Uri baseUrl;
    public bool bEnableSMS = false;
    public bool bEnableMail = false;
    public string strPrivateModeTimeout = "240";
    public string strPublicModeTimeout = "20";

    public WorkspaceInfo objWorkspaceInfo = null;

    void Page_PreInit(object sender, EventArgs e)
    {

        // Deny requests with "additional path information"
        if (Request.PathInfo.Length != 0)
        {
            Response.StatusCode = 404;
            Response.End();
        }

        // gives us https://<machine>/rdweb/pages/<lang>/
	    baseUrl = new Uri(new Uri(GetRealRequestUri(), Request.FilePath), ".");
        sLocalHelp = ConfigurationManager.AppSettings["LocalHelp"];
        if ((sLocalHelp != null) && (sLocalHelp == "true"))
        {
            sHelpSourceServer = "./rap-help.htm";
        }
        else
        {
            sHelpSourceServer = "http://go.microsoft.com/fwlink/?LinkId=141038";
        }
        

        strPrivateModeTimeout = ConfigurationManager.AppSettings["PrivateModeSessionTimeoutInMinutes"];
        strPublicModeTimeout = ConfigurationManager.AppSettings["PublicModeSessionTimeoutInMinutes"];

        bOTP = ConfigurationManager.AppSettings["OTP"] == "true";
        bEnableSMS = ConfigurationManager.AppSettings["EnableSMS"] == "true";
        bEnableMail = ConfigurationManager.AppSettings["EnableMail"] == "true";
    }

    void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Session["UserPass"] = "";
            Session["DomainUserName"] = "";
        }
        
        if ( Request.QueryString != null )
        {
            NameValueCollection objQueryString = Request.QueryString;
            if ( objQueryString["ReturnUrl"] != null )
            {
                string strSmsToken = ConfigurationManager.AppSettings["SmsToken"];
                if (strSmsToken == null || !(strSmsToken.Equals("true", StringComparison.CurrentCultureIgnoreCase)))
                {
                    strReturnUrlPage = objQueryString["ReturnUrl"];
                    strReturnUrl = "?ReturnUrl=" + HttpUtility.UrlEncode(strReturnUrlPage);
                }
                else
                {
                    strReturnUrlPage = objQueryString["ReturnUrl"].ToLower();
                    strReturnUrl = "?ReturnUrl=" + HttpUtility.UrlEncode(strReturnUrlPage.Replace("default.aspx", "smstoken.aspx")); 
                }
            }
            if ( objQueryString["Error"] != null )
            {
                if ( objQueryString["Error"].Equals("WkSInUse", StringComparison.CurrentCultureIgnoreCase) )
                {
                    bWorkspaceInUse = true;
                }
                else if ( objQueryString["Error"].Equals("WkSDisconnected", StringComparison.CurrentCultureIgnoreCase) )
                {
                    bWorkspaceDisconnected = true;
                }
                else if ( objQueryString["Error"].Equals("UnauthorizedAccess", StringComparison.CurrentCultureIgnoreCase) )
                {
                    bFailedAuthorization = true;
                }
                else if (objQueryString["Error"].Equals("LoginSMSFailed", StringComparison.CurrentCultureIgnoreCase))
                {
                    bFailedSMSLogon = true;
                }
                else if (objQueryString["Error"].Equals("LoginRadiusFailed", StringComparison.CurrentCultureIgnoreCase))
                {
                    bFailedRadiusLogon = true;
                }
                else if ( objQueryString["Error"].Equals("ServerConfigChanged", StringComparison.CurrentCultureIgnoreCase) )
                {
                    bServerConfigChanged = true;
                }
                else if ( objQueryString["Error"].Equals("PasswordExpired", StringComparison.CurrentCultureIgnoreCase) )
                {
                    string strPasswordChangeEnabled = ConfigurationManager.AppSettings["PasswordChangeEnabled"];

                    if (strPasswordChangeEnabled != null && strPasswordChangeEnabled.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                    {
                        bPasswordExpired = true;
                        if (objQueryString["UserName"] != null)
                        {
                            strPasswordExpiredQueryString = "?UserName=" + Uri.EscapeDataString(objQueryString["UserName"]);
                        }
                    }
                    else
                    {
                        bPasswordExpiredNoChange = true;
                    }
                }
            }
        }

        //
        // Special case to handle 'ServerConfigChanged' error from Response's Location header.
        //
        try
        {
            if ( Response.Headers != null )
            {
                NameValueCollection objResponseHeader = Response.Headers;
                if ( !String.IsNullOrEmpty( objResponseHeader["Location"] ) )
                {
                    Uri objLocationUri = new Uri( objResponseHeader["Location"] );
                    if ( objLocationUri.Query.IndexOf("ServerConfigChanged") != -1 )
                    {
                        if ( !bFailedAuthorization )
                        {
                            bServerConfigChanged = true;
                        }
                    }
                }
            }
        }
        catch (Exception objException)
        {
        }

        if ( HttpContext.Current.User.Identity.IsAuthenticated != true )
        {
            // Only do this if we are actually rendering the login page, if we are just redirecting there is no need for these potentially expensive calls
            objWorkspaceInfo = RdwaConfig.GetWorkspaceInfo();
            if ( objWorkspaceInfo != null )
            {
                strWorkSpaceID = objWorkspaceInfo.WorkspaceId;
                strRedirectorName = objWorkspaceInfo.RedirectorName;
                string strWorkspaceName = objWorkspaceInfo.WorkspaceName;
                if ( String.IsNullOrEmpty(strWorkspaceName ) == false )
                {
                    L_CompanyName_Text = strWorkspaceName;
                }
            }
            strRDPCertificates = RdwaConfig.GetRdpSigningCertificateHash();
        }

        if ( HttpContext.Current.User.Identity.IsAuthenticated == true )
        {
          //  if ((string)Session["SMSTOKEN"] == "NOT_SMS_AUTH")
           // {
          //      bFailedLogon = false;
         //       Session["SMSTOKEN"] = "";
         //   }
         //   else
         //   {
            string strSmsToken = ConfigurationManager.AppSettings["SmsToken"];
            if (strSmsToken == null || !(strSmsToken.Equals("true", StringComparison.CurrentCultureIgnoreCase)))
            {
                SafeRedirect(strReturnUrlPage);
            }
            else
            {
                Session["SMSTOKEN"] = "NOT_SMS_AUTH";
                SafeRedirect(strReturnUrlPage);
            }
        //    }
           
        }
        else if ( HttpContext.Current.Request.HttpMethod.Equals("POST", StringComparison.CurrentCultureIgnoreCase) == true )
        {
            bFailedLogon = true;
            if ( bFailedAuthorization )
            {
                bFailedAuthorization = false; // Make sure to show one message.
            }
        }

        if (bPasswordExpired)
        {
            bFailedLogon = false;
        }
        
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
    }

    public static Uri GetRealRequestUri()
    {
        if ((HttpContext.Current == null) || 
             (HttpContext.Current.Request == null))
             throw new ApplicationException("Cannot get current request.");
        return GetRealRequestUri(HttpContext.Current.Request);
    }

    public static Uri GetRealRequestUri(HttpRequest request)
    {
         if (String.IsNullOrEmpty(request.Headers["Host"]))
           return request.Url;
         UriBuilder ub = new UriBuilder(request.Url);
         string[] realHost = request.Headers["Host"].Split(':');
         string host = realHost[0];
         ub.Host = host;
         string portString = realHost.Length > 1 ? realHost[1] : "";
         int port;
         if (int.TryParse(portString, out port))
             ub.Port = port;
         return ub.Uri;
    }

    private void SafeRedirect(string strRedirectUrl)
    {
        string strRedirectSafeUrl = null;

        if (!String.IsNullOrEmpty(strRedirectUrl))
        {
            Uri redirectUri = new Uri(GetRealRequestUri(), strRedirectUrl);

            if (
                redirectUri.Authority.Equals(Request.Url.Authority) &&
                redirectUri.Scheme.Equals(Request.Url.Scheme)
               )
            {
            strRedirectSafeUrl = redirectUri.AbsoluteUri;   
            }

        }

        if (strRedirectSafeUrl == null)
        {
             string strSmsToken = ConfigurationManager.AppSettings["SmsToken"];
             if (strSmsToken == null || !(strSmsToken.Equals("true", StringComparison.CurrentCultureIgnoreCase)))
             {
                 strRedirectSafeUrl = "default.aspx";
             } else
             {
        
                string UserPass = Request.Form["UserPass"];
                string DomainUserName =Request.Form["DomainUserName"];
                string Delivery =  Request.Form["rDelivery"];
                Session["UserPass"] = UserPass;
                Session["DomainUserName"]= DomainUserName;
                Session["Delivery"] =  Delivery;
                strRedirectSafeUrl = "smstoken.aspx";
           
            }
        }
        Response.Redirect(strRedirectSafeUrl);       
    }
</script>
<RDWAPage 
    helpurl="<%=sHelpSourceServer%>" 
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
  <BodyAttr 
    onload="onLoginPageLoad(event)" 
    onunload="onPageUnload(event)"/>
  <HTMLMainContent>
  
      <form id="FrmLogin" name="FrmLogin" action="login.aspx<%=SecurityElement.Escape(strReturnUrl)%>" method="post" onsubmit="return onLoginFormSubmit()">

        <input type="hidden" name="WorkSpaceID" value="<%=SecurityElement.Escape(strWorkSpaceID)%>"/>
        <input type="hidden" name="RDPCertificates" value="<%=SecurityElement.Escape(strRDPCertificates)%>"/>
        <input type="hidden" name="PublicModeTimeout" value="<%=SecurityElement.Escape(strPublicModeTimeout)%>"/>
        <input type="hidden" name="PrivateModeTimeout" value="<%=SecurityElement.Escape(strPrivateModeTimeout)%>"/>
        <input type="hidden" name="WorkspaceFriendlyName" value="<%=SecurityElement.Escape(L_CompanyName_Text)%>"/>
        <input type="hidden" name="RedirectorName" value="<%=SecurityElement.Escape(strRedirectorName)%>"/>
       
        <input name="isUtf8" type="hidden" value="1"/>
        <input type="hidden" name="flags" value="0"/>



        <table width="300" border="0" align="center" cellpadding="0" cellspacing="0">

            <tr>
            <td height="20">&#160;</td>
            </tr>

            <tr>
            <td>
                <table width="300" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td width="130" align="right"><%=L_DomainUserNameLabel_Text%></td>
                    <td width="7"></td>
                    <td align="right">
                    <label><input id="DomainUserName" name="DomainUserName" type="text" class="textInputField" runat="server" size="25" autocomplete="off" /></label>
                    </td>
                </tr>
                </table>
            </td>
            </tr>
            <tr>
            <td height="7"></td>
            </tr>

            <tr>
            <td>
                <table width="300" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td width="130" align="right"><%=L_PasswordLabel_Text%></td>
                    <td width="7"></td>
                    <td align="right">
                    <label><input id="UserPass" name="UserPass" type="password" class="textInputField" runat="server" size="25" autocomplete="off" /></label>
                    </td>
                </tr>
                </table>
            </td>
            </tr>

    <%
    strDeliveryStyle = "style=\"display:none\"";
    if ( bOTP )
    {
   strDeliveryStyle = "style=\"display:\"";
    }
    %>
            <td height="7"></td>
            
            <tr id="trDelivery" <%=strDeliveryStyle%> >
            <td>
                <table width="300" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td width="105" align="right">Token delivery:</td>
                    <td width="7"></td>
                    <td align="left" width="170">

              <%      if (bEnableSMS == true) {  %>
                    <label> <input name="rDelivery" type="radio" size="25" value="SMS" 
                            checked="checked"/>SMS</label>
                            <%} %>

                    <%      if (bEnableMail == true) {  %>
                         <%      if (bEnableSMS == true) {  %>
                    <label> <input name="rDelivery" type="radio" size="25" value="EMAIL"/>E-Mail </label>
                       <%} else { %>
                        <label> <input name="rDelivery" type="radio" size="25" value="EMAIL" checked="checked"/>E-Mail </label>
                       <%} %>
                    <%} %>
                    </td>
                </tr>
                </table>
            </td>
            </tr>
               

    <%
    strErrorMessageRowStyle = "style=\"display:none\"";
    if ( bPasswordExpiredNoChange == true)
    {
    strErrorMessageRowStyle = "style=\"display:\"";
    }
    %>
            <tr id="trPasswordExpiredNoChange" <%=strErrorMessageRowStyle%> >
            <td>
                <table>
                <tr>
                    <td height="20">&#160;</td>
                </tr>
                <tr>
                    <td><span class="wrng"><%=L_PasswordExpiredNoChange_Text%></span></td>
                </tr>
                </table>
            </td>
            </tr>
               
    <%
    strErrorMessageRowStyle = "style=\"display:none\"";
    if ( bPasswordExpired == true)
    {
    strErrorMessageRowStyle = "style=\"display:\"";
    }
    %>
            <tr id="trPasswordExpired" <%=strErrorMessageRowStyle%> >
            <td>
                <table>
                <tr>
                    <td height="20">&#160;</td>
                </tr>
                <tr>
                    <td><span class="wrng"><%=L_PasswordExpiredChangeBeginning_Text%><a id = "passwordchangelink" href="password.aspx<%=strPasswordExpiredQueryString%>"><%=L_PasswordExpiredChangeLink_Text%></a><%=L_PasswordExpiredChangeEnding_Text%></span></td>
                </tr>
                </table>
            </td>
            </tr>

    <%
    strErrorMessageRowStyle = "style=\"display:none\"";
    if ( bWorkspaceInUse == true )
    {
    strErrorMessageRowStyle = "style=\"display:\"";
    }
    %>
            <tr id="trErrorWorkSpaceInUse" <%=strErrorMessageRowStyle%> >
            <td>
                <table>
                <tr>
                    <td height="20">&#160;</td>
                </tr>
                <tr>
                    <td><span class="wrng"><%=L_ExistingWorkspaceLabel_Text%></span></td>
                </tr>
                </table>
            </td>
            </tr>

    <%
    strErrorMessageRowStyle = "style=\"display:none\"";
    if ( bWorkspaceDisconnected == true )
    {
    strErrorMessageRowStyle = "style=\"display:\"";
    }
    %>
            <tr id="trErrorWorkSpaceDisconnected" <%=strErrorMessageRowStyle%> >
            <td>
                <table>
                <tr>
                    <td height="20">&#160;</td>
                </tr>
                <tr>
                    <td><span class="wrng"><%=L_DisconnectedWorkspaceLabel_Text%></span></td>
                </tr>
                </table>
            </td>
            </tr>

    <%
    strErrorMessageRowStyle = "style=\"display:none\"";
    if ( bFailedSMSLogon == true )
    {
    strErrorMessageRowStyle = "style=\"display:\"";
    }
    %>
            <tr id="tr1" <%=strErrorMessageRowStyle%> >
            <td>
                <table>
                <tr>
                    <td height="20">&#160;</td>
                </tr>
                <tr>
                    <td><span class="wrng"><%=L_LogonSMSFailureLabel_Text%></span></td>
                </tr>
                </table>
            </td>
            </tr>

    <%
    strErrorMessageRowStyle = "style=\"display:none\"";
    if ( bFailedRadiusLogon == true )
    {
    strErrorMessageRowStyle = "style=\"display:\"";
    }
    %>
            <tr id="tr2" <%=strErrorMessageRowStyle%> >
            <td>
                <table>
                <tr>
                    <td height="20">&#160;</td>
                </tr>
                <tr>
                    <td><span class="wrng"><%=L_LogonRadiusFailureLabel_Text%></span></td>
                </tr>
                </table>
            </td>
            </tr>

    <%
    strErrorMessageRowStyle = "style=\"display:none\"";
    if ( bFailedLogon == true )
    {
    strErrorMessageRowStyle = "style=\"display:\"";
    }
    %>
            <tr id="trErrorIncorrectCredentials" <%=strErrorMessageRowStyle%> >
            <td>
                <table>
                <tr>
                    <td height="20">&#160;</td>
                </tr>
                <tr>
                    <td><span class="wrng"><%=L_LogonFailureLabel_Text%></span></td>
                </tr>
                </table>
            </td>
            </tr>

            <tr id="trErrorDomainNameMissing" style="display:none" >
            <td>
                <table>
                <tr>
                    <td height="20">&#160;</td>
                </tr>
                <tr>
                    <td><span class="wrng"><%=L_DomainNameMissingLabel_Text%></span></td>
                </tr>
                </table>
            </td>
            </tr> 


  
    <%
    strErrorMessageRowStyle = "style=\"display:none\"";
    if ( bFailedAuthorization )
    {
    strErrorMessageRowStyle = "style=\"display:\"";
    }
    %>
            <tr id="trErrorUnauthorizedAccess" <%=strErrorMessageRowStyle%> >
            <td>
                <table>
                <tr>
                    <td height="20">&#160;</td>
                </tr>
                <tr>
                    <td><span class="wrng"><%=L_AuthorizationFailureLabel_Text%></span></td>
                </tr>
                </table>
            </td>
            </tr>

    <%
    strErrorMessageRowStyle = "style=\"display:none\"";
    if ( bServerConfigChanged )
    {
    strErrorMessageRowStyle = "style=\"display:\"";
    }
    %>
            <tr id="trErrorServerConfigChanged" <%=strErrorMessageRowStyle%> >
            <td>
                <table>
                <tr>
                    <td height="20">&#160;</td>
                </tr>
                <tr>
                    <td><span class="wrng"><%=L_ServerConfigChangedLabel_Text%></span></td>
                </tr>
                </table>
            </td>
            </tr>

            <tr>
            <td height="20">&#160;</td>
            </tr>
            <tr>
            <td height="1" bgcolor="#CCCCCC"></td>
            </tr>
            <tr>
            <td height="20">&#160;</td>
            </tr>

            <tr>
            <td>
                <table border="0" cellspacing="0" cellpadding="0">
                <tr>
                    <td><%=L_SecurityLabel_Text%>&#160;<span id="spanToggleSecExplanation" style="display:none">(<a href="javascript:onclickExplanation('lnkShwSec')" id="lnkShwSec"><%=L_ShowExplanationLabel_Text%></a><a href="javascript:onclickExplanation('lnkHdSec')" id="lnkHdSec" style="display:none"><%=L_HideExplanationLabel_Text%></a>)</span></td>
                </tr>
                </table>
            </td>
            </tr>
            <tr>
            <td height="5"></td>
            </tr>

            <tr>
            <td>
                <table border="0" cellspacing="0" cellpadding="0" style="display:none" id="tablePublicOption" >
                <tr>
                    <td width="30">
                    <label><input id="rdoPblc" type="radio" name="MachineType" value="public" class="rdo" onclick="onClickSecurity()" /></label>
                    </td>
                    <td><%=L_PublicLabel_Text%></td>
                </tr>
                <tr id="trPubExp" style="display:none" >
      			        <td width="30"></td>
      			        <td><span class="expl"><%=L_PublicExplanationLabel_Text%></span></td>
                </tr>
                <tr>
                    <td height="7"></td>
                </tr>
                </table>
            </td>
            </tr>

            <tr>
            <td>
                <table border="0" cellspacing="0" cellpadding="0" style="display:none" id="tablePrivateOption" >
                <tr>
                    <td width="30">
                    <label><input id="rdoPrvt" type="radio" name="MachineType" value="private" class="rdo" onclick="onClickSecurity()" checked="checked" /></label>
                    </td>
                    <td><%=L_PrivateLabel_Text%></td>
                </tr>
                <tr id="trPrvtExp" style="display:none" >
                  	    <td width="30"></td>
        			    <td><span class="expl"><%=L_PrivateExplanationLabel_Text%></span></td>
                </tr>
                <tr>
                    <td height="7"></td>
                </tr>
                </table>
            </td>
            </tr>

            <tr>
            <td>
                <table border="0" cellspacing="0" cellpadding="0">
                <tr id="trPrvtWrn" style="display:none" >
                    <td width="30"></td>
                    <td><span class="wrng"><%=L_PrivateWarningLabel_Text%></span></td>
                </tr>
                </table>
            </td>
            </tr>

            <tr>
            <td>
                <table border="0" cellspacing="0" cellpadding="0">
                <tr id="trPrvtWrnNoAx" style="display:none">
                    <td><span class="wrng"><%=L_PrivateWarningLabelNoAx_Text%></span></td>
                </tr>
                </table>
            </td>
            </tr>

            <tr>
            <td height="20">&#160;</td>
            </tr>

            <tr>
            <td height="20">&#160;</td>
            </tr>
            <tr>
            <td align="right"><label><input type="submit" class="formButton" id="btnSignIn" value="<%=L_SignInLabel_Text%>" /></label>
            </td>
            </tr>

            <tr>
            <td height="20">&#160;</td>
            </tr>
            <tr>
            <td height="1" bgcolor="#CCCCCC"></td>
            </tr>

            <tr>
            <td height="20">&#160;</td>
            </tr>
            <tr>
            <td><%=L_TSWATimeoutLabel_Text%></td>
            </tr>

            <tr>
            <td height="30">&#160;</td>
            </tr>

        </table>

      </form>

  
  </HTMLMainContent>
</RDWAPage>
