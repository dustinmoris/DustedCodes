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
    $env:GOOGLE_APPLICATION_CREDENTIALS = "$env:HOME/.secrets/google-app-creds.json"
    dotnet-run $app $Run
    return
}
elseif ($Docker.IsPresent -or $Deploy.IsPresent)
{
    $imageName = "dusted-codes:$version"

    Write-Host "Building Docker image..." -ForegroundColor Magenta
    Invoke-Cmd "docker build -t $imageName ./src/DustedCodes/"

    if ($Deploy.IsPresent)
    {
        $gcpProjectName     = "dusted-codes"
        $gcpRegion          = "europe-west2"
        $gcpClusterName     = "dusted-kubes-eu"
        $gcrName            = "eu.gcr.io/$gcpProjectName"
        $remoteImageName    = "$gcrName/$imageName"

        Write-Host "Deploying Docker image..." -ForegroundColor Magenta


        Invoke-Cmd "docker tag $imageName $remoteImageName"
        Invoke-Cmd "gcloud config set project $gcpProjectName"
        Invoke-Cmd "gcloud container clusters get-credentials $gcpClusterName --region $gcpRegion"
        Invoke-Cmd "docker push $remoteImageName"
    }
}