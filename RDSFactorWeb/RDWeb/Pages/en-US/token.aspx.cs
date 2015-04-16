using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RADAR;
using System.Configuration;


public partial class Pages_en_US_token : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string tmpUser = Request.QueryString["User"];
        string DomainUserName = tmpUser.Replace("UserName:s:", "");
        HttpCookie sessionId = new HttpCookie("RadiusSessionId");
        sessionId = Request.Cookies["RadiusSessionId"];

        // Read the cookie information and display it.
        if (sessionId != null)
        {
            string RadiusServer = ConfigurationManager.AppSettings["RadiusServer"];
            string RadiusSecret = ConfigurationManager.AppSettings["RadiusSecret"];

            RADIUSClient client = new RADIUSClient(RadiusServer, 1812, RadiusSecret);

            VendorSpecificAttribute vsa = new VendorSpecificAttribute(VendorSpecificType.Generic, "LAUNCH");
            RADIUSAttributes atts = new RADIUSAttributes();
            vsa.SetRADIUSAttribute(ref atts);

            try
            {
                RADIUSPacket response = client.Authenticate(DomainUserName, sessionId.Value, atts);
                if (response.Code == RadiusPacketCode.AccessAccept)
                {
                    Response.Write("Ready to launch application. Granted access!");
                }
                else
                {
                    Response.Write("Failure to authenticate session launch");
                }
            }
            catch (Exception ex)
            {
                Response.Write("Exception!! failure. " + ex.Message);
            }
        }
        else
        {
            // redrect to login Response.Write("not found");
        }
    }
}