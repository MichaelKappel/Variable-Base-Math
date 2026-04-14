# Protocol5 Site Operations

## Canonical UAI sources

Canonical machine-readable UAI assets are authored in the repo, not hand-edited in published `SiteContent` copies.

Source of truth:

- `spec/discovery/uai-1.json`
- `spec/discovery/uai-1-examples.json`
- `spec/registry/uai-1.registry.json`
- `spec/registry/symbols.json`
- `spec/schema/uai-1.schema.json`
- `spec/schema/uai-1.types.ts`
- `examples/*.uai.json`
- `UAI/*.md` for human-readable UAI library pages

Generated publish paths:

- `/UAI-1.json`
- `/UAI-1-examples.json`
- `/registry/uai-1-examples.json`
- `/registry/uai-1.json`
- `/registry/symbols.json`
- `/schema/uai-1.schema.json`
- `/UAI-1/registry/uai-1.registry.json`
- `/UAI-1/schema/uai-1.schema.json`
- `/UAI-1/schema/uai-1.types.ts`
- `/UAI-1/examples/*.uai.json`
- `/downloads/UAI-1-Package.zip`
- `/downloads/protocol5-uai-1-csharp-web-starter.zip`
- `/downloads/Protocol5.UAI.CSharp.1.0.0.nupkg`

## Generation workflow

Run these from repo root before publishing Protocol5:

```powershell
a. powershell -ExecutionPolicy Bypass -File tools\Generate-Protocol5UaiPages.ps1
b. powershell -ExecutionPolicy Bypass -File tools\Build-Protocol5UaiCSharpWebsiteSupport.ps1 -Configuration Release
```

`Generate-Protocol5UaiPages.ps1` regenerates the UAI library pages, machine endpoints, schema copies, registry copies, and example copies under `Protocol5.com/SiteContent`.

`Build-Protocol5UaiCSharpWebsiteSupport.ps1` packs the `Protocol5.UAI.CSharp` package and emits both the canonical developer bundle name (`UAI-1-Package.zip`) and the legacy starter ZIP compatibility copy.

## What the host serves

`Protocol5.com.Host/Program.cs` serves:

- `SiteContent` at the site root
- `wwwroot` at the site root for calculator-shell assets
- `/_framework` from the publish root for the Blazor calculator
- explicit clean routes for `/`, `/Fibonacci`, `/Prime`, `/Home/*`, `/UAI`, `/UAI-1*`, and the charter pages
- calculator tools at `/calculator`, `/converter`, and `/encryption`

Static assets under `/schema/*`, `/registry/*`, and `/downloads/*` are served by the `SiteContent` root static-file tree.

## Public contracts that must not break

These are live compatibility paths and must keep working as deployed:

- `/Fibonacci/999.htm`
- `/Prime/999.htm`
- `/Fibonacci/index.htm`
- `/Prime/index.htm`
- `/Prime/`
- `/AI_Declaration_of_Independence.htm`
- `/Cognitive_Liberty_Charter.htm`
- `/calculator`
- `/converter`
- `/encryption`

Do not rewrite the generated Fibonacci or Prime trees into MVC routes or extensionless URLs.

## Safe publish workflow

Use `Publish-Protocol5.ps1` as a stage-and-verify script.

Default behavior:

1. Regenerate UAI pages and download bundles.
2. Publish `Protocol5.com.Host` to the staging directory.
3. Verify required updated files exist in staging.
4. Write a JSON comparison report for the staged output versus the target publish root.
5. Stop without syncing production.

Apply changes only after review:

```powershell
powershell -ExecutionPolicy Bypass -File .\Publish-Protocol5.ps1 -PublishRoot C:\Publish\Protocol5
powershell -ExecutionPolicy Bypass -File .\Publish-Protocol5.ps1 -PublishRoot C:\Publish\Protocol5 -ApplyToPublishRoot
```

The publish report is written to `.artifacts/publish/Protocol5.publish-report.json` by default.

Preserved compatibility areas include:

- `SiteContent\Fibonacci\*.htm`
- `SiteContent\Prime\*.htm`
- `SiteContent\Fibonacci\index.htm`
- `SiteContent\Prime\index.htm`
- `SiteContent\Fibonaccis\*`

## Smoke test before deploy

Run the public-path smoke test against a running host or the live site:

```powershell
powershell -ExecutionPolicy Bypass -File tools\Smoke-TestProtocol5PublicPaths.ps1 -BaseUrl https://protocol5.com
```

The smoke test verifies the legacy math routes, the public UAI pages, the tool routes, the root charter pages, and the canonical machine/download assets.