@echo off
:: Create release 

rd /s /q release
mkdir release

xcopy /E server\bin\Release release\server\bin\Release\
xcopy /E web release\web\

:: Add relevant bat scripts
xcopy *.bat release
del release\release.bat

:: Yup. This a zip command on windows
powershell.exe -nologo -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::CreateFromDirectory('release', 'rdsfactor.zip'); }"