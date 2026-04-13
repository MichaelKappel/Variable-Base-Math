param(
    [string]$PublishRoot = "C:\Publish\Protocol5",
    [string]$Configuration = "Release",
    [string]$StagingRoot = "",
    [string]$PreserveContentFrom = ""
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$hostProject = Join-Path $repoRoot "Protocol5.com.Host\Protocol5.com.Host.csproj"
$uaiPageScript = Join-Path $repoRoot "tools\Generate-Protocol5UaiPages.ps1"
$uaiStarterScript = Join-Path $repoRoot "tools\Build-Protocol5UaiCSharpWebsiteSupport.ps1"
$defaultLiveRoot = 'E:\Sites\Protocol5.com\$web'

function Resolve-FullPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    return [System.IO.Path]::GetFullPath($Path)
}

function Get-Protocol5PreserveRoot {
    param(
        [Parameter(Mandatory = $true)]
        [string]$PublishRoot,
        [Parameter(Mandatory = $true)]
        [string]$ExplicitRoot,
        [Parameter(Mandatory = $true)]
        [string]$DefaultLiveRoot
    )

    if (-not [string]::IsNullOrWhiteSpace($ExplicitRoot)) {
        $resolvedExplicitRoot = Resolve-FullPath -Path $ExplicitRoot
        if (-not (Test-Path (Join-Path $resolvedExplicitRoot 'SiteContent'))) {
            throw "PreserveContentFrom must point to a Protocol5 publish root containing SiteContent. Value: $resolvedExplicitRoot"
        }

        return $resolvedExplicitRoot
    }

    if (Test-Path (Join-Path $PublishRoot 'SiteContent')) {
        return $PublishRoot
    }

    $resolvedDefaultLiveRoot = Resolve-FullPath -Path $DefaultLiveRoot
    if ((Test-Path (Join-Path $resolvedDefaultLiveRoot 'SiteContent')) -and
        -not [string]::Equals($resolvedDefaultLiveRoot, $PublishRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
        return $resolvedDefaultLiveRoot
    }

    return ""
}

function Invoke-Robocopy {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Source,
        [Parameter(Mandatory = $true)]
        [string]$Destination,
        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    & robocopy $Source $Destination @Arguments | Out-Null
    if ($LASTEXITCODE -gt 7) {
        throw "Robocopy failed for '$Source' -> '$Destination' with exit code $LASTEXITCODE."
    }
}

function Copy-Protocol5PreservedContent {
    param(
        [Parameter(Mandatory = $true)]
        [string]$SourceRoot,
        [Parameter(Mandatory = $true)]
        [string]$DestinationRoot
    )

    $sourceSiteContent = Join-Path $SourceRoot 'SiteContent'
    $destinationSiteContent = Join-Path $DestinationRoot 'SiteContent'

    $preservedDirectories = @(
        @{
            Name = 'Fibonacci'
            ExcludeFiles = @('index.html')
        },
        @{
            Name = 'Prime'
            ExcludeFiles = @('index.html')
        },
        @{
            Name = 'Fibonaccis'
            ExcludeFiles = @()
        }
    )

    foreach ($directory in $preservedDirectories) {
        $sourcePath = Join-Path $sourceSiteContent $directory.Name
        if (-not (Test-Path $sourcePath)) {
            continue
        }

        $destinationPath = Join-Path $destinationSiteContent $directory.Name
        New-Item -ItemType Directory -Path $destinationPath -Force | Out-Null

        $arguments = @('/E', '/R:1', '/W:1', '/NFL', '/NDL', '/NJH', '/NJS', '/NP')
        if ($directory.ExcludeFiles.Count -gt 0) {
            $arguments += '/XF'
            $arguments += $directory.ExcludeFiles
        }

        Write-Host "Preserving published $($directory.Name) content from $SourceRoot"
        Invoke-Robocopy -Source $sourcePath -Destination $destinationPath -Arguments $arguments
    }
}

function Sync-Protocol5PublishRoot {
    param(
        [Parameter(Mandatory = $true)]
        [string]$SourceRoot,
        [Parameter(Mandatory = $true)]
        [string]$DestinationRoot
    )

    New-Item -ItemType Directory -Path $DestinationRoot -Force | Out-Null
    Write-Host "Syncing staged publish output into $DestinationRoot"
    Invoke-Robocopy -Source $SourceRoot -Destination $DestinationRoot -Arguments @('/E', '/R:1', '/W:1', '/NFL', '/NDL', '/NJH', '/NJS', '/NP')
}

$resolvedPublishRoot = Resolve-FullPath -Path $PublishRoot
$resolvedStagingRoot = if ([string]::IsNullOrWhiteSpace($StagingRoot)) {
    Resolve-FullPath -Path (Join-Path $repoRoot '.artifacts\publish\Protocol5')
}
else {
    Resolve-FullPath -Path $StagingRoot
}

if ([string]::Equals($resolvedPublishRoot, $resolvedStagingRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "PublishRoot and StagingRoot must be different directories."
}

if (Test-Path $resolvedStagingRoot) {
    Remove-Item -LiteralPath $resolvedStagingRoot -Recurse -Force
}

New-Item -ItemType Directory -Path $resolvedStagingRoot -Force | Out-Null

Write-Host "Generating Protocol5 UAI pages"
& powershell -ExecutionPolicy Bypass -File $uaiPageScript

Write-Host "Building Protocol5 UAI C# website starter downloads"
& powershell -ExecutionPolicy Bypass -File $uaiStarterScript -Configuration $Configuration

Write-Host "Publishing Protocol5.com.Host to staging at $resolvedStagingRoot"
dotnet publish $hostProject -c $Configuration /p:PublishDir="$resolvedStagingRoot\"

$preserveRoot = Get-Protocol5PreserveRoot -PublishRoot $resolvedPublishRoot -ExplicitRoot $PreserveContentFrom -DefaultLiveRoot $defaultLiveRoot
if (-not [string]::IsNullOrWhiteSpace($preserveRoot) -and
    -not [string]::Equals($preserveRoot, $resolvedPublishRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
    Copy-Protocol5PreservedContent -SourceRoot $preserveRoot -DestinationRoot $resolvedStagingRoot
}

Sync-Protocol5PublishRoot -SourceRoot $resolvedStagingRoot -DestinationRoot $resolvedPublishRoot

Write-Host "Protocol5 publish complete."
