# UAI-1 Examples
Version: v1
Status: authoritative companion

## Rule

Canonical data is authoritative.
Human gloss is NOT.

---

## Example 1

```json
[
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
    ["Ų", "2026-04-13T00:00:00Z"]
  ],
  ["\"", 555001]
]
```

## Invalid Example

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
  [],
  []
]
```

This is invalid:

- contains non-canonical value
- must be rejected
