<%@ Page Language="C#" %>
<HTML>
<HEAD>
</HEAD>
<BODY>
</BODY>
</HTML>

    <script language="C#" runat="server">

    void goToFolder(string getLangVal)
    {
        Response.Redirect(getLangVal + "/Default.aspx" + Request.Url.Query,true);
    }

    void Page_Load(Object sender, EventArgs e)
    {
      // Deny requests with "additional path information"
      if (Request.PathInfo.Length != 0)
      {
            Response.StatusCode = 404;
            Response.End();
      }
            
      string langCode = null;
      System.Globalization.CultureInfo culture;

      // For each request initialize the culture values with
      // the user language as specified by the browser.
      if (Request.UserLanguages != null)
      {
            for (int i = 0; i < Request.UserLanguages.Length; i++)
            {
                  string strLanguage = Request.UserLanguages[i];
 
                  // Note that the languages other than the first are in 
                  // the format of
                  // "<Language>;<q=<weight>>" (e.g., "en-us;q=0.5")
                  if (strLanguage.IndexOf(';') >= 0)
                  {
                        strLanguage = strLanguage.Substring(0, strLanguage.IndexOf(';'));
                  }
 
                  try
                  {
                      culture = System.Globalization.CultureInfo.CreateSpecificCulture(strLanguage);

                      // Make sure that the directory is present
                      string physicalPath = Server.MapPath(culture.Name + "/" + "default.aspx");

                      if ((physicalPath.Length != 0) && (System.IO.File.Exists(physicalPath)))
                      {
                          langCode = culture.Name;
      
                          break;
                      }
                  }
                  catch (Exception)
                  {
                      // Some cultures are not supported and will cause an 
                      // exception to throw.  In this case, 
                      // we ignore the exception and try the next culture 
                      // based on client language preference.  
                      // Note that if no culture can be set properly, then 
                      // the default culture will be used.
                  }
            }
      }

      if (String.IsNullOrEmpty(langCode))
      {
          // Default to Installed language
          culture = System.Globalization.CultureInfo.InstalledUICulture;

          // But first make sure that the directory is present
          string physicalPath = Server.MapPath(culture.Name + "/" + "default.aspx");

          if ((physicalPath.Length != 0) && (System.IO.File.Exists(physicalPath)))
          {
              langCode = culture.Name;
          }
          else
          {
		try
		{
          		// In some cases we have to use common culture parents to determine where to redirect
			// We gather up the hierarchies of the ui culture and of installed language cultures
			// comparing their hierarchies until we find some sort of match
			physicalPath = Server.MapPath(".");
			System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(physicalPath);
			System.IO.DirectoryInfo[] cultureDirectories = dirInfo.GetDirectories("*-*");
			langCode = "";

			while(culture != culture.Parent && culture.Parent != null)
			{
				culture = culture.Parent;
			
				foreach(System.IO.DirectoryInfo cultureDir in cultureDirectories)
				{
					System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.CreateSpecificCulture(cultureDir.Name);

					while(ci != ci.Parent && ci.Parent != null)
					{
						ci = ci.Parent;

						if(ci.Name == culture.Name)
						{
							langCode = cultureDir.Name;
							break;
						}
					}
				}
				
				if(langCode != "")
					break;
			}

		}
		catch(Exception)
		{
			// See above about cultures and exceptions, in this case we fallback to en-us
		}
          }

	     // fallback to english if all else fails
          physicalPath = Server.MapPath(langCode + "/" + "default.aspx");
          if ((physicalPath.Length == 0) || !(System.IO.File.Exists(physicalPath)))
          {
              langCode = "en-us";
          }
      }

      goToFolder(langCode);
      return;
    }

    </script>
