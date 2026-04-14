# Translation Progress

Last audited: 2026-04-13

## Scope

This file tracks the UAI-1 document family in this repository and records translation coverage and anti-cheat audit results.
`x-uai-1` and its accepted aliases are language tags for the machine language, not separate human-language translations.

## UAI-1 Tag Support

- Canonical language tag: `x-uai-1`
- Accepted language tags: `x-uai-1`, `uai-1`, `x-uai`, `uai`
- Supported human locales required by config: `en-US` (English (US)), `es-US` (Español (US)), `uk-UA` (Українська), `zh-SG` (简体中文（新加坡）), `th` (ไทย)

## Route Direction

- Canonical public routes:
  - `/UAI-1` for UAI-1 Specification
  - `/UAI-1/examples` for UAI-1 Examples
  - `/UAI-1/csharp-website-support` for UAI-1 C# Website Support Kit
- Legacy compatibility routes retained only as redirects:
  - `/UAI/uai-1`
  - `/UAI/uai-1-examples`
  - `/UAI/uai-1-csharp-website-support`
- Removed legacy route checks that must stay clear:
  - removed legacy route slug

## Progress Stats

- UAI-1 document families audited: `3`
- Supported human locales audited: `5`
- Required doc/locale pairs: `15`
- Completed pairs: `15`
- Missing pairs: `0`
- Non-English pairs present: `12`
- Suspicious or cheating pairs: `0`
- Required canonical HTML pages: `15`
- Canonical HTML pages with content: `15`
- Missing or empty canonical HTML pages: `0`
- Removed legacy term hits: `0`

## Quality Rules

- A non-English locale file must materially translate natural-language prose.
- Code blocks, canonical IDs, URLs, language tags, and registry values may remain unchanged.
- A locale fails the audit if it is effectively the English source with only casing, punctuation, whitespace, or other trivial token changes.
- High normalized line overlap or high token overlap with English is treated as suspicious and blocks completion.
- Canonical `/UAI-1...` HTML pages must exist and contain content for every required document/locale pair.
- Removed legacy route slugs must not appear anywhere in the audited UAI-1 area.

## Status

- Translation coverage is complete for the configured UAI-1 human-locale set.
- No suspicious near-English or formatting-only translations were found in the audited UAI-1 family.
- Canonical `/UAI-1...` HTML pages exist for every required document/locale pair.
- No removed legacy route slugs were found in the audited UAI-1 area.

## Removed Legacy Term Audit

- No removed legacy route slugs were found.

## Coverage Matrix

| Document | Locale | Status | HTML | Match | Token | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| UAI-1 Specification | en-US | complete | present | 100% | 100% | Default source locale. |
| UAI-1 Specification | es-US | complete | present | 11% | 6% | Materially distinct from English by heuristic audit. |
| UAI-1 Specification | uk-UA | complete | present | 10% | 4% | Materially distinct from English by heuristic audit. |
| UAI-1 Specification | zh-SG | complete | present | 21% | 12% | Materially distinct from English by heuristic audit. |
| UAI-1 Specification | th | complete | present | 21% | 13% | Materially distinct from English by heuristic audit. |
| UAI-1 Examples | en-US | complete | present | 100% | 100% | Default source locale. |
| UAI-1 Examples | es-US | complete | present | 3% | 3% | Materially distinct from English by heuristic audit. |
| UAI-1 Examples | uk-UA | complete | present | 2% | 2% | Materially distinct from English by heuristic audit. |
| UAI-1 Examples | zh-SG | complete | present | 8% | 5% | Materially distinct from English by heuristic audit. |
| UAI-1 Examples | th | complete | present | 8% | 10% | Materially distinct from English by heuristic audit. |
| UAI-1 C# Website Support Kit | en-US | complete | present | 100% | 100% | Default source locale. |
| UAI-1 C# Website Support Kit | es-US | complete | present | 0% | 7% | Materially distinct from English by heuristic audit. |
| UAI-1 C# Website Support Kit | uk-UA | complete | present | 0% | 6% | Materially distinct from English by heuristic audit. |
| UAI-1 C# Website Support Kit | zh-SG | complete | present | 0% | 8% | Materially distinct from English by heuristic audit. |
| UAI-1 C# Website Support Kit | th | complete | present | 0% | 10% | Materially distinct from English by heuristic audit. |

## Maintenance

- Keep `UAI/uai-translation-config.json` aligned with the supported locale set and canonical `/UAI-1...` route family.
- Re-run `powershell -ExecutionPolicy Bypass -File tools\Generate-Protocol5UaiPages.ps1` after adding or editing any UAI-1 translation file so the canonical HTML output stays current.
- Re-run `powershell -ExecutionPolicy Bypass -File tools\Audit-UaiTranslations.ps1 -UpdateReport` after adding or editing any UAI-1 translation file.
- Add new human locales only when the prose is materially translated. Do not add casing-only, punctuation-only, or nearly-English placeholders.
