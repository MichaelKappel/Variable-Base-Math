# Protocol5 UAI-1 C# Website Starter

This download contains the first Protocol5 starter kit for adding UAI-1 support to C# websites.

## Included items

- `downloads/Protocol5.UAI.CSharp.0.1.0.nupkg`
- `src/Protocol5.UAI.CSharp/` source project
- `LICENSE`

## Fastest install path

From the directory that contains the downloaded `.nupkg`:

```powershell
dotnet add package Protocol5.UAI.CSharp --source .\downloads
```

## Source-first path

If you want to review or modify the library before using it:

1. Open `src/Protocol5.UAI.CSharp/Protocol5.UAI.CSharp.csproj`
2. Build it with `dotnet build`
3. Reference the project directly from your website

## What the starter does

- Normalizes website language negotiation to `x-uai-1`
- Exposes `CultureInfo` helpers for UAI-aware sites
- Uses `InvariantCulture` for canonical UAI serialization
- Adds Radix 63404 encoding and decoding
- Includes ASP.NET Core middleware for request detection and `Content-Language`

## Canonical rule

UAI-1 is machine-facing and canonical. Use `x-uai-1` for website negotiation, but use `InvariantCulture` when serializing canonical UAI values.
