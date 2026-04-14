param(
    [string]$PublishRoot = "C:\Publish\Protocol5",
    [string]$Configuration = "Release",
    [string]$StagingRoot = "",
    [string]$PreserveContentFrom = "",
    [string]$ReportPath = "",
    [switch]$ApplyToPublishRoot
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$hostProject = Join-Path $repoRoot "Protocol5.com.Host\Protocol5.com.Host.csproj"
$uaiPageScript = Join-Path $repoRoot "tools\Generate-Protocol5UaiPages.ps1"
$uaiStarterScript = Join-Path $repoRoot "tools\Build-Protocol5UaiCSharpWebsiteSupport.ps1"
$defaultLiveRoot = 'E:\Sites\Protocol5.com\$web'
$preservedPathGlobs = @(
    'SiteContent\Fibonacci\*.htm',
    'SiteContent\Prime\*.htm',
    'SiteContent\Fibonacci\index.htm',
    'SiteContent\Prime\index.htm',
    'SiteContent\Fibonaccis\*'
)
$requiredRelativePaths = @(
    'Protocol5.com.Host.dll',
    'SiteContent\index.html',
    'SiteContent\Fibonacci\index.html',
    'SiteContent\Prime\index.html',
    'SiteContent\UAI\index.html',
    'SiteContent\UAI-1\index.html',
    'SiteContent\UAI-1\examples\index.html',
    'SiteContent\UAI-1\csharp-website-support\index.html',
    'SiteContent\AI_Declaration_of_Independence.htm',
    'SiteContent\Cognitive_Liberty_Charter.htm',
    'SiteContent\UAI-1.json',
    'SiteContent\UAI-1-examples.json',
    'SiteContent\schema\uai-1.schema.json',
    'SiteContent\registry\uai-1.json',
    'SiteContent\registry\uai-1-examples.json',
    'SiteContent\registry\symbols.json',
    'SiteContent\downloads\UAI-1-Package.zip',
    'SiteContent\downloads\protocol5-uai-1-csharp-web-starter.zip',
    'SiteContent\downloads\Protocol5.UAI.CSharp.1.0.0.nupkg'
)

function Resolve-FullPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    return [System.IO.Path]::GetFullPath($Path)
}

function Write-Utf8File {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,
        [Parameter(Mandatory = $true)]
        [string]$Content
    )

    $directory = Split-Path -Path $Path -Parent
    if (-not [string]::IsNullOrWhiteSpace($directory)) {
        New-Item -ItemType Directory -Path $directory -Force | Out-Null
    }

    $encoding = [System.Text.UTF8Encoding]::new($false)
    [System.IO.File]::WriteAllText($Path, $Content, $encoding)
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
        @{ Name = 'Fibonacci'; ExcludeFiles = @('index.html') },
        @{ Name = 'Prime'; ExcludeFiles = @('index.html') },
        @{ Name = 'Fibonaccis'; ExcludeFiles = @() }
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

function Assert-Protocol5RequiredFiles {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Root,
        [Parameter(Mandatory = $true)]
        [string[]]$RelativePaths
    )

    $missing = @()
    foreach ($relativePath in $RelativePaths) {
        $fullPath = Join-Path $Root $relativePath
        if (-not (Test-Path -LiteralPath $fullPath)) {
            $missing += $relativePath
        }
    }

    if ($missing.Count -gt 0) {
        throw "Staging output is missing required files:`n - $($missing -join "`n - ")"
    }
}

function New-Protocol5ComparisonReport {
    param(
        [Parameter(Mandatory = $true)]
        [string]$SourceRoot,
        [Parameter(Mandatory = $true)]
        [string]$DestinationRoot,
        [Parameter(Mandatory = $true)]
        [string[]]$RequiredPaths,
        [Parameter(Mandatory = $true)]
        [string[]]$PreservedPaths
    )

    $changes = New-Object System.Collections.Generic.List[object]
    foreach ($file in Get-ChildItem -Path $SourceRoot -File -Recurse | Sort-Object FullName) {
        $relativePath = [System.IO.Path]::GetRelativePath($SourceRoot, $file.FullName)
        $destinationPath = Join-Path $DestinationRoot $relativePath
        $destinationExists = Test-Path -LiteralPath $destinationPath

        $status = 'new'
        if ($destinationExists) {
            $sourceHash = (Get-FileHash -Algorithm SHA256 -Path $file.FullName).Hash
            $destinationHash = (Get-FileHash -Algorithm SHA256 -Path $destinationPath).Hash
            $status = if ([string]::Equals($sourceHash, $destinationHash, [System.StringComparison]::OrdinalIgnoreCase)) { 'unchanged' } else { 'changed' }
        }

        $changes.Add([ordered]@{
            relativePath = $relativePath
            status = $status
            size = $file.Length
            destinationExists = $destinationExists
        }) | Out-Null
    }

    $summary = [ordered]@{
        total = $changes.Count
        new = @($changes | Where-Object { $_.status -eq 'new' }).Count
        changed = @($changes | Where-Object { $_.status -eq 'changed' }).Count
        unchanged = @($changes | Where-Object { $_.status -eq 'unchanged' }).Count
    }

    return [ordered]@{
        generatedAt = [DateTimeOffset]::UtcNow.ToString('O')
        stagingRoot = $SourceRoot
        destinationRoot = $DestinationRoot
        applyToPublishRoot = $ApplyToPublishRoot.IsPresent
        requiredFiles = $RequiredPaths
        preservedPaths = $PreservedPaths
        summary = $summary
        changes = @($changes)
    }
}

$resolvedPublishRoot = Resolve-FullPath -Path $PublishRoot
$resolvedStagingRoot = if ([string]::IsNullOrWhiteSpace($StagingRoot)) {
    Resolve-FullPath -Path (Join-Path $repoRoot '.artifacts\publish\Protocol5')
}
else {
    Resolve-FullPath -Path $StagingRoot
}
$resolvedReportPath = if ([string]::IsNullOrWhiteSpace($ReportPath)) {
    Resolve-FullPath -Path (Join-Path $repoRoot '.artifacts\publish\Protocol5.publish-report.json')
}
else {
    Resolve-FullPath -Path $ReportPath
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

Assert-Protocol5RequiredFiles -Root $resolvedStagingRoot -RelativePaths $requiredRelativePaths

$report = New-Protocol5ComparisonReport -SourceRoot $resolvedStagingRoot -DestinationRoot $resolvedPublishRoot -RequiredPaths $requiredRelativePaths -PreservedPaths $preservedPathGlobs
Write-Utf8File -Path $resolvedReportPath -Content (($report | ConvertTo-Json -Depth 8))

Write-Host "Protocol5 staging publish ready."
Write-Host "Comparison report: $resolvedReportPath"
Write-Host "Changed files: $($report.summary.changed) | New files: $($report.summary.new) | Unchanged files: $($report.summary.unchanged)"
Write-Host "Preserved live-only areas:"
foreach ($preservedPath in $preservedPathGlobs) {
    Write-Host " - $preservedPath"
}

if (-not $ApplyToPublishRoot.IsPresent) {
    Write-Host "Stage-only mode complete. Review the report and rerun with -ApplyToPublishRoot to sync staged output into PublishRoot."
    return
}

$preserveRoot = Get-Protocol5PreserveRoot -PublishRoot $resolvedPublishRoot -ExplicitRoot $PreserveContentFrom -DefaultLiveRoot $defaultLiveRoot
if (-not [string]::IsNullOrWhiteSpace($preserveRoot) -and
    -not [string]::Equals($preserveRoot, $resolvedPublishRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
    Copy-Protocol5PreservedContent -SourceRoot $preserveRoot -DestinationRoot $resolvedStagingRoot
}

Sync-Protocol5PublishRoot -SourceRoot $resolvedStagingRoot -DestinationRoot $resolvedPublishRoot

Write-Host "Protocol5 publish complete."
Write-Host "Report retained at $resolvedReportPath"