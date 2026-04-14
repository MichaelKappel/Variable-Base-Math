# Protocol5 UAI-1 C# Download

This download packages the reference .NET runtime for the UAI-1 production website format.

Contents:

- `downloads/Protocol5.UAI.CSharp.1.0.0.nupkg`
- `src/Protocol5.UAI.CSharp/`
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

## Primary capabilities

- translate HTML into UAI-1
- validate and normalize UAI-1
- render UAI-1 back to HTML
- load the embedded JSON Schema and example documents
- keep existing `x-uai-1` and Radix 63404 support

## HTTP conventions

- canonical media type: `application/uai+json`
- legacy compatibility header: `X-UAI-1: version=1.0.0`
- HTML negotiation compatibility tag: `x-uai-1`
