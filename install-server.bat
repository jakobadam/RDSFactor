@echo off
set PATH=%PATH%;%windir%\Microsoft.NET\Framework\v4.0.30319

:: build the radius server
msbuild server/RDSFactor.sln /property:Configuration=release

:: install it
InstallUtil server\bin\Release\RDSFactor.exe

:: start it
net start RDSFactor