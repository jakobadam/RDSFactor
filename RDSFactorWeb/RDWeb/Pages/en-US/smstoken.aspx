<?xml version="1.0" encoding="UTF-8"?>
<?xml-stylesheet type="text/xsl" href="../Site.xsl"?>
<?xml-stylesheet type="text/css" href="../RenderFail.css"?>
<% @Page Language="C#" Debug="false" ResponseEncoding="utf-8" ContentType="text/xml" %>
<% @Import Namespace="System" %>
<% @Import Namespace="System.Threading" %>
<% @Import Namespace="System.Security" %>
<% @Import Namespace="Microsoft.TerminalServices.Publishing.Portal.FormAuthentication" %>
<% @Import Namespace="Microsoft.TerminalServices.Publishing.Portal" %>
<% @Import Namespace="RADAR" %>

<script language="C#" runat=server>
    //
    // Customizable Text
    //
    string L_CompanyName_Text = "Work Resources";

    //
    // Localizable Text
    //
    const string L_SmsToken_Text = "Enter SMS Token:";
    const string L_LogonFailureLabel_Text = "The user name or password that you entered is not valid. Try typing it again.";
    const string L_SubmitLabel_Text = "Submit";
    const string L_CancelLabel_Text = "Cancel";
    const string L_RenderFailTitle_Text = "Error: Unable to display RD Web Access";
    const string L_RenderFailP1_Text = "An unexpected error has occurred that is preventing this page from being displayed correctly.";
    const string L_RenderFailP2_Text = "Viewing this page in Internet Explorer with the Enhanced Security Configuration enabled can cause such an error.";
    const string L_RenderFailP3_Text = "Please try loading this page without the Enhanced Security Configuration enabled. If this error continues to be displayed, please contact your administrator.";

    //
    // Page Variables
    //
    public string strErrorMessageRowStyle;
    public string strButtonsRowStyle;
    public bool bFailedLogon = false, bPasswordMismatchFailure = false, bPasswordBlankFailure = false, bComplexityFailure = false, bGenericFailure = false, bSuccess = false;
    public string sHelpSourceServer, sLocalHelp;
    public Uri baseUrl;
    private string strUserName;
    private string strPassword;
    private string strDomain;
    private string username;
    private string token;

    string RadiusServer = ConfigurationManager.AppSettings["RadiusServer"];
    string RadiusSecret = ConfigurationManager.AppSettings["RadiusSecret"];
    
    private RADIUSAttribute state;

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
    }

    void Page_Load(object sender, EventArgs e)
    {
       
        btnSignIn.Text = L_SubmitLabel_Text;
        btnCancel.Text = L_CancelLabel_Text;

        if (Page.IsPostBack) {
            return;
        }
        
        username = (string)Session["DomainUserName"];
        strPassword = (string)Session["UserPass"];               
        Label1.Text = (string)Session["Delivery"];

        bool Debug = true;
        RADIUSClient client = new RADIUSClient(RadiusServer, 1812, RadiusSecret);
        RADIUSAttributes atts = new RADIUSAttributes();
        client.Debug = Debug;
        try {    
            VendorSpecificAttribute vsa = new VendorSpecificAttribute(VendorSpecificType.Generic,(string)Session["Delivery"]);
            vsa.SetRADIUSAttribute(ref atts);

            RADIUSPacket response = client.Authenticate(username, strPassword, atts);
                   
	        if (response.Code == RadiusPacketCode.AccessChallenge) {
		        state = response.Attributes.GetFirstAttribute(RadiusAttributeType.State);
                Session["State"] = state;      
            }
            // Access-Accept
            else if (response.Code == RadiusPacketCode.AccessAccept) {
                string sessionGuid =  response.Attributes.GetFirstAttribute(RadiusAttributeType.ReplyMessage).GetString(); 
                Session["SESSIONGUID"] = sessionGuid;
                HttpCookie myCookie = new HttpCookie("RadiusSessionId");
                DateTime now = DateTime.Now;
                myCookie.Value = sessionGuid;
                myCookie.Expires = now.AddMinutes(480);
                Response.Cookies.Add(myCookie);
                Session["SMSTOKEN"] = "SMS_AUTH";
                Response.Redirect("default.aspx",false);  
            }
            else {
		        Session["UserPass"] = "";
                Session["DomainUserName"] = "";
                SafeRedirect("logoff.aspx?Error=LoginSMSFailed");
            }

	    }
	    catch (Exception ex) {
            Session["UserPass"] = "";
            Session["DomainUserName"] = "";
                  
            SafeRedirect("logoff.aspx?Error=LoginRadiusFailed");
        }
    }

    public static Uri GetRealRequestUri()
    {
        if ((HttpContext.Current == null) || (HttpContext.Current.Request == null))
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
            strRedirectSafeUrl = "default.aspx";
        }

        Response.Redirect(strRedirectSafeUrl,false);       
    }

    protected void btnSignIn_Click(object sender, EventArgs e)
    {

        username = (string)Session["DomainUserName"];  
        state = (RADIUSAttribute)Session["state"];
        
        RADIUSClient myRadius = new RADIUSClient(RadiusServer, 1812, RadiusSecret);
        RADIUSAttributes atts = new RADIUSAttributes();

		atts.Add(state);
		RADIUSPacket response = myRadius.Authenticate(username, SmsToken.Text, atts);

        if (response.Code == RadiusPacketCode.AccessAccept)
        {
            string sessionGuid = response.Attributes.GetFirstAttribute(RadiusAttributeType.ReplyMessage).GetString(); 
            Session["SESSIONGUID"] = sessionGuid;
            
            HttpCookie myCookie = new HttpCookie("RadiusSessionId");
            DateTime now = DateTime.Now;
            myCookie.Value = sessionGuid;
            myCookie.Expires = now.AddMinutes(480);
            Response.Cookies.Add(myCookie);

            Session["SMSTOKEN"] = "SMS_AUTH";
            SafeRedirect("default.aspx");
        }
        else
        {
            Session["SMSTOKEN"] = "NOT_SMS_AUTH";
            SafeRedirect("logoff.aspx?Error=LoginSMSFailed");
        }
        
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        SafeRedirect("logoff.aspx");
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
            <meta http-equiv="Content-Type" content="text/html; charset=unicode"/>
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

  <HTMLMainContent>
  
        <form id="FrmLogin" runat="server">

        <table width="350" border="0" align="center" cellpadding="0" cellspacing="0">

            <tr>
            <td height="20">&#160;</td>
            </tr>

            <tr>
            <td>
                <table width="350" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td width="180" align="right"><%=L_SmsToken_Text%></td>
                    <td width="7"></td>
                    <td align="right">
                        <asp:TextBox ID="SmsToken" class="textInputField" runat="server" size="25" autocomplete="off"></asp:TextBox>
                   
                    </td>
                </tr>
                </table>
            </td>
            </tr>

            <tr>
            <td height="7"></td>
            </tr>



  


    


    <%
    strErrorMessageRowStyle = "style=\"display:none\"";
    if ( bSuccess == true )
    {
    strErrorMessageRowStyle = "style=\"display:\"";
    }
    %>
            <tr id="tr1" <%=strErrorMessageRowStyle%> >
            <td>
                <table align = "center">
                
                </table>
            </td>
            </tr>

   

    <%
    strButtonsRowStyle = "style=\"display:none\"";
    if ( bSuccess == false )
    {
        strButtonsRowStyle = "style=\"display:\"";
    }
    %>
            <tr>
            <td height="20">
                <asp:Label ID="Label1" runat="server" Text="Label" Visible="False"></asp:Label>
                </td>
            </tr>
            <tr id="trButtons" <%=strButtonsRowStyle%> >
            <td align="right">
                <asp:Label ID="strDebug" runat="server" Visible="False"></asp:Label>
                <asp:Button ID="btnSignIn" runat="server"  class="formButton" OnClick="btnSignIn_Click"   />
                <asp:Button ID="btnCancel" runat="server" class="formButton" OnClick="btnCancel_Click" />
              
            </td>
            </tr>


            <tr>
            <td height="30">
                
                </td>
            </tr>

        </table>

        </form>

  
  </HTMLMainContent>
</RDWAPage>
