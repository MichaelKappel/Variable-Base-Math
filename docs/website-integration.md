# UAI-1 Website Integration

## Recommended architecture

For a production website:

1. Render HTML as usual.
2. Produce a canonical `.uai.json` document for the same page, for example `/docs/hello/index.uai.json` paired with `/docs/hello`.
3. Validate the UAI document before publish or before response emission.
4. Expose the canonical registry at `/UAI-1/registry/uai-1.registry.json` and the canonical schema at `/UAI-1/schema/uai-1.schema.json`.
5. Add `<link rel="alternate" type="application/uai+json" href="...">` to the human page head and emit the same relationship through the `Link` response header.
6. Treat the UAI document as a semantic cache or source-of-truth layer for downstream systems.

## Reference contracts

Use these canonical repo docs when implementing the website side of UAI:

- [../spec/integration-contracts.md](../spec/integration-contracts.md)
- [../spec/translator-contract.md](../spec/translator-contract.md)
- [../spec/website-export-contract.md](../spec/website-export-contract.md)
- [../spec/registry-resolution-contract.md](../spec/registry-resolution-contract.md)
- [../spec/radix-63404-contract.md](../spec/radix-63404-contract.md)

## Package-first reference flow

`Protocol5.UAI.CSharp` now supports the full reference path directly:

```csharp
using Protocol5.UAI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProtocol5UaiWebsiteSupport();

var app = builder.Build();
app.UseProtocol5UaiWebsiteSupport();
app.MapProtocol5UaiCanonicalArtifacts();
app.MapProtocol5UaiHtmlEndpoint(
    "/docs/hello/index.uai.json",
    static () => "<html lang=\"en\"><body><h1>Hello UAI</h1><p>Ready in minutes.</p></body></html>",
    new UaiHtmlTranslationOptions
    {
        SourceUri = "https://example.org/docs/hello",
        DocumentId = "docs-hello",
        PageType = "article"
    });

app.Run();
```

That single setup covers install, export, validate, route, and testable machine output.

## Discovery

Recommended HTML tag:

```html
<link rel="alternate" type="application/uai+json" href="/docs/hello/index.uai.json">
```

Recommended response headers:

```http
Content-Type: application/uai+json; version=1.0.0
X-UAI-1: 1.0
Link: </UAI-1/registry/uai-1.registry.json>; rel="describedby", </UAI-1/schema/uai-1.schema.json>; rel="describedby"; type="application/schema+json"
```

Canonical public artifacts:

- machine discovery: `/UAI-1.json`
- examples index: `/UAI-1-examples.json`
- registry index: `/registry/uai-1.json`
- symbols index: `/registry/symbols.json`
- schema index: `/schema/uai-1.schema.json`
- registry: `/UAI-1/registry/uai-1.registry.json`
- schema: `/UAI-1/schema/uai-1.schema.json`
- types: `/UAI-1/schema/uai-1.types.ts`
- example directory: `/UAI-1/examples`

## Publish checklist

- generated UAI validates against the JSON Schema
- generated UAI validates with the reference validator
- every asset reference resolves
- symbol meanings are not fabricated
- unsupported widgets are preserved as `unknown`

## Export And Render Helpers

Offline export:

```csharp
var exporter = new UaiHtmlExporter();
var export = exporter.ExportToFile("Pages/hello.html", "wwwroot/docs/hello/index.uai.json", new UaiHtmlTranslationOptions
{
    SourceUri = "https://example.org/docs/hello",
    DocumentId = "docs-hello",
    PageType = "article"
});
```

Round-trip render:

```csharp
var html = new UaiHtmlRenderer().Render(export.Document);
```

## ASP.NET Core notes

`Protocol5.UAI.CSharp` includes:

- middleware that keeps existing `x-uai-1` behavior
- canonical embedded artifact routing via `MapProtocol5UaiCanonicalArtifacts()`
- per-page HTML export routing via `MapProtocol5UaiHtmlEndpoint(...)`
- direct document routing via `MapProtocol5UaiDocumentEndpoint(...)`

Use the middleware when you need compatibility with existing Protocol5 behavior, but prefer standards-based `Accept` and `Content-Type` for new integrations.