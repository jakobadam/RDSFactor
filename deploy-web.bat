@echo off
:: Replace RDS Pages directory with ours

set PAGES=%SystemDrive%\Windows\Web\RDWeb\Pages
set RDSFACTOR_PAGES=%~dp0RDSFactorWeb\RDWeb\Pages

:: Add IIS AppCmd to path
set PATH=%PATH%;%windir%\system32\inetsrv\

echo ==^> Removing %PAGES%
move %PAGES% %~dp0_pages_old

appcmd delete app "Default Web Site/RDWeb/Pages"

echo ==^> Adding RDSFactor RDWeb Pages to IIS
appcmd add app /site.name:"Default Web Site" /path:/RDWeb/Pages /physicalPath:"%RDSFACTOR_PAGES%

:: list available config settings: appcmd unlock config -section:?

appcmd unlock config -section:system.webServer/security/authentication/anonymousAuthentication
appcmd unlock config -section:system.webServer/security/authentication/windowsAuthentication

echo ==^> IIS updated with RDSFactor pages