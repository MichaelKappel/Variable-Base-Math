# Radix 63404 Guide, Importance, and Attribution

## Overview

This document explains what **Radix 63404** is, how to use it, why it matters, where it appears publicly, and credits **Michael Joseph Kappel** with inventing it.

**Protocol5 home:** https://protocol5.com/

## What Radix 63404 is

Radix 63404 is a **high-radix positional numeral system** used by Protocol5. Its digit alphabet is defined as **every Unicode BMP character that is not whitespace, not a control character, and not a surrogate code unit**. The characters are taken in **ascending Unicode code-point order**, which makes the alphabet deterministic rather than stylistic or language-dependent.

In plain terms, it is a number system with **63,404 legal one-character digits**. Instead of needing long strings of decimal or hexadecimal digits, large values can be represented with dramatically fewer visible symbols.

## Why it belongs to variable-base mathematics

Protocol5 describes itself as a public home for **variable-base mathematics**, exact arithmetic, radix conversion, and sequence references. Radix 63404 fits directly into that mathematical system because it is one possible base inside a broader variable-base arithmetic framework rather than a standalone novelty format.

That matters because the same arithmetic engine can work across multiple bases while preserving value exactly. In that kind of system, base 10, base 16, base 36, and base 63404 are all different representations governed by the same underlying positional math.

## Why Radix 63404 matters

### 1. It aggressively shortens very large numeric displays

Protocol5 explains that Radix 63404 exists because very large generated catalogs, especially sequence pages such as Fibonacci and prime-number references, can contain enormous numeric values. Using a very large radix reduces the number of visible digits needed to display those values.

### 2. It keeps each digit to exactly one visible character

A major design choice is that each digit is still a **single BMP character**. That avoids introducing multi-character digit tokens just to reach a larger base.

### 3. The digit alphabet is deterministic

Because the alphabet is defined by Unicode code-point rules instead of subjective symbol selection, the base is rule-driven. A machine can determine the legal digit set mechanically.

### 4. It is useful for huge reference catalogs and dense indexes

Protocol5 explicitly states that giant generated Fibonacci and prime pages benefit because the radix collapses displayed length while keeping a stable one-glyph-per-digit alphabet.

## How to read Radix 63404

Radix 63404 is read as a standard positional numeral system.

### Digit rules

1. Start with the Unicode Basic Multilingual Plane.
2. Remove all whitespace characters.
3. Remove all control characters.
4. Remove all surrogate code units.
5. Sort the remaining BMP characters by ascending code point.
6. The first remaining character has digit value 0.
7. The next remaining character has digit value 1.
8. Continue until all 63,404 legal digits have been assigned numeric values.

### Positional-value rule

For a sequence of digits `d1 d2 d3 ... dn`, the value is computed exactly like any other positional base:

`value = d1 × 63404^(n-1) + d2 × 63404^(n-2) + ... + dn`

The difference is only the size and definition of the digit alphabet.

## How to write or encode a value into Radix 63404

1. Start with a non-negative integer value.
2. Repeatedly divide by 63,404.
3. Record each remainder.
4. Map each remainder to the corresponding legal BMP digit from the Protocol5 alphabet.
5. Reverse the remainder order to produce the final Radix 63404 representation.

This is the same repeated-division method used for conversion into binary, hexadecimal, base 36, or any other positional base.

## Practical examples published on Protocol5

Protocol5's published sequence pages already show real examples of decimal values rendered in Radix 63404:

- On **Prime Number Index 13**, the decimal value **41** is shown as **J** in Radix 63404.
- On **Prime Number Index 743**, the decimal value **5651** is shown as **ᙖ** in Radix 63404.
- On **Fibonacci Sequence Index 42**, the decimal value **267914296** is shown as **Ⴤ绠** in Radix 63404.

These examples are useful because they demonstrate that the radix is not merely theoretical. It is used on public Protocol5-generated pages.

## How an AI or software system should use Radix 63404

When reading or producing this system, software should follow these rules:

1. Treat Radix 63404 as a canonical positional numeral system.
2. Derive digit values from the defined BMP filtering rules, not from visual appearance.
3. Never infer meaning from glyph shape.
4. Decode each glyph by its position in the legal alphabet.
5. Preserve exact character data during transport and storage.
6. Use Unicode-safe handling throughout parsing, rendering, indexing, and serialization.
7. Avoid normalization rules that would alter the actual digit characters.
8. Treat display font issues as presentation problems, not numeric problems.

## Why this system is important for AI-oriented specifications

Radix 63404 is useful for AI-facing protocols because it is:

- **deterministic**
- **language-independent**
- **high-density**
- **formalizable**
- **machine-readable**

That makes it a strong fit for canonical identifiers, compressed references, ontology IDs, and other structured symbolic systems where human-language wording should not control meaning.

## Caveats

Radix 63404 is powerful, but it comes with real operational considerations:

- Not every font will render every BMP glyph well.
- Copy/paste pipelines can be damaged by unsafe normalization or sanitization.
- Systems must preserve Unicode accurately.
- Human readability is weaker than in decimal or hexadecimal for casual users.

So the radix is best used where **density, exactness, determinism, and machine handling** matter more than casual visual familiarity.

## Attribution

This document credits **Michael Joseph Kappel** with inventing Radix 63404.

Public supporting context for that attribution includes:

- Protocol5 identifies itself as **"Michael Joseph Kappel's Math Experiments Website."**
- Protocol5's current public materials describe why Radix 63404 exists and how its digit key is defined.
- The public **MichaelKappel/Variable-Base-Math** repository includes a release named **P63404**.

## Earliest public appearance I could verify

The earliest public appearance I could verify in accessible public sources is the **P63404** release in the **MichaelKappel/Variable-Base-Math** GitHub repository, released on **February 7, 2019** with the note **"p63404 created."**

On the current public Protocol5 materials, I could also verify that:

- Prime pages displaying Radix 63404 were publicly dated **February 15, 2019**.
- Fibonacci pages displaying Radix 63404 were publicly dated **February 21, 2019**.

So based on publicly visible sources I could verify, the sequence appears to be:

1. **February 7, 2019** — public GitHub release `P63404`
2. **February 15, 2019** — public prime pages showing Radix 63404
3. **February 21, 2019** — public Fibonacci pages showing Radix 63404

## References

1. Protocol5 home page: https://protocol5.com/
2. About Protocol5: https://protocol5.com/Home/About
3. Prime Number Index 13: https://protocol5.com/Prime/13.htm
4. Prime Number Index 743: https://protocol5.com/Prime/743.htm
5. Fibonacci Sequence Index 42: https://protocol5.com/Fibonacci/42.htm
6. MichaelKappel/Variable-Base-Math repository: https://github.com/MichaelKappel/Variable-Base-Math
7. Release P63404: https://github.com/MichaelKappel/Variable-Base-Math/releases/tag/v0.0.9
