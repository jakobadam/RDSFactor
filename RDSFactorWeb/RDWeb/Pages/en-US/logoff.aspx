<% @Page Language="C#" Debug="true" %>

<script language="C#" runat=server>

    void Page_Load(object sender, EventArgs e)
    {
        // Deny requests with "additional path information"
        if (Request.PathInfo.Length != 0)
        {
            Response.StatusCode = 404;
            Response.End();
        }

        string strQueryString = "";

        if ( HttpContext.Current.User.Identity.IsAuthenticated == true )
        {
            Response.Redirect( "default.aspx" + AppendTenantIdToQuery(String.Empty) );
        }
        else
        {
            if ( Request.QueryString != null )
            {
                NameValueCollection objQueryString = Request.QueryString;

                if ( objQueryString["Error"] != null )
                {
                    if ( objQueryString["Error"].Equals("WkSInUse", StringComparison.CurrentCultureIgnoreCase) )
                    {
                        strQueryString = "?Error=WkSInUse";
                    }
                    else if ( objQueryString["Error"].Equals("WkSDisconnected", StringComparison.CurrentCultureIgnoreCase) )
                    {
                        strQueryString = "?Error=WkSDisconnected";
                    }
                }
            }
            Response.Redirect( "login.aspx" + AppendTenantIdToQuery(strQueryString) );
        }
    }

    // BUGBUG: Temporary workaround while we need to expose the tenant ID as a query string to end-users
    private const string tenantIdLabel = "tenantId";
    public static string AppendTenantIdToQuery(string strQueryString)
    {
        if(HttpContext.Current.Request.QueryString != null)
        {
            if(!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString[tenantIdLabel]))
            {
                string strTenantIdParams = tenantIdLabel + "=" + HttpUtility.UrlEncode(HttpContext.Current.Request.QueryString[tenantIdLabel]);
                if(String.IsNullOrEmpty(strQueryString))
                {
                    return "?" + strTenantIdParams; 
                }
                else
                {
                    return strQueryString + "&" + strTenantIdParams;
                }
            }
        }
        
        return strQueryString;
    }

</script>
