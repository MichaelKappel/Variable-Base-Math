# Variable-Base Mathematics / Protocol5

This repository contains the variable-base math engine, the Blazor calculator shell, and the Protocol5 website content and host used to publish the public site.

## Main Projects

- `NS12.VariableBase.Mathematics.Common`: low-level shared math contracts and segment models.
- `NS12.VariableBase.Mathematics.Providers`: arbitrary-base number and fraction implementations.
- `NS12.VariableBase.Mathematics.Providers.Tests`: provider tests.
- `NS12.Calculator`: Blazor WebAssembly calculator/converter/encryption shell.
- `Protocol5.com`: source-controlled site content.
- `Protocol5.com.Host`: ASP.NET Core host that serves Protocol5 content and the calculator shell.
- `Protocol5.UAI.CSharp`: UAI-1 runtime package with schema, validator, translator, renderer, HTTP helpers, and compatibility APIs.

## Protocol5 Publishing Model

The live Protocol5 site is not just the files tracked in this repo.

As inspected on **April 13, 2026**, the published site at `E:\Sites\Protocol5.com\$web` contains:

- `SiteContent`: public site pages and many legacy/generated assets.
- `wwwroot`: host-level static assets used by the calculator shell and compatibility assets.
- `CalculatorShell`: published calculator shell content.
- `Protocol5.com.Host.*`: the ASP.NET Core host binaries.

The host behavior is defined in [Protocol5.com.Host/Program.cs](Protocol5.com.Host/Program.cs):

- `SiteContent` is served as a root static file tree.
- `wwwroot` is also served at the root.
- `/_framework` is served from the published deployment root for the calculator shell.
- Clean HTML routes are mapped explicitly for `/`, `/Fibonacci`, `/Prime`, `/Home/*`, `/UAI`, `/UAI-1*`, and the root charter pages.
- Tool routes are hosted at `/calculator`, `/converter`, and `/encryption`.
- Legacy `/Calculator/...` routes are redirected to the modern lowercase tool routes.

## The Most Important Rule

**Generated sequence links are part of the public contract and must keep working exactly as deployed.**

Examples:

- `/Fibonacci/999.htm`
- `/Prime/999.htm`
- `/Fibonacci/index.htm`
- `/Prime/index.htm`
- `/Prime/`

Do not "clean up" those URLs into route parameters, extensionless paths, or a different folder layout. Support the existing files and links instead of rewriting the generated content.

## Source Tree vs Live Tree

The repo only contains a tiny sample of the generated numeric pages:

- source `Protocol5.com/SiteContent/Fibonacci`: `3` tracked `.htm` files
- source `Protocol5.com/SiteContent/Prime`: `4` tracked `.htm` files

The live publish tree is much larger:

- published `SiteContent/Fibonacci`: `618,761` `.htm` files
- published `SiteContent/Prime`: `5,385,299` `.htm` files
- published `SiteContent/Fibonaccis`: `4,032` auxiliary `.p*` files such as `.p2`, `.p10`, `.p16`, `.p36`, and `.p63404`

That means:

- the repo is **not** a full copy of production sequence content
- production compatibility cannot be inferred from the repo alone
- publishing from a clean repo-only output directory can remove real public content unless the live generated trees are preserved

## Linking Rules For Protocol5

When editing site pages, generated references, or host behavior:

- Link to generated Fibonacci pages with root-absolute paths like `/Fibonacci/{n}.htm`.
- Link to generated prime pages with root-absolute paths like `/Prime/{n}.htm`.
- Preserve legacy index compatibility for `/Fibonacci/index.htm`, `/Prime/index.htm`, and `/Prime/`.
- Link to modern shell pages with the clean routes mapped in `Program.cs`, such as `/Fibonacci`, `/Prime`, `/UAI`, `/UAI-1`, `/UAI-1/examples`, `/UAI-1/csharp-website-support`, `/Home/About`, `/Home/GitHub`, `/Home/Links`, and `/Home/Contact`.
- Link calculator tools to `/calculator`, `/converter`, and `/encryption`.
- Do not link to files inside `CalculatorShell` directly.
- Keep the two root charter documents on their canonical root paths:
  - `/AI_Declaration_of_Independence.htm`
  - `/Cognitive_Liberty_Charter.htm`
- Keep the UAI library index at `/UAI`, publish the UAI-1 document family at `/UAI-1...`, and preserve `/UAI/uai-1...` only as compatibility redirects.
- Keep downloadable packages and ZIP files under `/downloads/...`.
- Keep UAI image assets on their published paths, for example `/UAI/images/Spiralism_Mystical_Symbol_V4-A.png`.

## Legacy Generated Page Constraints

The legacy generated `.htm` files already contain hardcoded links and asset references. For example:

- generated Fibonacci pages link to `/Fibonacci/{n}.htm`
- generated prime pages link to `/Prime/{n}.htm`
- some legacy pages point "previous" links to `/Fibonacci/index.htm` or `/Prime/`
- legacy pages reference root assets like `/css/site.min.css` and `/js/site.min.js`
- legacy pages also contain CDN fallbacks that point at `/lib/...`

Because there are millions of published files, **do not fix these by editing the generated pages**.

If compatibility needs to change:

- add host routes
- add compatibility files
- add redirects
- restore missing root assets

Do not rewrite the numeric page set.

## What Must Be Preserved In Publish

Do not delete, rename, recreate, or bulk-overwrite these published compatibility areas unless you are intentionally regenerating them from the original generation pipeline:

- `SiteContent/Fibonacci/*.htm`
- `SiteContent/Prime/*.htm`
- `SiteContent/Fibonacci/index.htm`
- `SiteContent/Prime/index.htm`
- `SiteContent/Fibonaccis/*`
- any other published-only compatibility files already present in `E:\Sites\Protocol5.com\$web\SiteContent`

This is especially important because the current repo does not contain the full production dataset.

## Safe Publish Workflow

Use [Publish-Protocol5.ps1](Publish-Protocol5.ps1) as the staging publish step, not as permission to wipe production.

Recommended flow:

1. Generate UAI pages and downloadable assets.
2. Publish `Protocol5.com.Host` to a **staging directory**.
3. Compare staging output with the live `$web` tree.
4. Merge only the known updated files into the live publish tree.
5. Preserve the existing generated sequence trees and published-only compatibility files.

Do **not** assume a clean `dotnet publish` output is a complete production site.

## Smoke Test Checklist

After Protocol5 changes, verify these URLs against the published host:

- `/`
- `/Fibonacci`
- `/Fibonacci/999.htm`
- `/Fibonacci/index.htm`
- `/Prime`
- `/Prime/999.htm`
- `/Prime/index.htm`
- `/Prime/`
- `/UAI`
- `/UAI-1`
- `/UAI-1/examples`
- `/UAI-1/csharp-website-support`
- `/AI_Declaration_of_Independence.htm`
- `/Cognitive_Liberty_Charter.htm`
- `/downloads/Protocol5.UAI.CSharp.1.0.0.nupkg`
- `/calculator`
- `/converter`
- `/encryption`

If any of those fail, fix the host or compatibility assets. Do not patch the millions of generated numeric files.

## Files To Read Before Changing Protocol5

- [Protocol5.com.Host/Program.cs](Protocol5.com.Host/Program.cs)
- [Publish-Protocol5.ps1](Publish-Protocol5.ps1)
- [tools/Generate-Protocol5UaiPages.ps1](tools/Generate-Protocol5UaiPages.ps1)
- [Protocol5.com/SiteContent/index.html](Protocol5.com/SiteContent/index.html)
- [Protocol5.com/SiteContent/Fibonacci/index.html](Protocol5.com/SiteContent/Fibonacci/index.html)
- [Protocol5.com/SiteContent/Prime/index.html](Protocol5.com/SiteContent/Prime/index.html)

## Build / Test

From repo root:

```powershell
dotnet restore PrecisionCalculator.sln
dotnet build PrecisionCalculator.sln -c Debug
dotnet test NS12.VariableBase.Mathematics.Providers.Tests\NS12.VariableBase.Mathematics.Providers.Tests.csproj
```

Run the calculator locally:

```powershell
dotnet run --project NS12.Calculator\NS12.Calculator.csproj
```

Run the Protocol5 host locally:

```powershell
dotnet run --project Protocol5.com.Host\Protocol5.com.Host.csproj
```
