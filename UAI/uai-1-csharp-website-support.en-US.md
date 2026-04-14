# UAI-1 C# Website Support Kit

This page publishes the Protocol5 starter download for adding UAI-1 support to C# websites, especially ASP.NET Core sites that want a practical `CultureInfo` and `Accept-Language` integration path without making canonical UAI semantics depend on local human-language formatting rules.

## Document Information

- **Audience:** C# and ASP.NET website developers
- **Download ZIP:** [protocol5-uai-1-csharp-web-starter.zip](/downloads/protocol5-uai-1-csharp-web-starter.zip)
- **Download NuGet package:** [Protocol5.UAI.CSharp.0.1.0.nupkg](/downloads/Protocol5.UAI.CSharp.0.1.0.nupkg)
- **Download ZIP checksum:** [protocol5-uai-1-csharp-web-starter.zip.sha256](/downloads/protocol5-uai-1-csharp-web-starter.zip.sha256)
- **Canonical language tag:** `x-uai-1`
- **Canonical serialization culture:** `InvariantCulture`
- **Microsoft docs:** [CultureInfo](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureinfo), [InvariantCulture](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureinfo-invariantculture), [CultureAndRegionInfoBuilder](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureandregioninfobuilder)

## What the download contains

- `Protocol5.UAI.CSharp` source project
- A packed `Protocol5.UAI.CSharp` `.nupkg`
- Radix 63404 encode/decode helpers
- ASP.NET Core middleware that recognizes UAI requests from query string, cookie, or `Accept-Language`
- A small install readme for teams that prefer a direct download over a package feed

## Why the starter uses `x-uai-1`

For websites, the clean split is:

- Use `x-uai-1` for HTML `lang`, request negotiation, and `Content-Language`
- Use `CultureInfo.GetCultureInfo("x-uai-1")` when the runtime supports it
- Use `CultureInfo.InvariantCulture` when serializing canonical UAI values

That last rule matters because UAI-1 is defined as a canonical machine language. Decimal separators, date formats, and local display conventions should never change the serialized meaning of a UAI message.

## Why this is not just a Windows culture installer

Microsoft's globalization guidance makes an important distinction:

- `CultureInfo` is the normal runtime entry point for culture-aware behavior
- `CultureAndRegionInfoBuilder` exists for creating custom cultures, but that path is Windows-specific and not the right default for cross-platform website adoption

Because of that, this Protocol5 starter does **not** require OS-level custom-culture registration. It gives websites a practical path first, and leaves platform-specific culture registration as an optional advanced step.

## Quick start

Install from the downloaded package file:

```powershell
dotnet add package Protocol5.UAI.CSharp --source .\downloads
```

Then wire it into an ASP.NET Core site:

```csharp
using Protocol5.UAI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProtocol5UaiWebsiteSupport();

var app = builder.Build();
app.UseProtocol5UaiWebsiteSupport();

app.MapGet("/uai-demo", (HttpContext context) =>
{
    var sampleCanonicalId = Radix63404.Encode(5651);

    return Results.Json(new
    {
        protocol = UaiCultureInfo.CanonicalVersion,
        language = context.GetProtocol5HtmlLanguage(),
        sampleCanonicalId
    });
});

app.Run();
```

## HTML recommendation

If a page is meant to declare UAI-aware content directly, use:

```html
<html lang="x-uai-1">
```

## Canonical formatting rule

When serializing canonical UAI values, always use `InvariantCulture`:

```csharp
using Protocol5.UAI;

var confidence = 0.875m.ToString(UaiCultureInfo.CanonicalSerializationCulture);
```

## Radix 63404 examples included in the kit

```csharp
Radix63404.Encode(41);        // J
Radix63404.Encode(5651);      // ᙖ
Radix63404.Encode(267914296); // Ⴤ绠
```

## Download links

- [Download the full starter ZIP](/downloads/protocol5-uai-1-csharp-web-starter.zip)
- [Download the NuGet package directly](/downloads/Protocol5.UAI.CSharp.0.1.0.nupkg)
- [Read the UAI-1 specification](/UAI-1)
- [Read the Radix 63404 guide](/UAI/radix-63404-guide-and-attribution)
