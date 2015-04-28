using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_en_US_cook : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        HttpCookie myCookie = new HttpCookie("RadiusSessionId");
        DateTime now = DateTime.Now;

        // Set the cookie value.
        myCookie.Value = now.ToString();
        // Set the cookie expiration date.
        myCookie.Expires = now.AddMinutes(480);

        // Add the cookie.
        Response.Cookies.Add(myCookie);

        Response.Write("<p> The cookie has been written.");
    }
}