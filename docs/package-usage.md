# Protocol5.UAI.CSharp Package Usage

Package id: `Protocol5.UAI.CSharp`

Current package version: `1.0.0`

## What changed

The package now includes:

- the UAI-1 document model
- JSON serializer and parser
- validator and normalizer
- HTML renderer
- HTML-to-UAI translator
- embedded schema and canonical examples
- HTTP helpers for `application/uai+json` and legacy `X-UAI-1`
- the existing Radix 63404 and `x-uai-1` helpers

## Install

```powershell
dotnet add package Protocol5.UAI.CSharp
```

## Core usage

```csharp
using Protocol5.UAI;

var translator = new UaiHtmlTranslator();
var document = translator.Translate(html, new UaiHtmlTranslationOptions
{
    SourceUri = "https://example.org/page",
    PageType = "generic"
});

var validator = new UaiDocumentValidator();
var result = validator.Validate(document);
if (!result.IsValid)
{
    throw new InvalidOperationException("Document failed validation.");
}

var json = UaiDocumentSerializer.Serialize(document);
var renderedHtml = new UaiHtmlRenderer().Render(document);
```

## Embedded artifacts

The package embeds:

- the canonical JSON Schema
- the canonical example `.uai.json` files

Example:

```csharp
var schemaJson = UaiConstants.GetEmbeddedSchemaText();
var homepageExample = UaiConstants.GetEmbeddedExampleText("homepage.uai.json");
```

## Compatibility

- `UaiCultureInfo` remains available for `x-uai-1`
- `Radix63404` remains available
- `X-UAI-1` is now explicitly treated as a legacy HTTP compatibility header
- `application/uai+json` is the canonical media type
