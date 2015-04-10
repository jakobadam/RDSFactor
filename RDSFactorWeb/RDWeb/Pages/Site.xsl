<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
                xmlns:appfeed="http://schemas.microsoft.com/ts/2007/05/tswf"
                xmlns:str="urn:microsoft.com:rdwastrings">
  <xsl:output method="html" doctype-public="-//W3C//DTD HTML 4.01//EN" doctype-system="http://www.w3.org/TR/html4/strict.dtd" encoding="UTF-8"/>

  <xsl:variable name="baseurl" select="/RDWAPage/@baseurl"/>
  <xsl:variable name="rdcinstallurl" select="/RDWAPage/AppFeed[1]/@rdcinstallurl"/>
  <xsl:variable name="showpubliccheckbox" select="/RDWAPage/AppFeed[1]/@showpubliccheckbox = 'true'"/>
  <xsl:variable name="showoptimizeexperience" select="/RDWAPage/AppFeed[1]/@showoptimizeexperience = 'true'"/>
  <xsl:variable name="optimizeexperiencestate" select="/RDWAPage/AppFeed[1]/@optimizeexperiencestate = 'true'"/>
  <xsl:variable name="privatemode" select="/RDWAPage/AppFeed[1]/@privatemode = 'true'"/>
  <xsl:variable name="appfeedcontents" select="/RDWAPage/AppFeed[1]"/>
  <xsl:variable name="strings" select="document(concat($baseurl,'RDWAStrings.xml'))/str:strings/string"/>
  
  <!-- top level document structure -->
  <xsl:template match="/RDWAPage">
    <html>
      <head id="Head1">
        <xsl:if test="$baseurl">
        <base><xsl:attribute name="href"><xsl:value-of select="$baseurl"/></xsl:attribute></base>
        </xsl:if>
        <title ID="PAGE_TITLE"><xsl:value-of select="$strings[@id = 'PageTitle']"/></title>
        <meta name="ROBOTS" content="NOINDEX, NOFOLLOW"/>
        <meta http-equiv="X-UA-Compatible" content="IE=9"/>
        <link href="tswa.css" rel="stylesheet" type="text/css" />
        <xsl:apply-templates select="Style"/>
          
        <script language="javascript" type="text/javascript" src='../renderscripts.js'/>
        <script language="javascript" type="text/javascript" src='../webscripts-domain.js'/>
        <script language="javascript" type="text/javascript">
          var sHelpSource = &quot;<xsl:value-of select="@helpurl"/>&quot;;          
          <xsl:value-of select="HeaderJS[1]"/>
          <xsl:if test="$baseurl">
          strBaseUrl = &quot;<xsl:value-of select="$baseurl"/>&quot;;
          </xsl:if>
        </script>
      </head>

      <body>
        <xsl:apply-templates select="BodyAttr/@*"/>

        <noscript><xsl:copy-of select="$strings[@id = 'NoScriptWarning']/node()"/></noscript>

        <xsl:apply-templates select="@domainuser"/>

        <xsl:comment>Page Table</xsl:comment>
        <table border="0" align="center" cellpadding="0" cellspacing="0">

          <xsl:comment>1st Row (Empty)</xsl:comment>
          <tr>
            <td height="20"></td>
          </tr>

          <xsl:comment>2nd Row (Top Border Images)</xsl:comment>
          <tr>
            <td>
              <table width="932" border="0" cellpadding="0" cellspacing="0">
                <tr>
                  <td width="15" height="15" background="../images/top_left.png"></td>
                  <td width="902" height="15" background="../images/top_mid.png"></td>
                  <td width="15" height="15" background="../images/top_right.png"></td>
                </tr>
              </table>
            </td>
          </tr>

          <xsl:comment>3rd Row (Main)</xsl:comment>
          <tr>
            <td>

              <xsl:comment>Contents and Controls Table (1 Row, 3 Columns)</xsl:comment>
              <table width="932" border="0" cellpadding="0" cellspacing="0">

                <tr>
                  <xsl:comment>Col 1 - Left Border Images</xsl:comment>
                  <td width="15" background="../images/left_mid.png">&#160;</td>

                  <xsl:comment>Col 2 - Contents and Controls</xsl:comment>
                  <td width="902">

                    <xsl:comment>Inner Contents and Controls Table (8 Rows, 1 Column)</xsl:comment>
                    <table width="900" border="0" cellpadding="0" cellspacing="0" bgcolor="#FFFFFF" class="pageBorder">

                      <xsl:comment>1st Row - RD Web Access Logo</xsl:comment>
                      <tr>
                        <td height="24" bgcolor="#7EA3BE">
                          <table border="0" align="right" cellpadding="0" cellspacing="0">
                            <tr>
                              <td>
                                <img src="../images/logo_01.png" width="16" height="16"/>
                              </td>
                              <td width="5">&#160;</td>
                              <td class="headingTSWA">
                                <xsl:value-of select="$strings[@id = 'HeadingRDWA']"/>
                              </td>
                              <td width="9">&#160;</td>
                            </tr>
                          </table>
                        </td>
                      </tr>

                      <xsl:comment>Row - Empty</xsl:comment>
                      <tr>
                        <td height="1"></td>
                      </tr>

                      <xsl:comment>3rd Row - Customizable Banner and Text Row</xsl:comment>
                      <tr>
                        <td height="90" background="../images/banner_01.jpg">
                          <table border="0" cellspacing="0" cellpadding="0">
                            <tr>
                              <td width="30">&#160;</td>

                              <xsl:comment>Replaceable Company Logo Image</xsl:comment>
                              <td>
                                <img src="../images/logo_02.png" width="48" height="48"/>
                              </td>
                              <td width="10">&#160;</td>

                              <xsl:comment>Replaceable Company Logo Text and Application Type</xsl:comment>
                              <td>
                                <table border="0" cellspacing="0" cellpadding="0">
                                  <tr>
                                    <td class="headingCompanyName"><xsl:value-of select="@workspacename"/></td>
                                  </tr>
                                  <tr>
                                    <td class="headingApplicationName">
                                      <xsl:value-of select="$strings[@id = 'HeadingApplicationName']"/>
                                    </td>
                                  </tr>
                                </table>
                              </td>

                            </tr>
                          </table>
                        </td>
                      </tr>

                      <xsl:comment>4th Row - Navigation Table</xsl:comment>
                      <xsl:choose>
                        <xsl:when test="NavBar[1]">
                          <xsl:apply-templates select="NavBar[1]"/>
                        </xsl:when>
                        <xsl:otherwise>
                          <tr>
                            <td class="cellSecondaryNavigationBar" height="40">
                              <xsl:comment>Login Page only contains Help link</xsl:comment>
                              <table border="0" cellpadding="0" cellspacing="0" class="linkSecondaryNavigiationBar">
                                <tr>
                                  <td>
                                    <a id='PORTAL_HELP' href="javascript:onClickHelp()">
                                      <xsl:value-of select="$strings[@id = 'Help']"/>
                                    </a>
                                  </td>
                                  <td width="30">&#160;</td>
                                </tr>
                              </table>
                            </td>
                          </tr>
                        </xsl:otherwise>
                      </xsl:choose>

                      <xsl:comment>5th Row - Image</xsl:comment>
                      <tr>
                        <td>
                          <img src="../images/bar_03.jpg" width="900" height="10"/>
                        </td>
                      </tr>

                      <xsl:comment>6th Row - Empty</xsl:comment>
                      <tr>
                        <td height="10"></td>
                      </tr>

                      <xsl:comment>7th Row - Visible Controls</xsl:comment>
                      <tr>
                        <td>
                          <xsl:choose>
                            <xsl:when test="HTMLMainContent[1]">
                              <xsl:copy-of select="HTMLMainContent[1]/*"/>
                            </xsl:when>
                            <xsl:when test="AppFeed[1]">
                              <div id="homemain">
                                <div id="content">
                                  <xsl:apply-templates select="AppFeed[1]"/>
                                </div>
                              </div>
                            </xsl:when>
                          </xsl:choose>
                        </td>
                      </tr>

                      <xsl:copy-of select="ExtraRows[1]/*"/>

                      <xsl:comment>8th Row - Footer</xsl:comment>
                      <tr>
                        <td height="50" background="../images/banner_02.jpg" bgcolor="#D7E7F7">
                          <table width="100%" border="0" cellpadding="0" cellspacing="0">
                            <tr>
                              <xsl:comment>Windows Server Logo</xsl:comment>
                              <td>
                                <table border="0" cellspacing="0" cellpadding="0">
                                  <tr>
                                    <td width="30">&#160;</td>
                                    <td>
                                      <img src="../images/WS_h_c.png" width="143" height="20"/>
                                    </td>
                                  </tr>
                                </table>
                              </td>
                              <xsl:comment>Microsoft Logo</xsl:comment>
                              <td class="cellMSLogo">
                                <table border="0" cellspacing="0" cellpadding="0">
                                  <tr>
                                    <td>
                                      <img src="../images/mslogo_black.png" width="63" height="10"/>
                                    </td>
                                    <td width="30">&#160;</td>
                                  </tr>
                                </table>
                              </td>
                            </tr>
                          </table>
                        </td>
                      </tr>
                      
                    </table>
                  </td>
                  <xsl:comment>Col 3 - Right Border Images</xsl:comment>
                  <td width="15" background="../images/right_mid.png">&#160;</td>
                </tr>

              </table>
            </td>
          </tr>

          <xsl:comment>4th Row (Bottom Border Images) </xsl:comment>
          <tr>
            <td>
              <table width="932" border="0" cellpadding="0" cellspacing="0">
                <tr>
                  <td width="15" height="15" background="../images/bottom_left.png"></td>
                  <td width="902" height="15" background="../images/bottom_mid.png"></td>
                  <td width="15" height="15" background="../images/bottom_right.png"></td>
                </tr>
              </table>
            </td>
          </tr>


          <xsl:comment>5th Row (Empty)</xsl:comment>
          <tr>
            <td height="20"></td>
          </tr>
        </table>
      </body>      
    </html>
  </xsl:template>
  
  <xsl:template match="/RDWAPage/Style">
    <xsl:choose>
      <xsl:when test="@condition">
        <xsl:comment>[<xsl:value-of select="@condition"/>]&gt;
          &lt;style&gt;
            <xsl:value-of select="."/>
          &lt;/style&gt;
          &lt;![endif]</xsl:comment>
      </xsl:when>
      <xsl:otherwise>
        <style>
          <xsl:value-of select="."/>
        </style>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="/RDWAPage/BodyAttr/@*">
    <xsl:attribute name="{name(.)}"><xsl:value-of select="."/></xsl:attribute>
  </xsl:template>

  <xsl:template match="/RDWAPage/@domainuser">
    <form name="FrmUserInfo" id="FrmUserInfo">
      <input type="hidden" id="DomainUserName" name="DomainUserName">
        <xsl:attribute name="value"><xsl:value-of select="."/></xsl:attribute>
      </input>
    </form>
  </xsl:template>
  
  <xsl:template match="/RDWAPage/NavBar">
    <tr>
      <td height="40">

        <table width="100%" border="0" cellpadding="0" cellspacing="0">
          <tr>
            <td>

              <table border="0" cellpadding="0" cellspacing="0" class="linkPrimaryNavigiationBar">
                <tr>
                  <td width="30">&#160;</td>
                  <xsl:apply-templates select="Tab"/>
                </tr>
              </table>

            </td>

            <td class="cellSecondaryNavigationBar">

              <table border="0" cellpadding="0" cellspacing="0" class="linkSecondaryNavigiationBar">
                <tr>
                  <td>
                    <a id='PORTAL_HELP' href="javascript:onClickHelp()">
                      <xsl:value-of select="$strings[@id = 'Help']"/>
                    </a>
                  </td>
                  <xsl:if test="@showsignout = 'true'">
                    <td width="15">&#160;</td>
                    <td class="dividerInNavigationBar">|</td>
                    <td width="15">&#160;</td>
                    <td>
                      <a id='PORTAL_SIGNOUT' href="javascript:onUserDisconnect()" target="_self">
                        <xsl:value-of select="$strings[@id = 'SignOut']"/>
                      </a>
                    </td>
                  </xsl:if>
                  <td width="30">&#160;</td>
                </tr>
              </table>

            </td>
          </tr>
        </table>

      </td>
    </tr>
  </xsl:template>

  <xsl:template match="/RDWAPage/NavBar/Tab">
    <xsl:if test="position() != 1">
      <td width="15">&#160;</td>
      <td class="dividerInNavigationBar">|</td>
      <td width="15">&#160;</td>
    </xsl:if>
    <xsl:choose>
      <xsl:when test="../@activetab = @id">
        <td class="headingForActivePageInNavigationBar">
          <xsl:attribute name='id'><xsl:value-of select='@id'/></xsl:attribute>
          <xsl:value-of select='.'/>
        </td>
      </xsl:when>
      <xsl:otherwise>
        <td>
          <a target="_self">
            <xsl:attribute name='href'><xsl:value-of select='@href'/></xsl:attribute>
            <xsl:attribute name='id'><xsl:value-of select='@id'/></xsl:attribute>
            <xsl:value-of select='.'/>
          </a>
        </td>
      </xsl:otherwise>
    </xsl:choose>
      
  </xsl:template>

  <xsl:template match="/RDWAPage/AppFeed">    
    <xsl:apply-templates select="$appfeedcontents/appfeed:ResourceCollection"/>
  </xsl:template>

  <xsl:template match="appfeed:ResourceCollection">
    <xsl:variable name="feedidprefix">AppFeed_<xsl:value-of select="generate-id()"/></xsl:variable>
    <div style="display:none;height:0px; width:0px; background-color:Transparent;">
      <xsl:attribute name="id"><xsl:value-of select="$feedidprefix"/>oDivMsRdpClient</xsl:attribute>

      <script type="text/javascript" language="javascript">
        var MsRdpClientShell;
        var ActiveXMode;

        function <xsl:value-of select="$feedidprefix"/>window_onload() {

          ActiveXMode = <xsl:value-of select="$feedidprefix"/>LoadControl();
          
          if (ActiveXMode &amp;&amp; <xsl:value-of select="$feedidprefix"/>Controls.PORTAL_REMOTE_DESKTOPS != null)
          {
            <xsl:value-of select="$feedidprefix"/>Controls.PORTAL_REMOTE_DESKTOPS.style.display = "inline";
          }

          <xsl:value-of select="$feedidprefix"/>Controls.PleaseWait.style.display="none";
          
          <xsl:value-of select="$feedidprefix"/>EnableAppDisplay();
        }

        
        function <xsl:value-of select="$feedidprefix"/>EnableAppDisplay() {
          <xsl:value-of select="$feedidprefix"/>Controls.AppDisplay.style.display = "block";
        
          if (ActiveXMode)
          {
            <xsl:if test="$showpubliccheckbox">
            <xsl:value-of select="$feedidprefix"/>Controls.contentPublicCheckbox.style.display = "block";
            </xsl:if>

            <xsl:if test="$showoptimizeexperience">
            <xsl:value-of select="$feedidprefix"/>Controls.contentShowOptimizeExperience.style.display = "block";
            </xsl:if>
          }
        }
        
        
        function <xsl:value-of select="$feedidprefix"/>LoadControl() {
          var retval = true;
          
          try
          {
            var WebAccessControlPresent = <xsl:value-of select="$feedidprefix"/>IsWebAccessControlPresent();
          
            var obj = "&lt;object type='application/x-oleobject'";
            obj += "id='MsRdpClient' name='MsRdpClient'";
            obj += "onerror='<xsl:value-of select="$feedidprefix"/>OnControlLoadError'";
            obj += "height='0' width='0'";
            if ( WebAccessControlPresent ) {
              obj += "classid='CLSID:6A5B0C7C-5CCB-4F10-A043-B8DE007E1952'>";
            }
            else {
              obj += "classid='CLSID:7390f3d8-0439-4c05-91e3-cf5cb290c3d0'>";
            }
            obj += "&lt;/object&gt;";
            obj += "&lt;script language='javascript' type='text/javascript'&gt; var MsRdpClient = document.getElementById('MsRdpClient'); &lt;\/script&gt;";
           
            document.getElementById("<xsl:value-of select="$feedidprefix"/>oDivMsRdpClient").insertAdjacentHTML("beforeEnd",obj); 
            if ( WebAccessControlPresent ) {
              MsRdpClientShell = MsRdpClient;
            }
            else {
              MsRdpClientShell = MsRdpClient.MsRdpClientShell;
            }
              
            if (!MsRdpClient || MsRdpClientShell == null) {
              retval = false;
              <xsl:value-of select="$feedidprefix"/>OnControlLoadError();
            }
          }
          catch(e)
          {
            retval = false;
          }         
          
          return retval;
        }

        
        function <xsl:value-of select="$feedidprefix"/>OnControlLoadError() {
          <xsl:value-of select="$feedidprefix"/>ActiveXMode = false;
        }


        function <xsl:value-of select="$feedidprefix"/>IsWebAccessControlPresent() {
          var retval = false;
          try {
              var WebAccessControl = new ActiveXObject("MsRdpWebAccess.MsRdpClientShell");
              if ( WebAccessControl ) {
                  retval = true;
              }
          }
          catch(e) {
              retval = false;
          }
          return retval;
        }

        
        <xsl:if test="$showpubliccheckbox">
        function <xsl:value-of select="$feedidprefix"/>toggle(e) {
          if (e.id == "<xsl:value-of select="$feedidprefix"/>p2M") {
            <xsl:value-of select="$feedidprefix"/>Controls.p2M.style.display = "none";
            <xsl:value-of select="$feedidprefix"/>Controls.p2L.style.display = "";
            <xsl:value-of select="$feedidprefix"/>Controls.privateMore.style.display = "";
            <xsl:value-of select="$feedidprefix"/>Controls.contentPublicCheckbox.className = "tswa_PublicCheckboxMore";
          <xsl:choose>
            <xsl:when test="$showoptimizeexperience = 'true'">
            <xsl:value-of select="$feedidprefix"/>Controls.contentShowOptimizeExperience.className = "tswa_ShowOptimizeExperienceShiftedUp";
            <xsl:value-of select="$feedidprefix"/>Controls.AppDisplay.style.height = (<xsl:value-of select="$feedidprefix"/>Controls.AppDisplay.offsetHeight - 60) + "px"; //  440px
          </xsl:when>
            <xsl:otherwise>
            <xsl:value-of select="$feedidprefix"/>Controls.AppDisplay.style.height = (<xsl:value-of select="$feedidprefix"/>Controls.AppDisplay.offsetHeight - 40) + "px"; //  440px
          </xsl:otherwise>
          </xsl:choose>
          } else if (e.id == "<xsl:value-of select="$feedidprefix"/>p2L") {
            <xsl:value-of select="$feedidprefix"/>Controls.p2M.style.display = "";
            <xsl:value-of select="$feedidprefix"/>Controls.p2L.style.display = "none";
            <xsl:value-of select="$feedidprefix"/>Controls.privateMore.style.display = "none";
            <xsl:value-of select="$feedidprefix"/>Controls.contentPublicCheckbox.className = "tswa_PublicCheckboxLess";

          <xsl:choose>
            <xsl:when test="$showoptimizeexperience = 'true'">
            <xsl:value-of select="$feedidprefix"/>Controls.contentShowOptimizeExperience.className = "tswa_ShowOptimizeExperience";
            <xsl:value-of select="$feedidprefix"/>Controls.AppDisplay.style.height = (<xsl:value-of select="$feedidprefix"/>Controls.AppDisplay.offsetHeight + 60) + "px"; //  440px
          </xsl:when>
            <xsl:otherwise>
            <xsl:value-of select="$feedidprefix"/>Controls.AppDisplay.style.height = (<xsl:value-of select="$feedidprefix"/>Controls.AppDisplay.offsetHeight + 40) + "px"; //  440px
          </xsl:otherwise>
          </xsl:choose>
          }
        }
        </xsl:if>
      </script>
    </div>

    <div>
      <xsl:attribute name='id'><xsl:value-of select="$feedidprefix"/>content</xsl:attribute>

      <div class="tswa_appboard" style="display:none;">
        <xsl:attribute name='id'><xsl:value-of select="$feedidprefix"/>PleaseWait</xsl:attribute>
        <table cellspacing="0" cellpadding="0" border="0" width="100%" height="100%">
          <tr>
            <td align="center" valign="middle">
              <xsl:copy-of select="$strings[@id = 'SearchingForApps']/node()"/><img src="../images/rapwait.gif" style="width:122px;height:32px;vertical-align:middle;"/>
            </td>
          </tr>
        </table>
      </div>

      <script type="text/javascript" language="javascript">
        document.getElementById("<xsl:value-of select="$feedidprefix"/>PleaseWait").style.display="block";
      </script>

      <div class="tswa_appboard tswa_appdisplay" style="display:none;top:0px;overflow:auto;">
        <xsl:attribute name='id'><xsl:value-of select="$feedidprefix"/>AppDisplay</xsl:attribute>
        
        <!-- deal with folder icons and labels -->
        <div class="tswa_CurrentFolderLabel"><xsl:value-of select="$strings[@id = 'CurrentFolder']"/><span><xsl:attribute name='id'><xsl:value-of select="$feedidprefix"/>CurrentFolderPath</xsl:attribute><xsl:value-of select="appfeed:Publisher[1]/@DisplayFolder"/></span></div>
          
        <!-- Display the parent folder icon, if needed -->
        <xsl:if test="appfeed:Publisher[1]/@DisplayFolder != '/'">
          <div tabindex="0"
               onkeypress="onmouseup()"
               class="tswa_up_boss" 
               onmouseover='tswa_bossOver(this)' 
               onmouseout='tswa_bossOut(this)'
               title='Up'>
            <xsl:attribute name='onmouseup'>window.location.href='<xsl:value-of select='$baseurl'/>Default.aspx'</xsl:attribute>
            <div class="tswa_boss_spacer">
              <img class="tswa_vis0" src='../images/ivmo.png' />
              <img class="tswa_iconimg" src="../images/up.png"/>
              <div class="tswa_ttext">
                <xsl:value-of select="$strings[@id = 'ParentFolder']"/>
              </div>
            </div>
          </div>
        </xsl:if>

        <!-- display icons for any subfolders -->
        <xsl:apply-templates select="appfeed:Publisher[1]/appfeed:SubFolders[1]/appfeed:Folder">
          <xsl:sort select="@Name"/>
        </xsl:apply-templates>
        
        <xsl:apply-templates select="appfeed:Publisher[1]/appfeed:Resources[1]/appfeed:Resource"/>
      </div>

      <xsl:if test="$showoptimizeexperience = 'true'">
        <div class='tswa_ShowOptimizeExperience' style="display:none;">
          <xsl:attribute name='id'><xsl:value-of select="$feedidprefix"/>contentShowOptimizeExperience</xsl:attribute>
          <table width="100%" cellspacing="0" cellpadding="0" border="0">
            <tr>
              <td valign="top" style="width:30px;font-size:12px;">
                <input type="checkbox" value="ON">
                  <xsl:attribute name='id'><xsl:value-of select="$feedidprefix"/>chkShowOptimizeExperience</xsl:attribute>
                  <xsl:if test="$optimizeexperiencestate = 'true'">
                    <xsl:attribute name="checked">checked</xsl:attribute>
                  </xsl:if>
                </input>
              </td>
              <td valign="top" style="padding-top:2px;font-size:12px;">
                <xsl:attribute name='id'><xsl:value-of select="$feedidprefix"/>ShowOptimizeExperienceText</xsl:attribute>
                <xsl:value-of select="$strings[@id = 'OptimizeMyExperience']"/>
              </td>
            </tr>
          </table>
        </div>
      </xsl:if>

      <xsl:if test="$showpubliccheckbox">
        <div class='tswa_PublicCheckboxLess' style="display:none;">
          <xsl:attribute name='id'><xsl:value-of select="$feedidprefix"/>contentPublicCheckbox</xsl:attribute>

          <table width="100%" cellspacing="0" cellpadding="0" border="0">
            <tr>
              <td valign="top" style="width:30px;font-size:12px;">
                <input type="checkbox" value="ON">
                  <xsl:attribute name='id'><xsl:value-of select="$feedidprefix"/>PublicCheckbox</xsl:attribute>
                  <xsl:if test="$privatemode = 'true'">
                    <xsl:attribute name="checked">checked</xsl:attribute>
                  </xsl:if>
                </input>
              </td>
              <td valign="top" style="padding-top:2px;font-size:12px;">
                <xsl:attribute name='id'><xsl:value-of select="$feedidprefix"/>SecurityText2</xsl:attribute>
                <xsl:value-of select="$strings[@id = 'PrivateComputer']"/>
                <span>
                  <xsl:attribute name='id'><xsl:value-of select="$feedidprefix"/>p2M</xsl:attribute>
                  <xsl:attribute name='onclick'><xsl:value-of select="$feedidprefix"/>toggle(this);</xsl:attribute>
                  (<a><xsl:attribute name="href">javascript:<xsl:value-of select="$feedidprefix"/>toggle(this)</xsl:attribute><xsl:value-of select="$strings[@id = 'MoreInformation']"/></a>)


                </span><span style="display:none">
                  <xsl:attribute name='id'><xsl:value-of select="$feedidprefix"/>privateMore</xsl:attribute>
                  <br />
                  <xsl:value-of select="$strings[@id = 'PrivateMore']"/>
                  <span>
                    <xsl:attribute name='id'><xsl:value-of select="$feedidprefix"/>p2L</xsl:attribute>
                    <xsl:attribute name='onclick'><xsl:value-of select="$feedidprefix"/>toggle(this);</xsl:attribute>
                    (<a><xsl:attribute name="href">javascript:<xsl:value-of select="$feedidprefix"/>toggle(this)</xsl:attribute><xsl:value-of select="$strings[@id = 'HideMore']"/></a>)
                  </span>
                </span>
              </td>
            </tr>
          </table>
        </div>
      </xsl:if>

    </div>
    <script type="text/javascript" language="javascript">
      
      var <xsl:value-of select="$feedidprefix"/>Controls = {
            AppDisplay: document.getElementById("<xsl:value-of select="$feedidprefix"/>AppDisplay"),
            contentPublicCheckbox: document.getElementById("<xsl:value-of select="$feedidprefix"/>contentPublicCheckbox"),
            PublicCheckbox: document.getElementById("<xsl:value-of select="$feedidprefix"/>PublicCheckbox"),
            contentShowOptimizeExperience: document.getElementById("<xsl:value-of select="$feedidprefix"/>contentShowOptimizeExperience"),
            chkShowOptimizeExperience: document.getElementById("<xsl:value-of select="$feedidprefix"/>chkShowOptimizeExperience"),
            p2M: document.getElementById("<xsl:value-of select="$feedidprefix"/>p2M"),
            p2L: document.getElementById("<xsl:value-of select="$feedidprefix"/>p2L"),
            privateMore: document.getElementById("<xsl:value-of select="$feedidprefix"/>privateMore"),
            PleaseWait: document.getElementById("<xsl:value-of select="$feedidprefix"/>PleaseWait"),
      PORTAL_REMOTE_DESKTOPS: document.getElementById("PORTAL_REMOTE_DESKTOPS")
      };

      function tswa_bossOver(obj){
        obj.children[0].children[0].className = 'tswa_vis1';
        obj.children[0].style.padding = "10px 3px 2px 2px";
      }
      function tswa_bossOut(obj){
        obj.children[0].children[0].className = "tswa_vis0";
        obj.children[0].style.padding = "12px 1px 0px 4px";
      }

      function goRDP(pid, rdpContents, url) {
      if (ActiveXMode) {
        try {
          goRDPAx(pid, rdpContents);
        } catch (e) {
          location.href = url;
        }
      }
      else {
      location.href = url;
      }
      }


      function goRDPAx(pid, arg) {
      var strRdpFileContents = arg;

      // Try adding the User Name to RdpContents.
      if ( typeof getUserNameRdpProperty == 'function' ) {
      strRdpFileContents += getUserNameRdpProperty();
      }

      <xsl:choose>
          <xsl:when test="$showpubliccheckbox">        
        MsRdpClientShell.PublicMode = !<xsl:value-of select="$feedidprefix"/>Controls.PublicCheckbox.checked;
        </xsl:when>
          <xsl:otherwise>
        MsRdpClientShell.PublicMode = <xsl:choose>
              <xsl:when test="not($privatemode)">true</xsl:when>
              <xsl:otherwise>false</xsl:otherwise>
            </xsl:choose>;
          </xsl:otherwise>
        </xsl:choose>
      
        <xsl:if test="$showoptimizeexperience">
        if (<xsl:value-of select="$feedidprefix"/>Controls.chkShowOptimizeExperience.checked) {
          var objRegExp = new RegExp("connection type:i:([0-9]+)", "i");
          var iIndex = strRdpFileContents.search( objRegExp );
          <!-- Add 'connection type' if it does exist otherwise replace. -->
          if ( -1 == iIndex ) {
            if ( "\\n" != strRdpFileContents.charAt(strRdpFileContents.length-1) ) { 
              strRdpFileContents += "\\r\\n"; 
            }
            strRdpFileContents += "connection type:i:6\\r\\n";
            } else { 
              strRdpFileContents = strRdpFileContents.replace(objRegExp, "connection type:i:6");
            }
        }
        </xsl:if>
      
        MsRdpClientShell.RdpFileContents = unescape(strRdpFileContents);
     
        try {
            MsRdpClientShell.Launch();
        }
        catch(e){
            throw e;
        }
      }

      
      function goNonRDP(pid, arg) {
        try {
          location.href = unescape(arg);
        }
        catch(e){
          throw e;
        }
      }

      

      <xsl:value-of select="$feedidprefix"/>window_onload();
    </script>
      
  </xsl:template>

  <xsl:template match="appfeed:Resource">
    <div 
      class="tswa_boss" 
      tabindex="0"
      onkeypress="onmouseup()" 
      onmouseover='tswa_bossOver(this)' 
      onmouseout='tswa_bossOut(this)'>
      <xsl:attribute name='onmouseup'>
        <xsl:choose>
          <xsl:when test="appfeed:HostingTerminalServers/appfeed:HostingTerminalServer[1]/appfeed:ResourceFile/@FileExtension = '.rdp' and appfeed:HostingTerminalServers/appfeed:HostingTerminalServer[1]/appfeed:ResourceFile/appfeed:Content and appfeed:HostingTerminalServers/appfeed:HostingTerminalServer[1]/appfeed:ResourceFile/@URL">goRDP(this, '<xsl:value-of select='appfeed:HostingTerminalServers/appfeed:HostingTerminalServer[1]/appfeed:ResourceFile/appfeed:Content'/>', '<xsl:value-of select='appfeed:HostingTerminalServers/appfeed:HostingTerminalServer[1]/appfeed:ResourceFile/@URL'/>');</xsl:when>
          <xsl:otherwise>goNonRDP(this, '<xsl:value-of select='appfeed:HostingTerminalServers/appfeed:HostingTerminalServer[1]/appfeed:ResourceFile/appfeed:Content'/>');</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <xsl:attribute name='title'><xsl:value-of select="@Title"/></xsl:attribute>
      <div class="tswa_boss_spacer">
        <img class="tswa_vis0" src='../images/ivmo.png' />
        <img class="tswa_iconimg">
          <xsl:attribute name="src">
            <xsl:value-of select="appfeed:Icons/appfeed:Icon32[@Dimensions = '32x32' and @FileType = 'Png']/@FileURL"/>
          </xsl:attribute>
        </img>
        <div class="tswa_ttext">
          <xsl:value-of select="@Title"/>
        </div>
      </div>
    </div>
  </xsl:template>

  <xsl:template match="appfeed:Folder">
    <xsl:variable name="folderName" select="substring-after(@Name, '/')"/>
    <div
      class="tswa_folder_boss"
      tabindex="0"
      onkeypress="onmouseup()"
      onmouseover='tswa_bossOver(this)'
      onmouseout='tswa_bossOut(this)'>
      <xsl:attribute name='onmouseup'>window.location.href='<xsl:value-of select='$baseurl'/>Default.aspx/' + encodeURIComponent('<xsl:value-of select='$folderName'/>')</xsl:attribute>
      <xsl:attribute name='title'>
        <xsl:value-of select="$folderName"/>
      </xsl:attribute>
      <div class="tswa_boss_spacer">
        <img class="tswa_vis0" src='../images/ivmo.png' />
        <img class="tswa_iconimg" src="../images/folder.png"/>
        <div class="tswa_ttext">
          <xsl:value-of select="$folderName"/>
        </div>
      </div>
    </div>
  </xsl:template>
  
</xsl:stylesheet>
