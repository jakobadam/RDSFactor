@echo off
set PATH=%PATH%;%windir%\Microsoft.NET\Framework\v4.0.30319

:: build the rdsfactor radius server
msbuild RDSFactor/RDSFactor.sln /property:Configuration=release

:: install it
InstallUtil RDSFactor\bin\Release\RDSFactor.exe