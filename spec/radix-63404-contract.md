# Radix 63404 Integration Contract

Version: `1.0.0`

Status: normative

This document defines the canonical Radix 63404 encoding used by the Protocol5 reference library and by any UAI field that carries a Radix 63404 value.

## 1. Base and alphabet

Radix 63404 has base `63404`.

Its alphabet is constructed by iterating every UTF-16 code unit from `0` to `65535` and keeping each character that is not:

- whitespace
- a control character
- a surrogate

The digit value equals the zero-based index of that retained character in iteration order.

The first legal digit is `!`.

## 2. Encode rules

Encoders MUST accept only non-negative integers.

Zero MUST encode to the first legal digit.

Positive integers MUST be encoded by repeated division by `63404`, then emitted most-significant digit first.

Encoders MUST NOT:

- emit sign characters
- trim or normalize digits
- change case
- substitute look-alike characters

## 3. Decode rules

Decoders MUST reject null and empty input.

Each character MUST be interpreted as a single Radix 63404 digit according to the canonical alphabet.

Decoders MUST evaluate left to right using ordinary positional arithmetic in base `63404`.

Whitespace inside the value is invalid.

## 4. Interoperability requirements

To remain compatible with the reference .NET implementation, cross-language implementations MUST mirror the .NET `char` alphabet construction over the Basic Multilingual Plane.

They MUST NOT substitute a Unicode-scalar or grapheme-cluster enumeration model.

Consumers MUST decode the full Radix 63404 string before semantic resolution when a field stores a canonical encoded value.

## 5. Reference values

- `0` encodes to `!`
- `41` encodes to `J`

A conformant implementation SHOULD also pass large integer round-trip tests.

## 6. Failure behavior

Encoding a negative integer is a hard failure.

Decoding an input containing an illegal digit is a hard failure.

A `TryDecode`-style API MAY return `false` instead of throwing, but it MUST NOT silently substitute a different value.

## 7. Minimum conformance

A Radix 63404 implementation is conformant only if it:

1. encodes zero to `!`
2. encodes `41` to `J`
3. round-trips a large positive integer through encode and decode
4. rejects illegal digits