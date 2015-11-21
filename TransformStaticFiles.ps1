[CmdletBinding()]
param
(
	[Parameter(Position = 1, Mandatory = $true)]
	[string] $SolutionDir
)

Function Get-LatestMarkdownDeep
{
    [CmdletBinding()]
    param
    (
        [string] $DestinationFolder
    )

	$markdownLibraryFolder = "$DestinationFolder\MarkdownDeep"

	if (!(Test-Path $markdownLibraryFolder))
	{
		# 1. Download latest MarkdownDeep Library
		$downloadUrl = "http://www.toptensoftware.com/downloads/MarkdownDeep.zip"
		$destination = "$DestinationFolder\MarkdownDeep.zip"
		$webClient = New-Object System.Net.WebClient
		$webClient.DownloadFile($downloadUrl, $destination)

		# 2. Extract zip files
		Expand-Archive -Source $destination -Destination $markdownLibraryFolder
	}

    # 3. Register assembly
    Add-Type -Path "$markdownLibraryFolder\bin\MarkdownDeep.dll"
}

# Remove this function when PowerShell 5 is installed on the build machine
Function Expand-Archive
{
    [CmdletBinding()]
    param
    (
        [string] $Source,
        [string] $Destination
    )

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::ExtractToDirectory($Source, $Destination)
}

Function ConvertTo-Html
{
    [CmdletBinding()]
    param 
    (
        [System.IO.FileInfo] $MarkdownFile
    )    

    $markdown = New-Object -TypeName MarkdownDeep.Markdown
    $markdownContent = [System.IO.File]::ReadAllText($MarkdownFile.FullName)
    $htmlContent = $markdown.Transform($markdownContent)
    $htmlFilePath = $MarkdownFile.FullName.Replace(".md", ".html")
    Set-Content -Path $htmlFilePath -Value $htmlContent -Force
}

# -----------------
# BEGIN:

$ErrorActionPreference = "Stop"

Write-Output "----"
Write-Output "POST BUILD EVENT"
Write-Output "----"

$tempFolder = "$SolutionDir\_Build_Temp"
if (!(Test-Path $tempFolder)) 
{ 
	Write-Output "Creating temporary folder..."
	New-Item -Path $tempFolder -ItemType Directory -Force 
}

Write-Output "Downloading latest MarkdownDeep library..."
Get-LatestMarkdownDeep -DestinationFolder $tempFolder

Write-Output "Compiling articles..."
Get-ChildItem "$SolutionDir\DustedCodes.Blog\App_Data\Articles" -Filter *.md | % {
	Write-Output "Compiling $_"
	ConvertTo-Html $_ 
}

# ONLY ON APPVEYOR BUILDS
# ----------------
# Change the csproj File to include static HTML files instead of Markdown files for the articles.
# The files need to be swapped so that MSBuild picks the right files for the WebDeploy package.
# ----------------

if ($env:APPVEYOR -eq $null -or $env:APPVEYOR -eq $false) 
{ 
	Write-Output "Skipping .csproj file transformation (only on AppVeyor builds)."
	return
}

# Load the .csproj file content
Write-Output "Loading $csprojFile..."
$csprojFile = "$SolutionDir\DustedCodes.Blog\DustedCodes.Blog.csproj"
[xml]$content = Get-Content -Path $csprojFile

# Find the Include elements and modify the extension from .md to .html
Write-Output "Modifying Includes.."
$content.Project.ItemGroup.Content | Where-Object { 
    $_.Include -ne $null -and $_.Include.Contains("App_Data\Articles\") -and $_.Include.EndsWith(".md") 
} | % {
	Write-Output "Updating " + $_.Include + "..."
    $_.Include = $_.Include.Replace(".md", ".html")
}

# Update the .csproj file
$content.Save($csprojFile)