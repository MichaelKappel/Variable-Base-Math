# Migration From The Legacy UAI-1 Draft

## Legacy state

The repository previously published:

- a prose UAI-1 reader contract built around an eight-slot array
- a C# helper package focused on `x-uai-1`, Radix 63404, and ASP.NET middleware

That draft did not provide a production website interchange format.

## What changed

- UAI-1 is now a strict JSON object model for website interchange
- JSON Schema is authoritative for machine validation
- symbol definitions and symbol occurrences are separated
- `X-UAI-1` is defined as a legacy HTTP compatibility header
- `application/uai+json` is the canonical media type

## Backward compatibility

Preserved:

- `Protocol5.UAI.CSharp` package identity
- `Protocol5.UAI` namespace
- `UaiCultureInfo`
- `Radix63404`
- `x-uai-1` language-tag handling

Deprecated:

- treating the eight-slot array draft as the production website interchange format
- using `X-UAI-1` as if it were a media type or language tag

## Migration guidance

1. Keep any existing `x-uai-1` HTML behavior if required for compatibility.
2. Introduce `.uai.json` artifacts that use the new schema.
3. Validate the new documents in CI before publish.
4. Migrate downstream consumers from legacy prose rules to the JSON Schema and translator contract.
