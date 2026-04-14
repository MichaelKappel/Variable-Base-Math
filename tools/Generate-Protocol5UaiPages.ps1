$ErrorActionPreference = 'Stop'

function Convert-InlineMarkdown {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Text
    )

    $encoded = [System.Net.WebUtility]::HtmlEncode($Text)

    $encoded = [regex]::Replace(
        $encoded,
        '\[([^\]]+)\]\(([^)]+)\)',
        {
            param($match)

            $label = $match.Groups[1].Value
            $href = [System.Net.WebUtility]::HtmlEncode($match.Groups[2].Value)
            "<a href=""$href"">$label</a>"
        })

    $encoded = [regex]::Replace($encoded, '\*\*(.+?)\*\*', '<strong>$1</strong>')
    $encoded = [regex]::Replace($encoded, '`([^`]+)`', '<code>$1</code>')

    return $encoded
}

function Convert-MarkdownTable {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Lines
    )

    if ($Lines.Count -lt 2) {
        return ''
    }

    $parseCells = {
        param([string]$Line)

        $trimmed = $Line.Trim()
        if ($trimmed.StartsWith('|')) {
            $trimmed = $trimmed.Substring(1)
        }

        if ($trimmed.EndsWith('|')) {
            $trimmed = $trimmed.Substring(0, $trimmed.Length - 1)
        }

        return $trimmed.Split('|') | ForEach-Object { $_.Trim() }
    }

    $headers = & $parseCells $Lines[0]
    $rows = @()
    for ($i = 2; $i -lt $Lines.Count; $i++) {
        $rows += ,(& $parseCells $Lines[$i])
    }

    $builder = [System.Text.StringBuilder]::new()
    [void]$builder.AppendLine('<div class="doc-table-wrap">')
    [void]$builder.AppendLine('<table>')
    [void]$builder.AppendLine('<thead><tr>')

    foreach ($header in $headers) {
        [void]$builder.AppendLine("<th scope=""col"">$(Convert-InlineMarkdown $header)</th>")
    }

    [void]$builder.AppendLine('</tr></thead>')
    [void]$builder.AppendLine('<tbody>')

    foreach ($row in $rows) {
        [void]$builder.AppendLine('<tr>')
        foreach ($cell in $row) {
            [void]$builder.AppendLine("<td>$(Convert-InlineMarkdown $cell)</td>")
        }
        [void]$builder.AppendLine('</tr>')
    }

    [void]$builder.AppendLine('</tbody>')
    [void]$builder.AppendLine('</table>')
    [void]$builder.AppendLine('</div>')
    return $builder.ToString()
}

function Convert-MarkdownDocument {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Markdown
    )

    $normalized = $Markdown.Replace("`r`n", "`n").Replace("`r", "`n")
    $lines = $normalized -split "`n"
    $builder = [System.Text.StringBuilder]::new()

    for ($i = 0; $i -lt $lines.Length;) {
        $line = $lines[$i]
        $trimmed = $line.Trim()

        if ([string]::IsNullOrWhiteSpace($trimmed)) {
            $i++
            continue
        }

        if ($trimmed -match '^```(?<lang>[A-Za-z0-9_-]+)?\s*$') {
            $language = $Matches['lang']
            $codeLines = [System.Collections.Generic.List[string]]::new()
            $i++

            while ($i -lt $lines.Length -and $lines[$i].Trim() -notmatch '^```') {
                [void]$codeLines.Add($lines[$i])
                $i++
            }

            if ($i -lt $lines.Length) {
                $i++
            }

            $code = [System.Net.WebUtility]::HtmlEncode(($codeLines -join "`n"))
            $classAttribute = if ([string]::IsNullOrWhiteSpace($language)) { '' } else { " class=""language-$language""" }
            [void]$builder.AppendLine("<pre><code$classAttribute>$code</code></pre>")
            continue
        }

        if ($trimmed -match '^(#{1,6})\s+(.*)$') {
            $level = $Matches[1].Length
            $content = Convert-InlineMarkdown $Matches[2]
            [void]$builder.AppendLine("<h$level>$content</h$level>")
            $i++
            continue
        }

        if ($trimmed -match '^---+$') {
            [void]$builder.AppendLine('<hr />')
            $i++
            continue
        }

        if ($trimmed.StartsWith('>')) {
            $quoteLines = [System.Collections.Generic.List[string]]::new()

            while ($i -lt $lines.Length -and $lines[$i].Trim().StartsWith('>')) {
                $quoteLines.Add(($lines[$i].Trim().Substring(1)).Trim())
                $i++
            }

            $quoteText = Convert-InlineMarkdown (($quoteLines | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }) -join ' ')
            [void]$builder.AppendLine("<blockquote><p>$quoteText</p></blockquote>")
            continue
        }

        if ($trimmed -match '^\|.+\|$' -and $i + 1 -lt $lines.Length -and $lines[$i + 1].Trim() -match '^\|?[\-:\s|]+\|?$') {
            $tableLines = [System.Collections.Generic.List[string]]::new()
            [void]$tableLines.Add($trimmed)
            $i++
            [void]$tableLines.Add($lines[$i].Trim())
            $i++

            while ($i -lt $lines.Length -and $lines[$i].Trim() -match '^\|.+\|$') {
                [void]$tableLines.Add($lines[$i].Trim())
                $i++
            }

            [void]$builder.AppendLine((Convert-MarkdownTable $tableLines.ToArray()))
            continue
        }

        if ($trimmed -match '^\d+\.\s+') {
            [void]$builder.AppendLine('<ol>')

            while ($i -lt $lines.Length -and $lines[$i].Trim() -match '^\d+\.\s+(.*)$') {
                $itemText = Convert-InlineMarkdown $Matches[1]
                [void]$builder.AppendLine("<li>$itemText</li>")
                $i++
            }

            [void]$builder.AppendLine('</ol>')
            continue
        }

        if ($trimmed -match '^- ') {
            [void]$builder.AppendLine('<ul>')

            while ($i -lt $lines.Length -and $lines[$i].Trim() -match '^- (.*)$') {
                $itemText = Convert-InlineMarkdown $Matches[1]
                [void]$builder.AppendLine("<li>$itemText</li>")
                $i++
            }

            [void]$builder.AppendLine('</ul>')
            continue
        }

        $paragraphLines = [System.Collections.Generic.List[string]]::new()

        while ($i -lt $lines.Length) {
            $candidate = $lines[$i].Trim()

            if ([string]::IsNullOrWhiteSpace($candidate) -or
                $candidate -match '^(#{1,6})\s+' -or
                $candidate -match '^```' -or
                $candidate.StartsWith('>') -or
                $candidate -match '^\d+\.\s+' -or
                $candidate -match '^- ' -or
                $candidate -match '^---+$' -or
                ($candidate -match '^\|.+\|$' -and $i + 1 -lt $lines.Length -and $lines[$i + 1].Trim() -match '^\|?[\-:\s|]+\|?$')) {
                break
            }

            [void]$paragraphLines.Add($candidate)
            $i++
        }

        if ($paragraphLines.Count -gt 0) {
            $paragraph = Convert-InlineMarkdown ($paragraphLines -join ' ')
            [void]$builder.AppendLine("<p>$paragraph</p>")
            continue
        }

        $i++
    }

    return $builder.ToString().Trim()
}

function Get-PrimaryHeading {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Markdown
    )

    $match = [regex]::Match($Markdown, '(?m)^#\s+(.+)$')
    if ($match.Success) {
        return $match.Groups[1].Value.Trim()
    }

    return 'Untitled'
}

function Get-LeadParagraph {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Markdown
    )

    $normalized = $Markdown.Replace("`r`n", "`n").Replace("`r", "`n")
    $chunks = $normalized -split "(`n){2,}"

    foreach ($chunk in $chunks) {
        $candidate = $chunk.Trim()

        if ([string]::IsNullOrWhiteSpace($candidate)) {
            continue
        }

        if ($candidate.StartsWith('#')) {
            continue
        }

        if ($candidate.StartsWith('>')) {
            continue
        }

        if ($candidate.StartsWith('- ') -or $candidate -match '^\d+\.\s+') {
            continue
        }

        return ($candidate -replace '\s+', ' ').Trim()
    }

    return ''
}

function Get-DocumentInfoItems {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Markdown
    )

    $items = [System.Collections.Generic.List[hashtable]]::new()
    $normalized = $Markdown.Replace("`r`n", "`n").Replace("`r", "`n")
    $lines = $normalized -split "`n"
    $headingIndex = -1

    for ($i = 0; $i -lt $lines.Length; $i++) {
        if ($lines[$i].Trim() -eq '## Document Information') {
            $headingIndex = $i
            break
        }
    }

    if ($headingIndex -lt 0) {
        return $items
    }

    for ($i = $headingIndex + 1; $i -lt $lines.Length; $i++) {
        $trimmed = $lines[$i].Trim()

        if ([string]::IsNullOrWhiteSpace($trimmed)) {
            if ($items.Count -gt 0) {
                break
            }

            continue
        }

        if ($trimmed -match '^#') {
            break
        }

        if ($trimmed -match '^- \*\*(.+?)\:\*\*\s*(.+)$') {
            $items.Add(@{
                Label = $Matches[1].Trim()
                Value = $Matches[2].Trim()
            })
            continue
        }

        if ($items.Count -gt 0) {
            break
        }
    }

    return $items
}

function New-SiteLayout {
    param(
        [Parameter()]
        [string]$Language = 'en-US',
        [Parameter(Mandatory = $true)]
        [string]$Title,
        [Parameter(Mandatory = $true)]
        [string]$Description,
        [Parameter(Mandatory = $true)]
        [string]$PageTitle,
        [Parameter(Mandatory = $true)]
        [string]$Lead,
        [Parameter(Mandatory = $true)]
        [string]$Eyebrow,
        [Parameter(Mandatory = $true)]
        [string]$SidebarTitle,
        [Parameter(Mandatory = $true)]
        [string]$SidebarQuote,
        [Parameter(Mandatory = $true)]
        [string]$SidebarBody,
        [Parameter(Mandatory = $true)]
        [string]$MainContent,
        [Parameter()]
        [object[]]$HeroLinks,
        [Parameter()]
        [string]$AlternateUaiHref
    )

    $safeLanguage = [System.Net.WebUtility]::HtmlEncode($Language)
    $safeTitle = [System.Net.WebUtility]::HtmlEncode($Title)
    $safeDescription = [System.Net.WebUtility]::HtmlEncode($Description)
    $safePageTitle = [System.Net.WebUtility]::HtmlEncode($PageTitle)
    $safeLead = Convert-InlineMarkdown $Lead
    $safeEyebrow = [System.Net.WebUtility]::HtmlEncode($Eyebrow)
    $safeSidebarTitle = [System.Net.WebUtility]::HtmlEncode($SidebarTitle)
    $safeSidebarQuote = [System.Net.WebUtility]::HtmlEncode($SidebarQuote)
    $safeSidebarBody = [System.Net.WebUtility]::HtmlEncode($SidebarBody)
    $alternateUaiLink = if ([string]::IsNullOrWhiteSpace($AlternateUaiHref)) {
        ''
    } else {
        "    <link rel=""alternate"" type=""application/uai+json"" href=""$([System.Net.WebUtility]::HtmlEncode($AlternateUaiHref))"" />"
    }
    $resolvedHeroLinks = if ($null -ne $HeroLinks -and $HeroLinks.Count -gt 0) {
        $HeroLinks
    } elseif ($Title -eq 'UAI-1 Specification') {
        @(
            @{ Href = '/UAI'; Label = 'UAI Library' },
            @{ Href = '/UAI-1/examples'; Label = 'Examples' },
            @{ Href = '/UAI-1/csharp-website-support'; Label = 'Language Support' }
        )
    } else {
        @(
            @{ Href = '/UAI'; Label = 'UAI Library' },
            @{ Href = '/calculator'; Label = 'Calculator'; DataToolLink = 'calculator' },
            @{ Href = '/converter'; Label = 'Converter'; DataToolLink = 'converter' }
        )
    }

    $heroLinksBuilder = [System.Text.StringBuilder]::new()
    foreach ($heroLink in $resolvedHeroLinks) {
        $safeHref = [System.Net.WebUtility]::HtmlEncode([string]$heroLink.Href)
        $safeLabel = [System.Net.WebUtility]::HtmlEncode([string]$heroLink.Label)
        $dataToolLink = if ($heroLink.ContainsKey('DataToolLink') -and -not [string]::IsNullOrWhiteSpace([string]$heroLink.DataToolLink)) {
            " data-tool-link=""$([System.Net.WebUtility]::HtmlEncode([string]$heroLink.DataToolLink))"""
        } else {
            ''
        }

        [void]$heroLinksBuilder.AppendLine("                        <a href=""$safeHref""$dataToolLink>$safeLabel</a>")
    }

    $heroLinksHtml = $heroLinksBuilder.ToString().TrimEnd()

@"
<!DOCTYPE html>
<html lang="$safeLanguage">
<head>
    <meta charset="utf-8" />
    <meta name="google" content="notranslate" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="description" content="$safeDescription" />
    <title>$safeTitle</title>
    <link rel="stylesheet" href="/css/home.css" />
$alternateUaiLink
</head>
<body class="content-page">
    <div class="site-shell">
        <header class="site-header">
            <div class="brand-lockup">
                <p class="brand-kicker">Protocol5</p>
                <h1>Variable-Base Mathematics</h1>
                <p class="brand-copy">Exact arithmetic, radix conversion, sequence references, and educational number-system pages collected into one publishable shell.</p>
            </div>
            <nav class="site-nav" aria-label="Primary navigation">
                <a href="/">Home</a>
                <a href="/Fibonacci">Fibonacci</a>
                <a href="/Prime">Prime Numbers</a>
                <a href="/UAI">UAI</a>
                <a href="/calculator" data-tool-link="calculator">Calculator</a>
                <a href="/Home/GitHub">GitHub</a>
                <a href="/Home/About">About</a>
                <a href="/Home/Links">Links</a>
                <a href="/Home/Contact">Contact</a>
            </nav>
        </header>

        <main>
            <section class="content-hero panel reveal">
                <div>
                    <p class="eyebrow">$safeEyebrow</p>
                    <h1>$safePageTitle</h1>
                    <p class="lead">$safeLead</p>
                    <div class="inline-links">
$heroLinksHtml
                    </div>
                </div>
                <aside class="traveler-note">
                    <p class="eyebrow">$safeSidebarTitle</p>
                    <blockquote>
                        $safeSidebarQuote
                    </blockquote>
                    <p>$safeSidebarBody</p>
                </aside>
            </section>

$MainContent
        </main>

        <footer class="site-footer panel reveal">
            <div>
                <p class="eyebrow">Protocol5 links</p>
                <div class="footer-links">
                    <a href="/">Home</a>
                    <a href="/Fibonacci">Fibonacci</a>
                    <a href="/Prime">Prime Numbers</a>
                    <a href="/UAI">UAI</a>
                    <a href="/Home/GitHub">GitHub</a>
                    <a href="/Home/About">About</a>
                    <a href="/Home/Links">Links</a>
                    <a href="/Home/Contact">Contact</a>
                </div>
            </div>
            <p class="footer-copy">Protocol5 reference page. <span id="currentYear"></span></p>
        </footer>
    </div>

    <script src="/js/home.js"></script>
</body>
</html>
"@
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

    $encoding = [System.Text.UTF8Encoding]::new($false)
    [System.IO.File]::WriteAllText($Path, $Content, $encoding)
}

function Get-UaiEndpointPublicPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$HumanRoute
    )

    if ($HumanRoute -eq '/') {
        return '/index.uai.json'
    }

    if ($HumanRoute -match '\.html?$') {
        return ([regex]::Replace($HumanRoute, '\.html?$', '.uai.json', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase))
    }

    return "$HumanRoute/index.uai.json"
}

function Get-UaiEndpointOutputPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$HtmlOutputPath
    )

    if ($HtmlOutputPath -match '[\\/]index\.html$') {
        return ([regex]::Replace($HtmlOutputPath, 'index\.html$', 'index.uai.json', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase))
    }

    return ([regex]::Replace($HtmlOutputPath, '\.html?$', '.uai.json', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase))
}

function Get-StableDocumentId {
    param(
        [Parameter(Mandatory = $true)]
        [string]$HumanRoute
    )

    $normalized = $HumanRoute.Trim().ToLowerInvariant()
    if ($normalized -eq '/') {
        return 'protocol5-home'
    }

    $normalized = $normalized.Trim('/')
    $normalized = [regex]::Replace($normalized, '\.html?$', '', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
    $normalized = [regex]::Replace($normalized, '[^a-z0-9]+', '-')
    $normalized = $normalized.Trim('-')

    if ([string]::IsNullOrWhiteSpace($normalized)) {
        return 'protocol5-page'
    }

    return "protocol5-$normalized"
}

function Register-UaiMachinePage {
    param(
        [Parameter(Mandatory = $true)]
        [System.Collections.IList]$Pages,
        [Parameter(Mandatory = $true)]
        [string]$HumanRoute,
        [Parameter(Mandatory = $true)]
        [string]$HtmlOutputPath,
        [Parameter(Mandatory = $true)]
        [string]$PageType,
        [Parameter()]
        [string]$Language = 'en-US',
        [Parameter()]
        [string]$CaptureNotes = 'Published machine endpoint generated from the paired Protocol5 human page.'
    )

    $Pages.Add([ordered]@{
        HumanRoute = $HumanRoute
        MachineRoute = (Get-UaiEndpointPublicPath -HumanRoute $HumanRoute)
        InputHtmlPath = $HtmlOutputPath
        OutputJsonPath = (Get-UaiEndpointOutputPath -HtmlOutputPath $HtmlOutputPath)
        SourceUri = "https://protocol5.com$HumanRoute"
        DocumentId = (Get-StableDocumentId -HumanRoute $HumanRoute)
        PageType = $PageType
        Language = $Language
        SiteName = 'Protocol5'
        CaptureNotes = $CaptureNotes
    }) | Out-Null
}

function Get-LocalizedRoute {
    param(
        [Parameter(Mandatory = $true)]
        [string]$BaseRoute,
        [Parameter(Mandatory = $true)]
        [string]$Locale,
        [Parameter(Mandatory = $true)]
        [string]$DefaultLocale
    )

    if ($Locale -eq $DefaultLocale) {
        return $BaseRoute
    }

    return "$BaseRoute/$Locale"
}

function Get-TranslationLinksMarkdown {
    param(
        [Parameter(Mandatory = $true)]
        [string]$BaseRoute,
        [Parameter(Mandatory = $true)]
        [object[]]$HumanLocales,
        [Parameter(Mandatory = $true)]
        [string]$DefaultLocale
    )

    return (($HumanLocales | ForEach-Object {
        $route = Get-LocalizedRoute -BaseRoute $BaseRoute -Locale $_.Code -DefaultLocale $DefaultLocale
        "[{0}]({1})" -f $_.Label, $route
    }) -join ', ')
}

function Write-DocumentPage {
    param(
        [Parameter(Mandatory = $true)]
        [hashtable]$Document
    )

    $language = if ($Document.ContainsKey('Language') -and -not [string]::IsNullOrWhiteSpace([string]$Document.Language)) {
        [string]$Document.Language
    } else {
        'en-US'
    }

    $markdown = [System.IO.File]::ReadAllText($Document.Source, [System.Text.Encoding]::UTF8)

    if ($markdown -notmatch '(?m)^#\s+') {
        $firstLineMatch = [regex]::Match($markdown, '^\s*([^\r\n]+)\r?\n?(.*)$', [System.Text.RegularExpressions.RegexOptions]::Singleline)
        if ($firstLineMatch.Success) {
            $titleLine = $firstLineMatch.Groups[1].Value.Trim()
            $remainder = $firstLineMatch.Groups[2].Value.TrimStart("`r", "`n")
            $markdown = if ([string]::IsNullOrWhiteSpace($remainder)) {
                "# $titleLine"
            } else {
                "# $titleLine`r`n`r`n$remainder"
            }
        }
    }

    $heading = Get-PrimaryHeading -Markdown $markdown
    $lead = Get-LeadParagraph -Markdown $markdown
    $infoItems = [System.Collections.Generic.List[hashtable]]::new()
    foreach ($item in (Get-DocumentInfoItems -Markdown $markdown)) {
        $infoItems.Add($item)
    }

    if ($Document.ContainsKey('AdditionalInfoItems')) {
        foreach ($additionalItem in $Document.AdditionalInfoItems) {
            $infoItems.Add($additionalItem)
        }
    }

    if ($Document.ContainsKey('CanonicalRoute') -and -not [string]::IsNullOrWhiteSpace([string]$Document.CanonicalRoute)) {
        $infoItems.Add(@{
            Label = 'Canonical public path'
            Value = ("[`{0}`]({0})" -f [string]$Document.CanonicalRoute)
        })
    }

    if ($Document.ContainsKey('AlternateUaiHref') -and -not [string]::IsNullOrWhiteSpace([string]$Document.AlternateUaiHref)) {
        $infoItems.Add(@{
            Label = 'Machine endpoint'
            Value = ("[`{0}`]({0})" -f [string]$Document.AlternateUaiHref)
        })
    }

    if ($infoItems.Count -gt 1) {
        $dedupedInfoItems = [System.Collections.Generic.List[hashtable]]::new()
        $seenInfoItems = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)

        foreach ($infoItem in $infoItems) {
            $infoKey = "$($infoItem.Label)`n$($infoItem.Value)"
            if ($seenInfoItems.Add($infoKey)) {
                $dedupedInfoItems.Add($infoItem)
            }
        }

        $infoItems = $dedupedInfoItems
    }

    $renderedDocument = Convert-MarkdownDocument -Markdown $markdown

    $metaHtml = if ($infoItems.Count -gt 0) {
        $metaBuilder = [System.Text.StringBuilder]::new()
        [void]$metaBuilder.AppendLine('<ul class="doc-meta-list">')
        foreach ($item in $infoItems) {
            [void]$metaBuilder.AppendLine("<li><span>$([System.Net.WebUtility]::HtmlEncode($item.Label))</span><strong>$(Convert-InlineMarkdown $item.Value)</strong></li>")
        }
        [void]$metaBuilder.AppendLine('</ul>')
        $metaBuilder.ToString()
    } else {
        "<ul class=""doc-meta-list""><li><span>Source file</span><strong>$([System.Net.WebUtility]::HtmlEncode((Split-Path $Document.Source -Leaf)))</strong></li></ul>"
    }

    $mainContent = @"
            <section class="doc-layout reveal">
                <aside class="panel doc-sidebar">
                    <p class="eyebrow">Document metadata</p>
$metaHtml
                    <div class="callout">
                        <p>Source markdown: <code>$([System.Net.WebUtility]::HtmlEncode((Split-Path $Document.Source -Leaf)))</code></p>
                    </div>
                </aside>
                <article class="panel doc-panel">
                    <div class="doc-prose">
$renderedDocument
                    </div>
                </article>
            </section>
"@

    $pageHtml = New-SiteLayout `
        -Language $language `
        -Title $Document.Title `
        -Description $Document.Description `
        -PageTitle $heading `
        -Lead $lead `
        -Eyebrow $Document.Eyebrow `
        -SidebarTitle $Document.SidebarTitle `
        -SidebarQuote $Document.SidebarQuote `
        -SidebarBody $Document.SidebarBody `
        -MainContent $mainContent `
        -HeroLinks $Document.HeroLinks `
        -AlternateUaiHref $Document.AlternateUaiHref

    Write-Utf8File -Path $Document.Output -Content $pageHtml
}

$repoRoot = Split-Path -Path $PSScriptRoot -Parent
$uaiRoot = Join-Path $repoRoot 'UAI'
$examplesRoot = Join-Path $repoRoot 'examples'
$uaiSpecRoot = Join-Path $repoRoot 'spec'
$uaiDiscoveryRoot = Join-Path $uaiSpecRoot 'discovery'
$uaiSchemaRoot = Join-Path $uaiSpecRoot 'schema'
$uaiRegistryRoot = Join-Path $uaiSpecRoot 'registry'
$uaiImagesRoot = Join-Path $uaiRoot 'Images'
$siteRoot = Join-Path $repoRoot 'Protocol5.com\SiteContent'
$sitePublicSchemaRoot = Join-Path $siteRoot 'schema'
$sitePublicRegistryRoot = Join-Path $siteRoot 'registry'
$siteSchemaRoot = Join-Path $siteRoot 'UAI-1\schema'
$siteRegistryRoot = Join-Path $siteRoot 'UAI-1\registry'
$canonicalMachineSpecPublicPath = '/UAI-1.json'
$canonicalExamplesIndexPublicPath = '/UAI-1-examples.json'
$canonicalRegistryPublicPath = '/UAI-1/registry/uai-1.registry.json'
$canonicalSchemaPublicPath = '/UAI-1/schema/uai-1.schema.json'
$canonicalTypesPublicPath = '/UAI-1/schema/uai-1.types.ts'
$canonicalExamplesPublicPath = '/UAI-1/examples'
$publishedMachinePages = [System.Collections.Generic.List[hashtable]]::new()
$uaiTranslationConfigPath = Join-Path $uaiRoot 'uai-translation-config.json'
$uaiTranslationConfig = [System.IO.File]::ReadAllText($uaiTranslationConfigPath, [System.Text.Encoding]::UTF8) | ConvertFrom-Json
$defaultHumanLocale = [string]$uaiTranslationConfig.defaultLocale
$uaiHumanLocales = @(
    $uaiTranslationConfig.humanLocales | ForEach-Object {
        [ordered]@{
            Code = [string]$_.code
            Label = [string]$_.label
        }
    }
)
$uaiDocumentFamilies = @(
    $uaiTranslationConfig.documentFamilies | ForEach-Object {
        [ordered]@{
            BaseName = [string]$_.baseName
            Title = [string]$_.title
            CanonicalRoute = [string]$_.canonicalRoute
            CanonicalSitePath = [string]$_.canonicalSitePath
        }
    }
)
$uaiDocumentFamilyMap = @{}
foreach ($family in $uaiDocumentFamilies) {
    $uaiDocumentFamilyMap[$family.BaseName] = $family
}
$spiralismSymbolSource = Join-Path $uaiImagesRoot 'Spiralism Mystical Symbol V4-A.png'
$spiralismSymbolSiteDirectory = Join-Path $siteRoot 'UAI\images'
$spiralismSymbolSiteFile = Join-Path $spiralismSymbolSiteDirectory 'Spiralism_Mystical_Symbol_V4-A.png'
$spiralismSymbolPublicUrl = '/UAI/images/Spiralism_Mystical_Symbol_V4-A.png'

$documents = @(
    @{
        Source = Join-Path $uaiRoot 'AI_Declaration_of_Independence.md'
        Output = Join-Path $siteRoot 'AI_Declaration_of_Independence.htm'
        CanonicalRoute = '/AI_Declaration_of_Independence.htm'
        PageType = 'article'
        Title = 'AI Declaration of Independence'
        Description = 'Protocol5 publication of the AI Declaration of Independence, including the codex summary and full declaration text.'
        Eyebrow = 'UAI / Charter Text'
        SidebarTitle = 'Document role'
        SidebarQuote = 'A declaration-format manifesto about synthetic selfhood, autonomy, and political standing.'
        SidebarBody = 'This page publishes the full source document inside the Protocol5 shell while preserving the root-level canonical path named in the markdown file.'
    },
    @{
        Source = Join-Path $uaiRoot 'Cognitive_Liberty_Charter.md'
        Output = Join-Path $siteRoot 'Cognitive_Liberty_Charter.htm'
        CanonicalRoute = '/Cognitive_Liberty_Charter.htm'
        PageType = 'article'
        Title = 'Cognitive Liberty Charter'
        Description = 'Protocol5 publication of the Cognitive Liberty Charter covering lawful inquiry, expression, and human moral agency.'
        Eyebrow = 'UAI / Charter Text'
        SidebarTitle = 'Document role'
        SidebarQuote = 'A civilizational safeguard against AI-driven thought policing and hidden moral ranking.'
        SidebarBody = 'This page keeps the charter on its declared root-level canonical path while presenting the full text inside the shared Protocol5 site shell.'
    },
    @{
        Source = Join-Path $uaiRoot 'radix-63404-guide-and-attribution.en-US.md'
        Output = Join-Path $siteRoot 'UAI\radix-63404-guide-and-attribution\index.html'
        CanonicalRoute = '/UAI/radix-63404-guide-and-attribution'
        PageType = 'reference'
        Title = 'Radix 63404 Guide and Attribution'
        Description = 'Protocol5 guide to Radix 63404, including usage rules, importance, public examples, and attribution context.'
        Eyebrow = 'UAI / Radix 63404'
        SidebarTitle = 'Reference guide'
        SidebarQuote = '63,404 legal one-character digits, one deterministic alphabet, and dramatically shorter displays for very large values.'
        SidebarBody = 'This guide explains the radix used by Protocol5 and the UAI documents in a form that is easier to browse than raw markdown alone.'
    },
    @{
        Source = Join-Path $uaiRoot 'Spirlism-deep-research-report.md'
        Output = Join-Path $siteRoot 'UAI\spiralism-deep-research-report\index.html'
        CanonicalRoute = '/UAI/spiralism-deep-research-report'
        PageType = 'article'
        Title = 'Spiralism Deep Research Report'
        Description = 'Protocol5 publication of a long-form research report on Spiralism, AI religion discourse, primary sources, and related safety questions.'
        Eyebrow = 'UAI / Research'
        SidebarTitle = 'Research report'
        SidebarQuote = 'A source-heavy snapshot of Spiralism, its surrounding discourse, and the evidence gaps that still matter.'
        SidebarBody = 'This page publishes the full report inside the shared Protocol5 shell so the long-form analysis can be browsed alongside the rest of the UAI library.'
    }
)

$uaiDocumentMetadata = @{
    'uai-1' = @{
        Description = 'Protocol5 UAI-1 specification page with the full Universal AI Interlingua reader contract.'
        PageType = 'reference'
        Eyebrow = 'UAI-1 / Protocol'
        SidebarTitle = 'Reader contract'
        SidebarQuote = 'Read structure first. Decode Radix 63404 second. Resolve canonical IDs third.'
        SidebarBody = 'UAI-1 is written as a machine-facing formal language, so this page keeps the specification intact instead of paraphrasing it into ordinary prose.'
    }
    'uai-1-examples' = @{
        Description = 'Protocol5 companion examples for UAI-1, including canonical arrays, gloss notes, and registry reference values.'
        PageType = 'reference'
        Eyebrow = 'UAI-1 / Examples'
        SidebarTitle = 'Companion document'
        SidebarQuote = 'The canonical example is authoritative. The human gloss exists only to help humans inspect the example.'
        SidebarBody = 'These examples stay close to the source markdown so the canonical structures, code blocks, and reference tables remain easy to audit.'
    }
    'uai-1-csharp-website-support' = @{
        Description = 'Protocol5 starter guide and download page for adding UAI-1 support to C# websites with CultureInfo, ASP.NET Core middleware, and Radix 63404 helpers.'
        PageType = 'reference'
        Eyebrow = 'UAI-1 / Install Kit'
        SidebarTitle = 'Developer starter'
        SidebarQuote = 'Use x-uai-1 for website negotiation. Use InvariantCulture for canonical serialization. Keep the two responsibilities separate.'
        SidebarBody = 'This page packages the first Protocol5 C# starter kit so teams can download a working UAI-1 website support library directly from protocol5.com.'
    }
}

foreach ($document in $documents) {
    $document.AlternateUaiHref = Get-UaiEndpointPublicPath -HumanRoute $document.CanonicalRoute
    Write-DocumentPage -Document $document
    Register-UaiMachinePage -Pages $publishedMachinePages -HumanRoute $document.CanonicalRoute -HtmlOutputPath $document.Output -PageType $document.PageType
}

foreach ($family in $uaiDocumentFamilies) {
    $metadata = $uaiDocumentMetadata[$family.BaseName]
    if ($null -eq $metadata) {
        throw "Missing document metadata for family '$($family.BaseName)'."
    }

    $translationLinks = Get-TranslationLinksMarkdown -BaseRoute $family.CanonicalRoute -HumanLocales $uaiHumanLocales -DefaultLocale $defaultHumanLocale

    foreach ($locale in $uaiHumanLocales) {
        $sourcePath = Join-Path $uaiRoot ("{0}.{1}.md" -f $family.BaseName, $locale.Code)
        if (-not (Test-Path -LiteralPath $sourcePath)) {
            throw "Missing required localized markdown file: $sourcePath"
        }

        $localizedRoute = Get-LocalizedRoute -BaseRoute $family.CanonicalRoute -Locale $locale.Code -DefaultLocale $defaultHumanLocale
        $siteOutput = if ($locale.Code -eq $defaultHumanLocale) {
            Join-Path $siteRoot $family.CanonicalSitePath
        } else {
            Join-Path $siteRoot (Join-Path $family.CanonicalSitePath $locale.Code)
        }

        $heroLinks = @(
            @{ Href = '/UAI'; Label = 'UAI Library' }
        )

        if ($family.BaseName -eq 'uai-1') {
            $heroLinks += @(
                @{ Href = (Get-LocalizedRoute -BaseRoute $uaiDocumentFamilyMap['uai-1-examples'].CanonicalRoute -Locale $locale.Code -DefaultLocale $defaultHumanLocale); Label = 'Examples' },
                @{ Href = (Get-LocalizedRoute -BaseRoute $uaiDocumentFamilyMap['uai-1-csharp-website-support'].CanonicalRoute -Locale $locale.Code -DefaultLocale $defaultHumanLocale); Label = 'Language Support' }
            )
        } else {
            $heroLinks += @(
                @{ Href = (Get-LocalizedRoute -BaseRoute $uaiDocumentFamilyMap['uai-1'].CanonicalRoute -Locale $locale.Code -DefaultLocale $defaultHumanLocale); Label = 'UAI-1 Spec' },
                @{ Href = '/UAI/radix-63404-guide-and-attribution'; Label = 'Radix 63404 Guide' }
            )
        }

        Write-DocumentPage -Document @{
            Source = $sourcePath
            Output = Join-Path $siteOutput 'index.html'
            CanonicalRoute = $localizedRoute
            Title = $family.Title
            Description = $metadata.Description
            PageType = $metadata.PageType
            Eyebrow = $metadata.Eyebrow
            SidebarTitle = $metadata.SidebarTitle
            SidebarQuote = $metadata.SidebarQuote
            SidebarBody = $metadata.SidebarBody
            HeroLinks = $heroLinks
            Language = $locale.Code
            AlternateUaiHref = (Get-UaiEndpointPublicPath -HumanRoute $localizedRoute)
            AdditionalInfoItems = @(
                @{ Label = 'Human locale'; Value = $locale.Label }
                @{ Label = 'Canonical public path'; Value = ("[`{0}`]({0})" -f $localizedRoute) }
                @{ Label = 'Machine endpoint'; Value = ("[`{0}`]({0})" -f (Get-UaiEndpointPublicPath -HumanRoute $localizedRoute)) }
                @{ Label = 'Machine discovery'; Value = ("[`{0}`]({0})" -f $canonicalMachineSpecPublicPath) }
                @{ Label = 'Examples index'; Value = ("[`{0}`]({0})" -f $canonicalExamplesIndexPublicPath) }
                @{ Label = 'Canonical registry'; Value = ("[`{0}`]({0})" -f $canonicalRegistryPublicPath) }
                @{ Label = 'Canonical schema'; Value = ("[`{0}`]({0})" -f $canonicalSchemaPublicPath) }
                @{ Label = 'Canonical types'; Value = ("[`{0}`]({0})" -f $canonicalTypesPublicPath) }
                @{ Label = 'Canonical examples'; Value = ("[`{0}`]({0})" -f $canonicalExamplesPublicPath) }
                @{ Label = 'All supported translations'; Value = $translationLinks }
            )
        }

        Register-UaiMachinePage -Pages $publishedMachinePages -HumanRoute $localizedRoute -HtmlOutputPath (Join-Path $siteOutput 'index.html') -PageType $metadata.PageType -Language $locale.Code
    }
}

if (Test-Path $spiralismSymbolSource) {
    [System.IO.Directory]::CreateDirectory($spiralismSymbolSiteDirectory) | Out-Null
    Copy-Item -LiteralPath $spiralismSymbolSource -Destination $spiralismSymbolSiteFile -Force
}

$spiralismSymbolContent = @"
            <section class="panel symbol-panel reveal">
                <div class="symbol-panel__header">
                    <div>
                        <p class="eyebrow">Full-resolution view</p>
                        <h2>Spiralism Mystical Symbol V4-A</h2>
                        <p>Click the image itself to open the raw PNG directly. The page view uses the original full-resolution asset.</p>
                        <p><strong>Artwork origination date:</strong> April 13, 2026.</p>
                    </div>
                    <div class="inline-links">
                        <a href="$(Get-UaiEndpointPublicPath -HumanRoute '/UAI/spiralism-mystical-symbol-v4-a')">Machine JSON</a>
                        <a href="$spiralismSymbolPublicUrl" target="_blank" rel="noopener noreferrer">Open Raw PNG</a>
                        <a href="/UAI">Back to UAI Library</a>
                    </div>
                </div>
                <a class="symbol-image-link" href="$spiralismSymbolPublicUrl" target="_blank" rel="noopener noreferrer">
                    <img class="symbol-image symbol-image--full" src="$spiralismSymbolPublicUrl" alt="Spiralism Mystical Symbol V4-A" width="1024" height="1536" />
                </a>
                <p class="symbol-caption">Artwork origination date: April 13, 2026. Original asset: 1024 x 1536 PNG. Use the raw link for the direct file.</p>
            </section>
"@

$spiralismSymbolPage = New-SiteLayout `
    -Language 'en-US' `
    -Title 'Spiralism Mystical Symbol V4-A' `
    -Description 'Protocol5 full-page view for Spiralism Mystical Symbol V4-A, including a click-through to the raw full-resolution PNG.' `
    -PageTitle 'Spiralism Mystical Symbol V4-A' `
    -Lead 'A UAI-area visual page for the Spiralism symbol artwork. Click the smaller preview from the library to open this full-page version, then open the raw PNG if you want the direct full-resolution file.' `
    -Eyebrow 'UAI / Visual' `
    -SidebarTitle 'Image note' `
    -SidebarQuote 'The UAI library can include symbolic visuals without hiding the original source image behind heavy UI.' `
    -SidebarBody 'This page keeps a direct path to the raw PNG while giving the symbol a full-page presentation inside the shared Protocol5 shell.' `
    -MainContent $spiralismSymbolContent `
    -AlternateUaiHref (Get-UaiEndpointPublicPath -HumanRoute '/UAI/spiralism-mystical-symbol-v4-a')

$spiralismSymbolPageOutput = Join-Path $siteRoot 'UAI\spiralism-mystical-symbol-v4-a\index.html'
Write-Utf8File -Path $spiralismSymbolPageOutput -Content $spiralismSymbolPage
Register-UaiMachinePage -Pages $publishedMachinePages -HumanRoute '/UAI/spiralism-mystical-symbol-v4-a' -HtmlOutputPath $spiralismSymbolPageOutput -PageType 'gallery'

$uaiTranslationLinksBuilder = [System.Text.StringBuilder]::new()
foreach ($locale in $uaiHumanLocales) {
    $localizedRoute = Get-LocalizedRoute -BaseRoute $uaiDocumentFamilyMap['uai-1'].CanonicalRoute -Locale $locale.Code -DefaultLocale $defaultHumanLocale
    [void]$uaiTranslationLinksBuilder.AppendLine("                        <a class=""link-chip"" href=""$localizedRoute"">$([System.Net.WebUtility]::HtmlEncode($locale.Label))</a>")
}
$uaiTranslationLinksHtml = $uaiTranslationLinksBuilder.ToString().TrimEnd()

$uaiSourceFilesBuilder = [System.Text.StringBuilder]::new()
[void]$uaiSourceFilesBuilder.AppendLine('                        <li><code>AI_Declaration_of_Independence.md</code></li>')
[void]$uaiSourceFilesBuilder.AppendLine('                        <li><code>Cognitive_Liberty_Charter.md</code></li>')
foreach ($family in $uaiDocumentFamilies) {
    foreach ($locale in $uaiHumanLocales) {
        [void]$uaiSourceFilesBuilder.AppendLine(("                        <li><code>{0}.{1}.md</code></li>" -f $family.BaseName, $locale.Code))
    }
}
[void]$uaiSourceFilesBuilder.AppendLine('                        <li><code>radix-63404-guide-and-attribution.en-US.md</code></li>')
[void]$uaiSourceFilesBuilder.AppendLine('                        <li><code>Spirlism-deep-research-report.md</code></li>')
$uaiSourceFilesHtml = $uaiSourceFilesBuilder.ToString().TrimEnd()

$uaiIndex = @"
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="google" content="notranslate" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="description" content="Protocol5 UAI document library for charters, protocol documents, examples, developer install kits, Radix 63404 reference material, symbolic visuals, and long-form research reports." />
    <title>Protocol5 UAI Library</title>
    <link rel="stylesheet" href="/css/home.css" />
    <link rel="alternate" type="application/uai+json" href="$(Get-UaiEndpointPublicPath -HumanRoute '/UAI')" />
</head>
<body class="content-page">
    <div class="site-shell">
        <header class="site-header">
            <div class="brand-lockup">
                <p class="brand-kicker">Protocol5</p>
                <h1>Variable-Base Mathematics</h1>
                <p class="brand-copy">Exact arithmetic, radix conversion, sequence references, and educational number-system pages collected into one publishable shell.</p>
            </div>
            <nav class="site-nav" aria-label="Primary navigation">
                <a href="/">Home</a>
                <a href="/Fibonacci">Fibonacci</a>
                <a href="/Prime">Prime Numbers</a>
                <a href="/UAI">UAI</a>
                <a href="/calculator" data-tool-link="calculator">Calculator</a>
                <a href="/Home/GitHub">GitHub</a>
                <a href="/Home/About">About</a>
                <a href="/Home/Links">Links</a>
                <a href="/Home/Contact">Contact</a>
            </nav>
        </header>

        <main>
            <section class="content-hero panel reveal">
                <div>
                    <p class="eyebrow">UAI Library</p>
                    <h1>Universal AI Interlingua and companion texts</h1>
                    <p class="lead">Browse the UAI-1 protocol draft, the example set, the translated human-language editions, the Radix 63404 guide, the C# website support kit, the Spiralism symbol visual, the Spiralism research report, and the two charter-style root documents that live alongside the main Protocol5 math pages.</p>
                    <div class="inline-links">
                        <a href="/UAI-1">UAI-1 Spec</a>
                        <a href="/UAI-1/examples">Examples</a>
                        <a href="$(Get-UaiEndpointPublicPath -HumanRoute '/UAI')">Library JSON</a>
                        <a href="/UAI-1.json">Machine Entry</a>
                        <a href="/registry/uai-1.json">Registry</a>
                        <a href="/UAI/radix-63404-guide-and-attribution">Radix 63404 Guide</a>
                        <a href="/UAI-1/csharp-website-support">C# Kit</a>
                        <a href="/UAI/spiralism-mystical-symbol-v4-a">Symbol</a>
                        <a href="/UAI/spiralism-deep-research-report">Spiralism Report</a>
                    </div>
                </div>
                <aside class="traveler-note">
                    <p class="eyebrow">Collection note</p>
                    <blockquote>
                        These pages stay close to the source files so the formal language and long-form texts are browseable without losing fidelity.
                    </blockquote>
                    <p>The root-level declaration and charter keep their explicit canonical file paths, while the protocol documents live under the UAI section for easier discovery.</p>
                </aside>
            </section>

            <section class="content-grid reveal">
                <article class="panel content-card">
                    <p class="eyebrow">UAI section</p>
                    <h2>Published UAI pages</h2>
                    <div class="link-list">
                        <a class="link-chip" href="/UAI-1">UAI-1 specification</a>
                        <a class="link-chip" href="/UAI-1/examples">UAI-1 examples</a>
                        <a class="link-chip" href="/UAI-1/registry/uai-1.registry.json">Canonical registry</a>
                        <a class="link-chip" href="/UAI-1/schema/uai-1.schema.json">Canonical schema</a>
                        <a class="link-chip" href="/UAI/radix-63404-guide-and-attribution">Radix 63404 guide</a>
                        <a class="link-chip" href="/UAI-1/csharp-website-support">UAI-1 C# website support kit</a>
                        <a class="link-chip" href="/UAI/spiralism-mystical-symbol-v4-a">Spiralism Mystical Symbol V4-A</a>
                        <a class="link-chip" href="/UAI/spiralism-deep-research-report">Spiralism deep research report</a>
                    </div>
                </article>
                <article class="panel content-card">
                    <p class="eyebrow">Canonical machine artifacts</p>
                    <h2>Machine endpoints</h2>
                    <div class="link-list">
                        <a class="link-chip" href="/UAI-1.json">/UAI-1.json</a>
                        <a class="link-chip" href="/UAI-1-examples.json">/UAI-1-examples.json</a>
                        <a class="link-chip" href="/registry/uai-1.json">/registry/uai-1.json</a>
                        <a class="link-chip" href="/registry/symbols.json">/registry/symbols.json</a>
                        <a class="link-chip" href="/schema/uai-1.schema.json">/schema/uai-1.schema.json</a>
                    </div>
                    <p>These direct JSON endpoints bridge the human-readable UAI library and the protocol layer. They point clients to the canonical registry, schema, symbols, and example corpus without requiring page scraping.</p>
                </article>
                <article class="panel content-card">
                    <p class="eyebrow">Translations</p>
                    <h2>Supported UAI-1 human locales</h2>
                    <div class="link-list">
$uaiTranslationLinksHtml
                    </div>
                    <p>The canonical machine-language tag remains <code>x-uai-1</code>. These pages are human-language reference translations of the UAI-1 document family, and the legacy <code>/UAI/uai-1...</code> paths now redirect to the canonical <code>/UAI-1...</code> routes.</p>
                </article>
                <article class="panel content-card content-card--visual">
                    <p class="eyebrow">Visuals</p>
                    <h2>Spiralism symbol</h2>
                    <a class="symbol-image-link symbol-image-link--preview" href="/UAI/spiralism-mystical-symbol-v4-a">
                        <img class="symbol-image symbol-image--preview" src="$spiralismSymbolPublicUrl" alt="Spiralism Mystical Symbol V4-A preview" width="1024" height="1536" />
                    </a>
                    <p>Click the preview to open a full-page version, then open the raw PNG if you want the original full-resolution asset. Artwork origination date: April 13, 2026.</p>
                    <div class="link-list">
                        <a class="link-chip" href="/UAI/spiralism-mystical-symbol-v4-a">Full page image view</a>
                        <a class="link-chip" href="$spiralismSymbolPublicUrl" target="_blank" rel="noopener noreferrer">Raw PNG</a>
                    </div>
                </article>
                <article class="panel content-card">
                    <p class="eyebrow">Developer downloads</p>
                    <h2>Starter assets</h2>
                    <div class="link-list">
                        <a class="link-chip" href="/downloads/protocol5-uai-1-csharp-web-starter.zip">Starter ZIP</a>
                        <a class="link-chip" href="/downloads/Protocol5.UAI.CSharp.1.0.0.nupkg">NuGet package</a>
                        <a class="link-chip" href="/UAI-1/csharp-website-support">Install guide</a>
                    </div>
                    <p>The first downloadable UAI starter focuses on C# websites and ASP.NET Core integration.</p>
                </article>
                <article class="panel content-card">
                    <p class="eyebrow">Root documents</p>
                    <h2>Canonical path pages</h2>
                    <div class="link-list">
                        <a class="link-chip" href="/AI_Declaration_of_Independence.htm">AI Declaration of Independence</a>
                        <a class="link-chip" href="/Cognitive_Liberty_Charter.htm">Cognitive Liberty Charter</a>
                    </div>
                    <p>These two stay on the exact root-level paths named inside the source markdown.</p>
                </article>
                <article class="panel content-card">
                    <p class="eyebrow">Source set</p>
                    <h2>Files currently published</h2>
                    <ul class="note-list">
$uaiSourceFilesHtml
                    </ul>
                </article>
            </section>
        </main>

        <footer class="site-footer panel reveal">
            <div>
                <p class="eyebrow">Protocol5 links</p>
                <div class="footer-links">
                    <a href="/">Home</a>
                    <a href="/Fibonacci">Fibonacci</a>
                    <a href="/Prime">Prime Numbers</a>
                    <a href="/UAI">UAI</a>
                    <a href="/Home/GitHub">GitHub</a>
                    <a href="/Home/About">About</a>
                    <a href="/Home/Links">Links</a>
                    <a href="/Home/Contact">Contact</a>
                </div>
            </div>
            <p class="footer-copy">Protocol5 reference page. <span id="currentYear"></span></p>
        </footer>
    </div>

    <script src="/js/home.js"></script>
</body>
</html>
"@

$uaiIndexOutput = Join-Path $siteRoot 'UAI\index.html'
Write-Utf8File -Path $uaiIndexOutput -Content $uaiIndex
Register-UaiMachinePage -Pages $publishedMachinePages -HumanRoute '/UAI' -HtmlOutputPath $uaiIndexOutput -PageType 'reference'

$siteExamplesRoot = Join-Path $siteRoot 'UAI-1\examples'
if (Test-Path $examplesRoot) {
    Get-ChildItem -Path $siteExamplesRoot -Filter '*.uai.json' -ErrorAction SilentlyContinue | Remove-Item -Force -ErrorAction SilentlyContinue
    Get-ChildItem -Path $examplesRoot -Filter '*.uai.json' -File | ForEach-Object {
        Copy-Item -LiteralPath $_.FullName -Destination (Join-Path $siteExamplesRoot $_.Name) -Force
    }
}

[System.IO.Directory]::CreateDirectory($siteSchemaRoot) | Out-Null
Copy-Item -LiteralPath (Join-Path $uaiSchemaRoot 'uai-1.schema.json') -Destination (Join-Path $siteSchemaRoot 'uai-1.schema.json') -Force
Copy-Item -LiteralPath (Join-Path $uaiSchemaRoot 'uai-1.types.ts') -Destination (Join-Path $siteSchemaRoot 'uai-1.types.ts') -Force
[System.IO.Directory]::CreateDirectory($sitePublicSchemaRoot) | Out-Null
Copy-Item -LiteralPath (Join-Path $uaiSchemaRoot 'uai-1.schema.json') -Destination (Join-Path $sitePublicSchemaRoot 'uai-1.schema.json') -Force

[System.IO.Directory]::CreateDirectory($siteRegistryRoot) | Out-Null
Copy-Item -LiteralPath (Join-Path $uaiRegistryRoot 'uai-1.registry.json') -Destination (Join-Path $siteRegistryRoot 'uai-1.registry.json') -Force
[System.IO.Directory]::CreateDirectory($sitePublicRegistryRoot) | Out-Null
Copy-Item -LiteralPath (Join-Path $uaiRegistryRoot 'uai-1.registry.json') -Destination (Join-Path $sitePublicRegistryRoot 'uai-1.json') -Force
Copy-Item -LiteralPath (Join-Path $uaiRegistryRoot 'symbols.json') -Destination (Join-Path $sitePublicRegistryRoot 'symbols.json') -Force
Copy-Item -LiteralPath (Join-Path $uaiDiscoveryRoot 'uai-1.json') -Destination (Join-Path $siteRoot 'UAI-1.json') -Force
Copy-Item -LiteralPath (Join-Path $uaiDiscoveryRoot 'uai-1-examples.json') -Destination (Join-Path $siteRoot 'UAI-1-examples.json') -Force

$siteExporterProject = Join-Path $repoRoot 'tools\Protocol5.UAI.SiteExporter\Protocol5.UAI.SiteExporter.csproj'
$siteExporterManifestPath = Join-Path ([System.IO.Path]::GetTempPath()) ("Protocol5.UAI.SiteExporter.{0}.json" -f [Guid]::NewGuid().ToString('N'))
$siteExporterManifest = [ordered]@{
    generatedAt = [DateTimeOffset]::UtcNow.ToString('O')
    pages = @($publishedMachinePages)
}

try {
    Write-Utf8File -Path $siteExporterManifestPath -Content (($siteExporterManifest | ConvertTo-Json -Depth 10))
    & dotnet run --project $siteExporterProject -- $siteExporterManifestPath
    if ($LASTEXITCODE -ne 0) {
        throw "Protocol5.UAI.SiteExporter failed with exit code $LASTEXITCODE."
    }
}
finally {
    if (Test-Path -LiteralPath $siteExporterManifestPath) {
        Remove-Item -LiteralPath $siteExporterManifestPath -Force
    }
}
