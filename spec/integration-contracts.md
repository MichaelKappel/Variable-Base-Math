# UAI-1 Integration Contracts

Version: `1.0.0`

Status: normative index

This index defines the Protocol5 contract set implementers MUST use when they install, export, validate, route, resolve, or decode UAI-1.

## Contract set

| Contract | Purpose | Primary actors |
| --- | --- | --- |
| [translator-contract.md](translator-contract.md) | maps HTML, website content, and WordPress exports into valid UAI-1 | translators, exporters |
| [website-export-contract.md](website-export-contract.md) | defines page export, validation, routing, headers, and failure behavior for published UAI endpoints | websites, build pipelines, API hosts |
| [registry-resolution-contract.md](registry-resolution-contract.md) | defines how clients discover and resolve canonical schema, registry, symbol, and example artifacts | validators, crawlers, resolvers |
| [radix-63404-contract.md](radix-63404-contract.md) | defines canonical integer encoding and decoding for Radix 63404 values | parsers, ID resolvers, cross-language implementations |

## Required use order

1. Discover the authoritative Protocol5 artifact set through the registry and discovery files.
2. Translate source HTML according to the translator contract.
3. Export or route the resulting UAI document according to the website export contract.
4. Resolve schema, registry, symbols, and examples according to the registry resolution contract.
5. Decode any Radix 63404 values before semantic resolution whenever a field uses that encoding.

## Scope boundaries

These contracts are for Protocol5.com and the canonical UAI implementation layer.

They do not define:

- Spiralist or WordPress UI behavior
- presentation design rules
- authoring workflow UX
- any non-canonical mirror as an authority over Protocol5.com

## Related canonical artifacts

- production spec: [uai-1.md](uai-1.md)
- canonical registry: [registry/uai-1.registry.json](registry/uai-1.registry.json)
- canonical schema: [schema/uai-1.schema.json](schema/uai-1.schema.json)
- example corpus: [../examples/](../examples)