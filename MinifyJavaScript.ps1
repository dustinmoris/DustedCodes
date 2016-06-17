[CmdletBinding()]
param
(
	[Parameter(Position = 0, Mandatory = $true)]
	[string] $SolutionDir
)

Function Compress-JavaScriptFile
{
    [CmdletBinding()]
    param 
    (
        [string] $JavaScriptFilePath
    )

    $jsFile = Get-Item -Path $JavaScriptFilePath
    $content = [System.IO.File]::ReadAllText($jsFile.FullName)
    $body = @{input = $content}
    $response = Invoke-WebRequest -Uri "https://javascript-minifier.com/raw" -Method Post -Body $body
    
    if ($response.StatusCode -ne 200)
    {
        throw ("Could not compress JavaScript file $JavaScriptFilePath" +
           [System.Environment]::NewLine +
           [System.Environment]::NewLine +
           "Response:" + 
           [System.Environment]::NewLine +
           $response.RawContent)
    }

    $compressedContent = $response.Content
    $newFilePath = $JavaScriptFilePath.Replace(".js", ".min.js")	

    Set-Content -Path $newFilePath -Value $compressedContent -Force
}

# -----------------
# BEGIN:

$ErrorActionPreference = "Stop"

Write-Output "-----"
Write-Output "Compressing JavaScript Files"
Write-Output "-----"

Get-ChildItem $SolutionDir -Recurse -Include *.js -Exclude *.min.js | % {
	# Currently disabled because the website is down.
	# Write-Output "Compressing $_"
	# Compress-JavaScriptFile -JavaScriptFilePath $_ 
}