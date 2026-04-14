# Protocol5.UAI.CSharp Package Usage

Package id: `Protocol5.UAI.CSharp`

Current package version: `1.0.0`

The package is the reference developer on-ramp for:

- install
- load canonical assets
- validate
- export
- route
- render
- test

## Reference Contracts

The normative Protocol5 integration contracts live in `spec/`:

- [../spec/integration-contracts.md](../spec/integration-contracts.md)
- [../spec/translator-contract.md](../spec/translator-contract.md)
- [../spec/website-export-contract.md](../spec/website-export-contract.md)
- [../spec/registry-resolution-contract.md](../spec/registry-resolution-contract.md)
- [../spec/radix-63404-contract.md](../spec/radix-63404-contract.md)

## Install

```powershell
dotnet add package Protocol5.UAI.CSharp
```

## Route

Minimal ASP.NET Core setup:

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

What that setup gives you:

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
- your own routed page endpoint, such as `/docs/hello/index.uai.json`

## Validate

```csharp
var loader = new UaiCanonicalAssetLoader();
var exampleJson = loader.LoadExampleText("homepage.uai.json");
var canonicalValidation = new UaiSchemaValidator().ValidateCanonicalJson(exampleJson);
if (!canonicalValidation.IsValid)
{
    throw new InvalidOperationException("Document failed canonical schema validation.");
}

var parser = new UaiDocumentParser();
var validator = new UaiDocumentValidator();
var document = parser.Parse(exampleJson);
var validation = validator.Validate(document);
if (!validation.IsValid)
{
    throw new InvalidOperationException("Document failed validation.");
}

var canonicalJson = UaiDocumentSerializer.Serialize(document);
```

## Export

```csharp
var exporter = new UaiHtmlExporter();
var export = exporter.Export(html, new UaiHtmlTranslationOptions
{
    SourceUri = "https://example.org/page",
    DocumentId = "example-page",
    PageType = "landing-page"
});

var json = export.Json;
var document = export.Document;
```

Export a published HTML file to a `.uai.json` file:

```csharp
exporter.ExportToFile("Pages/page.html", "wwwroot/page/index.uai.json", new UaiHtmlTranslationOptions
{
    SourceUri = "https://example.org/page",
    DocumentId = "example-page",
    PageType = "landing-page"
});
```

## Site Exporter CLI Sample

```powershell
dotnet run --project tools\Protocol5.UAI.SiteExporter\Protocol5.UAI.SiteExporter.csproj -- tools\Protocol5.UAI.SiteExporter\samples\export-manifest.sample.json
```

## Render

```csharp
var renderedHtml = new UaiHtmlRenderer().Render(document);
```

## Test

```csharp
var json = await client.GetStringAsync("/docs/hello/index.uai.json");
var document = new UaiDocumentParser().Parse(json);
var validation = new UaiDocumentValidator().Validate(document);
Assert.IsTrue(validation.IsValid);
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
var homepageExample = UaiConstants.GetEmbeddedExampleText("homepage.uai.json");
var exampleNames = UaiConstants.GetEmbeddedExampleFileNames();

var loader = new UaiCanonicalAssetLoader();
var homepageDocument = loader.LoadExampleDocument("homepage.uai.json");
var canonicalValidation = new UaiSchemaValidator().ValidateCanonical(homepageDocument);
```

## Compatibility

- `UaiCultureInfo` remains available for `x-uai-1`
- `Radix63404` remains available
- `X-UAI-1` is treated as a legacy HTTP compatibility header
- `application/uai+json` is the canonical media type