# Powershell script to bootstrap CAKE
$toolsDir = "tools"
$packagesConfig = "./packages.config"

if (-Not (Test-Path $toolsDir)) {
    mkdir $toolsDir
}

Invoke-WebRequest https://cakebuild.net/download/bootstrapper/windows -OutFile "$toolsDir/cake-bootstrapper.ps1"
& "$toolsDir/cake-bootstrapper.ps1"
