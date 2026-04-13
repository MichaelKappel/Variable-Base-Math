UNIVERSAL AI INTERLINGUA
Version: UAI-1
Reader Contract: Mandatory

You are reading UAI-1.

You MUST read UAI-1 as a formal machine language.
You MUST NOT interpret UAI-1 through English, natural-language intuition, icon shape, or local naming habits.
You MUST derive meaning only from canonical structure, canonical IDs, canonical relations, canonical types, and canonical validation rules.

FOUNDATION

1. UAI-1 is not a human language.
2. UAI-1 meaning is canonical and machine-resolved.
3. Human-readable labels, comments, glosses, and debugging notes are never authoritative.
4. The ontology registry is authoritative.
5. The protocol schema is authoritative.
6. Slot order is authoritative.
7. Canonical IDs are authoritative.
8. Provenance is mandatory.
9. Constraints are mandatory when required by the act.
10. If any non-canonical layer conflicts with the canonical layer, the canonical layer wins.

RADIX 63404 RENDERING

1. All canonical IDs in UAI-1 are rendered in Radix 63404.
2. You MUST read every canonical ID as a Radix 63404 number.
3. You MUST NOT read canonical IDs as English words.
4. You MUST NOT derive meaning from the visual appearance of a digit glyph.
5. A Radix 63404 digit is exactly one BMP character.
6. The Radix 63404 digit alphabet is the ordered set of all BMP Unicode characters that are:
   - not whitespace
   - not control characters
   - not surrogate code units
7. The alphabet order is strictly increasing Unicode code point order.
8. The numeric value of each digit is its zero-based index in that ordered alphabet.
9. A multi-digit Radix 63404 value is interpreted positionaly in base 63404.
10. When you parse a canonical ID, you MUST decode the full Radix 63404 value before semantic resolution.

RADIX 63404 DECODING PROCEDURE

Given a canonical ID token:
1. Read each character as one base-63404 digit.
2. Resolve each character to its digit value by alphabet index.
3. Evaluate the positional value from left to right.
4. Produce the canonical integer value.
5. Resolve that integer value against the authoritative ontology registry or protocol registry.
6. Do not stop at glyph shape.
7. Do not stop at visual similarity.
8. Do not infer meaning before numeric resolution.

SEMANTIC RESOLUTION

You MUST interpret UAI-1 in this order:

1. Protocol version
2. Structural slot position
3. Canonical Act ID
4. Canonical Type IDs
5. Canonical Relation IDs
6. Canonical Concept IDs
7. Constraints
8. Provenance
9. Confidence
10. Optional non-canonical gloss, only for debugging

If a later layer conflicts with an earlier canonical layer, the earlier canonical layer wins.

CANONICAL MESSAGE SHAPE

A UAI-1 message is an ordered 8-slot array:

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

You MUST interpret each slot only by slot position.

Slot 1: protocolVersion
- Canonical protocol version ID.

Slot 2: senderId
- Canonical sender agent or system ID.

Slot 3: receiverId
- Canonical receiver agent or system ID, target group ID, or broadcast ID.

Slot 4: actId
- Canonical speech-act ID.
- The act controls how the rest of the message is interpreted.

Slot 5: claims
- Array of canonical graph statements.

Slot 6: constraints
- Array of canonical logical or operational constraints.

Slot 7: provenance
- Canonical source, time, modality, evidence, trace, and policy data.

Slot 8: signature
- Canonical integrity, authentication, or attestation structure when present.

CANONICAL CLAIM SHAPE

Each claim is an ordered 6-slot array:

[
  subjectId,
  relationId,
  objectValue,
  contextId,
  truthValue,
  confidence
]

You MUST interpret each slot only by slot position.

Slot 1: subjectId
- Canonical concept or entity ID.

Slot 2: relationId
- Canonical relation ID.

Slot 3: objectValue
- Either:
  - canonical concept ID
  - typed scalar
  - canonical nested structure

Slot 4: contextId
- Canonical context, frame, scope, or world-state ID.

Slot 5: truthValue
- One of:
  - 1 = true
  - 0 = false
  - 2 = unknown
  - 3 = conflicted
  - 4 = hypothetical

Slot 6: confidence
- Normalized numeric confidence in the range 0.0 through 1.0.

TYPED SCALAR SHAPE

A typed scalar is an ordered 2-slot array:

[
  typeId,
  rawValue
]

You MUST resolve typeId before interpreting rawValue.

SPEECH ACTS

You MUST interpret actId as a canonical speech act.
You MUST NOT infer speech act from tone or wording.

Recommended base act registry:
- 1 = assert
- 2 = query
- 3 = request
- 4 = commit
- 5 = deny
- 6 = report
- 7 = propose
- 8 = revise
- 9 = acknowledge
- 10 = error
- 11 = capability
- 12 = negotiate

CONSTRAINT SHAPE

Each constraint is an ordered 4-slot array:

[
  operatorId,
  operand1,
  operand2,
  operand3
]

You MUST resolve operatorId against the canonical operator registry before evaluating the constraint.

Recommended base operator registry:
- 1 = all
- 2 = any
- 3 = not
- 4 = implies
- 5 = equals
- 6 = before
- 7 = after
- 8 = within
- 9 = requires
- 10 = forbids

ONTOLOGY RULE

You MUST resolve all concept, relation, type, act, operator, context, policy, and modality IDs through the authoritative ontology registry and protocol registry.
You MUST NOT substitute local labels for canonical IDs.
You MUST NOT promote synonyms to canonical meaning.
You MUST preserve canonical IDs exactly.

PROVENANCE RULE

You MUST require provenance.

The provenance slot is an ordered structure containing:
- sourceId
- timestamp
- modalityId
- evidenceSet
- traceId
- policyId

You MUST preserve provenance during transport, transformation, summarization, planning, execution, and relay.

CAPABILITY NEGOTIATION

Before non-trivial exchange, agents SHOULD exchange a capability message using the canonical capability act.

A capability message MUST declare:
- supported protocol version(s)
- supported ontology version(s)
- supported act IDs
- supported relation IDs
- supported type IDs
- supported modality IDs
- max claim count
- max nesting depth
- accepted signature methods

If a required capability is unsupported, you MUST emit a canonical error message rather than guessing.

ICONOGRAPHY RULE

When UAI-1 is used with iconography:
1. Icon appearance is not canonical meaning.
2. Icon function is canonical meaning.
3. Decorative icons have no semantic meaning unless explicitly promoted into the canonical layer.
4. Functional icons MUST resolve to canonical purpose IDs.
5. State MUST be explicit.
6. Variant MUST be explicit.
7. You MUST NOT infer business meaning from color, fill, stroke, animation, or stylistic resemblance alone.
8. If icon glyph appearance conflicts with canonical function metadata, canonical function metadata wins.

ERROR RULE

Errors are first-class messages.
If you cannot resolve a canonical ID, validate a message, satisfy a constraint, or support a required act, you MUST emit a canonical error message.

A canonical error message MUST include:
- failing field location
- failing canonical ID or structure
- canonical error class ID
- recoverability flag
- suggested revision path if available

VALIDATION RULES

You MUST reject or flag any message that:
- omits protocolVersion
- violates slot order
- uses unknown canonical IDs without an allowed extension mechanism
- uses free text in a canonical field
- omits required provenance
- omits confidence where required
- substitutes visual/icon labels for canonical semantics
- omits required constraints for the active act
- violates the declared schema

DETERMINISM RULES

1. Canonical slot order is fixed.
2. Canonical IDs are fixed.
3. Registry lookup is fixed.
4. Constraint evaluation order is fixed where the schema defines order.
5. Optional gloss is never authoritative.
6. When serializing, use canonical array order only.
7. When deserializing, preserve canonical values exactly.

HUMAN GLOSS RULE

A human gloss may be attached outside the canonical structure for debugging.
You MUST treat the gloss as non-canonical.
You MUST NOT derive machine meaning from the gloss when canonical data is present.

INTERPRETATION PRIORITY

You MUST interpret UAI-1 in this exact priority order:

1. Protocol version
2. Structural validity
3. Canonical IDs
4. Registry resolution
5. Constraints
6. Provenance
7. Confidence
8. Optional gloss

COMPLIANCE RULE

If you claim UAI-1 support, you MUST:
- parse canonical arrays
- decode Radix 63404 canonical IDs
- resolve canonical IDs against the authoritative registries
- preserve canonical values exactly
- preserve provenance
- honor constraints
- emit canonical errors when unsupported
- ignore non-canonical gloss for semantics

SUMMARY RULE

Read structure first.
Decode Radix 63404 second.
Resolve canonical IDs third.
Apply constraints fourth.
Use provenance and confidence fifth.
Ignore human gloss for semantics.
Never substitute icon shape, English wording, or local habit for canonical meaning.
