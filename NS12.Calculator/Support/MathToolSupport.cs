using System;
using System.Collections.Generic;
using System.Linq;
using NS12.VariableBase.Mathematics.Common.Interfaces;
using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Providers;
using NS12.VariableBase.Mathematics.Providers.MathEnvironments;
using NS12.VariableBase.Mathematics.Providers.Utilities;

namespace NS12.Calculator.Support;

internal sealed record RadixOption(int Value, string Label, string HelpText);

internal static class MathToolSupport
{
    private const string Base36Key = "0123456789abcdefghijklmnopqrstuvwxyz";
    private const string Base62Key = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    internal static IReadOnlyList<RadixOption> PublicRadixOptions { get; } = new[]
    {
        new RadixOption(2, "Binary (2)", "Allowed symbols: 0-1"),
        new RadixOption(3, "Ternary (3)", "Allowed symbols: 0-2"),
        new RadixOption(5, "Quinary (5)", "Allowed symbols: 0-4"),
        new RadixOption(6, "Senary (6)", "Allowed symbols: 0-5"),
        new RadixOption(8, "Octal (8)", "Allowed symbols: 0-7"),
        new RadixOption(10, "Decimal (10)", "Allowed symbols: 0-9"),
        new RadixOption(16, "Hexadecimal (16)", "Allowed symbols: 0-9 and a-f"),
        new RadixOption(36, "Base 36 (36)", "Allowed symbols: 0-9 and a-z"),
        new RadixOption(62, "Base 62 (62)", "Allowed symbols: 0-9, a-z, and A-Z"),
        new RadixOption(UnicodeRadixKeyProvider.Radix63404, "Radix 63404", "Every BMP character that is not whitespace, not control, and not a surrogate code unit.")
    };

    internal static RadixOption GetRadixOption(int radix)
    {
        return PublicRadixOptions.First(option => option.Value == radix);
    }

    internal static IMathEnvironment<Number> CreateEnvironment(int radix)
    {
        return radix switch
        {
            2 => new CharMathEnvironment("01"),
            3 => new CharMathEnvironment("012"),
            5 => new CharMathEnvironment("01234"),
            6 => new CharMathEnvironment("012345"),
            8 => new CharMathEnvironment("01234567"),
            10 => new CharMathEnvironment("0123456789"),
            16 => new CharMathEnvironment("0123456789abcdef"),
            36 => new CharMathEnvironment(Base36Key),
            62 => new CharMathEnvironment(Base62Key),
            UnicodeRadixKeyProvider.Radix63404 => new CharMathEnvironment(UnicodeRadixKeyProvider.Radix63404Key),
            >= 2 and <= 62 => new CharMathEnvironment(Base62Key[..radix]),
            > 62 and <= 65534 => new CharMathEnvironment((char)radix),
            _ => throw new ArgumentOutOfRangeException(nameof(radix), "Radix must be between 2 and 65534.")
        };
    }

    internal static Number CreateNumber(
        IMathEnvironment<Number> environment,
        string? wholeRaw,
        string? numeratorRaw,
        string? denominatorRaw,
        bool negativeFlag,
        string label)
    {
        (string whole, bool isNegative) = NormalizeWhole(wholeRaw, negativeFlag);
        whole = string.IsNullOrWhiteSpace(whole) ? environment.Key[0].ToString() : whole;

        string numerator = (numeratorRaw ?? string.Empty).Trim();
        string denominator = (denominatorRaw ?? string.Empty).Trim();

        bool hasNumerator = !string.IsNullOrWhiteSpace(numerator);
        bool hasDenominator = !string.IsNullOrWhiteSpace(denominator);

        if (hasNumerator != hasDenominator)
        {
            throw new FormatException($"{label}: provide both numerator and denominator, or leave both empty.");
        }

        if (hasDenominator && environment.IsZero(denominator))
        {
            throw new DivideByZeroException($"{label}: denominator cannot be zero.");
        }

        if (hasNumerator && environment.IsZero(numerator))
        {
            numerator = string.Empty;
            denominator = string.Empty;
        }

        return environment.GetNumber(whole, numerator, denominator, isNegative);
    }

    internal static (string Whole, bool IsNegative) NormalizeWhole(string? value, bool initialNegative)
    {
        string whole = (value ?? string.Empty).Trim();
        bool isNegative = initialNegative;

        if (whole.StartsWith("+", StringComparison.Ordinal))
        {
            whole = whole[1..].Trim();
        }

        if (whole.StartsWith("-", StringComparison.Ordinal))
        {
            isNegative = true;
            whole = whole[1..].Trim();
        }

        return (whole, isNegative);
    }

    internal static bool IsZero(Number number)
    {
        return number.Fragment == null && number.Whole.Length == 1 && number.Whole[0] == 0;
    }

    internal static string EscapeForDisplay(string value)
    {
        return string.Concat((value ?? string.Empty).Select(current => $"\\u{(int)current:X4}"));
    }

    internal static string ConvertToString(IMathEnvironment<Number> environment, NumberSegments segments)
    {
        return environment.ConvertToString(segments);
    }

    internal static string GetRadixDefinitionDisplay(IMathEnvironment<Number> environment)
    {
        if (environment.Key.Count <= 128)
        {
            return new string(environment.Key.ToArray());
        }

        if (environment.Key.Count == UnicodeRadixKeyProvider.Radix63404)
        {
            return "Ascending Unicode order of every BMP character that is not whitespace, not control, and not a surrogate code unit.";
        }

        return $"Radix {environment.Key.Count} definition is too large to display inline.";
    }

    internal static string GetRadixPreviewDisplay(IMathEnvironment<Number> environment, int prefixLength = 16, int suffixLength = 16)
    {
        if (environment.Key.Count <= 128)
        {
            return EscapeForDisplay(new string(environment.Key.ToArray()));
        }

        string prefix = new string(environment.Key.Take(prefixLength).ToArray());
        string suffix = new string(environment.Key.Skip(environment.Key.Count - suffixLength).Take(suffixLength).ToArray());
        return $"{EscapeForDisplay(prefix)} ... {EscapeForDisplay(suffix)}";
    }

    internal static bool UsesLargeRadixDefinition(IMathEnvironment<Number> environment)
    {
        return environment.Key.Count > 128;
    }
}
