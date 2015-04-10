using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RadiusClient;
using System.Configuration;


public partial class Pages_en_US_token : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string tmpUser = Request.QueryString["User"];
        string DomainUserName = tmpUser.Replace("UserName:s:", "");
     //   Label1.Text = DomainUserName;
        HttpCookie myCookie = new HttpCookie("RadiusSessionId");
        myCookie = Request.Cookies["RadiusSessionId"];

        // Read the cookie information and display it.
        if (myCookie != null)
        {
           
            string RadiusServer = ConfigurationManager.AppSettings["RadiusServer"];
            string RadiusSecret = ConfigurationManager.AppSettings["RadiusSecret"];
            Radius_Client myRadius = new Radius_Client(RadiusServer, 1812);
          
            RADIUSPacket rp = default(RADIUSPacket);

            VendorSpecificAttribute vsa = new VendorSpecificAttribute(VendorSpecificType.Generic, "LAUNCH");
            RADIUSAttributes atts = new RADIUSAttributes();
           

      //  Dim ost As New RADIUSAttribute(RadiusAttributeType.VendorSpecific, att.VendorName & att.VendorType & att.VendorValue)



            vsa.SetRADIUSAttribute(ref atts);

     //       RADIUSAttribute AppLaunch = new RADIUSAttribute(RadiusAttributeType.VendorSpecific, "LAUNCH");
            //RADIUSAttribute AppLaunch = new RADIUSAttribute(RadiusAttributeType.VendorSpecific, att.VendorName + att.VendorType + att.VendorValue);
          //  atts.Add(AppLaunch);
           // myRadius.Debug = true;
            try
            {
                rp = myRadius.Authenticate(RadiusSecret, DomainUserName, myCookie.Value, atts);

                if ((int)rp.Code == 2)
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

          //  Response.Write("<p>" + myCookie.Name + "<p>" + myCookie.Value);
        }
        else
        {
            // redrect to login Response.Write("not found");
        }
    }
}