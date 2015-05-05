@echo off
set PATH=%PATH%;%windir%\Microsoft.NET\Framework\v4.0.30319

:: build the radius server
:: msbuild server/RDSFactor.sln /property:Configuration=release

:: Unblock the downloaded file, otherwise stupid output from InstallUtil
powershell -Command {Unblock-File -Path server\bin\Release\RDSFactor.exe}

:: install it
InstallUtil server\bin\Release\RDSFactor.exe

:: start it
net start RDSFactor