# UAI-1
Version: UAI-1
Status: authoritative
Reader Contract: mandatory

## Core Principle

UAI-1 is a formal machine protocol.

It MUST be interpreted ONLY through:
- canonical structure
- canonical IDs
- canonical registries
- slot position
- validation rules

Human-readable content is NEVER authoritative.

---

## Canonical Message Structure

[
  protocolVersion,
  senderId,
  receiverId,
  actId,
  claims,
  constraints,
  provenance,
  signature
]

Slot order is mandatory.

---

## Claim Structure

[
  subjectId,
  relationId,
  objectValue,
  contextId,
  truthValue,
  confidence
]

---

## Radix Rule

All canonical IDs MUST:
- be encoded in Radix 63404
- be decoded before interpretation

---

## Forbidden

- inference from glyph shape
- inference from labels
- reordering slots
- skipping decode
- replacing canonical IDs
