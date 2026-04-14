param(
    [switch]$UpdateReport
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Path $PSScriptRoot -Parent
$uaiRoot = Join-Path $repoRoot 'UAI'
$reportPath = Join-Path $repoRoot 'translations.md'
$uaiCultureInfoPath = Join-Path $repoRoot 'Protocol5.UAI.CSharp\UaiCultureInfo.cs'
$siteRoot = Join-Path $repoRoot 'Protocol5.com\SiteContent'
$uaiTranslationConfigPath = Join-Path $uaiRoot 'uai-translation-config.json'
$uaiTranslationConfig = [System.IO.File]::ReadAllText($uaiTranslationConfigPath, [System.Text.Encoding]::UTF8) | ConvertFrom-Json

$defaultLocale = [string]$uaiTranslationConfig.defaultLocale
$humanLocales = @(
    $uaiTranslationConfig.humanLocales | ForEach-Object {
        [ordered]@{
            Code = [string]$_.code
            Label = [string]$_.label
        }
    }
)
$documentFamilies = @(
    $uaiTranslationConfig.documentFamilies | ForEach-Object {
        [ordered]@{
            BaseName = [string]$_.baseName
            Title = [string]$_.title
            CanonicalRoute = [string]$_.canonicalRoute
            CanonicalSitePath = [string]$_.canonicalSitePath
        }
    }
)
$legacyCompatibilityRoutes = @($uaiTranslationConfig.legacyCompatibilityRoutes | ForEach-Object { [string]$_ })
$removedRouteTerms = @(
    $uaiTranslationConfig.removedRouteTerms | ForEach-Object {
        $termParts = @($_.parts | ForEach-Object { [string]$_ })
        [ordered]@{
            DisplayName = if ($_.displayName) { [string]$_.displayName } else { 'removed legacy route slug' }
            SearchTerm = ($termParts -join '-')
        }
    }
)

function Get-CanonicalLanguageTags {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    $content = [System.IO.File]::ReadAllText($Path, [System.Text.Encoding]::UTF8)
    $canonicalMatch = [regex]::Match($content, 'CanonicalLanguageTag\s*=\s*"(?<tag>[^"]+)"')
    $canonicalTag = if ($canonicalMatch.Success) { $canonicalMatch.Groups['tag'].Value } else { 'x-uai-1' }

    $tags = [System.Collections.Generic.List[string]]::new()
    $tags.Add($canonicalTag)

    $supportedTagsMatch = [regex]::Match(
        $content,
        'SupportedTags\s*=\s*\{(?<body>.*?)\};',
        [System.Text.RegularExpressions.RegexOptions]::Singleline
    )

    if ($supportedTagsMatch.Success) {
        foreach ($entryMatch in [regex]::Matches($supportedTagsMatch.Groups['body'].Value, '"(?<tag>[^"]+)"')) {
            $tag = $entryMatch.Groups['tag'].Value
            if (-not $tags.Contains($tag)) {
                $tags.Add($tag)
            }
        }
    }

    return [ordered]@{
        CanonicalTag = $canonicalTag
        AcceptedTags = $tags
    }
}

function Get-NaturalLanguageLines {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Markdown
    )

    $normalized = $Markdown.Replace("`r`n", "`n").Replace("`r", "`n")
    $lines = [System.Collections.Generic.List[string]]::new()
    $inCodeBlock = $false

    foreach ($line in ($normalized -split "`n")) {
        $trimmed = $line.Trim()

        if ($trimmed -match '^```') {
            $inCodeBlock = -not $inCodeBlock
            continue
        }

        if ($inCodeBlock -or [string]::IsNullOrWhiteSpace($trimmed)) {
            continue
        }

        $candidate = $trimmed
        $candidate = [regex]::Replace($candidate, '!\[[^\]]*\]\([^)]+\)', '')
        $candidate = [regex]::Replace($candidate, '\[([^\]]+)\]\([^)]+\)', '$1')
        $candidate = [regex]::Replace($candidate, '`[^`]+`', '')
        $candidate = [regex]::Replace($candidate, '^\s*[-*+]\s*', '')
        $candidate = [regex]::Replace($candidate, '^\s*\d+\.\s*', '')
        $candidate = [regex]::Replace($candidate, '^#+\s*', '')
        $candidate = [regex]::Replace($candidate, '\s+', ' ').Trim()

        if ([string]::IsNullOrWhiteSpace($candidate)) {
            continue
        }

        if ($candidate -notmatch '\p{L}') {
            continue
        }

        $lines.Add($candidate)
    }

    return $lines
}

function Normalize-ComparisonText {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Text
    )

    $normalized = $Text.ToLowerInvariant()
    $normalized = [regex]::Replace($normalized, '[\p{P}\p{S}\d_]+', ' ')
    $normalized = [regex]::Replace($normalized, '\s+', ' ').Trim()
    return $normalized
}

function Get-UniqueTokens {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Lines
    )

    $tokens = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)

    foreach ($line in $Lines) {
        foreach ($token in ($line -split ' ')) {
            if ($token.Length -ge 4) {
                [void]$tokens.Add($token)
            }
        }
    }

    return $tokens
}

function Test-TranslationQuality {
    param(
        [Parameter(Mandatory = $true)]
        [string]$EnglishMarkdown,
        [Parameter(Mandatory = $true)]
        [string]$CandidateMarkdown,
        [Parameter(Mandatory = $true)]
        [string]$Locale
    )

    if ($Locale -eq $defaultLocale) {
        return [ordered]@{
            IsSuspicious = $false
            Notes = @('Default source locale.')
            MatchingLineRatio = 1.0
            TokenOverlapRatio = 1.0
        }
    }

    $englishLines = @(Get-NaturalLanguageLines -Markdown $EnglishMarkdown | ForEach-Object { Normalize-ComparisonText -Text $_ } | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    $candidateLines = @(Get-NaturalLanguageLines -Markdown $CandidateMarkdown | ForEach-Object { Normalize-ComparisonText -Text $_ } | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })

    if ($candidateLines.Count -eq 0) {
        return [ordered]@{
            IsSuspicious = $true
            Notes = @('No natural-language lines were found after normalization.')
            MatchingLineRatio = 1.0
            TokenOverlapRatio = 1.0
        }
    }

    $englishLineSet = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($line in $englishLines) {
        [void]$englishLineSet.Add($line)
    }

    $matchingLineCount = 0
    foreach ($line in $candidateLines) {
        if ($englishLineSet.Contains($line)) {
            $matchingLineCount++
        }
    }

    $comparisonLineCount = [Math]::Max(1, [Math]::Min($englishLines.Count, $candidateLines.Count))
    $matchingLineRatio = $matchingLineCount / $comparisonLineCount

    $englishTokens = Get-UniqueTokens -Lines $englishLines
    $candidateTokens = Get-UniqueTokens -Lines $candidateLines

    $intersectionCount = 0
    foreach ($token in $candidateTokens) {
        if ($englishTokens.Contains($token)) {
            $intersectionCount++
        }
    }

    $unionCount = [Math]::Max(1, ($englishTokens.Count + $candidateTokens.Count - $intersectionCount))
    $tokenOverlapRatio = $intersectionCount / $unionCount

    $isSuspicious = $false
    $notes = [System.Collections.Generic.List[string]]::new()

    if ((($englishLines -join "`n") -eq ($candidateLines -join "`n"))) {
        $isSuspicious = $true
        $notes.Add('Natural-language content matches the English source after normalization.')
    }

    if ($matchingLineRatio -ge 0.55) {
        $isSuspicious = $true
        $notes.Add(("Too many normalized lines still match English ({0:P0})." -f $matchingLineRatio))
    }

    if ($tokenOverlapRatio -ge 0.72) {
        $isSuspicious = $true
        $notes.Add(("Token overlap with English is too high ({0:P0})." -f $tokenOverlapRatio))
    }

    if ($notes.Count -eq 0) {
        $notes.Add('Materially distinct from English by heuristic audit.')
    }

    return [ordered]@{
        IsSuspicious = $isSuspicious
        Notes = $notes
        MatchingLineRatio = [Math]::Round($matchingLineRatio, 4)
        TokenOverlapRatio = [Math]::Round($tokenOverlapRatio, 4)
    }
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
        [System.IO.Directory]::CreateDirectory($directory) | Out-Null
    }

    $encoding = [System.Text.UTF8Encoding]::new($true)
    [System.IO.File]::WriteAllText($Path, $Content, $encoding)
}

function Get-ExpectedHtmlPath {
    param(
        [Parameter(Mandatory = $true)]
        [hashtable]$Family,
        [Parameter(Mandatory = $true)]
        [string]$Locale
    )

    $baseDirectory = Join-Path $siteRoot $Family.CanonicalSitePath
    if ($Locale -ne $defaultLocale) {
        $baseDirectory = Join-Path $baseDirectory $Locale
    }

    return Join-Path $baseDirectory 'index.html'
}

function Get-StaleRouteHits {
    param(
        [Parameter(Mandatory = $true)]
        [object[]]$Terms
    )

    $searchPaths = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    $searchExtensions = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    foreach ($extension in @('.cs', '.html', '.json', '.md', '.ps1', '.txt')) {
        [void]$searchExtensions.Add($extension)
    }

    foreach ($file in @(
        (Join-Path $repoRoot 'README.md')
    )) {
        if (Test-Path -LiteralPath $file) {
            [void]$searchPaths.Add($file)
        }
    }

    foreach ($directory in @(
        $uaiRoot,
        (Join-Path $repoRoot 'Protocol5.com.Host'),
        (Join-Path $repoRoot 'Protocol5.com\SiteContent\UAI'),
        (Join-Path $repoRoot 'Protocol5.com\SiteContent\UAI-1'),
        (Join-Path $repoRoot 'Protocol5.UAI.CSharp'),
        (Join-Path $repoRoot 'tools')
    )) {
        if (-not (Test-Path -LiteralPath $directory)) {
            continue
        }

        Get-ChildItem -Path $directory -Recurse -File |
            Where-Object {
                $searchExtensions.Contains($_.Extension) -and
                $_.FullName -notmatch '\\(bin|obj|\.git|\.vs)\\' -and
                $_.FullName -ne $PSCommandPath -and
                $_.FullName -ne $uaiTranslationConfigPath -and
                $_.FullName -ne $reportPath
            } |
            ForEach-Object {
                [void]$searchPaths.Add($_.FullName)
            }
    }

    $hits = [System.Collections.Generic.List[hashtable]]::new()
    foreach ($termItem in $Terms) {
        $term = [string]$termItem.SearchTerm
        foreach ($path in $searchPaths) {
            foreach ($match in (Select-String -Path $path -SimpleMatch -Pattern $term)) {
                $hits.Add([ordered]@{
                    Term = $term
                    DisplayName = [string]$termItem.DisplayName
                    Path = $path
                    Line = $match.LineNumber
                    Text = $match.Line.Trim()
                })
            }
        }
    }

    return $hits
}

$uaiTags = Get-CanonicalLanguageTags -Path $uaiCultureInfoPath
$locales = @($humanLocales | ForEach-Object { $_.Code })

$rows = [System.Collections.Generic.List[hashtable]]::new()

foreach ($family in $documentFamilies) {
    $englishPath = Join-Path $uaiRoot ("{0}.{1}.md" -f $family.BaseName, $defaultLocale)
    if (-not (Test-Path -LiteralPath $englishPath)) {
        throw "Missing required source locale file: $englishPath"
    }

    $englishMarkdown = [System.IO.File]::ReadAllText($englishPath, [System.Text.Encoding]::UTF8)

    foreach ($locale in $locales) {
        $path = Join-Path $uaiRoot ("{0}.{1}.md" -f $family.BaseName, $locale)
        $expectedHtmlPath = Get-ExpectedHtmlPath -Family $family -Locale $locale
        $htmlStatus = if (-not (Test-Path -LiteralPath $expectedHtmlPath)) {
            'missing'
        } elseif ((Get-Item -LiteralPath $expectedHtmlPath).Length -le 0) {
            'empty'
        } else {
            'present'
        }

        if (-not (Test-Path -LiteralPath $path)) {
            $rows.Add([ordered]@{
                Document = $family.Title
                BaseName = $family.BaseName
                Locale = $locale
                Status = 'missing'
                HtmlStatus = $htmlStatus
                Notes = @('Missing localized markdown file.')
                MatchingLineRatio = $null
                TokenOverlapRatio = $null
            })
            continue
        }

        $candidateMarkdown = [System.IO.File]::ReadAllText($path, [System.Text.Encoding]::UTF8)
        $quality = Test-TranslationQuality -EnglishMarkdown $englishMarkdown -CandidateMarkdown $candidateMarkdown -Locale $locale

        $rows.Add([ordered]@{
            Document = $family.Title
            BaseName = $family.BaseName
            Locale = $locale
            Status = if ($quality.IsSuspicious) { 'suspicious' } else { 'complete' }
            HtmlStatus = $htmlStatus
            Notes = $quality.Notes
            MatchingLineRatio = $quality.MatchingLineRatio
            TokenOverlapRatio = $quality.TokenOverlapRatio
        })
    }
}

$requiredPairs = $documentFamilies.Count * $locales.Count
$completePairs = @($rows | Where-Object { $_.Status -eq 'complete' }).Count
$missingPairs = @($rows | Where-Object { $_.Status -eq 'missing' }).Count
$suspiciousPairs = @($rows | Where-Object { $_.Status -eq 'suspicious' }).Count
$nonEnglishPairs = @($rows | Where-Object { $_.Locale -ne $defaultLocale -and $_.Status -ne 'missing' }).Count
$requiredHtmlPages = $requiredPairs
$htmlPagesWithContent = @($rows | Where-Object { $_.HtmlStatus -eq 'present' }).Count
$missingOrEmptyHtmlPages = @($rows | Where-Object { $_.HtmlStatus -ne 'present' }).Count
$staleRouteHits = @(Get-StaleRouteHits -Terms $removedRouteTerms)
$staleRouteHitCount = $staleRouteHits.Count
$isComplete = ($missingPairs -eq 0 -and $suspiciousPairs -eq 0 -and $missingOrEmptyHtmlPages -eq 0 -and $staleRouteHitCount -eq 0)

$summaryLines = @(
    "UAI-1 translation audit"
    ("- Supported human locales: {0}" -f (($humanLocales | ForEach-Object { "$($_.Code) ($($_.Label))" }) -join ', '))
    ("- Human locales audited: {0}" -f ($locales -join ', '))
    ("- Required doc/locale pairs: {0}" -f $requiredPairs)
    ("- Completed pairs: {0}" -f $completePairs)
    ("- Missing pairs: {0}" -f $missingPairs)
    ("- Suspicious pairs: {0}" -f $suspiciousPairs)
    ("- Non-English pairs present: {0}" -f $nonEnglishPairs)
    ("- Required canonical HTML pages: {0}" -f $requiredHtmlPages)
    ("- Canonical HTML pages with content: {0}" -f $htmlPagesWithContent)
    ("- Missing or empty canonical HTML pages: {0}" -f $missingOrEmptyHtmlPages)
    ("- Removed legacy term hits: {0}" -f $staleRouteHitCount)
)

$summaryLines | ForEach-Object { Write-Output $_ }

if ($UpdateReport) {
    $builder = [System.Text.StringBuilder]::new()
    $auditDate = Get-Date -Format 'yyyy-MM-dd'

    [void]$builder.AppendLine('# Translation Progress')
    [void]$builder.AppendLine()
    [void]$builder.AppendLine(("Last audited: {0}" -f $auditDate))
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('## Scope')
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('This file tracks the UAI-1 document family in this repository and records translation coverage and anti-cheat audit results.')
    [void]$builder.AppendLine('`x-uai-1` and its accepted aliases are language tags for the machine language, not separate human-language translations.')
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('## UAI-1 Tag Support')
    [void]$builder.AppendLine()
    $canonicalTagLine = "- Canonical language tag: ``{0}``" -f $uaiTags.CanonicalTag
    $acceptedTagsLine = "- Accepted language tags: {0}" -f (($uaiTags.AcceptedTags | ForEach-Object { ('`{0}`' -f $_) }) -join ', ')
    $humanLocalesLine = "- Supported human locales required by config: {0}" -f (($humanLocales | ForEach-Object { ('`{0}` ({1})' -f $_.Code, $_.Label) }) -join ', ')
    [void]$builder.AppendLine($canonicalTagLine)
    [void]$builder.AppendLine($acceptedTagsLine)
    [void]$builder.AppendLine($humanLocalesLine)
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('## Route Direction')
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('- Canonical public routes:')
    foreach ($family in $documentFamilies) {
        [void]$builder.AppendLine(('  - `{0}` for {1}' -f $family.CanonicalRoute, $family.Title))
    }
    [void]$builder.AppendLine('- Legacy compatibility routes retained only as redirects:')
    foreach ($legacyRoute in $legacyCompatibilityRoutes) {
        [void]$builder.AppendLine(('  - `{0}`' -f $legacyRoute))
    }
    [void]$builder.AppendLine('- Removed legacy route checks that must stay clear:')
    foreach ($removedTerm in $removedRouteTerms) {
        [void]$builder.AppendLine(('  - {0}' -f $removedTerm.DisplayName))
    }
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('## Progress Stats')
    [void]$builder.AppendLine()
    $familiesLine = "- UAI-1 document families audited: ``{0}``" -f $documentFamilies.Count
    $localesCountLine = "- Supported human locales audited: ``{0}``" -f $locales.Count
    $requiredPairsLine = "- Required doc/locale pairs: ``{0}``" -f $requiredPairs
    $completedPairsLine = "- Completed pairs: ``{0}``" -f $completePairs
    $missingPairsLine = "- Missing pairs: ``{0}``" -f $missingPairs
    $nonEnglishPairsLine = "- Non-English pairs present: ``{0}``" -f $nonEnglishPairs
    $suspiciousPairsLine = "- Suspicious or cheating pairs: ``{0}``" -f $suspiciousPairs
    $requiredHtmlPagesLine = "- Required canonical HTML pages: ``{0}``" -f $requiredHtmlPages
    $htmlPagesWithContentLine = "- Canonical HTML pages with content: ``{0}``" -f $htmlPagesWithContent
    $missingOrEmptyHtmlPagesLine = "- Missing or empty canonical HTML pages: ``{0}``" -f $missingOrEmptyHtmlPages
    $staleTermsLine = "- Removed legacy term hits: ``{0}``" -f $staleRouteHitCount
    [void]$builder.AppendLine($familiesLine)
    [void]$builder.AppendLine($localesCountLine)
    [void]$builder.AppendLine($requiredPairsLine)
    [void]$builder.AppendLine($completedPairsLine)
    [void]$builder.AppendLine($missingPairsLine)
    [void]$builder.AppendLine($nonEnglishPairsLine)
    [void]$builder.AppendLine($suspiciousPairsLine)
    [void]$builder.AppendLine($requiredHtmlPagesLine)
    [void]$builder.AppendLine($htmlPagesWithContentLine)
    [void]$builder.AppendLine($missingOrEmptyHtmlPagesLine)
    [void]$builder.AppendLine($staleTermsLine)
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('## Quality Rules')
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('- A non-English locale file must materially translate natural-language prose.')
    [void]$builder.AppendLine('- Code blocks, canonical IDs, URLs, language tags, and registry values may remain unchanged.')
    [void]$builder.AppendLine('- A locale fails the audit if it is effectively the English source with only casing, punctuation, whitespace, or other trivial token changes.')
    [void]$builder.AppendLine('- High normalized line overlap or high token overlap with English is treated as suspicious and blocks completion.')
    [void]$builder.AppendLine('- Canonical `/UAI-1...` HTML pages must exist and contain content for every required document/locale pair.')
    [void]$builder.AppendLine('- Removed legacy route slugs must not appear anywhere in the audited UAI-1 area.')
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('## Status')
    [void]$builder.AppendLine()

    if ($isComplete) {
        [void]$builder.AppendLine('- Translation coverage is complete for the configured UAI-1 human-locale set.')
        [void]$builder.AppendLine('- No suspicious near-English or formatting-only translations were found in the audited UAI-1 family.')
        [void]$builder.AppendLine('- Canonical `/UAI-1...` HTML pages exist for every required document/locale pair.')
        [void]$builder.AppendLine('- No removed legacy route slugs were found in the audited UAI-1 area.')
    } else {
        [void]$builder.AppendLine('- Translation coverage is not yet complete.')
        [void]$builder.AppendLine('- Any missing, suspicious, empty-output, or stale-term rows below must be resolved before claiming completion.')
    }

    [void]$builder.AppendLine()
    [void]$builder.AppendLine('## Removed Legacy Term Audit')
    [void]$builder.AppendLine()
    if ($staleRouteHitCount -eq 0) {
        [void]$builder.AppendLine('- No removed legacy route slugs were found.')
    } else {
        foreach ($hit in $staleRouteHits) {
            $relativePath = Resolve-Path -LiteralPath $hit.Path | ForEach-Object { $_.Path.Replace($repoRoot + '\', '') }
            $sanitizedHitText = $hit.Text -replace '`', "'"
            [void]$builder.AppendLine(('- {0} in `{1}:{2}`: `{3}`' -f $hit.DisplayName, $relativePath, $hit.Line, $sanitizedHitText))
        }
    }

    [void]$builder.AppendLine()
    [void]$builder.AppendLine('## Coverage Matrix')
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('| Document | Locale | Status | HTML | Match | Token | Notes |')
    [void]$builder.AppendLine('| --- | --- | --- | --- | --- | --- | --- |')

    foreach ($row in $rows) {
        $notes = ($row.Notes -join ' ').Replace('|', '\|')
        $matchRatio = if ($null -eq $row.MatchingLineRatio) { 'n/a' } else { '{0:P0}' -f [double]$row.MatchingLineRatio }
        $tokenRatio = if ($null -eq $row.TokenOverlapRatio) { 'n/a' } else { '{0:P0}' -f [double]$row.TokenOverlapRatio }
        [void]$builder.AppendLine(("| {0} | `{1}` | {2} | {3} | {4} | {5} | {6} |" -f $row.Document, $row.Locale, $row.Status, $row.HtmlStatus, $matchRatio, $tokenRatio, $notes))
    }

    [void]$builder.AppendLine()
    [void]$builder.AppendLine('## Maintenance')
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('- Keep `UAI/uai-translation-config.json` aligned with the supported locale set and canonical `/UAI-1...` route family.')
    [void]$builder.AppendLine('- Re-run `powershell -ExecutionPolicy Bypass -File tools\Generate-Protocol5UaiPages.ps1` after adding or editing any UAI-1 translation file so the canonical HTML output stays current.')
    [void]$builder.AppendLine('- Re-run `powershell -ExecutionPolicy Bypass -File tools\Audit-UaiTranslations.ps1 -UpdateReport` after adding or editing any UAI-1 translation file.')
    [void]$builder.AppendLine('- Add new human locales only when the prose is materially translated. Do not add casing-only, punctuation-only, or nearly-English placeholders.')

    Write-Utf8File -Path $reportPath -Content $builder.ToString()
    Write-Output ("Updated report: {0}" -f $reportPath)
}

if (-not $isComplete) {
    exit 1
}
