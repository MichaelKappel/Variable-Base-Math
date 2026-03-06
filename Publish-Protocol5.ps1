param(
    [string]$PublishRoot = "C:\Publish\Protocol5",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$hostProject = Join-Path $repoRoot "Protocol5.com.Host\Protocol5.com.Host.csproj"

New-Item -ItemType Directory -Path $PublishRoot -Force | Out-Null

Write-Host "Publishing Protocol5.com.Host to $PublishRoot"
dotnet publish $hostProject -c $Configuration /p:PublishDir="$PublishRoot\"

Write-Host "Protocol5 publish complete."
