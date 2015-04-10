﻿//
//    Copyright (C) Microsoft.  All rights reserved.
//
using Microsoft.TerminalServices.Publishing.Portal;
using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using System.Web.Configuration;
using System.Xml;
using Microsoft.TerminalServices.Publishing.Portal.FormAuthentication;

namespace TSWF_PagesVdir
{
    public partial class TsWebFeed : System.Web.UI.Page
    {
        public string oXML;

        protected void Page_Init(object sender, EventArgs e)
        {
            AuthenticationMode eAuthenticationMode = AuthenticationMode.None;
            string strUserIdentity = "";

            AuthenticationSection objAuthenticationSection = ConfigurationManager.GetSection("system.web/authentication") as AuthenticationSection;
            if ( objAuthenticationSection  != null )
            {
                eAuthenticationMode = objAuthenticationSection.Mode;
            }

            if ( eAuthenticationMode == AuthenticationMode.Forms )
            {
                if ( HttpContext.Current.User.Identity.IsAuthenticated == false )
                {
                    Uri baseUrl = new Uri(PageContentsHelper.GetBaseUri(Context), Request.FilePath + "/..");
                    string queryString = PageContentsHelper.AppendTenantIdToQuery("?ReturnUrl=../WebFeed.aspx");
                    Response.Redirect(baseUrl.ToString() + "default.aspx" + queryString);
                }

                TSFormAuthTicketInfo objTSFormAuthTicketInfo = new TSFormAuthTicketInfo(HttpContext.Current);
                strUserIdentity = objTSFormAuthTicketInfo.UserIdentity;
                //
                // TODO: Need to correctly call this
                //
                //Response.Write("\nshowWorkspaceNotification();");
            }

            HttpContext.Current.Response.AppendHeader("Vary", "Accept");
            
            FeedXmlVersion xmlVersion;
            string contentType;
            ContentNegotiationHelper.GetPreferredXmlVersion(out xmlVersion, out contentType);

            // downlevel requests cannot specify a folder
            if (xmlVersion == FeedXmlVersion.Win7
                && Request.PathInfo.Length != 0)
            {
                Response.StatusCode = 404;
                Response.End();
            }

            HttpContext.Current.Response.ContentType = contentType;

            string rdpRenderMode = HttpContext.Current.Request.Params.Get("rdpRender");
            bool rdpEmbed = !String.IsNullOrEmpty(rdpRenderMode) && rdpRenderMode.Trim().ToLower(CultureInfo.InvariantCulture).Equals("inline");
            
            try
            {
                WebFeed tswf = new WebFeed(rdpEmbed ? RdpType.Both : RdpType.Link, false);
                oXML = tswf.GenerateFeed(strUserIdentity, xmlVersion, Request.PathInfo, true);
            }
            catch (WorkspaceUnknownFolderException)
            {
                Response.StatusCode = 404;
                Response.End();
            }
            catch (InvalidTenantException)
            {
                Response.StatusCode = 404;
                Response.End();
            }
            catch (WorkspaceUnavailableException wue)
            {
                // This exception is raised when we cannot contact the appropriate sources to obtain the workspace information.
                // This is an edge case that can ocurr e.g. if the cpub server we're pointing to is down.
                Response.StatusCode = 503;
                // Write a simple text error string to the body, so that if an administrator happens to be accessing the feed via a browser, we can point them in the right direction
                // We need to wrap this in some sort of tags, otherwise IE does not display the message (since it expects an XML content-type)
                Response.Write("<Error>" + wue.Message + "</Error>");
                Response.End();
            }
        }
    }
}
