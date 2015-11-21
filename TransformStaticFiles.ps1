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