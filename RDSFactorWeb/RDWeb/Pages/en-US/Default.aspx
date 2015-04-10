<?xml version="1.0" encoding="UTF-8"?>
<% @Page Language="C#" Debug="false" ResponseEncoding="utf-8" ContentType="text/xml" %>
<% @Import Namespace="System.Globalization" %>
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
    const string L_BadFolderErrorTitle_Text = "Folder does not exist. Redirecting...";
    const string L_BadFolderErrorBody_Text = "You have attempted to load a folder that does not exist.  In a moment, you will be redirected to the top-level folder.";
    const string L_RenderFailTitle_Text = "Error: Unable to display RD Web Access";
    const string L_RenderFailP1_Text = "An unexpected error has occurred that is preventing this page from being displayed correctly.";
    const string L_RenderFailP2_Text = "Viewing this page in Internet Explorer with the Enhanced Security Configuration enabled can cause such an error.";
    const string L_RenderFailP3_Text = "Please try loading this page without the Enhanced Security Configuration enabled. If this error continues to be displayed, please contact your administrator.";

    //
    // Page Variables
    //
    public string sHelpSourceServer, sLocalHelp, sRDCInstallUrl, strWorkspaceName;
    public Uri baseUrl, stylesheetUrl, renderFailCssUrl;
    public bool bShowPublicCheckBox = false, bPrivateMode = false, bRTL = false;
    public int SessionTimeoutInMinutes = 0;
    public bool bShowOptimizeExperience = false, bOptimizeExperienceState = false;
    public AuthenticationMode eAuthenticationMode = AuthenticationMode.None;
    public string strTicketName = "";
    public string strDomainUserName = "", strUserIdentity = "";
    public string strAppFeed;

    public WorkspaceInfo objWorkspaceInfo = null;

    protected void Page_PreInit(object sender, EventArgs e)
    {

        string strReturnUrl = "";
        string strReturnUrlPage = "";
        
        // gives us https://<hostname>[:port]/rdweb/pages/<lang>/
        baseUrl = new Uri(new Uri(PageContentsHelper.GetBaseUri(Context), Request.FilePath), ".");
        
        try
        {
            string strShowOptimzeExperienceValue = ConfigurationManager.AppSettings["ShowOptimizeExperience"];
            if ( String.IsNullOrEmpty(strShowOptimzeExperienceValue) == false )
            {
                if ( strShowOptimzeExperienceValue.Equals( System.Boolean.TrueString, StringComparison.OrdinalIgnoreCase) )
                {
                    bShowOptimizeExperience = true;
                    string strOptimizeExperienceStateValue = ConfigurationManager.AppSettings["OptimizeExperienceState"];
                    if ( String.IsNullOrEmpty(strOptimizeExperienceStateValue) == false )
                    {
                        if ( strOptimizeExperienceStateValue.Equals( System.Boolean.TrueString, StringComparison.OrdinalIgnoreCase) )
                        {
                            bOptimizeExperienceState = true;
                        }
                    }
                }
            }
        }
        catch (Exception objException)
        {
        }

        AuthenticationSection objAuthenticationSection = ConfigurationManager.GetSection("system.web/authentication") as AuthenticationSection;
        if ( objAuthenticationSection  != null )
        {
            eAuthenticationMode = objAuthenticationSection.Mode;
        }

        if ( Request.QueryString != null )
        {
            NameValueCollection objQueryString = Request.QueryString;

            if ( objQueryString["ReturnUrl"] != null )
            {
                strReturnUrlPage = objQueryString["ReturnUrl"];
                strReturnUrl = "?ReturnUrl=" + HttpUtility.UrlEncode(strReturnUrlPage);
            }
        }

        if ( eAuthenticationMode == AuthenticationMode.Forms )
        {
            if ( HttpContext.Current.User.Identity.IsAuthenticated == false )
            {
                string strQueryString;
                if (String.IsNullOrEmpty(strReturnUrl))
                {
                    strQueryString = "?ReturnUrl=" + Request.Path;
                }
                else
                {
                    strQueryString = strReturnUrl;
                }

                Response.Redirect(new Uri(baseUrl, "login.aspx" + PageContentsHelper.AppendTenantIdToQuery(strQueryString)).AbsoluteUri);
            }

            TSFormAuthTicketInfo objTSFormAuthTicketInfo = new TSFormAuthTicketInfo(HttpContext.Current);
            strUserIdentity = objTSFormAuthTicketInfo.UserIdentity;
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
        else if ( eAuthenticationMode == AuthenticationMode.Windows )
        {
            bShowPublicCheckBox = true;
        }

        sRDCInstallUrl = ConfigurationManager.AppSettings["rdcInstallUrl"];

        sLocalHelp = ConfigurationManager.AppSettings["LocalHelp"];

        stylesheetUrl = new Uri(baseUrl, "../Site.xsl");
        renderFailCssUrl = new Uri(baseUrl, "../RenderFail.css");
       
        if ((sLocalHelp != null) && (sLocalHelp == "true"))
            sHelpSourceServer = "./rap-help.htm";
        else
            sHelpSourceServer = "http://go.microsoft.com/fwlink/?LinkId=141038";

        try {
          bRTL = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;
        } 
        catch( NullReferenceException ) {
        }

        WebFeed tswf = null;
        try
        {
            tswf = new WebFeed(RdpType.Both, true);
            strAppFeed = tswf.GenerateFeed(
                            strUserIdentity, 
                            FeedXmlVersion.Win8,             
                            (Request.PathInfo.Length > 0) ? Request.PathInfo : "/",
                            false); 
        }
        catch (WorkspaceUnknownFolderException)
        {
            BadFolderRedirect();
        }
        catch (InvalidTenantException)
        {
            Response.StatusCode = 404;
            Response.End();
        }
        catch (WorkspaceUnavailableException wue)
        {
            // This exception is raised when we cannot contact the appropriate sources to obtain the workspace information.
            // This is an edge case that can ocurr e.g. if the cpub server we're pointing to is down and the values are not specified in the Web.config.
            Response.StatusCode = 503;
            Response.End();
        }

        if ( tswf != null )
        {
            objWorkspaceInfo = tswf.GetFetchedWorkspaceInfo();
            if ( objWorkspaceInfo != null )
            {
                strWorkspaceName = objWorkspaceInfo.WorkspaceName;
            }
        }
        if ( String.IsNullOrEmpty(strWorkspaceName ) )
        {
            strWorkspaceName = L_CompanyName_Text;
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
    }

    private void BadFolderRedirect()
    {
        Response.ContentType = "text/html";
        Response.Write(            
@"<html>
   <head>
     <meta http-equiv=""refresh"" content=""10;url=" + Request.FilePath + @"""/>
     <title>" + L_BadFolderErrorTitle_Text + @"</title>
   </head>
   <body>
     <p id=""BadFolder1"">" + L_BadFolderErrorBody_Text + @"</p>     
   </body>
 </html>");
        Response.End();
    }

</script>
<%="<?xml-stylesheet type=\"text/xsl\" href=\"" + SecurityElement.Escape(stylesheetUrl.AbsoluteUri) + "\"?>"%>
<%="<?xml-stylesheet type=\"text/css\" href=\"" + SecurityElement.Escape(renderFailCssUrl.AbsoluteUri) + "\"?>"%>
<RDWAPage 
    helpurl="<%=sHelpSourceServer%>" 
    domainuser="<%=SecurityElement.Escape(strDomainUserName)%>" 
    workspacename="<%=SecurityElement.Escape(strWorkspaceName)%>" 
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
    <%  } %>
        iSessionTimeout = parseInt("<%=SessionTimeoutInMinutes%>");
    </HeaderJS>
    <BodyAttr 
        onload="onAuthenticatedPageload(event)" 
        onunload="onPageUnload(event)" 
        onmousedown="onUserActivity(event)" 
        onmousewheel="onUserActivity(event)" 
        onscroll="onUserActivity(event)" 
        onkeydown="onUserActivity(event)" />
    <NavBar
    <% if ( eAuthenticationMode == AuthenticationMode.Forms ) {  %>
        showsignout="true"
    <% } %>
        activetab="PORTAL_REMOTE_PROGRAMS"
    >
        <Tab id="PORTAL_REMOTE_PROGRAMS" href="Default.aspx"><%=L_RemoteAppProgramsLabel_Text%></Tab>
<%
    if (ConfigurationManager.AppSettings["ShowDesktops"].ToString() == "true")
    {
%>
        <Tab id="PORTAL_REMOTE_DESKTOPS" href="Desktops.aspx"><%=L_DesktopTab_Text%></Tab>
<%
    }
%>

    </NavBar>
    <Style>
      .tswa_appboard {width:850px;}
      .tswa_ShowOptimizeExperienceShiftedUp 
      {
        position:absolute;
        left:10px;
        top:397px;
        width:850px;
        height:20px;
        background-color:white;
      }
      
      #PORTAL_REMOTE_DESKTOPS
      {
          display:none;
      }
<%
    if ( bShowPublicCheckBox )
    {
%>
      .tswa_ShowOptimizeExperience
      {
        position:absolute;
        left:10px;
        top:445px;
        width:850px;
        height:20px;
        background-color:white;
      }
<%
    }
    else
    {
%>
      .tswa_ShowOptimizeExperience
      {
        position:absolute;
        left:10px;
        top:462px;
        width:850px;
        height:20px;
        background-color:white;
      }
<%
    }
%>
      .tswa_PublicCheckboxMore
      {
        position:absolute;
        left:10px;
        top:417px;
        width:850px;
        height:50px;
        border-top: 1px solid gray;
        background-color:white;
        z-index:4000;
        padding-top:4px;
      }
      
      .tswa_PublicCheckboxLess
      {
        position:absolute;
        left:10px;
        top:462px;
        width:850px;
        height:20px;
        background-color:white;
      }

<% if (bRTL) { %>
      /* Rules that are specific to RTL language environments */

      .tswa_appboard
      {
        padding-right:10px;
      }

      .tswa_boss, .tswa_folder_boss, .tswa_up_boss
      {
        float:right;
      }

      .tswa_error_icon
      {
        margin-left: 0px;
        padding-left: 0px;
        margin-right:10px;
        padding-right:45px;
      }

      .tswa_error_msg
      {
        margin-left:0px;
        padding-right:0px;
        margin-right:55px;
        padding-left:10px;
      }

<% } %>
    </Style>
    <Style condition="if IE 6">
      .tswa_appdisplay
      {
        background-color:transparent;left:5px;top:0px;height:450px;width:850px;
      }


      .tswa_ShowOptimizeExperienceShiftedUp
      {
        position:absolute;
        left:10px;
        top:415px;
        width:850px;
        height:20px;
        background-color:white;
      }

<%
    if ( bShowPublicCheckBox )
    {
%>
       .tswa_ShowOptimizeExperience
       {
         position:absolute;
         left:10px;
         top:463px;
         width:850px;
         height:20px;
         background-color:white;
       }
<%
    }
    else
    {
%>
       .tswa_ShowOptimizeExperience
       {
         position:absolute;
         left:10px;
         top:480px;
         width:850px;
         height:20px;
         background-color:white;
       }
<%
    }
%>
       .tswa_PublicCheckboxMore
       {
         position:absolute;
         left:10px;
         top:435px;
         width:850px;
         height:50px;
         border-top: 1px solid gray;
         background-color:white;
         z-index:4000;
         padding-top:4px;
       }
       
       .tswa_PublicCheckboxLess
       {
         position:absolute;
         left:10px;
         top:480px;
         width:850px;
         height:20px;
         background-color:white;
       }
    </Style>
    <Style condition="if gte IE 7">
      .tswa_appdisplay
      {
        background-color:transparent;
        left:5px;
        top:0px;
        height:440px;
        width:850px;
      }

      .tswa_ShowOptimizeExperienceShiftedUp
      {
        position:absolute;
        left:10px;
        top:397px;
        width:850px;
        height:20px;
        background-color:white;
      }
<%
    if ( bShowPublicCheckBox )
    {
%>
      .tswa_ShowOptimizeExperience
      {
        position:absolute;
        left:10px;
        top:445px;
        width:850px;
        height:20px;
        background-color:white;
      }
<%
    }
    else
    {
%>
      .tswa_ShowOptimizeExperience
      {
        position:absolute;
        left:10px;
        top:462px;
        width:850px;
        height:20px;
        background-color:white;
      }
<%
    }
%>
      .tswa_PublicCheckboxMore
      {
        position:absolute;
        left:10px;
        top:417px;
        width:850px;
        height:50px;
        border-top: 1px solid gray;
        background-color:white;
        z-index:4000;
        padding-top:4px;
      }
      
      .tswa_PublicCheckboxLess
      {
        position:absolute;
        left:10px;
        top:462px;
        width:850px;
        height:20px;
        background-color:white;
      }
    </Style>
    <AppFeed
        showpubliccheckbox="<%=bShowPublicCheckBox.ToString().ToLower()%>"
        privatemode="<%=bPrivateMode.ToString().ToLower()%>"
        showoptimizeexperience="<%=bShowOptimizeExperience.ToString().ToLower()%>"
        optimizeexperiencestate="<%=bOptimizeExperienceState.ToString().ToLower()%>"
        <%
        if (!String.IsNullOrEmpty(sRDCInstallUrl)) {
        %>
        rdcinstallurl="<%=SecurityElement.Escape(sRDCInstallUrl)%>"
        <%
        }
        %>
    >
        <%=strAppFeed%>
    </AppFeed>
</RDWAPage>
