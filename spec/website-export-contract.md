# UAI-1 Website Export Contract

Version: `1.0.0`

Status: normative

This document defines the contract between a human-facing website page and its paired UAI-1 machine representation.

## 1. Purpose

A conformant website export flow turns rendered website content into authoritative UAI-1 without drift between the human page and the machine endpoint.

This contract applies to:

- build-time export pipelines
- file-to-file export jobs
- on-demand API or ASP.NET endpoint generation
- package-backed reference implementations

## 2. Inputs

A conformant exporter accepts:

- an HTML string or a readable HTML file
- translation options that identify the source page and export behavior

Published exporters MUST operate on the rendered page surface, not unpublished authoring fragments or private editor state.

For authoritative production output:

- `SourceUri` MUST be the stable public page URI
- `PageType` MUST be one of the canonical UAI page types
- `DocumentId` SHOULD be supplied explicitly; if derived, it MUST be deterministic for the same source URI and title
- `PreserveUnsupportedAsUnknown` SHOULD remain enabled
- if `ContentHash` is absent, exporters SHOULD compute one from the HTML bytes; the reference exporter uses `sha256:<hex>`

A file-path fallback such as `file:///...` is acceptable for local development, but it is not a substitute for the public `SourceUri` of a published page.

## 3. Export pipeline

A conformant exporter MUST execute these stages in order:

1. Load the HTML input as UTF-8 text.
2. Determine the effective source URI, document ID, title, language, and page type.
3. Translate the DOM according to [translator-contract.md](translator-contract.md).
4. Normalize the resulting document.
5. Validate the document before authoritative output is written or emitted.
6. Serialize canonical UTF-8 JSON only if validation succeeds.

## 4. Output obligations

A published export MUST:

- be valid UAI-1 JSON
- preserve literal source text according to the translator contract
- contain exactly one root `document` node whose id equals `documentId`
- include the required `source`, `metadata`, `structure`, `semantics`, `symbols`, `assets`, `relationships`, `annotations`, `provenance`, and `extensions` containers
- preserve unsupported constructs as `unknown` instead of dropping them silently
- preserve document-local symbols, assets, and relationships exactly as validated

## 5. File export contract

When exporting to disk:

- the output path SHOULD end with `.uai.json`
- paired public paths SHOULD follow a stable pattern such as `/docs/hello` and `/docs/hello/index.uai.json`
- the exporter MAY create missing output directories automatically
- the written file MUST be canonical UTF-8 JSON without post-write mutation

If the input path does not exist or cannot be read, export MUST fail.

## 6. Routed endpoint contract

A live UAI endpoint SHOULD:

- support `GET` and `HEAD`
- emit `Content-Type: application/uai+json; version=1.0.0`
- emit `X-UAI-1: 1.0` for legacy compatibility
- emit `Vary: Accept, X-UAI-1, Accept-Language`
- emit `Link` `describedby` references to the canonical registry and schema

The paired human page SHOULD advertise its machine representation through:

```html
<link rel="alternate" type="application/uai+json" href="/docs/hello/index.uai.json">
```

Reference websites SHOULD also expose the canonical Protocol5 artifacts so clients can reach `/UAI-1.json`, `/registry/uai-1.json`, `/schema/uai-1.schema.json`, and `/UAI-1/examples/...` without scraping HTML.

## 7. Failure behavior

If translation, normalization, or validation fails, the exporter MUST NOT publish the document as authoritative output.

Conformant implementations MUST:

- fail the build, export step, or HTTP request
- surface machine-readable validation codes and paths when available
- preserve unsupported fragments as `unknown` whenever safe preservation is possible

Implementations MUST NOT silently downgrade invalid output into best-effort authoritative JSON.

## 8. Reference implementation mapping

The reference .NET package satisfies this contract through:

- `UaiHtmlExporter.Export`
- `UaiHtmlExporter.ExportFile`
- `UaiHtmlExporter.ExportToFile`
- `MapProtocol5UaiHtmlEndpoint`
- `MapProtocol5UaiDocumentEndpoint`
- `MapProtocol5UaiCanonicalArtifacts`

The validator sample in `tools/Protocol5.UAI.Validator` is the reference smoke-test surface for checking exported files and the embedded canonical examples.

## 9. Minimum conformance

A website export implementation is conformant only if it:

1. produces valid UAI-1
2. preserves unsupported constructs through `unknown`
3. validates before publish or response emission
4. emits the canonical UAI media type for machine responses
5. keeps the human page and the machine page tied to the same stable public resource