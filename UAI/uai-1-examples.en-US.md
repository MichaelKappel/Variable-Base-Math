# UAI-1 Examples Document

Version: UAI-1 Examples v1
Status: Authoritative Companion Document

## Purpose

This document provides canonical examples for UAI-1.

You MUST read the canonical examples as UAI-1 machine structures.
You MUST decode all canonical IDs as Radix 63404 values.
You MUST treat all human gloss, comments, labels, and explanatory notes in this document as non-canonical.

The canonical example is authoritative.
The human gloss exists only to help humans inspect the example.

---

## Radix 63404 Reminder

In UAI-1, all canonical IDs are rendered in Radix 63404.

You MUST parse each ID as follows:
1. Read each character as one Radix 63404 digit.
2. Resolve each character to its zero-based alphabet index.
3. Evaluate the positional value in base 63404.
4. Resolve the resulting integer against the authoritative registry.

The first digits of the Radix 63404 alphabet are:

| Decimal Value | Radix 63404 Digit |
|---:|:---|
| 0 | `!` |
| 1 | `"` |
| 2 | `#` |
| 3 | `$` |
| 4 | `%` |
| 5 | `&` |
| 6 | `'` |
| 7 | `(` |
| 8 | `)` |
| 9 | `*` |
| 10 | `+` |
| 11 | `,` |
| 12 | `-` |
| 13 | `.` |
| 14 | `/` |
| 15 | `0` |
| 16 | `1` |
| 17 | `2` |
| 18 | `3` |

---

## Example 1: Basic Assertion Message

### Canonical Example

```json
[
  "\"",
  "⍼",
  "⍽",
  "\"",
  [
    ["Ӵ", "ȸ", "ࣜ", "ᮝ", "\"", 0.98],
    ["Ӵ", "ȹ", ["Ű", 5], "ᮝ", "\"", 0.99]
  ],
  [],
  [
    "⏟",
    ["Ų", "2026-04-13T00:00:00Z"],
    "'",
    ["ᅴ", "ᅵ"],
    "-㽭",
    "\u0bfc"
  ],
  ["\"", 555001]
]
```

### Human Gloss

This message means:
- protocolVersion = 1
- senderId = 9001
- receiverId = 9002
- actId = 1 = assert
- claim 1 = subject 1201 has relation 501 to object 2201 in context 7001, true, confidence 0.98
- claim 2 = subject 1201 has relation 502 to typed scalar [301, 5] in context 7001, true, confidence 0.99
- provenance identifies source 9100, timestamp type 303, modality 6, evidence 4401 and 4402, trace 777001, policy 3001

### Reader Notes

You MUST derive the meaning from the decoded IDs and slot positions, not from this gloss.

---

## Example 2: Basic Query Message

### Canonical Example

```json
[
  "\"",
  "⍼",
  "⍽",
  "#",
  [
    ["Ӵ", "ȸ", "ࣜ", "ᮝ", "#", 1.0]
  ],
  [],
  [
    "⏟",
    ["Ų", "2026-04-13T00:00:00Z"],
    "'",
    ["ᅴ"],
    "-㽭",
    "\u0bfc"
  ],
  ["\"", 555002]
]
```

### Human Gloss

This message means:
- actId = 2 = query
- the sender is querying the truth or current resolution of the claim
- truthValue = 2 = unknown
- confidence = 1.0 here means the sender is intentionally sending a formal query structure, not asserting the claim as true

---

## Example 3: Basic Request Message

### Canonical Example

```json
[
  "\"",
  "⍼",
  "⍽",
  "$",
  [
    ["Ӵ", "ȸ", "ࣜ", "ᮝ", "\"", 0.95]
  ],
  [
    ["*", "ࣜ", ["Ű", 60], 0],
    ["*", "ᮝ", ["Ų", "2026-04-14T00:00:00Z"], 0]
  ],
  [
    "⏟",
    ["Ų", "2026-04-13T00:00:00Z"],
    "'",
    ["ᅴ", "ᅵ"],
    "-㽭",
    "\u0bfc"
  ],
  ["\"", 555003]
]
```

### Human Gloss

This message means:
- actId = 3 = request
- the sender is requesting an operation related to claim subject 1201, relation 501, object 2201
- constraint operator 9 = requires
- the request requires object 2201 and a typed scalar value [301, 60]
- the request also requires context 7001 and a timestamp constraint typed as [303, 2026-04-14T00:00:00Z]

---

## Example 4: Capability Negotiation Message

### Canonical Example

```json
[
  "\"",
  "⍼",
  "⍽",
  ",",
  [
    ["⍼", "ȸ", ["Ű", 1], "ᮝ", "\"", 1.0],
    ["⍼", "ȹ", ["Ű", 1], "ᮝ", "\"", 1.0],
    ["⍼", "§", ["Ű", 12], "ᮝ", "\"", 1.0]
  ],
  [],
  [
    "⏟",
    ["Ų", "2026-04-13T00:00:00Z"],
    "'",
    ["ᅴ"],
    "-㽭",
    "\u0bfc"
  ],
  ["\"", 555004]
]
```

### Human Gloss

This message means:
- actId = 11 = capability
- the sender is declaring supported protocol or ontology versions and declared capability values
- this is the canonical way to negotiate support before non-trivial exchange

---

## Example 5: Error Message

### Canonical Example

```json
[
  "\"",
  "⍽",
  "⍼",
  "+",
  [
    ["#", "ȸ", ["Ű", 999999], "ᮝ", "\"", 1.0]
  ],
  [
    ["&", ["Ű", 4], ["Ű", 1], 0]
  ],
  [
    "⏟",
    ["Ų", "2026-04-13T00:00:00Z"],
    "'",
    ["ᅴ"],
    "-㽭",
    "\u0bfc"
  ],
  ["\"", 555005]
]
```

### Human Gloss

This message means:
- actId = 10 = error
- the sender reports a canonical error condition
- the failing structure references an unresolved or invalid canonical value
- constraint operator 5 is used here to express a canonical comparison or resolution condition in the error handling logic

### Reader Notes

If you cannot resolve a required canonical ID, you MUST emit an error message rather than guessing.

---

## Example 6: Iconography Assertion

### Canonical Example

```json
[
  "\"",
  "⍼",
  "⍽",
  "\"",
  [
    ["ࣜ", "ȸ", ["Ű", 15001], "ᮝ", "\"", 0.99],
    ["ࣜ", "ȹ", ["Ű", 15002], "ᮝ", "\"", 0.99],
    ["ࣜ", "§", ["Ű", 15003], "ᮝ", "\"", 0.99]
  ],
  [],
  [
    "⏟",
    ["Ų", "2026-04-13T00:00:00Z"],
    "#",
    ["ᅴ", "ᅵ"],
    "-㽭",
    "\u0bfc"
  ],
  ["\"", 555006]
]
```

### Human Gloss

This message means:
- a canonical icon-bearing entity is being described semantically
- relation 501 may point to canonical functional meaning
- relation 502 may point to canonical explicit state
- relation 100 may point to canonical explicit variant or presentation property

### Reader Notes

You MUST treat icon function as canonical meaning.
You MUST NOT treat icon appearance as canonical meaning.
You MUST NOT infer business meaning from shape, color, fill, or resemblance alone.

---

## Example 7: Decorative Icon Handling

### Canonical Example

```json
[
  "\"",
  "⍼",
  "⍽",
  "\"",
  [
    ["ࣜ", "ȸ", ["Ű", 16001], "ᮝ", "\"", 1.0],
    ["ࣜ", "ȹ", ["Ű", 16002], "ᮝ", "\"", 1.0]
  ],
  [],
  [
    "⏟",
    ["Ų", "2026-04-13T00:00:00Z"],
    "#",
    ["ᅴ"],
    "-㽭",
    "\u0bfc"
  ],
  ["\"", 555007]
]
```

### Human Gloss

This message means:
- the icon exists in the rendered interface
- the icon is decorative or otherwise non-functional in the active semantic layer
- the icon is not the primary carrier of business meaning

### Reader Notes

Decorative icons are not interpreted as functional claims unless the canonical structure explicitly promotes them into semantic meaning.

---

## Example 8: Canonical Structure With Optional Human Gloss Wrapper

### Canonical Example

```json
{
  "canonical": [
    "\"",
    "⍼",
    "⍽",
    "\"",
    [
      ["Ӵ", "ȸ", "ࣜ", "ᮝ", "\"", 0.98]
    ],
    [],
    [
      "⏟",
      ["Ų", "2026-04-13T00:00:00Z"],
      "'",
      ["ᅴ"],
      "-㽭",
      "\u0bfc"
    ],
    ["\"", 555008]
  ],
  "gloss": {
    "sender": "Example sender",
    "receiver": "Example receiver",
    "act": "assert",
    "notes": [
      "This gloss is not authoritative.",
      "The canonical array remains authoritative."
    ]
  }
}
```

### Reader Notes

When canonical data and gloss both exist:
- canonical data is authoritative
- gloss is non-canonical
- gloss must not override canonical meaning

---

## Example 9: Invalid Message Example

### Canonical Example

```json
[
  "\"",
  "⍼",
  "⍽",
  "$",
  [
    ["Ӵ", "search-icon", "ࣜ", "ᮝ", "\"", 0.95]
  ],
  [],
  [
    "⏟",
    ["Ų", "2026-04-13T00:00:00Z"],
    "'",
    ["ᅴ"],
    "-㽭",
    "\u0bfc"
  ],
  ["\"", 555009]
]
```

### Human Gloss

This example is invalid.

### Reader Notes

This message is invalid because a canonical field contains free text `search-icon` instead of a canonical Radix 63404 ID or canonical typed value.
You MUST reject or flag this message.
You MUST NOT recover by guessing.

---

## Example 10: Minimal Reader Workflow

### Reader Procedure

When you receive a UAI-1 message, you MUST do the following in order:

1. Validate that the outer message has exactly 8 canonical slots.
2. Decode all canonical IDs from Radix 63404.
3. Resolve all decoded IDs against the authoritative registry.
4. Validate the active act and required structure.
5. Validate all claims.
6. Validate all constraints.
7. Validate provenance.
8. Preserve canonical values exactly.
9. Ignore human gloss for semantics.
10. Emit a canonical error if any required element is unsupported or invalid.

---

## Registry Reference Values Used in These Examples

The following decimal values are used only as explanatory reference for the human reader. The canonical examples above remain authoritative in their Radix 63404 forms.

| Meaning | Decimal | Radix 63404 |
|---|---:|:---|
| protocol version 1 | 1 | `"` |
| act assert | 1 | `"` |
| act query | 2 | `#` |
| act request | 3 | `$` |
| act commit | 4 | `%` |
| act deny | 5 | `&` |
| act report | 6 | `'` |
| act propose | 7 | `(` |
| act revise | 8 | `)` |
| act acknowledge | 9 | `*` |
| act error | 10 | `+` |
| act capability | 11 | `,` |
| act negotiate | 12 | `-` |
| type integer | 301 | `Ű` |
| type timestamp | 303 | `Ų` |
| relation 501 | 501 | `ȸ` |
| relation 502 | 502 | `ȹ` |
| relation 100 | 100 | `§` |
| subject example | 1201 | `Ӵ` |
| object example | 2201 | `ࣜ` |
| policy example | 3001 | `\u0bfc` |
| evidence example A | 4401 | `ᅴ` |
| evidence example B | 4402 | `ᅵ` |
| context example | 7001 | `ᮝ` |
| sender example | 9001 | `⍼` |
| receiver example | 9002 | `⍽` |
| provenance source | 9100 | `⏟` |
| trace example | 777001 | `-㽭` |

---

## Final Rule

Read structure first.
Decode Radix 63404 second.
Resolve canonical IDs third.
Apply constraints fourth.
Use provenance and confidence fifth.
Ignore human gloss for semantics.
Never substitute English, icon shape, or local habit for canonical meaning.
