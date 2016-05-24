
$currentDir = split-path $SCRIPT:MyInvocation.MyCommand.Path -parent

Import-Module (Join-Path $currentDir "..\..\Click2Cloud.Openshift.Cmdlets.dll") -DisableNameChecking

Import-Module (Join-Path $currentDir "..\..\Click2Cloud.Openshift.Common.dll") -DisableNameChecking

Import-Module (Join-Path $currentDir "..\..\Click2Cloud.Openshift.Node.dll") -DisableNameChecking
