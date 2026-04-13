param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

function Ensure-Directory {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    New-Item -ItemType Directory -Path $Path -Force | Out-Null
}

function Write-Utf8File {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,
        [Parameter(Mandatory = $true)]
        [string]$Content
    )

    $encoding = [System.Text.UTF8Encoding]::new($false)
    [System.IO.File]::WriteAllText($Path, $Content, $encoding)
}

$repoRoot = Split-Path -Path $PSScriptRoot -Parent
$projectRoot = Join-Path $repoRoot 'Protocol5.UAI.CSharp'
$projectFile = Join-Path $projectRoot 'Protocol5.UAI.CSharp.csproj'
$downloadReadme = Join-Path $projectRoot 'PROTOCOL5-DOWNLOAD.md'
$licenseFile = Join-Path $repoRoot 'LICENSE'
$siteDownloadRoot = Join-Path $repoRoot 'Protocol5.com\SiteContent\downloads'

$tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) 'Protocol5.UAI.CSharp.WebStarter'
$packageOutputRoot = Join-Path $tempRoot 'nupkg'
$archiveRoot = Join-Path $tempRoot 'archive'
$archiveDownloadRoot = Join-Path $archiveRoot 'downloads'
$archiveSourceRoot = Join-Path $archiveRoot 'src'
$archiveProjectRoot = Join-Path $archiveSourceRoot 'Protocol5.UAI.CSharp'

if (Test-Path $tempRoot) {
    Remove-Item -LiteralPath $tempRoot -Recurse -Force
}

Ensure-Directory -Path $packageOutputRoot
Ensure-Directory -Path $archiveDownloadRoot
Ensure-Directory -Path $archiveSourceRoot
Ensure-Directory -Path $siteDownloadRoot

dotnet pack $projectFile -c $Configuration -o $packageOutputRoot

$packageFile = Get-ChildItem -Path $packageOutputRoot -Filter 'Protocol5.UAI.CSharp.*.nupkg' |
    Sort-Object LastWriteTimeUtc -Descending |
    Select-Object -First 1

if (-not $packageFile) {
    throw "No Protocol5.UAI.CSharp package was produced."
}

Copy-Item -LiteralPath $projectRoot -Destination $archiveSourceRoot -Recurse -Force
Remove-Item -LiteralPath (Join-Path $archiveProjectRoot 'bin') -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -LiteralPath (Join-Path $archiveProjectRoot 'obj') -Recurse -Force -ErrorAction SilentlyContinue

Copy-Item -LiteralPath $packageFile.FullName -Destination $archiveDownloadRoot -Force
Copy-Item -LiteralPath $licenseFile -Destination (Join-Path $archiveRoot 'LICENSE') -Force
Copy-Item -LiteralPath $downloadReadme -Destination (Join-Path $archiveRoot 'README.md') -Force

$zipPath = Join-Path $siteDownloadRoot 'protocol5-uai-1-csharp-web-starter.zip'
if (Test-Path $zipPath) {
    Remove-Item -LiteralPath $zipPath -Force
}

Compress-Archive -Path (Join-Path $archiveRoot '*') -DestinationPath $zipPath -CompressionLevel Optimal

$packageDestination = Join-Path $siteDownloadRoot $packageFile.Name
Get-ChildItem -Path $siteDownloadRoot -Filter 'Protocol5.UAI.CSharp.*.nupkg' | Remove-Item -Force -ErrorAction SilentlyContinue
Copy-Item -LiteralPath $packageFile.FullName -Destination $packageDestination -Force

$zipHash = (Get-FileHash -Algorithm SHA256 -Path $zipPath).Hash.ToLowerInvariant()
$packageHash = (Get-FileHash -Algorithm SHA256 -Path $packageDestination).Hash.ToLowerInvariant()

Write-Utf8File -Path "$zipPath.sha256" -Content "$zipHash  $(Split-Path $zipPath -Leaf)`n"
Write-Utf8File -Path "$packageDestination.sha256" -Content "$packageHash  $(Split-Path $packageDestination -Leaf)`n"
