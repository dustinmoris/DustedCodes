# ----------------
# ABOUT:
#
# Change the csproj File to include static HTML files instead of Markdown files for the articles.
# The files need to be swapped so that MSBuild picks the right files for the WebDeploy package.
# ----------------

$ErrorActionPreference = "Stop"

# Load the .csproj file content
$csprojFile = "$PSScriptRoot\DustedCodes.Blog\DustedCodes.Blog.csproj"
[xml]$content = Get-Content -Path $csprojFile

# Find the Include elements and modify the extension from .md to .html
$content.Project.ItemGroup.Content | Where-Object { 
    $_.Include -ne $null -and $_.Include.Contains("App_Data\Articles\") -and $_.Include.EndsWith(".md") 
} | % {
    $_.Include = $_.Include.Replace(".md", ".html")
}

# Update the .csproj file
$content.Save($csprojFile)