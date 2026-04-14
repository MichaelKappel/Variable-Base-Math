# Protocol5.UAI.CSharp

`Protocol5.UAI.CSharp` is the reference .NET implementation for installing UAI-1 on a website, loading canonical Protocol5 artifacts, validating documents, exporting `.uai.json`, and routing machine endpoints.

## Reference Contracts

The normative Protocol5 integration contracts live in the repository `spec/` folder:

- [../spec/integration-contracts.md](../spec/integration-contracts.md)
- [../spec/translator-contract.md](../spec/translator-contract.md)
- [../spec/website-export-contract.md](../spec/website-export-contract.md)
- [../spec/registry-resolution-contract.md](../spec/registry-resolution-contract.md)
- [../spec/radix-63404-contract.md](../spec/radix-63404-contract.md)

## Install

```powershell
dotnet add package Protocol5.UAI.CSharp
```

## Route A Working Endpoint In Minutes

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
        PageType = "article",
        SiteName = "Example"
    });

app.Run();
```

That gives you:

- canonical discovery endpoints like `/UAI-1.json`, `/registry/uai-1-examples.json`, and `/UAI-1/schema/uai-1.schema.json`
- a working page endpoint at `/docs/hello/index.uai.json`
- `application/uai+json`, `X-UAI-1`, `Vary`, and `Link` headers
- validation on every exported endpoint response

## Load And Validate Canonical Examples

```csharp
var loader = new UaiCanonicalAssetLoader();
var exampleJson = loader.LoadExampleText("homepage.uai.json");

var canonicalValidation = new UaiSchemaValidator().ValidateCanonicalJson(exampleJson);
if (!canonicalValidation.IsValid)
{
    throw new InvalidOperationException("Canonical validation failed.");
}

var document = loader.LoadExampleDocument("homepage.uai.json");
var validation = new UaiDocumentValidator().Validate(document);
if (!validation.IsValid)
{
    throw new InvalidOperationException("UAI validation failed.");
}

var canonicalJson = UaiDocumentSerializer.Serialize(document);
```

## Export

```csharp
var exporter = new UaiHtmlExporter();
var export = exporter.Export("<html><body><h1>Hello</h1></body></html>", new UaiHtmlTranslationOptions
{
    SourceUri = "https://example.org/hello",
    DocumentId = "hello-doc",
    PageType = "article"
});

var json = export.Json;
```

File-to-file export:

```csharp
exporter.ExportToFile("Pages/hello.html", "wwwroot/docs/hello/index.uai.json", new UaiHtmlTranslationOptions
{
    SourceUri = "https://example.org/docs/hello",
    DocumentId = "docs-hello",
    PageType = "article"
});
```

## Site Exporter CLI Sample

```powershell
dotnet run --project tools\Protocol5.UAI.SiteExporter\Protocol5.UAI.SiteExporter.csproj -- tools\Protocol5.UAI.SiteExporter\samples\export-manifest.sample.json
```

## Render

```csharp
var renderedHtml = new UaiHtmlRenderer().Render(export.Document);
```

## Validator CLI Sample

```powershell
dotnet run --project tools\Protocol5.UAI.Validator\Protocol5.UAI.Validator.csproj -- --embedded-examples --roundtrip
```

## Embedded Artifacts

```csharp
var discoveryJson = UaiConstants.GetEmbeddedProtocolDiscoveryText();
var examplesIndexJson = UaiConstants.GetEmbeddedExamplesIndexText();
var registryJson = UaiConstants.GetEmbeddedRegistryText();
var symbolRegistryJson = UaiConstants.GetEmbeddedSymbolRegistryText();
var schemaJson = UaiConstants.GetEmbeddedSchemaText();
var typesText = UaiConstants.GetEmbeddedTypesText();
var exampleNames = UaiConstants.GetEmbeddedExampleFileNames();

var loader = new UaiCanonicalAssetLoader();
var homepageExample = loader.LoadExampleDocument("homepage.uai.json");
var canonicalValidation = new UaiSchemaValidator().ValidateCanonical(homepageExample);
```

## HTTP Conventions

Canonical media type:

```http
Content-Type: application/uai+json; version=1.0.0
```

Legacy compatibility header:

```http
X-UAI-1: 1.0
```

Existing `x-uai-1` handling remains available for HTML language negotiation and `Content-Language` support.