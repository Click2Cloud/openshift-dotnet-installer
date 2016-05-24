@echo off
echo Checking visual studio native tools path
set vcvar="C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\vcvarsall.bat"

if exist %vcvar% call %vcvar%

mkdir TestResults
rd /S /q TestResults
vstest.console.exe node\src\openshift-dotnet\Click2Cloud.Openshift.Tests\bin\Click2Cloud.Openshift.Tests.dll /Settings:node\src\openshift-dotnet\Click2Cloud.Openshift.Tests\local.runsettings /Logger:trx

if %errorlevel% neq 0 goto test_error

ren "TestResults\*.trx" openshiftdotnet.trx
exit /b 0

:test_error
exit /b 1