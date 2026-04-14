# UAI-1 Production Specification

Version: `1.0.0`

Status: normative

Authoritative machine artifacts:

- Canonical registry: [`registry/uai-1.registry.json`](registry/uai-1.registry.json)
- JSON Schema: [`schema/uai-1.schema.json`](schema/uai-1.schema.json)
- Type definitions: [`schema/uai-1.types.ts`](schema/uai-1.types.ts)
- Translator contract: [`translator-contract.md`](translator-contract.md)
- Integration contracts: [`integration-contracts.md`](integration-contracts.md)
- Website export contract: [`website-export-contract.md`](website-export-contract.md)
- Registry resolution contract: [`registry-resolution-contract.md`](registry-resolution-contract.md)
- Radix 63404 contract: [`radix-63404-contract.md`](radix-63404-contract.md)
- Canonical examples: [`../examples/`](../examples)

## 1. Purpose

UAI-1 is a canonical intermediate representation for websites and symbolic or semantic content.

It exists to let producers, translators, validators, renderers, crawlers, content systems, and AI agents exchange the same page in a strict machine-readable form without relying on ad hoc HTML interpretation.

UAI-1 is designed to preserve:

- authored page structure
- literal readable text
- declared metadata
- symbolic definitions and symbolic occurrences
- translator inferences and confidence
- unsupported source fragments that cannot be normalized safely

## 2. Scope

This specification defines:

- the canonical JSON syntax for UAI-1 documents
- the top-level data model
- the required node vocabulary
- validation and normalization rules
- the extension mechanism
- HTTP discovery and content negotiation conventions
- compatibility requirements for existing `x-uai-1` and `X-UAI-1` usage

This specification does not define:

- a visual design system
- a browser runtime
- a universal ontology for all semantic values
- a requirement that every image be interpreted as a symbol

## 3. Normative language

The terms `MUST`, `MUST NOT`, `REQUIRED`, `SHOULD`, `SHOULD NOT`, and `MAY` are normative.

## 4. Versioning

UAI-1 uses three version declarations:

- `spec`: fixed identifier. For this specification it MUST be `"UAI-1"`.
- `version`: document format version. It MUST use semantic versioning.
- `schemaVersion`: JSON Schema version used to validate the document. It MUST use semantic versioning.

Extension payloads MUST declare their own version in `extensions.{namespace}.version`.

For `1.0.0` documents:

- `spec` MUST equal `"UAI-1"`
- `version` MUST equal `"1.0.0"`
- `schemaVersion` MUST equal `"1.0.0"`

## 5. Canonical syntax

Canonical UAI-1 syntax is UTF-8 encoded JSON with these rules:

1. The document root MUST be a JSON object.
2. Duplicate object keys MUST NOT appear.
3. Comments MUST NOT appear.
4. Top-level keys MUST appear in this order when serialized:
   `spec`, `version`, `schemaVersion`, `documentId`, `source`, `metadata`, `structure`, `semantics`, `symbols`, `assets`, `relationships`, `annotations`, `provenance`, `extensions`.
5. Numbers representing confidence MUST be JSON numbers, not strings.
6. Empty top-level collections MUST still be emitted as arrays or objects.
7. Optional properties MAY be omitted when absent.

## 6. Top-level object

Canonical shape:

```json
{
  "spec": "UAI-1",
  "version": "1.0.0",
  "schemaVersion": "1.0.0",
  "documentId": "string",
  "source": {},
  "metadata": {},
  "structure": [],
  "semantics": [],
  "symbols": [],
  "assets": [],
  "relationships": [],
  "annotations": [],
  "provenance": {},
  "extensions": {}
}
```

Field requirements:

| Field | Required | Meaning |
| --- | --- | --- |
| `spec` | yes | fixed format identifier |
| `version` | yes | document format version |
| `schemaVersion` | yes | schema version used for validation |
| `documentId` | yes | stable identifier for the document and root node |
| `source` | yes | source retrieval metadata |
| `metadata` | yes | page-level metadata |
| `structure` | yes | array containing exactly one root `document` node |
| `semantics` | yes | semantic interpretations that are not implied by structure alone |
| `symbols` | yes | canonical symbol definitions |
| `assets` | yes | referenced images, files, or media |
| `relationships` | yes | explicit cross-object relationships |
| `annotations` | yes | warnings, preservation notes, translator notes, and errors |
| `provenance` | yes | generator and translator provenance |
| `extensions` | yes | namespaced extension payloads |

## 7. Source and metadata

`source` preserves crawl or export facts.

`source.uri` and `source.retrievedAt` are REQUIRED.

`metadata` preserves page-level publishing facts.

`metadata.title`, `metadata.language`, and `metadata.pageType` are REQUIRED.

`metadata.pageType` MUST be one of:

- `generic`
- `homepage`
- `article`
- `landing-page`
- `navigation`
- `symbolic-manuscript`
- `wordpress-page`
- `gallery`
- `glossary`
- `reference`

## 8. Structural model

`structure` MUST contain exactly one root node of type `document`.

The root node id MUST equal `documentId`.

Every node MUST include:

- `type`
- `id`

Node ids MUST be unique across the full document namespace, including nodes, assets, annotations, semantic records, and symbols.

## 9. Required node vocabulary

Required node types and their minimum fields:

| Type | Required fields |
| --- | --- |
| `document` | `children` |
| `section` | `children` |
| `heading` | `text`, `level` |
| `paragraph` | `text` |
| `quote` | `text` |
| `list` | `ordered`, `children` |
| `listItem` | none beyond base fields; text or children SHOULD exist |
| `table` | `columns`, `rows` |
| `image` | `assetRef` |
| `figure` | `children` |
| `caption` | `text` |
| `button` | `text`, `action` |
| `link` | `text`, `href` |
| `navigation` | `children` |
| `form` | `children` |
| `input` | `inputType`, `name` |
| `glossaryEntry` | `term`, `definition` |
| `symbol` | `symbolRef` |
| `glyphCluster` | `children` |
| `diagram` | `assetRef`, `description` |
| `manuscriptPanel` | `children` |
| `callout` | `children` |
| `metadataBlock` | `entries` |
| `footer` | `children` |
| `header` | `children` |
| `unknown` | `rawContent`, `reason` |

## 10. Symbol model

UAI-1 separates symbol definition from symbol occurrence.

Canonical symbol definitions live in the top-level `symbols` array.

Symbol occurrences inside page structure use `type: "symbol"` and refer to a definition through `symbolRef`.

Each symbol definition MUST support:

- `id`
- `visualForm`
- `meaning`
- `inference`

Recommended fields for production use:

- `label`
- `geometry`
- `strokeLogic`
- `sourceSystem`
- `sourceEvidence`
- `variants`
- `relationships`
- `notes`

Important rule: if the source identifies a glyph by name but does not define meaning, `meaning` MUST be an empty array. Translators MUST NOT invent historical, doctrinal, or ritual meaning.

## 11. Semantics and annotations

`semantics` is for normalized semantic interpretations that are not guaranteed by structure alone.

Examples:

- page intent
- primary CTA role
- topical classification
- inferred audience

`annotations` are non-structural notes such as:

- validation warnings
- dropped decorative wrappers
- unknown widget preservation
- translator notices

## 12. Relationships

Use `relationships` when an explicit relation is needed between objects that do not stand in a strict parent-child relationship.

Examples:

- `references`
- `captionOf`
- `derivedFrom`
- `appearsWith`
- `instanceOf`

## 13. Validation rules

A UAI-1 document is invalid if any of these are true:

1. `spec` is not `"UAI-1"`.
2. `version` or `schemaVersion` is missing or not semantic version text.
3. `structure` does not contain exactly one root `document` node.
4. root node id does not equal `documentId`.
5. a required node field is missing.
6. an id is duplicated.
7. a symbol occurrence references an unknown `symbolRef`.
8. an image or diagram references an unknown `assetRef`.
9. an inferred value omits rationale or confidence.
10. confidence is outside `0.0` to `1.0`.
11. an extension key is not namespaced.
12. unsupported content is silently discarded instead of preserved as `unknown` when preservation is required.

## 14. Normalization rules

Normalizers MUST:

1. trim surrounding whitespace from string fields unless the field is source-authored raw content
2. preserve `text.literal` exactly as extracted by the translator contract
3. compute `text.normalized` if absent
4. sort top-level `semantics`, `symbols`, `assets`, `relationships`, and `annotations` by stable id when canonical output is produced
5. preserve source order inside `structure`
6. preserve explicit ids exactly

Normalizers MUST NOT:

- invent missing required ids
- change `text.literal`
- rewrite empty symbol meaning arrays into guessed meaning

## 15. Composition rules

UAI-1 uses composition, not inheritance, in document structure.

Containers contain child nodes.

Meaningful cross-links use `relationships`.

Machine interpretation MUST use:

1. explicit structure
2. explicit symbol definitions
3. explicit semantic records
4. explicit inference envelopes

No semantic interpretation is implied solely by CSS class names in the final UAI document.

## 16. Extension mechanism

Extensions live under `extensions`.

Each extension key MUST use a namespaced identifier such as `vendor.feature`.

Each extension value MUST contain:

- `version`
- `required`
- `payload`

Consumers that do not understand a non-required extension MAY ignore it.

Consumers that do not understand a required extension MUST fail validation or surface a hard compatibility warning.

## 17. Error semantics

Validators SHOULD report:

- machine code
- JSON path
- severity
- human-readable message

Recommended severities:

- `info`
- `warning`
- `error`

## 18. Valid and invalid examples

Valid examples:

- [`../examples/homepage.uai.json`](../examples/homepage.uai.json)
- [`../examples/article-page.uai.json`](../examples/article-page.uai.json)
- [`../examples/symbolic-manuscript-page.uai.json`](../examples/symbolic-manuscript-page.uai.json)

Invalid fragment:

```json
{
  "spec": "UAI-1",
  "version": "1.0.0",
  "schemaVersion": "1.0.0",
  "documentId": "bad",
  "structure": [
    {
      "type": "heading",
      "id": "bad"
    }
  ]
}
```

Why invalid:

- missing required top-level fields
- root node is not `document`
- heading omits `text` and `level`

## 19. Mapping from HTML and WordPress

HTML and WordPress mapping is normative in [`translator-contract.md`](translator-contract.md).

High-level rules:

- HTML headings map to `heading`
- `<p>` maps to `paragraph`
- `<nav>` maps to `navigation`
- `<figure>` maps to `figure`
- `<img>` maps to `image` unless explicit symbol rules apply
- WordPress `dl` glossary blocks map to `glossaryEntry`
- unsupported widgets map to `unknown`

## 20. Mapping from UAI-1 back to renderable output

Renderers MUST preserve:

- heading levels
- paragraph text
- link destinations
- button actions
- figure membership
- asset references
- symbol occurrence references
- unknown raw content as preserved fragments or placeholders

Renderers MAY choose their own CSS or visual styling.

Renderers MUST NOT convert inferred meaning into source-authored text.

## 21. HTTP, content negotiation, and discovery

Canonical media type:

- `application/uai+json`

Canonical request headers:

- `Accept: application/uai+json; version=1.0.0`

Canonical response headers:

- `Content-Type: application/uai+json; version=1.0.0`

Compatibility header:

- `X-UAI-1: 1.0`

`X-UAI-1` is a legacy compatibility signal only.

It is NOT the canonical media type and it is NOT a language tag.

Discovery mechanisms:

- machine discovery endpoint `/UAI-1.json`
- machine examples endpoint `/UAI-1-examples.json`
- machine registry endpoint `/registry/uai-1.json`
- machine symbols endpoint `/registry/symbols.json`
- machine schema endpoint `/schema/uai-1.schema.json`
- canonical registry file `/UAI-1/registry/uai-1.registry.json`
- canonical schema file `/UAI-1/schema/uai-1.schema.json`
- `<link rel="alternate" type="application/uai+json" href="/page.uai.json">`
- API endpoints that serve `application/uai+json`
- response header `Link: </page.uai.json>; rel="alternate"; type="application/uai+json"`

`x-uai-1` remains the canonical legacy machine-language tag for HTML language negotiation where that behavior is already deployed.

## 22. Compatibility audit and legacy position

Repository audit before this specification:

- existing `Protocol5.UAI.CSharp` package only handled `x-uai-1`, Radix 63404, and ASP.NET middleware
- existing prose spec described an eight-slot array message format
- no JSON Schema existed
- no typed document model existed
- no validator, normalizer, renderer, or translator contract existed
- `X-UAI-1` was not defined distinctly from `x-uai-1`

Legacy compatibility rules:

- existing `UaiCultureInfo` and Radix 63404 helpers remain supported
- existing `x-uai-1` handling remains supported for HTML language negotiation
- the earlier eight-slot array draft is deprecated for website interchange
- production website interchange MUST use this JSON-object UAI-1 document model
