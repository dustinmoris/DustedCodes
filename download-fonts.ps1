$fontsDir = $env:HOME + "/downloads/fonts"
$fontsToDownload = @(
    # --------------
    # Nunito
    # --------------
    ("Nunito-SemiBoldItalic", "cyrillic-ext", "https://fonts.gstatic.com/s/nunito/v12/XRXQ3I6Li01BKofIMN5cYtvHUT8tDntiXTI.woff2"),
    ("Nunito-SemiBoldItalic", "cyrillic", "https://fonts.gstatic.com/s/nunito/v12/XRXQ3I6Li01BKofIMN5cYtvOUT8tDntiXTI.woff2"),
    ("Nunito-SemiBoldItalic", "vietnamese", "https://fonts.gstatic.com/s/nunito/v12/XRXQ3I6Li01BKofIMN5cYtvFUT8tDntiXTI.woff2"),
    ("Nunito-SemiBoldItalic", "latin-ext", "https://fonts.gstatic.com/s/nunito/v12/XRXQ3I6Li01BKofIMN5cYtvEUT8tDntiXTI.woff2"),
    ("Nunito-SemiBoldItalic", "latin", "https://fonts.gstatic.com/s/nunito/v12/XRXQ3I6Li01BKofIMN5cYtvKUT8tDnti.woff2"),

    ("Nunito-Light", "cyrillic-ext", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofAnsSUbOvIWzgPDEtj.woff2"),
    ("Nunito-Light", "cyrillic", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofAnsSUZevIWzgPDEtj.woff2"),
    ("Nunito-Light", "vietnamese", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofAnsSUbuvIWzgPDEtj.woff2"),
    ("Nunito-Light", "latin-ext", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofAnsSUb-vIWzgPDEtj.woff2"),
    ("Nunito-Light", "latin", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofAnsSUYevIWzgPDA.woff2"),

    ("Nunito-Regular", "cyrillic-ext", "https://fonts.gstatic.com/s/nunito/v12/XRXV3I6Li01BKofIOOaBTMnFcQIG.woff2"),
    ("Nunito-Regular", "cyrillic", "https://fonts.gstatic.com/s/nunito/v12/XRXV3I6Li01BKofIMeaBTMnFcQIG.woff2"),
    ("Nunito-Regular", "vietnamese", "https://fonts.gstatic.com/s/nunito/v12/XRXV3I6Li01BKofIOuaBTMnFcQIG.woff2"),
    ("Nunito-Regular", "latin-ext", "https://fonts.gstatic.com/s/nunito/v12/XRXV3I6Li01BKofIO-aBTMnFcQIG.woff2"),
    ("Nunito-Regular", "latin", "https://fonts.gstatic.com/s/nunito/v12/XRXV3I6Li01BKofINeaBTMnFcQ.woff2"),

    ("Nunito-SemiBold", "cyrillic-ext", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofA6sKUbOvIWzgPDEtj.woff2"),
    ("Nunito-SemiBold", "cyrillic", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofA6sKUZevIWzgPDEtj.woff2"),
    ("Nunito-SemiBold", "vietnamese", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofA6sKUbuvIWzgPDEtj.woff2"),
    ("Nunito-SemiBold", "latin-ext", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofA6sKUb-vIWzgPDEtj.woff2"),
    ("Nunito-SemiBold", "latin", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofA6sKUYevIWzgPDA.woff2"),

    ("Nunito-Bold", "cyrillic-ext", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofAjsOUbOvIWzgPDEtj.woff2"),
    ("Nunito-Bold", "cyrillic", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofAjsOUZevIWzgPDEtj.woff2"),
    ("Nunito-Bold", "vietnamese", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofAjsOUbuvIWzgPDEtj.woff2"),
    ("Nunito-Bold", "latin-ext", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofAjsOUb-vIWzgPDEtj.woff2"),
    ("Nunito-Bold", "latin", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofAjsOUYevIWzgPDA.woff2"),

    ("Nunito-Black", "cyrillic-ext", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofAtsGUbOvIWzgPDEtj.woff2"),
    ("Nunito-Black", "cyrillic", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofAtsGUZevIWzgPDEtj.woff2"),
    ("Nunito-Black", "vietnamese", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofAtsGUbuvIWzgPDEtj.woff2"),
    ("Nunito-Black", "latin-ext", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofAtsGUb-vIWzgPDEtj.woff2"),
    ("Nunito-Black", "latin", "https://fonts.gstatic.com/s/nunito/v12/XRXW3I6Li01BKofAtsGUYevIWzgPDA.woff2"),

    # --------------
    # Nunito Sans
    # --------------

    ("NunitoSans-LightItalic", "vietnamese", "https://fonts.gstatic.com/s/nunitosans/v5/pe01MImSLYBIv1o4X1M8cce4G3JoY1wIUrt9w6fk2A.woff2"),
    ("NunitoSans-LightItalic", "latin-ext", "https://fonts.gstatic.com/s/nunitosans/v5/pe01MImSLYBIv1o4X1M8cce4G3JoY10IUrt9w6fk2A.woff2"),
    ("NunitoSans-LightItalic", "latin", "https://fonts.gstatic.com/s/nunitosans/v5/pe01MImSLYBIv1o4X1M8cce4G3JoY1MIUrt9w6c.woff2"),

    ("NunitoSans-Italic", "vietnamese", "https://fonts.gstatic.com/s/nunitosans/v5/pe0oMImSLYBIv1o4X1M8cce4E9ZKdn4qX5FHyg.woff2"),
    ("NunitoSans-Italic", "latin-ext", "https://fonts.gstatic.com/s/nunitosans/v5/pe0oMImSLYBIv1o4X1M8cce4E9dKdn4qX5FHyg.woff2"),
    ("NunitoSans-Italic", "latin", "https://fonts.gstatic.com/s/nunitosans/v5/pe0oMImSLYBIv1o4X1M8cce4E9lKdn4qX5E.woff2"),

    ("NunitoSans-Light", "vietnamese", "https://fonts.gstatic.com/s/nunitosans/v5/pe03MImSLYBIv1o4X1M8cc8WAc5iU1ECVZl_86Y.woff2"),
    ("NunitoSans-Light", "latin-ext", "https://fonts.gstatic.com/s/nunitosans/v5/pe03MImSLYBIv1o4X1M8cc8WAc5jU1ECVZl_86Y.woff2"),
    ("NunitoSans-Light", "latin", "https://fonts.gstatic.com/s/nunitosans/v5/pe03MImSLYBIv1o4X1M8cc8WAc5tU1ECVZl_.woff2"),

    ("NunitoSans-Regular", "vietnamese", "https://fonts.gstatic.com/s/nunitosans/v5/pe0qMImSLYBIv1o4X1M8cceyI9tAcVwob5A.woff2"),
    ("NunitoSans-Regular", "latin-ext", "https://fonts.gstatic.com/s/nunitosans/v5/pe0qMImSLYBIv1o4X1M8ccezI9tAcVwob5A.woff2"),
    ("NunitoSans-Regular", "latin", "https://fonts.gstatic.com/s/nunitosans/v5/pe0qMImSLYBIv1o4X1M8cce9I9tAcVwo.woff2"),

    ("NunitoSans-SemiBold", "vietnamese", "https://fonts.gstatic.com/s/nunitosans/v5/pe03MImSLYBIv1o4X1M8cc9iB85iU1ECVZl_86Y.woff2"),
    ("NunitoSans-SemiBold", "latin-ext", "https://fonts.gstatic.com/s/nunitosans/v5/pe03MImSLYBIv1o4X1M8cc9iB85jU1ECVZl_86Y.woff2"),
    ("NunitoSans-SemiBold", "latin", "https://fonts.gstatic.com/s/nunitosans/v5/pe03MImSLYBIv1o4X1M8cc9iB85tU1ECVZl_.woff2"),

    ("NunitoSans-Bold", "vietnamese", "https://fonts.gstatic.com/s/nunitosans/v5/pe03MImSLYBIv1o4X1M8cc8GBs5iU1ECVZl_86Y.woff2"),
    ("NunitoSans-Bold", "latin-ext", "https://fonts.gstatic.com/s/nunitosans/v5/pe03MImSLYBIv1o4X1M8cc8GBs5jU1ECVZl_86Y.woff2"),
    ("NunitoSans-Bold", "latin", "https://fonts.gstatic.com/s/nunitosans/v5/pe03MImSLYBIv1o4X1M8cc8GBs5tU1ECVZl_.woff2"),

    # --------------
    # Roboto Mono
    # --------------

    ("RobotoMono-Italic", "cyrillic-ext", "https://fonts.gstatic.com/s/robotomono/v7/L0x7DF4xlVMF-BfR8bXMIjhOm3CWWpCBC10HFw.woff2"),
    ("RobotoMono-Italic", "cyrillic", "https://fonts.gstatic.com/s/robotomono/v7/L0x7DF4xlVMF-BfR8bXMIjhOm3mWWpCBC10HFw.woff2"),
    ("RobotoMono-Italic", "greek-ext", "https://fonts.gstatic.com/s/robotomono/v7/L0x7DF4xlVMF-BfR8bXMIjhOm3GWWpCBC10HFw.woff2"),
    ("RobotoMono-Italic", "greek", "https://fonts.gstatic.com/s/robotomono/v7/L0x7DF4xlVMF-BfR8bXMIjhOm36WWpCBC10HFw.woff2"),
    ("RobotoMono-Italic", "vietnamese", "https://fonts.gstatic.com/s/robotomono/v7/L0x7DF4xlVMF-BfR8bXMIjhOm3KWWpCBC10HFw.woff2"),
    ("RobotoMono-Italic", "latin-ext", "https://fonts.gstatic.com/s/robotomono/v7/L0x7DF4xlVMF-BfR8bXMIjhOm3OWWpCBC10HFw.woff2"),
    ("RobotoMono-Italic", "latin", "https://fonts.gstatic.com/s/robotomono/v7/L0x7DF4xlVMF-BfR8bXMIjhOm32WWpCBC10.woff2"),

    ("RobotoMono-Regular", "cyrillic-ext", "https://fonts.gstatic.com/s/robotomono/v7/L0x5DF4xlVMF-BfR8bXMIjhGq3-cXbKDO1w.woff2"),
    ("RobotoMono-Regular", "cyrillic", "https://fonts.gstatic.com/s/robotomono/v7/L0x5DF4xlVMF-BfR8bXMIjhPq3-cXbKDO1w.woff2"),
    ("RobotoMono-Regular", "greek-ext", "https://fonts.gstatic.com/s/robotomono/v7/L0x5DF4xlVMF-BfR8bXMIjhHq3-cXbKDO1w.woff2"),
    ("RobotoMono-Regular", "greek", "https://fonts.gstatic.com/s/robotomono/v7/L0x5DF4xlVMF-BfR8bXMIjhIq3-cXbKDO1w.woff2"),
    ("RobotoMono-Regular", "vietnamese", "https://fonts.gstatic.com/s/robotomono/v7/L0x5DF4xlVMF-BfR8bXMIjhEq3-cXbKDO1w.woff2"),
    ("RobotoMono-Regular", "latin-ext", "https://fonts.gstatic.com/s/robotomono/v7/L0x5DF4xlVMF-BfR8bXMIjhFq3-cXbKDO1w.woff2"),
    ("RobotoMono-Regular", "latin", "https://fonts.gstatic.com/s/robotomono/v7/L0x5DF4xlVMF-BfR8bXMIjhLq3-cXbKD.woff2")
)
$totalFonts = $fontsToDownload.Count

Write-Host "Downloading a total of $totalFonts fonts" -ForegroundColor Magenta

foreach($fontToDownload in $fontsToDownload) {
    $name  = $fontToDownload[0]
    $range = $fontToDownload[1]
    $url   = $fontToDownload[2]

    $dotParts   = $url.Split('.')
    $ext        = $dotParts[$dotParts.Length - 1]
    $fontName   = $name.Split('-')[0]
    $targetDir  = "$fontsDir/$fontName"
    $targetPath = "$targetDir/$name-$range.$ext"

    Write-Host "---"
    Write-Host "Downloading font $name ($range) of $fontName into $targetPath"

    if ($name -eq "" -or $range -eq "" -or $url -eq "") {
        Write-Host "Undefined font." -ForegroundColor Red
        continue
    }

    if (Test-Path $targetPath) {
        Write-Host "Skipping download, because the target font already exists." -ForegroundColor Yellow
        continue
    }

    if (Test-Path $targetDir) {
        Write-Host "The directory $targetDir already exists." -ForegroundColor Gray
    } else {
        Write-Host "Creating directory $targetDir..." -ForegroundColor DarkYellow
        New-Item -Type Directory -Path $targetDir | Out-Null
    }

    Invoke-WebRequest -Uri $url -OutFile $targetPath
    Write-Host "Font successfully downloaded." -ForegroundColor DarkGreen
}