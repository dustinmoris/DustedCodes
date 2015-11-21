Function Get-LatestMarkdownDeep
{
    [CmdletBinding()]
    param
    (
        [string] $DestinationFolder
    )

    # 1. Download latest MarkdownDeep Library
    $downloadUrl = "http://www.toptensoftware.com/downloads/MarkdownDeep.zip"
    $destination = "$DestinationFolder\MarkdownDeep.zip"
    $webClient = New-Object System.Net.WebClient
    $webClient.DownloadFile($downloadUrl, $destination)

    # 2. Extract zip files
    Expand-Archive -Source $destination -Destination "$DestinationFolder\_Build_Temp\MarkdownDeep"

    # 3. Register assembly
    Add-Type -Path "$DestinationFolder\_Build_Temp\MarkdownDeep\bin\MarkdownDeep.dll"
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
# BEGIN BUILD:

Get-LatestMarkdownDeep -DestinationFolder $PSScriptRoot

Get-ChildItem "$PSScriptRoot\DustedCodes.Blog\App_Data\Articles" -Filter *.md | % { ConvertTo-Html $_ }