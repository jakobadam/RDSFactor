
function onLoginFormSubmit()
{
    var bStopSubmission = false;
    var iErrorCode;
    var objForm = document.getElementById("FrmLogin");
    var strDomainName = null;
    var strDomainUserName = "";
    var strPassword = "";
    var strWorkspaceId = "";
    var strWorkspaceFriendlyName = "";
    var strRedirectorName = "";
    var strRDPCertificates = "";
    var bPrivateMode = document.getElementById("rdoPrvt").checked;
    var strTimeout = "0";

    hideElement(document.getElementById("trErrorWorkSpaceInUse"));
    hideElement(document.getElementById("trErrorWorkSpaceDisconnected"));
    hideElement(document.getElementById("trErrorIncorrectCredentials"));
    hideElement(document.getElementById("trErrorDomainNameMissing"));
    hideElement(document.getElementById("trErrorUnauthorizedAccess"));
    hideElement(document.getElementById("trErrorServerConfigChanged"));

    if ( objForm != null )
    {
        strDomainUserName = objForm.elements["DomainUserName"].value;
        strPassword = objForm.elements["UserPass"].value;
        strWorkspaceId = objForm.elements["WorkSpaceID"].value;
        strRDPCertificates = objForm.elements["RDPCertificates"].value;
        strWorkspaceFriendlyName = objForm.elements["WorkspaceFriendlyName"].value;
        strRedirectorName = objForm.elements["RedirectorName"].value;

        if( bPrivateMode )
        {
            strTimeout =  objForm.elements["PrivateModeTimeout"].value;
        }
        else
        {
            strTimeout =  objForm.elements["PublicModeTimeout"].value;
        }

        if ( -1 != strDomainUserName.indexOf("\\") )
        {
            strDomainName = strDomainUserName.substring( 0, strDomainUserName.indexOf("\\") );
        }
        else if ( -1 != strDomainUserName.indexOf("@") )
        {
            strDomainName = strDomainUserName.substring( strDomainUserName.indexOf("@") + 1, strDomainUserName.length );
        }

    }

    if ( strDomainUserName == null || strDomainUserName == "" ||
         strPassword == null || strPassword == "" )
    {
        showElement(document.getElementById("trErrorIncorrectCredentials"));
        bStopSubmission = true;
    }
    else if ( strDomainName == null || strDomainName == "" || strDomainName == "." )
    {
        showElement(document.getElementById("trErrorDomainNameMissing"));
        bStopSubmission = true;
    }
    else
    {
        if (strWorkspaceId != null &&
            strWorkspaceId != "" && 
            GetActiveXSSOMode())
        {
            try
            {
                var iWorkspaceVersion = GetWorkspaceObjectVersion();
                var objWorkspace = GetWorkspaceObject();

                if (iWorkspaceVersion >= 2)
                {
                    objWorkspace.StartWorkspaceEx(
                        strWorkspaceId,
                        strWorkspaceFriendlyName,
                        strRedirectorName,
                        strDomainUserName,
                        strPassword,
                        strRDPCertificates,
                        parseInt(strTimeout),
                        0 );
                }
                else
                {
                    objWorkspace.StartWorkspace(
                        strWorkspaceId,
                        strDomainUserName,
                        strPassword,
                        strRDPCertificates,
                        parseInt(strTimeout),
                        0);
                }
            }
            catch (objException)
            {
                iErrorCode = (objException.number & 0xFFFF);

                //
                // 183 = ERROR_ALREADY_EXISTS.
                //
                if ( iErrorCode == 183 )
                {
                    showElement(document.getElementById("trErrorWorkSpaceInUse"));
                    bStopSubmission = true;
                }
            }
        }
    }

    // return false to stop form submission
    return !bStopSubmission;
}

function onLoginPageLoad(e)
{
    var strDomainUserName = ""; // CrumbName: Name
    var strMachineType = "";    // CrumbName: MachineType
    var strWorkSpaceID = "";    // CrumbName: WorkSpaceID
    var strCookieContents = "";
    var iIndex;
    var bActiveXSSOMode = GetActiveXSSOMode(); // as a side-effect, this caches the workspace ActiveX object

    onPageload(e);  // call the parent event

    document.getElementById("tableLoginForm").style.display = "";
    document.getElementById("tdDomainUserNameLabel").style.display = "";

    if (bActiveXSSOMode) {
        
        document.getElementById("tablePublicOption").style.display = "";
        document.getElementById("tablePrivateOption").style.display = "";
        document.getElementById("spanToggleSecExplanation").style.display = "";
        document.getElementById("rdoPblc").checked = true;
    }
    else {

        document.getElementById("trPrvtWrnNoAx").style.display = "";
    }
    onClickSecurity();

    strCookieContents = getCookieContents(strTSWACookieName);

    if ( null != strCookieContents )
    {
        strDomainUserName = decodeURIComponent( getCookieCrumbValue(strCookieContents, "Name") );
        strMachineType = getCookieCrumbValue(strCookieContents, "MachineType");
        strWorkSpaceID = decodeURIComponent( getCookieCrumbValue(strCookieContents, "WorkSpaceID") );

        if ( strMachineType != "" &&
             strMachineType == "private")
        {
            document.getElementById("DomainUserName").value = strDomainUserName;
            document.getElementById("rdoPrvt").checked = "private";
            onClickSecurity();
        }

        //
        // Set focus on UserName or Password field.
        //
        if ( strDomainUserName != "" )
        {
            document.getElementById("UserPass").focus();
        }
        else
        {
            document.getElementById("DomainUserName").focus();
        }
    }
    else
    {
         document.getElementById("DomainUserName").focus();
    }


}

