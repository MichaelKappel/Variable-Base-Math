using System.Numerics;
using System.Text;

namespace Protocol5.UAI;

public static class Radix63404
{
    public const int Base = 63404;

    private static readonly char[] Alphabet;
    private static readonly int[] DigitValues;
    private static readonly BigInteger BigIntegerBase = new(Base);

    static Radix63404()
    {
        Alphabet = new char[Base];
        DigitValues = new int[char.MaxValue + 1];
        Array.Fill(DigitValues, -1);

        var nextIndex = 0;

        for (var codePoint = 0; codePoint <= char.MaxValue; codePoint++)
        {
            var candidate = (char)codePoint;
            if (!IsLegalDigit(candidate))
            {
                continue;
            }

            if (nextIndex >= Base)
            {
                throw new InvalidOperationException("The Radix 63404 alphabet contains more digits than expected.");
            }

            Alphabet[nextIndex] = candidate;
            DigitValues[candidate] = nextIndex;
            nextIndex++;
        }

        if (nextIndex != Base)
        {
            throw new InvalidOperationException($"The Radix 63404 alphabet resolved to {nextIndex} digits instead of {Base}.");
        }
    }

    public static bool IsLegalDigit(char digit)
    {
        return !char.IsWhiteSpace(digit) &&
               !char.IsControl(digit) &&
               !char.IsSurrogate(digit);
    }

    public static bool TryGetDigitValue(char digit, out int value)
    {
        value = DigitValues[digit];
        return value >= 0;
    }

    public static int GetDigitValue(char digit)
    {
        if (!TryGetDigitValue(digit, out var value))
        {
            throw new FormatException($"'{digit}' is not a legal Radix 63404 digit.");
        }

        return value;
    }

    public static char GetDigit(int value)
    {
        if (value < 0 || value >= Base)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, $"Radix 63404 digit values must be between 0 and {Base - 1}.");
        }

        return Alphabet[value];
    }

    public static string Encode(BigInteger value)
    {
        if (value.Sign < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Radix 63404 only supports non-negative values.");
        }

        if (value.IsZero)
        {
            return Alphabet[0].ToString();
        }

        var digits = new StringBuilder();
        var remaining = value;

        while (remaining > BigInteger.Zero)
        {
            remaining = BigInteger.DivRem(remaining, BigIntegerBase, out var remainder);
            digits.Append(GetDigit((int)remainder));
        }

        var chars = digits.ToString().ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    public static BigInteger Decode(string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (value.Length == 0)
        {
            throw new FormatException("Radix 63404 values cannot be empty.");
        }

        var result = BigInteger.Zero;

        foreach (var digit in value)
        {
            result *= BigIntegerBase;
            result += GetDigitValue(digit);
        }

        return result;
    }

    public static bool TryDecode(string? value, out BigInteger decodedValue)
    {
        decodedValue = BigInteger.Zero;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            decodedValue = Decode(value);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
