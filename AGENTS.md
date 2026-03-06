# AGENTS.md

## Purpose
This repository implements arbitrary-precision math in variable bases, with a Blazor WebAssembly calculator UI on top.

## Solution Layout
- `PrecisionCalculator.sln`: main solution entry.
- `NS12.VariableBase.Mathematics.Common` (`netstandard2.1`): shared interfaces and segment models.
- `NS12.VariableBase.Mathematics.Providers` (`netstandard2.1`): core math implementations (`Number`, `Fraction`, operators, algorithms, environment).
- `NS12.VariableBase.Mathematics.Providers.Tests` (`net6.0`, MSTest): unit tests for provider/environment behavior.
- `NS12.Calculator` (`net7.0`, Blazor WASM): UI project.

## Core Architecture
- `NumberSegments` is the low-level numeric representation.
  - Digits are stored least-significant-first (index `0` is the lowest place value).
  - Many algorithms assume this ordering; preserve it in all new code.
- `CharMathEnvironment` defines base/key mapping and number parsing/formatting.
- `Number` is the primary numeric type:
  - Whole part: `NumberSegments`
  - Optional fractional part: `Fraction`
  - Delegates arithmetic/comparison to static `Number.Operator` (`NumberOperator`).
- `BasicMathAlgorithm` performs segment math (`Add`, `Subtract`, `Multiply`, `Divide`, `SquareRoot`, compare helpers).
- `FractionOperator` handles `Fraction` arithmetic for numbers with fractional parts.

## Build, Test, Run
Run from repo root:

```powershell
dotnet restore PrecisionCalculator.sln
dotnet build PrecisionCalculator.sln -c Debug
dotnet test NS12.VariableBase.Mathematics.Providers.Tests\NS12.VariableBase.Mathematics.Providers.Tests.csproj
```

Run calculator UI:

```powershell
dotnet run --project NS12.Calculator\NS12.Calculator.csproj
```

Notes:
- `NS12.Calculator` targets `net7.0` (out of support warning is expected).
- If tests fail with locked `testhost` binaries, terminate stale `testhost` processes and rerun.

## Known Issues / Constraints
- Several operator members are intentionally unimplemented (`NotImplementedException`) and should not be called without implementation:
  - `NumberOperator` (multiple methods)
  - `FractionOperator` (`IsBottom`, `IsEven`, `Convert`, `Square`, `SquareRoot`, `ConvertToBase10`)
- `IterativePrimeAlgorithm.IsPrime` throws `NotImplementedException` for numbers with more than 3 segments.
- `%` currently returns the fractional remainder form (`0 r/b`) instead of an integer remainder (`r`).
- Provider tests include a broad arithmetic matrix (`50` cases per implemented base) covering `+`, `-`, `*`, `/`, `%`.

## Editing Guidance
- Prefer extending behavior in `Providers` + `Providers.Tests` together.
- Keep environment compatibility checks (`a.Environment == b.Environment`) intact unless intentionally adding cross-environment support.
- Preserve segment validity checks (digit range, no leading zero in multi-digit values).
- Avoid committing generated artifacts (`bin/`, `obj/`, `.vs/`), even though this repo currently contains many tracked generated files.

## High-Value Test Areas When Changing Math Logic
- Cross-base arithmetic consistency (`binary`, `decimal`, `base36`).
- Fractional division/reduction (`Number.Reduce`, `NumberOperator.Divide`).
- Comparison semantics with negative numbers and fractions.
- Parsing/formatting round-trips (`ParseNumberSegments` <-> `ConvertToString`).
