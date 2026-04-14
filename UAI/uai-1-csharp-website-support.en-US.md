# UAI-1 C# Website Support Kit

This page publishes the Protocol5 NuGet package and starter ZIP for getting a working UAI endpoint onto a C# website in minutes. The package is the reference implementation path for install, load, validate, export, route, render, and test.

## Document Information

- **Audience:** C# and ASP.NET website developers
- **Canonical download ZIP:** [UAI-1-Package.zip](/downloads/UAI-1-Package.zip)
- **Legacy compatibility ZIP:** [protocol5-uai-1-csharp-web-starter.zip](/downloads/protocol5-uai-1-csharp-web-starter.zip)
- **Download NuGet package:** [Protocol5.UAI.CSharp.1.0.0.nupkg](/downloads/Protocol5.UAI.CSharp.1.0.0.nupkg)
- **Canonical download ZIP checksum:** [UAI-1-Package.zip.sha256](/downloads/UAI-1-Package.zip.sha256)
- **Legacy ZIP checksum:** [protocol5-uai-1-csharp-web-starter.zip.sha256](/downloads/protocol5-uai-1-csharp-web-starter.zip.sha256)
- **Canonical language tag:** `x-uai-1`
- **Canonical serialization culture:** `InvariantCulture`
- **Reference package API:** `AddProtocol5UaiWebsiteSupport`, `MapProtocol5UaiCanonicalArtifacts`, `MapProtocol5UaiHtmlEndpoint`
- **Microsoft docs:** [CultureInfo](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureinfo), [InvariantCulture](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureinfo-invariantculture), [CultureAndRegionInfoBuilder](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureandregioninfobuilder)

## What the package now covers

- Install the package into an ASP.NET Core site
- Load canonical examples and discovery assets from the package
- Validate UAI-1 JSON against the canonical schema and reference validator
- Export HTML into canonical `.uai.json`
- Route canonical machine artifacts such as `/UAI-1.json` and `/schema/uai-1.schema.json`
- Route page-specific UAI endpoints such as `/docs/hello/index.uai.json`
- Render UAI-1 back to HTML
- Test endpoint output with the same parser and validator used by the package

## Quick start

Install from NuGet:

```powershell
dotnet add package Protocol5.UAI.CSharp
```

Then wire it into an ASP.NET Core site:

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

That setup gives you a working page endpoint plus the canonical embedded machine artifacts exposed by the package.

## Validate

```csharp
var document = new UaiDocumentParser().Parse(json);
var validation = new UaiDocumentValidator().Validate(document);
if (!validation.IsValid)
{
    throw new InvalidOperationException("UAI validation failed.");
}
```

## Load, validate, and emit canonical JSON

```csharp
var loader = new UaiCanonicalAssetLoader();
var exampleJson = loader.LoadExampleText("homepage.uai.json");
var schemaValidation = new UaiSchemaValidator().ValidateCanonicalJson(exampleJson);
if (!schemaValidation.IsValid)
{
    throw new InvalidOperationException("Canonical validation failed.");
}

var example = loader.LoadExampleDocument("homepage.uai.json");
var validation = new UaiDocumentValidator().Validate(example);
var canonicalJson = UaiDocumentSerializer.Serialize(example);
```

## Export

```csharp
var exporter = new UaiHtmlExporter();
var export = exporter.ExportToFile("Pages/hello.html", "wwwroot/docs/hello/index.uai.json", new UaiHtmlTranslationOptions
{
    SourceUri = "https://example.org/docs/hello",
    DocumentId = "docs-hello",
    PageType = "article"
});
```

## Render

```csharp
var renderedHtml = new UaiHtmlRenderer().Render(export.Document);
```

## Test

```csharp
var json = await client.GetStringAsync("/docs/hello/index.uai.json");
var document = new UaiDocumentParser().Parse(json);
Assert.IsTrue(new UaiDocumentValidator().Validate(document).IsValid);
```

## HTML recommendation

If a page is meant to declare UAI-aware content directly, use:

```html
<html lang="x-uai-1">
```

And pair the human page with the machine endpoint:

```html
<link rel="alternate" type="application/uai+json" href="/docs/hello/index.uai.json">
```

## Canonical formatting rule

When serializing canonical UAI values, always use `InvariantCulture`:

```csharp
using Protocol5.UAI;

var confidence = 0.875m.ToString(UaiCultureInfo.CanonicalSerializationCulture);
```

## Radix 63404 examples included in the kit

```csharp
Radix63404.Encode(41);        // simple single-glyph id
Radix63404.Encode(5651);      // sample two-glyph id
Radix63404.Encode(267914296); // sample three-glyph id
```

## Download links

- [Download the canonical Protocol5 bundle](/downloads/UAI-1-Package.zip)
- [Download the legacy starter ZIP alias](/downloads/protocol5-uai-1-csharp-web-starter.zip)
- [Download the NuGet package directly](/downloads/Protocol5.UAI.CSharp.1.0.0.nupkg)
- [Read the UAI-1 specification](/UAI-1)
- [Browse the UAI library](/UAI)
- [Read the Radix 63404 guide](/UAI/radix-63404-guide-and-attribution)