# Protocol5 UAI-1 C# Download

This download packages the reference .NET implementation for getting a working UAI endpoint onto a website quickly. The stable Protocol5 bundle URL is `/downloads/UAI-1-Package.zip`, while the older starter ZIP name remains available as a compatibility alias.

Contents:

- `downloads/Protocol5.UAI.CSharp.1.0.0.nupkg`
- `src/Protocol5.UAI.CSharp/`
- `tools/Protocol5.UAI.SiteExporter/`
- `tools/Protocol5.UAI.Validator/`
- `LICENSE`
- `README.md`

The NuGet package itself also contains:

- `contentFiles/any/any/Protocol5.UAI/spec/...`
- `contentFiles/any/any/Protocol5.UAI/examples/...`
- `contentFiles/any/any/Protocol5.UAI/docs/...`

## Install from the local package

```powershell
dotnet add package Protocol5.UAI.CSharp --source .\downloads
```

## Reference implementation flow

The package now covers the full developer path directly:

- install the package
- validate UAI documents
- export HTML into canonical `.uai.json`
- route canonical machine artifacts
- route per-page UAI endpoints
- render UAI documents back to HTML
- test the endpoint output with the same parser and validator

## Site exporter sample app

The starter ZIP now includes a runnable exporter sample:

```powershell
dotnet run --project .\tools\Protocol5.UAI.SiteExporter\Protocol5.UAI.SiteExporter.csproj -- .\tools\Protocol5.UAI.SiteExporter\samples\export-manifest.sample.json
```

That command generates `tools\Protocol5.UAI.SiteExporter\samples\output\hello.uai.json`, which you can then validate with the bundled validator.

## Validator sample app

The starter ZIP also includes a sample validator CLI:

```powershell
dotnet run --project .\tools\Protocol5.UAI.Validator\Protocol5.UAI.Validator.csproj -- --embedded-examples --roundtrip
```

## Minimal ASP.NET Core setup

```csharp
using Protocol5.UAI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProtocol5UaiWebsiteSupport();

var app = builder.Build();
app.UseProtocol5UaiWebsiteSupport();
app.MapProtocol5UaiCanonicalArtifacts();
app.MapProtocol5UaiHtmlEndpoint(
    "/docs/hello/index.uai.json",
    static () => "<html><body><h1>Hello UAI</h1><p>Ready in minutes.</p></body></html>",
    new UaiHtmlTranslationOptions
    {
        SourceUri = "https://example.org/docs/hello",
        DocumentId = "docs-hello",
        PageType = "article"
    });

app.Run();
```

## HTTP conventions

- canonical media type: `application/uai+json`
- legacy compatibility header: `X-UAI-1: 1.0`
- HTML negotiation compatibility tag: `x-uai-1`