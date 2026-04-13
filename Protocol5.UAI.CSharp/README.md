# Protocol5.UAI.CSharp

`Protocol5.UAI.CSharp` is a small starter library for adding UAI-1 support to C# websites.

It focuses on the parts that websites need first:

- `x-uai-1` language-tag handling
- `CultureInfo` integration that stays safe for cross-platform sites
- Radix 63404 encode/decode helpers for canonical identifiers
- ASP.NET Core middleware that recognizes UAI requests from query string, cookie, or `Accept-Language`

## Why the kit uses `x-uai-1`

UAI-1 is machine-facing and canonical. It should not depend on user locale for semantic meaning.

For websites, the safest split is:

- Use `x-uai-1` as the public language tag for HTML and HTTP negotiation
- Use `CultureInfo.InvariantCulture` for canonical serialization
- Use `CultureInfo.GetCultureInfo("x-uai-1")` when the runtime supports it

This matches the practical guidance in Microsoft Learn:

- `CultureInfo`: https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureinfo
- ASP.NET Core localization: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/localization/select-language-culture?view=aspnetcore-9.0

## Install

```powershell
dotnet add package Protocol5.UAI.CSharp
```

If you downloaded the `.nupkg` directly from Protocol5, install it from a local path instead:

```powershell
dotnet add package Protocol5.UAI.CSharp --source .
```

## ASP.NET Core quick start

```csharp
using Protocol5.UAI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProtocol5UaiWebsiteSupport();

var app = builder.Build();
app.UseProtocol5UaiWebsiteSupport();

app.MapGet("/uai-demo", (HttpContext context) =>
{
    var sampleId = Radix63404.Encode(5651);

    return Results.Json(new
    {
        protocol = UaiCultureInfo.CanonicalVersion,
        language = context.GetProtocol5HtmlLanguage(),
        sampleCanonicalId = sampleId
    });
});

app.Run();
```

## HTML recommendation

When you serve a page intended for UAI-aware agents or tooling, set the document language explicitly:

```html
<html lang="x-uai-1">
```

## Canonical formatting rule

When you serialize canonical UAI values, always use `InvariantCulture`:

```csharp
using Protocol5.UAI;

var confidence = 0.875m.ToString(UaiCultureInfo.CanonicalSerializationCulture);
```

## Radix 63404 examples

These values line up with the Protocol5 reference material:

```csharp
Radix63404.Encode(41);        // J
Radix63404.Encode(5651);      // ᙖ
Radix63404.Encode(267914296); // Ⴤ绠
```
