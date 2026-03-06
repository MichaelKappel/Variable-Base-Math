using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS12.VariableBase.Mathematics.Common.Interfaces;
using NS12.VariableBase.Mathematics.Providers.MathEnvironments;

namespace NS12.VariableBase.Mathematics.Providers.Tests;

internal static class NumberOperator_Arithmetic_TestHelper
{
    public static IEnumerable<object[]> GetArithmeticCases(string baseName, string key)
    {
        List<(BigInteger A, BigInteger B)> values = BuildNumbers(key.Length);
        for (int index = 0; index < values.Count; index++)
        {
            yield return new object[]
            {
                baseName,
                key,
                index + 1,
                ToBaseN(values[index].A, key),
                ToBaseN(values[index].B, key)
            };
        }
    }

    public static string GetArithmeticCaseDisplayName(MethodInfo _, object[] data)
    {
        return $"NumberOperator_{data[0]}_case_{data[2]}";
    }

    public static void AssertArithmeticOperatorsAcrossBase(string baseName, string key, int caseIndex, string firstRaw, string secondRaw)
    {
        IMathEnvironment<Number> environment = new CharMathEnvironment(key);
        Number first = environment.GetNumber(firstRaw);
        Number second = environment.GetNumber(secondRaw);

        BigInteger firstValue = ParseBaseN(firstRaw, key);
        BigInteger secondValue = ParseBaseN(secondRaw, key);

        Number addition = first + second;
        AssertNumber(addition, environment, ToBaseN(firstValue + secondValue, key), false, null, null, $"{baseName}#{caseIndex}: add");

        Number subtraction = first - second;
        BigInteger subtractionMagnitude = BigInteger.Abs(firstValue - secondValue);
        bool subtractionNegative = firstValue < secondValue;
        AssertNumber(subtraction, environment, ToBaseN(subtractionMagnitude, key), subtractionNegative, null, null, $"{baseName}#{caseIndex}: subtract");

        Number multiplication = first * second;
        AssertNumber(multiplication, environment, ToBaseN(firstValue * secondValue, key), false, null, null, $"{baseName}#{caseIndex}: multiply");

        BigInteger quotient = BigInteger.DivRem(firstValue, secondValue, out BigInteger remainder);
        Number division = first / second;
        if (remainder == BigInteger.Zero)
        {
            AssertNumber(division, environment, ToBaseN(quotient, key), false, null, null, $"{baseName}#{caseIndex}: divide");
        }
        else
        {
            AssertNumber(
                division,
                environment,
                ToBaseN(quotient, key),
                false,
                ToBaseN(remainder, key),
                secondRaw,
                $"{baseName}#{caseIndex}: divide");
        }

        Number modulo = first % second;
        if (remainder == BigInteger.Zero)
        {
            AssertNumber(modulo, environment, environment.Key[0].ToString(), false, null, null, $"{baseName}#{caseIndex}: modulo");
        }
        else
        {
            AssertNumber(
                modulo,
                environment,
                environment.Key[0].ToString(),
                false,
                ToBaseN(remainder, key),
                secondRaw,
                $"{baseName}#{caseIndex}: modulo");
        }
    }

    private static List<(BigInteger A, BigInteger B)> BuildNumbers(int baseValue)
    {
        var result = new List<(BigInteger A, BigInteger B)>(50)
        {
            (new BigInteger(0), new BigInteger(1)),
            (new BigInteger(1), new BigInteger(1)),
            (new BigInteger(baseValue - 1), new BigInteger(2)),
            (new BigInteger(baseValue * baseValue + 1), new BigInteger(baseValue + 1)),
            (BigInteger.Pow(baseValue, 5) + 12345, BigInteger.Pow(baseValue, 2) + 7),
            (BigInteger.Pow(baseValue, 8) + BigInteger.Pow(baseValue, 2) + 99, BigInteger.Pow(baseValue, 3) + 13)
        };

        for (int index = 6; index < 50; index++)
        {
            int exponentA = 4 + (index % 14);
            int exponentB = 2 + ((index * 3) % 8);

            BigInteger first =
                BigInteger.Pow(baseValue, exponentA) * (index + 11)
                + (BigInteger.Parse("1234567890123456789") * ((index % 7) + 1))
                + (index * index + 3);

            BigInteger second =
                BigInteger.Pow(baseValue, exponentB) * ((index % 9) + 2)
                + (index * 37 + 5);

            if (index % 5 == 0 && first > second)
            {
                (first, second) = (second, first);
            }

            if (second == BigInteger.Zero)
            {
                second = BigInteger.One;
            }

            result.Add((first, second));
        }

        return result;
    }

    private static void AssertNumber(
        Number actual,
        IMathEnvironment<Number> environment,
        string expectedWhole,
        bool expectedNegative,
        string? expectedNumerator,
        string? expectedDenominator,
        string context)
    {
        string actualWhole = environment.ConvertToString(actual.Whole);
        Assert.AreEqual(expectedWhole, actualWhole, $"{context}: whole");
        Assert.AreEqual(expectedNegative, actual.IsNegative, $"{context}: sign");

        if (expectedNumerator == null || expectedDenominator == null)
        {
            Assert.IsNull(actual.Fragment, $"{context}: fragment should be null");
            return;
        }

        Assert.IsNotNull(actual.Fragment, $"{context}: fragment should not be null");
        string actualNumerator = environment.ConvertToString(actual.Fragment!.Numerator.Whole);
        string actualDenominator = environment.ConvertToString(actual.Fragment.Denominator.Whole);
        Assert.AreEqual(expectedNumerator, actualNumerator, $"{context}: numerator");
        Assert.AreEqual(expectedDenominator, actualDenominator, $"{context}: denominator");
    }

    private static BigInteger ParseBaseN(string raw, string key)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return BigInteger.Zero;
        }

        int radix = key.Length;
        var map = new Dictionary<char, int>(key.Length);
        for (int index = 0; index < key.Length; index++)
        {
            map[key[index]] = index;
        }

        BigInteger result = BigInteger.Zero;
        foreach (char c in raw.Trim())
        {
            result = (result * radix) + map[c];
        }

        return result;
    }

    private static string ToBaseN(BigInteger value, string key)
    {
        if (value == BigInteger.Zero)
        {
            return key[0].ToString();
        }

        int radix = key.Length;
        BigInteger remaining = value;
        var chars = new Stack<char>();
        while (remaining > BigInteger.Zero)
        {
            remaining = BigInteger.DivRem(remaining, radix, out BigInteger remainder);
            chars.Push(key[(int)remainder]);
        }

        return new string(chars.ToArray());
    }
}
