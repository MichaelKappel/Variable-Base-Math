# UAI-1 Translator Contract

Version: `1.0.0`

Status: normative

This document defines how a translator converts websites, HTML fragments, and WordPress exports into valid UAI-1 without hidden interpretation drift.

## 1. Output obligations

A translator MUST produce:

- a valid UAI-1 document
- explicit preservation of literal source text
- explicit separation of structure and semantics
- explicit marking of inference
- explicit preservation of unsupported constructs through `unknown`

## 2. Six interpretation layers

Every translator decision MUST belong to exactly one of these layers:

| Layer | Meaning | Where it belongs |
| --- | --- | --- |
| Literal source content | exact extracted authored text | `text.literal`, `rawContent`, `sourceRef.htmlFragment` |
| Structural interpretation | DOM or document structure | `structure` nodes |
| Semantic interpretation | normalized meaning not guaranteed by structure alone | `semantics` |
| Symbolic interpretation | explicit symbol definition or symbol occurrence | `symbols`, `type: "symbol"`, `glyphCluster` |
| Inferred meaning | translator-added meaning | `inference` object with rationale and confidence |
| Non-authorial enrichment | operator notes, migration notes, warnings | `annotations`, `extensions` |

No content from a later layer may be emitted as if it came from an earlier layer.

## 3. Rule 1: preserve source text exactly

For text-bearing HTML blocks, translators MUST populate `text.literal` from visible DOM text after:

1. decoding HTML entities
2. converting `<br>` to line breaks
3. removing indentation whitespace introduced only by source formatting
4. collapsing repeated horizontal whitespace outside `pre`, `code`, and `textarea`

Translators MUST NOT:

- paraphrase source text
- translate source text
- silently summarize source text

## 4. Rule 2: separate structure from meaning

These are not equivalent:

- paragraph text and doctrine
- image and symbol
- navigation and marketing intent
- button label and conversion objective

Structure belongs in `structure`.

Meaning beyond structure belongs in `semantics`.

## 5. Rule 3: mark inference explicitly

Whenever a translator infers anything not explicitly authored, it MUST set:

```json
{
  "inference": {
    "isInferred": true,
    "rationale": "why the translator inferred this",
    "confidence": 0.73
  }
}
```

Confidence MUST be in the range `0.0` to `1.0`.

If a value is source-provided, `isInferred` MUST be `false` or the inference object MAY be omitted.

## 6. Rule 4: decorative versus semantic symbols

A decorative shape MUST NOT be promoted to a semantic symbol unless one of these is true:

- the source labels it as a symbol, glyph, sigil, seal, icon, or equivalent
- the source embeds an explicit machine hint such as `data-uai-symbol-id`
- a higher-level authoring system provides a symbol registry entry

Purely decorative graphics MAY remain as `image`, `diagram`, or omitted layout decoration if they carry no authorial content.

## 7. Rule 5: avoid redundant symbol definitions

If the same canonical symbol appears more than once on the page:

- define it once in top-level `symbols`
- create multiple `type: "symbol"` occurrences that reference `symbolRef`

Do not duplicate the same symbol definition for each occurrence.

## 8. Rule 6: canonical symbol definitions versus instances

Top-level `symbols[]` defines the symbol.

Structural `type: "symbol"` nodes represent instances or occurrences.

Example:

- `symbols[0].id = "symbol.flow-spiral"` is the definition
- `structure[].children[].symbolRef = "symbol.flow-spiral"` is an occurrence

## 9. Rule 7: preserve unknowns

Unsupported constructs MUST be preserved as `unknown` unless they are purely non-authorial layout wrappers.

Required shape:

```json
{
  "type": "unknown",
  "id": "page.unknown.1",
  "rawContent": "<custom-widget></custom-widget>",
  "sourceFragment": "<custom-widget></custom-widget>",
  "reason": "unsupported-widget"
}
```

## 10. Structural mapping rules

### Headings

- `<h1>` to `<h6>` map to `heading`
- `level` equals the numeric heading level
- heading text goes in `text.literal`

### Paragraphs

- `<p>` maps to `paragraph`
- inline formatting does not change node type
- inline formatting not represented structurally MUST remain preserved in `sourceRef.htmlFragment`

### Lists

- `<ul>` and `<ol>` map to `list`
- `ordered` is `false` for `<ul>` and `true` for `<ol>`
- each `<li>` maps to `listItem`

### Navigation

- `<nav>` maps to `navigation`
- menu wrappers with `role="navigation"` or recognizable menu classes MAY map to `navigation`
- child anchors map to `link`
- child anchors styled as buttons map to `button`

### Links

- standalone `<a>` elements map to `link`
- `href` is REQUIRED
- visible link text goes in `text.literal`
- `linkPurpose` SHOULD be `navigation`, `external`, or `fragment`

### Buttons and CTAs

- `<button>` maps to `button`
- `<a>` with `role="button"` or button-like classes maps to `button`
- the visible label goes in `text.literal`
- target or submit action goes in `action`

### Forms

- `<form>` maps to `form`
- child `<input>`, `<select>`, and `<textarea>` map to `input`
- `<button type="submit">` remains `button`

### Inputs

- `inputType` is the HTML input type or `select` or `textarea`
- `name` is REQUIRED when available
- visible label MUST be captured in `label` when it can be resolved
- option lists go in `options`

### Tables

- `<table>` maps to `table`
- headers map to `columns`
- rows map to `rows`
- cell text goes in `rows[].cells[].text`

### Images

- `<img>` maps to `image` unless diagram or symbol rules apply
- source URI becomes an `asset`
- `alt` becomes `altText`
- empty `alt` or `aria-hidden="true"` SHOULD set `decorative: true`

### Figures and captions

- `<figure>` maps to `figure`
- contained `<img>` maps to `image`
- `<figcaption>` maps to `caption`

### Glossaries

- glossary-like `<dl>` structures map to `glossaryEntry`
- metadata-like `<dl>` structures map to `metadataBlock`
- translators SHOULD prefer `glossaryEntry` only when the source behaves as term-definition content

### Metadata blocks

- visible key-value page metadata maps to `metadataBlock`
- invisible head metadata belongs in top-level `metadata` and `source`, not `metadataBlock`

### Diagrams

- images or SVGs explicitly labeled as diagram, chart, graph, schema, or map MAY map to `diagram`
- unlabeled diagrams SHOULD remain `image` or `unknown`

### Manuscript panels

- multi-panel facsimile or folio content MAY map to `manuscriptPanel`
- panel numbering belongs in `panelNumber`
- folio or shelfmark goes in `folio`

## 11. Symbol mapping rules

Use `symbol` only when one of these is true:

- the source explicitly declares a symbol id
- the source labels the graphic as a symbol or glyph
- the authoring system already associates the graphic with a canonical symbol definition

If the translator cannot identify a stable symbol definition, it MUST keep the item as `image`, `diagram`, or `unknown`.

Translators MUST NOT invent:

- historical lineage
- doctrinal meaning
- ritual meaning
- cultural attribution

## 12. Semantic ambiguity rules

When ambiguity exists, translators MUST choose the least interpretive valid representation.

Examples:

- ambiguous decorative spiral -> `image`, not `symbol`
- marketing paragraph -> `paragraph`, not semantic doctrine
- card grid with ordinary links -> `section` plus `link`, not `navigation` unless it functions as navigation

## 13. Missing values

If a supported field has no source value:

- omit the optional field
- do not use placeholder prose such as `"unknown"`

If a required construct cannot be represented:

- emit `unknown`
- add an `annotation` if useful

## 14. Confidence rules

Confidence applies only to inference or explicit probabilistic interpretation.

Do not attach confidence to source-authored literal text.

Do not attach confidence to required structural facts such as `<h1>` mapping to `heading`.

## 15. Repeated blocks

Repeated page cards, repeated teasers, or repeated navigation entries are preserved as repeated structure nodes in source order.

Repeated identical symbols SHOULD share one symbol definition.

## 16. WordPress-specific rules

For WordPress content:

- post title maps to `metadata.title`
- post slug or permalink maps to `source.uri` and `metadata.canonicalUrl`
- rendered block HTML maps according to the ordinary HTML rules above
- post meta intended for readers MAY map to `metadataBlock`
- hidden post meta needed for provenance MAY map to `provenance` or `extensions`

## 17. Non-authorial enrichment

Translator-added notes belong in:

- `annotations`
- `extensions`
- `semantics` with `source` set to a translator value and `inference.isInferred = true`

They MUST NOT appear inside `text.literal`.

## 18. Determinism requirements

Given the same DOM and the same translator options, the translator MUST produce the same:

- root `documentId`
- node ordering
- node types
- symbol definition ids
- asset ids
- inference markers

This means translators SHOULD:

- traverse the DOM in document order
- use stable id generation
- use explicit heuristics for button-like links and navigation-like containers
- avoid model-dependent freeform guessing

## 19. Translator failure behavior

If translation cannot safely continue:

- emit validation errors
- preserve offending fragments as `unknown` when possible
- do not silently drop content

## 20. Minimum conformance

A translator is conformant only if it:

1. produces schema-valid UAI-1
2. preserves literal text
3. preserves unsupported constructs through `unknown`
4. separates structure, semantics, and inference
5. distinguishes symbol definitions from symbol occurrences
6. does not fabricate unsupported symbolic meaning
