# ----------------------------------------------
# Build script
# ----------------------------------------------

param
(
    [switch] $Release,
    [switch] $Docker,
    [switch] $Deploy,
    [string] $Run
)

$ErrorActionPreference = "Stop"

Import-module "$PSScriptRoot/.psscripts/build-functions.ps1" -Force

Write-BuildHeader "Starting Dusted Codes build script"

$app = "./src/DustedCodes/DustedCodes.fsproj"

$version = Get-ProjectVersion $app
Update-AppVeyorBuildVersion $version

if (Test-IsAppVeyorBuildTriggeredByGitTag)
{
    $gitTag = Get-AppVeyorGitTag
    Test-CompareVersions $version $gitTag
}

Write-DotnetCoreVersions
Remove-OldBuildArtifacts

$configuration = if ($Release.IsPresent -or $Docker.IsPresent -or $Deploy.IsPresent -or $env:APPVEYOR -eq $true) { "Release" } else { "Debug" }

Write-Host "Building application..." -ForegroundColor Magenta
dotnet-build $app "-c $configuration"

Write-SuccessFooter "Build script completed successfully!"

if ($Run)
{
    Write-Host "Launching application..." -ForegroundColor Magenta

    dotnet-run $app $Run
    return
}
elseif ($Docker.IsPresent -or $Deploy.IsPresent)
{
    Write-Host "Publishing web application..." -ForegroundColor Magenta
    dotnet-publish $app "-c $configuration"

    Write-Host "Building Docker image..." -ForegroundColor Magenta
    $targetFramework = Get-NetCoreTargetFramework $app
    $publishFolder = "./src/DustedCodes/bin/$configuration/$targetFramework/publish"
    Invoke-Cmd "docker build -t dustedcodes:$version $publishFolder"

    if ($Deploy.IsPresent)
    {
        Write-Host "Deploying Docker image..." -ForegroundColor Magenta

        Invoke-Cmd "docker tag dustedcodes:$version us.gcr.io/dustins-private-project/dustedcodes:$version"
        Invoke-Cmd "gcloud docker --authorize-only"
        Invoke-Cmd "gcloud docker -- push us.gcr.io/dustins-private-project/dustedcodes:$version"
        Invoke-Cmd "kubectl set image deployment/dustedcodes dustedcodes=us.gcr.io/dustins-private-project/dustedcodes:$version"
    }
}