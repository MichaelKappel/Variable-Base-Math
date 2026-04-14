# UAI-1 Registry Resolution Contract

Version: `1.0.0`

Status: normative

This document defines how clients discover, compare, and resolve the authoritative UAI-1 artifacts published by Protocol5.

## 1. Authoritative origin

The authoritative public origin is `https://protocol5.com`.

Consumers MAY cache or mirror artifacts, but a mirror does not outrank the canonical Protocol5 origin unless the operator intentionally forks the protocol surface.

## 2. Canonical entry points

The canonical Protocol5 artifact set includes these entry points:

- discovery: `/UAI-1.json`
- examples index: `/UAI-1-examples.json`
- registry alias: `/registry/uai-1.json`
- symbol registry: `/registry/symbols.json`
- schema alias: `/schema/uai-1.schema.json`
- canonical registry: `/UAI-1/registry/uai-1.registry.json`
- canonical schema: `/UAI-1/schema/uai-1.schema.json`
- canonical types: `/UAI-1/schema/uai-1.types.ts`

## 3. Resolution algorithm

A conformant resolver MUST:

1. Start from `/UAI-1.json` or a known canonical path from the registry.
2. Load the registry payload and verify `spec == "UAI-1"`, `version == "1.0.0"`, and `status == "authoritative"`.
3. Resolve public URLs by combining `canonicalPublicOrigin` with the relevant `canonicalPaths` entry.
4. Treat alias files and canonical files as the same logical artifact when their payload is identical.
5. Use `canonicalArtifacts.*.repoPath` only as source-control reference metadata, not as a public URL.

## 4. Schema resolution

Schema consumers SHOULD accept `/schema/uai-1.schema.json` as the convenience endpoint.

For normative comparisons, consumers SHOULD prefer `/UAI-1/schema/uai-1.schema.json`.

If the alias schema and the canonical schema do not match byte-for-byte, consumers MUST treat that as an authority failure.

## 5. Registry resolution

The registry alias `/registry/uai-1.json` and the canonical registry `/UAI-1/registry/uai-1.registry.json` describe the same authoritative registry payload.

If they diverge, the canonical registry file wins and the mismatch MUST be treated as an error.

## 6. Example resolution

The examples index `/UAI-1-examples.json` is the authoritative list of published example documents.

Each published example MUST live under `/UAI-1/examples/`.

Resolvers SHOULD use the examples index instead of hardcoding filenames when they need the current example corpus.

## 7. Symbol resolution

Inside a UAI document, `symbolRef` MUST resolve against that document's top-level `symbols[]` array.

The published symbol registry `/registry/symbols.json` is a canonical shared reference surface for Protocol5 examples and source-backed symbols.

It MUST NOT be used to excuse a missing required local symbol definition in a document that claims to be self-contained.

## 8. Version and status handling

Resolvers MUST fail or surface a hard compatibility warning if:

- `spec` is not `UAI-1`
- `version` is unknown to the consumer
- `status` is not `authoritative` when authoritative behavior is required

## 9. Failure behavior

Conformant resolvers MUST NOT:

- prefer a human-readable page over the machine registry when machine resolution is required
- invent substitute canonical paths
- replace canonical IDs with local labels during resolution

## 10. Minimum conformance

A resolver is conformant only if it:

1. discovers the registry and schema from the Protocol5 artifact set
2. distinguishes alias endpoints from canonical files while treating matching payloads as one artifact
3. resolves example documents through the examples index
4. resolves document-local `symbolRef` entries against local `symbols[]`
5. treats Protocol5 as the authority when competing mirrors disagree