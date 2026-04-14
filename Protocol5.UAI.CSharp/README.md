# Protocol5.UAI.CSharp

`Protocol5.UAI.CSharp` is the reference .NET runtime for the UAI-1 production website format.

It includes:

- the UAI-1 document model
- JSON serialization and parsing
- validation and normalization
- HTML rendering
- deterministic HTML-to-UAI translation helpers
- embedded schema and example documents
- HTTP helpers for `application/uai+json` and legacy `X-UAI-1`
- existing `x-uai-1` language-tag handling and Radix 63404 helpers

## Install

```powershell
dotnet add package Protocol5.UAI.CSharp
```

## Quick start

```csharp
using Protocol5.UAI;

var translator = new UaiHtmlTranslator();
var document = translator.Translate(html, new UaiHtmlTranslationOptions
{
    SourceUri = "https://example.org/page",
    PageType = "generic"
});

var validator = new UaiDocumentValidator();
var validation = validator.Validate(document);
if (!validation.IsValid)
{
    throw new InvalidOperationException("UAI validation failed.");
}

var json = UaiDocumentSerializer.Serialize(document);
var renderedHtml = new UaiHtmlRenderer().Render(document);
```

## Embedded schema and examples

```csharp
var schemaJson = UaiConstants.GetEmbeddedSchemaText();
var homepageExample = UaiConstants.GetEmbeddedExampleText("homepage.uai.json");
```

## HTTP conventions

Canonical media type:

```http
Content-Type: application/uai+json; version=1.0.0
```

Legacy compatibility header:

```http
X-UAI-1: version=1.0.0
```

Existing `x-uai-1` handling remains available for HTML language negotiation.

## ASP.NET Core compatibility

```csharp
using Protocol5.UAI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProtocol5UaiWebsiteSupport();

var app = builder.Build();
app.UseProtocol5UaiWebsiteSupport();
```

## Canonical docs

- Spec: `spec/uai-1.md`
- Translator contract: `spec/translator-contract.md`
- Schema: `spec/schema/uai-1.schema.json`
- Examples: `examples/*.uai.json`
