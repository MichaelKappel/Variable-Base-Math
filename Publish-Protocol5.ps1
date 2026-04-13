param(
    [string]$PublishRoot = "C:\Publish\Protocol5",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$hostProject = Join-Path $repoRoot "Protocol5.com.Host\Protocol5.com.Host.csproj"
$uaiPageScript = Join-Path $repoRoot "tools\Generate-Protocol5UaiPages.ps1"
$uaiStarterScript = Join-Path $repoRoot "tools\Build-Protocol5UaiCSharpWebsiteSupport.ps1"

New-Item -ItemType Directory -Path $PublishRoot -Force | Out-Null

Write-Host "Generating Protocol5 UAI pages"
& powershell -ExecutionPolicy Bypass -File $uaiPageScript

Write-Host "Building Protocol5 UAI C# website starter downloads"
& powershell -ExecutionPolicy Bypass -File $uaiStarterScript -Configuration $Configuration

Write-Host "Publishing Protocol5.com.Host to $PublishRoot"
dotnet publish $hostProject -c $Configuration /p:PublishDir="$PublishRoot\"

Write-Host "Protocol5 publish complete."
