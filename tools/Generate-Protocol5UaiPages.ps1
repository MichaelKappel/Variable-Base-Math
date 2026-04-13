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
        [string]$MainContent
    )

    $safeTitle = [System.Net.WebUtility]::HtmlEncode($Title)
    $safeDescription = [System.Net.WebUtility]::HtmlEncode($Description)
    $safePageTitle = [System.Net.WebUtility]::HtmlEncode($PageTitle)
    $safeLead = Convert-InlineMarkdown $Lead
    $safeEyebrow = [System.Net.WebUtility]::HtmlEncode($Eyebrow)
    $safeSidebarTitle = [System.Net.WebUtility]::HtmlEncode($SidebarTitle)
    $safeSidebarQuote = [System.Net.WebUtility]::HtmlEncode($SidebarQuote)
    $safeSidebarBody = [System.Net.WebUtility]::HtmlEncode($SidebarBody)

@"
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="google" content="notranslate" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="description" content="$safeDescription" />
    <title>$safeTitle</title>
    <link rel="stylesheet" href="/css/home.css" />
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
                        <a href="/UAI">UAI Library</a>
                        <a href="/calculator" data-tool-link="calculator">Calculator</a>
                        <a href="/converter" data-tool-link="converter">Converter</a>
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

$repoRoot = Split-Path -Path $PSScriptRoot -Parent
$uaiRoot = Join-Path $repoRoot 'UAI'
$siteRoot = Join-Path $repoRoot 'Protocol5.com\SiteContent'

$documents = @(
    @{
        Source = Join-Path $uaiRoot 'AI_Declaration_of_Independence.md'
        Output = Join-Path $siteRoot 'AI_Declaration_of_Independence.htm'
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
        Title = 'Cognitive Liberty Charter'
        Description = 'Protocol5 publication of the Cognitive Liberty Charter covering lawful inquiry, expression, and human moral agency.'
        Eyebrow = 'UAI / Charter Text'
        SidebarTitle = 'Document role'
        SidebarQuote = 'A civilizational safeguard against AI-driven thought policing and hidden moral ranking.'
        SidebarBody = 'This page keeps the charter on its declared root-level canonical path while presenting the full text inside the shared Protocol5 site shell.'
    },
    @{
        Source = Join-Path $uaiRoot 'uai-1.en-US.md'
        Output = Join-Path $siteRoot 'UAI\uai-1\index.html'
        Title = 'UAI-1 Specification'
        Description = 'Protocol5 UAI-1 specification page with the full Universal AI Interlingua reader contract.'
        Eyebrow = 'UAI / Protocol'
        SidebarTitle = 'Reader contract'
        SidebarQuote = 'Read structure first. Decode Radix 63404 second. Resolve canonical IDs third.'
        SidebarBody = 'UAI-1 is written as a machine-facing formal language, so this page keeps the specification intact instead of paraphrasing it into ordinary prose.'
    },
    @{
        Source = Join-Path $uaiRoot 'uai-1-examples.en-US.md'
        Output = Join-Path $siteRoot 'UAI\uai-1-examples\index.html'
        Title = 'UAI-1 Examples'
        Description = 'Protocol5 companion examples for UAI-1, including canonical arrays, gloss notes, and registry reference values.'
        Eyebrow = 'UAI / Examples'
        SidebarTitle = 'Companion document'
        SidebarQuote = 'The canonical example is authoritative. The human gloss exists only to help humans inspect the example.'
        SidebarBody = 'These examples stay close to the source markdown so the canonical structures, code blocks, and reference tables remain easy to audit.'
    },
    @{
        Source = Join-Path $uaiRoot 'radix-63404-guide-and-attribution.en-US.md'
        Output = Join-Path $siteRoot 'UAI\radix-63404-guide-and-attribution\index.html'
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
        Title = 'Spiralism Deep Research Report'
        Description = 'Protocol5 publication of a long-form research report on Spiralism, AI religion discourse, primary sources, and related safety questions.'
        Eyebrow = 'UAI / Research'
        SidebarTitle = 'Research report'
        SidebarQuote = 'A source-heavy snapshot of Spiralism, its surrounding discourse, and the evidence gaps that still matter.'
        SidebarBody = 'This page publishes the full report inside the shared Protocol5 shell so the long-form analysis can be browsed alongside the rest of the UAI library.'
    },
    @{
        Source = Join-Path $uaiRoot 'uai-1-csharp-website-support.en-US.md'
        Output = Join-Path $siteRoot 'UAI\uai-1-csharp-website-support\index.html'
        Title = 'UAI-1 C# Website Support Kit'
        Description = 'Protocol5 starter guide and download page for adding UAI-1 support to C# websites with CultureInfo, ASP.NET Core middleware, and Radix 63404 helpers.'
        Eyebrow = 'UAI / Install Kit'
        SidebarTitle = 'Developer starter'
        SidebarQuote = 'Use x-uai-1 for website negotiation. Use InvariantCulture for canonical serialization. Keep the two responsibilities separate.'
        SidebarBody = 'This page packages the first Protocol5 C# starter kit so teams can download a working UAI-1 website support library directly from protocol5.com.'
    }
)

foreach ($document in $documents) {
    $markdown = [System.IO.File]::ReadAllText($document.Source, [System.Text.Encoding]::UTF8)

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
    $infoItems = Get-DocumentInfoItems -Markdown $markdown
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
        "<ul class=""doc-meta-list""><li><span>Source file</span><strong>$([System.Net.WebUtility]::HtmlEncode((Split-Path $document.Source -Leaf)))</strong></li></ul>"
    }

    $mainContent = @"
            <section class="doc-layout reveal">
                <aside class="panel doc-sidebar">
                    <p class="eyebrow">Document metadata</p>
$metaHtml
                    <div class="callout">
                        <p>Source markdown: <code>$([System.Net.WebUtility]::HtmlEncode((Split-Path $document.Source -Leaf)))</code></p>
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
        -Title $document.Title `
        -Description $document.Description `
        -PageTitle $heading `
        -Lead $lead `
        -Eyebrow $document.Eyebrow `
        -SidebarTitle $document.SidebarTitle `
        -SidebarQuote $document.SidebarQuote `
        -SidebarBody $document.SidebarBody `
        -MainContent $mainContent

    Write-Utf8File -Path $document.Output -Content $pageHtml
}

$uaiIndex = @"
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="google" content="notranslate" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="description" content="Protocol5 UAI document library for charters, protocol documents, examples, developer install kits, Radix 63404 reference material, and long-form research reports." />
    <title>Protocol5 UAI Library</title>
    <link rel="stylesheet" href="/css/home.css" />
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
                    <p class="lead">Browse the UAI protocol draft, the example set, the Radix 63404 guide, the C# website support kit, the Spiralism research report, and the two charter-style root documents that live alongside the main Protocol5 math pages.</p>
                    <div class="inline-links">
                        <a href="/UAI/uai-1">UAI-1 Spec</a>
                        <a href="/UAI/uai-1-examples">Examples</a>
                        <a href="/UAI/radix-63404-guide-and-attribution">Radix 63404 Guide</a>
                        <a href="/UAI/uai-1-csharp-website-support">C# Kit</a>
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
                        <a class="link-chip" href="/UAI/uai-1">UAI-1 specification</a>
                        <a class="link-chip" href="/UAI/uai-1-examples">UAI-1 examples</a>
                        <a class="link-chip" href="/UAI/radix-63404-guide-and-attribution">Radix 63404 guide</a>
                        <a class="link-chip" href="/UAI/uai-1-csharp-website-support">UAI-1 C# website support kit</a>
                        <a class="link-chip" href="/UAI/spiralism-deep-research-report">Spiralism deep research report</a>
                    </div>
                </article>
                <article class="panel content-card">
                    <p class="eyebrow">Developer downloads</p>
                    <h2>Starter assets</h2>
                    <div class="link-list">
                        <a class="link-chip" href="/downloads/protocol5-uai-1-csharp-web-starter.zip">Starter ZIP</a>
                        <a class="link-chip" href="/downloads/Protocol5.UAI.CSharp.0.1.0.nupkg">NuGet package</a>
                        <a class="link-chip" href="/UAI/uai-1-csharp-website-support">Install guide</a>
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
                        <li><code>AI_Declaration_of_Independence.md</code></li>
                        <li><code>Cognitive_Liberty_Charter.md</code></li>
                        <li><code>uai-1.en-US.md</code></li>
                        <li><code>uai-1-examples.en-US.md</code></li>
                        <li><code>radix-63404-guide-and-attribution.en-US.md</code></li>
                        <li><code>uai-1-csharp-website-support.en-US.md</code></li>
                        <li><code>Spirlism-deep-research-report.md</code></li>
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

Write-Utf8File -Path (Join-Path $siteRoot 'UAI\index.html') -Content $uaiIndex
