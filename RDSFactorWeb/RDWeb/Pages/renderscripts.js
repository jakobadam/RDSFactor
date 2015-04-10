var helpPopup = null;
var strTSWACookieName = "TSWAAuthClientSideCookie";
var strTransparentGif = "../images/clear.gif";
var g_objWorkspace = null;
var g_activeXSSOMode = false;
var g_activeXSSOModeSet = false;
var g_workspaceObjectVersion = -1;
var bFormAuthenticationMode = false;
var iSessionTimeout = 0;
var iConstSecToMilliSec = 1000;
var iConstMinuteToMilliSec = (60 * iConstSecToMilliSec);
var iConstPreAutoDisconnectFactor = (4 / 5);    // To make it 80% of iAutoDisconnectSessionTimeout.
var iConstPreAutoDisconnectThreshold = (2 * iConstSecToMilliSec);    // After which onPreAutoDisconnect should not be set.
var iAutoDisconnectTimerId = null;
var iPreAutoDisconnectTimerId = null;
var iAutoDisconnectSessionTimeout = 0;
var iPreAutoDisconnectSessionTimeout = 0;
var objPageLoadTime = null;
var objLastUserActivityTime = null;
var objXmlHttp = null;
var bAsyncPageRequestSucceeded = false;
var bIsUserActive = false;
var strBaseUrl = "";


function clearAutoDisconnectTimer()
{
    if ( iAutoDisconnectTimerId != null )
    {
        clearTimeout( iAutoDisconnectTimerId );
        iAutoDisconnectTimerId = null;
    }
}

function clearPreAutoDisconnectTimer()
{
    if ( iPreAutoDisconnectTimerId != null )
    {
        clearTimeout( iPreAutoDisconnectTimerId );
        iPreAutoDisconnectTimerId = null;
    }
}

function onPageload(e) {
    //
    // Browser Name in the form : 'MSIE x.x'.
    //
    var iePattern = /MSIE (\d+)\./;
    var ieMatch = iePattern.exec(window.navigator.userAgent);

    if (ieMatch) {
        if (parseInt(ieMatch[1]) <= 6)
        {
            ApplyPngTransparency();
        }
    }
}

function onPageUnload(e)
{
    clearAutoDisconnectTimer();
    clearPreAutoDisconnectTimer();
    if ( helpPopup != null && helpPopup.closed  == false )
    {
        helpPopup.close();
    }
}

function onAuthenticatedPageload(e)
{
    if ( bFormAuthenticationMode == true )
    {
        objLastUserActivityTime = objPageLoadTime = new Date();
        iAutoDisconnectSessionTimeout = (iSessionTimeout * iConstMinuteToMilliSec);
        iPreAutoDisconnectSessionTimeout = (iConstPreAutoDisconnectFactor * iAutoDisconnectSessionTimeout);

        //
        // Show Workspace Notification
        // Setup onAutoDisconnect timer regardless of onAuthenticated succcess/failure to preserve old behavior
        //
        onAuthenticated();
        iAutoDisconnectTimerId = setTimeout( "onAutoDisconnect()", iAutoDisconnectSessionTimeout );
        //
        // Setup onPreAutoDisconnect timer to keep web and workspace runtime in sync with user activity
        // if the workspace runtime supports OnAuthenticatedEx i.e. updating expiry timer.
        //
        if ( iPreAutoDisconnectSessionTimeout > iConstPreAutoDisconnectThreshold )
        {
            iPreAutoDisconnectTimerId = setTimeout( "onPreAutoDisconnect()", iPreAutoDisconnectSessionTimeout );
        }
    }
    onPageload(e);
}

function onClickHelp()
{
    if ( helpPopup == null || helpPopup.closed == true )
    {
        helpPopup = window.open( sHelpSource,
                                 "_blank",
                                 "height=600px, width=600px, left=600, top=150, toolbar=no, resizable=yes, scrollbars=yes, menubar=no" );
    }
    else
    {
        helpPopup.close();
        helpPopup = null;
    }
}

function onAutoDisconnect()
{
    if ( bAsyncPageRequestSucceeded )
    {
        return;
    }

    var iErrorCode;
    var objWorkspace = null;
    var strWorkspaceId = "";
    var strCookieContents = getCookieContents(strTSWACookieName);

    if ( null != strCookieContents )
    {
        strWorkspaceId = decodeURIComponent( getCookieCrumbValue(strCookieContents, "WorkSpaceID") );
    }

    if (strWorkspaceId != null && 
        strWorkspaceId != "" &&
        GetActiveXSSOMode())
    {
        try
        {
            objWorkspace = GetWorkspaceObject();
            objWorkspace.ClearWorkspaceCredential( strWorkspaceId );
        }
        catch (objException)
        {
            objWorkspace = null;
            iErrorCode = (objException.number & 0xFFFF);
        }
    }

    window.location = strBaseUrl + "LogOff.aspx" + window.location.search;
}

function onUserDisconnect()
{
    var objWorkspace = null;
    var iErrorCode;
    var strWorkspaceId = "";
    var strCookieContents = getCookieContents(strTSWACookieName);

    if ( null != strCookieContents )
    {
        strWorkspaceId = decodeURIComponent( getCookieCrumbValue(strCookieContents, "WorkSpaceID") );
    }

    if (strWorkspaceId != null &&
        strWorkspaceId != "" &&
        GetActiveXSSOMode())
    {
        try
        {
            objWorkspace = GetWorkspaceObject();
            objWorkspace.DisconnectWorkspace( strWorkspaceId );
        }
        catch (objException)
        {
            objWorkspace = null;
            iErrorCode = (objException.number & 0xFFFF);
        }
    }

    window.location = strBaseUrl + "LogOff.aspx" + window.location.search;
}

function onClickSecurity() {
    var bPrivateMode = document.getElementById("rdoPrvt").checked;
    var objPassword = document.getElementById("UserPass");
    var objDomainUserName = document.getElementById("DomainUserName");

    if (GetActiveXSSOMode()) {
        document.getElementById("trPrvtWrn").style.display = bPrivateMode ? "" : "none";
    }
        
    if ( bPrivateMode )
    {
        document.FrmLogin["flags"].value |= 4;
        if ( objPassword && objDomainUserName )
        {
            objPassword.setAttribute("autocomplete", "on");
            objDomainUserName.setAttribute("autocomplete", "on");
        }
    }
    else
    {
        document.FrmLogin["flags"].value &= ~4;
        if ( objPassword && objDomainUserName )
        {
            objPassword.setAttribute("autocomplete", "off");
            objDomainUserName.setAttribute("autocomplete", "off");
        }
    }
}

function showElement(objElement)
{
    objElement.style.display = "";
}

function hideElement(objElement)
{
    objElement.style.display = "none";
}

function onclickExplanation(id)
{
    var objElement = document.getElementById(id);
    if ( objElement.tagName=="IMG" )
    {
        objElement = objElement.parentElement;
    }

    switch(objElement)
    {
        case document.getElementById("lnkShwSec"):
            hideElement(document.getElementById("lnkShwSec"));
            showElement(document.getElementById("lnkHdSec"));
            showElement(document.getElementById("trPubExp"));
            showElement(document.getElementById("trPrvtExp"));
            document.getElementById("lnkHdSec").focus();
            break;

        case document.getElementById("lnkHdSec"):
            showElement(document.getElementById("lnkShwSec"));
            hideElement(document.getElementById("lnkHdSec"));
            hideElement(document.getElementById("trPubExp"));
            hideElement(document.getElementById("trPrvtExp"));
            document.getElementById("lnkShwSec").focus();
            break;
    }
}

function getCookieContents(strNameOfCookie)
{
    var objCookieContents = null;
    var iStartIndex, iEndIndex;

    if ( strNameOfCookie != null &&
         strNameOfCookie != "" &&
         document.cookie.length > 0 )
    {
        iStartIndex = document.cookie.indexOf( strNameOfCookie + "=" );
        if ( iStartIndex != -1 )
        {
            iStartIndex = iStartIndex + strNameOfCookie.length + 1;
            iEndIndex = document.cookie.indexOf( ";", iStartIndex );
            if ( iEndIndex == -1 )
            {
                iEndIndex = document.cookie.length;
            }
            objCookieContents = document.cookie.substring( iStartIndex, iEndIndex );
        }
    }
    return objCookieContents;
}

function getCookieCrumbValue(strCookieContents, strCookieCrumbName)
{
    var strCookieCrumbValue = "";

    if ( strCookieContents != null &&
         strCookieContents != "" &&
         strCookieCrumbName != null &&
         strCookieCrumbName != ""
       )
    {
        //
        // strCookieContents is in the form '<Name1>=<Value1>&<Name2>=<Value2>'.
        //
        var objCookieCrumbs = strCookieContents.split("&");
        for (var iIndex = 0; iIndex < objCookieCrumbs.length; iIndex++)
        {
            var objCookieCrumb = objCookieCrumbs[iIndex].split("=");

            if ( strCookieCrumbName.toLowerCase() == objCookieCrumb[0].toLowerCase() )
            {
                strCookieCrumbValue = objCookieCrumb[1];
                break;
            }
            else
            {
                continue;
            }
        }
    }

    return strCookieCrumbValue;
}

function onAuthenticated()
{
    var iErrorCode;
    var objWorkspace = null;
    var bCountUnauthenticatedCredentials = true;
    var bIsOnAuthenticatedCalled = false;
    var bIsWorkspaceCredentialSpecified = false;
    var objForm = document.getElementById("FrmUserInfo");
    var strLoggedOnDomainUserName = objForm.elements["DomainUserName"].value;

    var strCookieContents = getCookieContents(strTSWACookieName);
    var strWorkspaceId = decodeURIComponent( getCookieCrumbValue(strCookieContents, "WorkSpaceID") );
    var strDomainUserName = decodeURIComponent( getCookieCrumbValue(strCookieContents, "Name") );

    if ( strWorkspaceId != null && strWorkspaceId != "" )
    {
        if ( strDomainUserName == null || strDomainUserName == "" ||
             strLoggedOnDomainUserName == null || strLoggedOnDomainUserName == ""
           )
        {
            //
            // This should never happen.
            //
            return;
        }

        var strQueryStringPreamble = "?";
        if (window.location.search) {
            strQueryStringPreamble = window.location.search + "&";
        }

        if ( strDomainUserName.toLowerCase() == strLoggedOnDomainUserName.toLowerCase()) {
            if (GetActiveXSSOMode()) {
                try {
                    objWorkspace = GetWorkspaceObject();
                    bIsWorkspaceCredentialSpecified = objWorkspace.IsWorkspaceCredentialSpecified(strWorkspaceId,
                                                                                               bCountUnauthenticatedCredentials);
                    if (bIsWorkspaceCredentialSpecified) {
                        bIsOnAuthenticatedCalled = true;
                        objWorkspace.OnAuthenticated(strWorkspaceId, strDomainUserName);
                    }
                }
                catch (objException) {
                    objWorkspace = null;
                    iErrorCode = (objException.number & 0xFFFF);

                    if (bIsOnAuthenticatedCalled) {
                        //
                        // 183 = ERROR_ALREADY_EXISTS.
                        //
                        if (iErrorCode == 183) {
                            window.location = strBaseUrl + "LogOff.aspx" + strQueryStringPreamble + "Error=WkSInUse";
                        }
                        //
                        // 1168 = ERROR_NOT_FOUND.
                        //
                        if (iErrorCode == 1168) {
                            window.location = strBaseUrl + "LogOff.aspx" + strQueryStringPreamble + "Error=WkSDisconnected";
                        }
                    }
                }
            }
        }
        else
        {
            //
            // Ideally check workspace state before redirecting; if it has been authenticatd as well.
            //
            window.location = strBaseUrl + "LogOff.aspx" + strQueryStringPreamble + "Error=WkSInUse";
        }
    }
}

function GetWorkspaceObject()
{
    if( g_objWorkspace == null )
    {
        var objClientShell = new ActiveXObject("MsRdpWebAccess.MsRdpClientShell");
        
        g_objWorkspace = objClientShell.MsRdpWorkspace3;
        if(g_objWorkspace != null)
        {
            g_workspaceObjectVersion = 3;
        }
        else
        {
            g_objWorkspace = objClientShell.MsRdpWorkspace2;
            if(g_objWorkspace != null)
            {
                g_workspaceObjectVersion = 2;
            }
            else
            {
                g_objWorkspace = objClientShell.MsRdpWorkspace;
                if(g_objWorkspace != null)
                {
                    g_workspaceObjectVersion = 1;
                }
                else
                {
                    g_workspaceObjectVersion = 0;
                }
            }
        }
    }

    return g_objWorkspace;
}

function GetActiveXSSOMode() {

    if (!g_activeXSSOModeSet) {
        g_activeXSSOMode = false;

        try {
            if (GetWorkspaceObject() != null) {
                g_activeXSSOMode = true;
            }
        }
        catch (objException) {
        }

        g_activeXSSOModeSet = true;
    }

    return g_activeXSSOMode;
}

function GetWorkspaceObjectVersion()
{
    if (g_workspaceObjectVersion == -1) {
        try {
            GetWorkspaceObject();
        }
        catch (objException) {
        }
    }

    return g_workspaceObjectVersion;
}

function ApplyPngTransparency()
{
    var objDocumentElements = document.all;
    var objElement;

    for (var iIndex = objDocumentElements.length - 1; iIndex >=0; iIndex--)
    {
        objElement = objDocumentElements[iIndex];
        // background pngs
        if ( objElement.currentStyle.backgroundImage.match(/\.png/i) != null )
        {
            ApplyPngTransparencyToBackground(objElement);
        }
        // image elements
        if ( objElement.tagName == 'IMG' && objElement.src.match(/\.png$/i) != null )
        {
            ApplyPngTransparencyToImage(objElement);
        }
    }
}

function ApplyPngTransparencyToBackground(objBackground)
{
    var strSizingMethod = 'scale';
    var strBackgroundImageUrl = objBackground.currentStyle.backgroundImage;
    var strBackgroundImage = strBackgroundImageUrl.substring(5, strBackgroundImageUrl.length-2);

    if ( objBackground.currentStyle.backgroundRepeat == 'no-repeat' )
    {
        strSizingMethod = 'crop';
    }
    objBackground.style.filter = "progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + strBackgroundImage + "', sizingMethod='" + strSizingMethod + "')";
    objBackground.style.backgroundImage = 'url(' + strTransparentGif + ')';
}

function ApplyPngTransparencyToImage(objImage)
{
    var strImage = objImage.src;
    objImage.style.width = objImage.width + "px";
    objImage.style.height = objImage.height + "px";
    objImage.style.filter = "progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + strImage + "', sizingMethod='scale')";
    objImage.src = strTransparentGif;
}

function getUserNameRdpProperty()
{
    var strUserNameRdpProperty = "";
    var strCookieContents = getCookieContents(strTSWACookieName);
    var strDomainUserName = decodeURIComponent( getCookieCrumbValue(strCookieContents, "Name") );

    if ( strDomainUserName != null && strDomainUserName != "" )
    {
        strUserNameRdpProperty = "UserName:s:" + strDomainUserName + "\n";
    }

    return strUserNameRdpProperty;
}

function getXmlHttpObject()
{
    if ( objXmlHttp == null )
    {
        if ( window.XMLHttpRequest )
        {
            // code for IE7+, Firefox, Chrome, Opera, Safari
            objXmlHttp = new XMLHttpRequest();
        }
        else if ( window.ActiveXObject )
        {
            // code for IE6, IE5
            objXmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
        }
    }
    return objXmlHttp;
}

function onXmlHttpRequestStateChanged()
{
    if ( objXmlHttp.readyState == 4 && objXmlHttp.status == 200 )
    {
        //
        // Set 'bAsyncPageRequestSucceeded' flag to true
        // to prevent onAutoDisconnect from disconnecting workspace.
        //
        bAsyncPageRequestSucceeded = true;

        //
        // Calculate User Acivity Window and call into workspace runtime asking to update the timeout.
        //
        var objCurrentTime = new Date();
        var iRemainingAutoDisconnectSessionTimeout = iAutoDisconnectSessionTimeout - (objCurrentTime.getTime() - objLastUserActivityTime.getTime());

        onAuthenticated();
        iAutoDisconnectSessionTimeout = iRemainingAutoDisconnectSessionTimeout;
        iPreAutoDisconnectSessionTimeout = (iConstPreAutoDisconnectFactor * iAutoDisconnectSessionTimeout);
        objLastUserActivityTime = objPageLoadTime = objCurrentTime;

        //
        // Reset AutoDisconnect timers.
        //
        clearAutoDisconnectTimer();
        clearPreAutoDisconnectTimer();
        if ( iPreAutoDisconnectSessionTimeout > iConstPreAutoDisconnectThreshold )
        {
            iPreAutoDisconnectTimerId = setTimeout( "onPreAutoDisconnect()", iPreAutoDisconnectSessionTimeout );
        }
        iAutoDisconnectTimerId = setTimeout( "onAutoDisconnect()", iAutoDisconnectSessionTimeout );

        //
        // Set bAsyncPageRequestSucceeded and bIsUserActive flags to false.
        //
        bAsyncPageRequestSucceeded = false;
        bIsUserActive = false;

        return;
    }
}

function onPreAutoDisconnect()
{
    //
    // If user has not made any activity,
    // reset onPreAutoDisconnect for the remaining time.
    //
    if ( bIsUserActive != true )
    {
        //
        // Calculate User Acivity Window and call into workspace runtime asking to update the timeout.
        //
        var objCurrentTime = new Date();
        var iRemainingAutoDisconnectSessionTimeout = iAutoDisconnectSessionTimeout - (objCurrentTime.getTime() - objPageLoadTime.getTime());
        iRemainingAutoDisconnectSessionTimeout = (iConstPreAutoDisconnectFactor * iRemainingAutoDisconnectSessionTimeout);

        //
        // Reset AutoDisconnect timers.
        //
        clearPreAutoDisconnectTimer();
        if ( iRemainingAutoDisconnectSessionTimeout > iConstPreAutoDisconnectThreshold )
        {
            iPreAutoDisconnectTimerId = setTimeout( "onPreAutoDisconnect()", iRemainingAutoDisconnectSessionTimeout );
        }
        return;
    }

    //
    // Turn off the flags.
    //
    bAsyncPageRequestSucceeded = false;

    //
    // Send a background async request to the server to refresh the tick.
    // Not using window.location.reload as that refreshes the page in browser leading to bad UX.
    //
    objXmlHttp = getXmlHttpObject();
    if ( objXmlHttp == null )
    {
        return; // browser does not support XMLHTTP - fail silently
    }

    objXmlHttp.onreadystatechange = onXmlHttpRequestStateChanged;
    objXmlHttp.open( "GET", window.location, true);
    objXmlHttp.send( null );
}

function onUserActivity( objEvent )
{
    objLastUserActivityTime = new Date();
    if ( bIsUserActive != true )
    {
        bIsUserActive = true;
    }
}
