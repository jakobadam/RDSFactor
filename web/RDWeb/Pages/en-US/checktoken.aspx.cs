using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

using RADAR;

public partial class CheckToken : System.Web.UI.Page
{

    String radiusServer = ConfigurationManager.AppSettings["RadiusServer"];
    String radiusSharedSecret = ConfigurationManager.AppSettings["RadiusSecret"];

    RADIUSClient radiusClient;
    String username;
    String token;

    public CheckToken()
    {
        radiusClient = new RADIUSClient(radiusServer, 1812, radiusSharedSecret);
    }

    // Check validity of token (radius session id) by authenticating against 
    // the RADIUS server
    //
    // Called when clicking on applications
    // 
    // Returns 401 if not valid
    protected void Page_Load(object sender, EventArgs e)
    {
        username = (string)Session["DomainUserName"];
        HttpCookie tokenCookie = Request.Cookies["RadiusSessionId"];

        if (tokenCookie == null)
        {
            throw new HttpException(401, "Token required");
        }
        token = tokenCookie.Value;

        VendorSpecificAttribute vsa = new VendorSpecificAttribute(VendorSpecificType.Generic, "LAUNCH");
        RADIUSAttributes atts = new RADIUSAttributes();
        vsa.SetRADIUSAttribute(ref atts);

        try
        {
            RADIUSPacket response = radiusClient.Authenticate(username, token, atts);
            if (response.Code == RadiusPacketCode.AccessAccept)
            {
                Response.Write("Ready to launch application. Granted access!");
            }
            else
            {
                throw new HttpException(401, "Token is no longer valid!");
            }
        }
        catch (Exception ex)
        {
            throw new HttpException(500, "Exception! failure. " + ex.Message);
        }
    }
}