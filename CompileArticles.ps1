[CmdletBinding()]
param
(
	[Parameter(Position = 1, Mandatory = $true)]
	[string] $SolutionDir
)

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

Write-Output "-----"
Write-Output "Compile static .html pages from Markdown"
Write-Output "-----"

Add-Type -Path "$SolutionDir\libraries\MarkdownDeep\1.5.0\MarkdownDeep.dll"

Get-ChildItem "$SolutionDir\DustedCodes.Blog\App_Data\Articles" -Filter *.md | % {
	Write-Output "Compiling $_"
	ConvertTo-Html $_ 
}