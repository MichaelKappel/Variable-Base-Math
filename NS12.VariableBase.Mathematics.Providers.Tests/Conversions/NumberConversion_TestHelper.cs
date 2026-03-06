using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS12.VariableBase.Mathematics.Common.Interfaces;
using NS12.VariableBase.Mathematics.Providers.MathEnvironments;

namespace NS12.VariableBase.Mathematics.Providers.Tests;

internal static class NumberConversion_TestHelper
{
    public static IEnumerable<object[]> GetCases(string sourceName, string targetName)
    {
        string[,] cases =
        {
            { "zero", "0", "false", "", "" },
            { "seventeen", "17", "false", "", "" },
            { "negative-whole", "11", "true", "", "" },
            { "simple-fraction", "3", "false", "1", "2" },
            { "negative-mixed-fraction", "5", "true", "2", "3" }
        };

        for (int index = 0; index < cases.GetLength(0); index++)
        {
            yield return new object[]
            {
                sourceName,
                targetName,
                index + 1,
                cases[index, 0],
                cases[index, 1],
                bool.Parse(cases[index, 2]),
                cases[index, 3],
                cases[index, 4]
            };
        }
    }

    public static string GetDisplayName(MethodInfo _, object[] data)
    {
        string label = data[3].ToString() ?? "case";
        return $"NumberConversion_{data[0]}_to_{data[1]}_{label.Replace('-', '_')}";
    }

    public static void AssertConversion(
        string sourceName,
        string sourceKey,
        string targetName,
        string targetKey,
        int caseIndex,
        string label,
        string wholeBase10,
        bool isNegative,
        string numeratorBase10,
        string denominatorBase10)
    {
        IMathEnvironment<Number> sourceEnvironment = new CharMathEnvironment(sourceKey);
        IMathEnvironment<Number> targetEnvironment = new CharMathEnvironment(targetKey);

        string sourceWhole = ToBaseN(BigInteger.Parse(wholeBase10), sourceKey);
        string sourceNumerator = string.IsNullOrWhiteSpace(numeratorBase10) ? string.Empty : ToBaseN(BigInteger.Parse(numeratorBase10), sourceKey);
        string sourceDenominator = string.IsNullOrWhiteSpace(denominatorBase10) ? string.Empty : ToBaseN(BigInteger.Parse(denominatorBase10), sourceKey);

        Number sourceValue = sourceEnvironment.GetNumber(sourceWhole, sourceNumerator, sourceDenominator, isNegative);
        Number convertedValue = Number.Reduce(sourceValue.Convert(targetEnvironment));

        string expectedTargetWhole = ToBaseN(BigInteger.Parse(wholeBase10), targetKey);
        string expectedTargetNumerator = string.IsNullOrWhiteSpace(numeratorBase10) ? string.Empty : ToBaseN(BigInteger.Parse(numeratorBase10), targetKey);
        string expectedTargetDenominator = string.IsNullOrWhiteSpace(denominatorBase10) ? string.Empty : ToBaseN(BigInteger.Parse(denominatorBase10), targetKey);

        AssertNumber(convertedValue, targetEnvironment, expectedTargetWhole, isNegative, expectedTargetNumerator, expectedTargetDenominator, $"{sourceName}->{targetName}#{caseIndex}:{label}");

        Number roundTripValue = Number.Reduce(convertedValue.Convert(sourceEnvironment));
        Assert.AreEqual(sourceValue, roundTripValue, $"{sourceName}->{targetName}#{caseIndex}:{label}: round-trip equality");
    }

    private static void AssertNumber(
        Number actual,
        IMathEnvironment<Number> environment,
        string expectedWhole,
        bool expectedNegative,
        string expectedNumerator,
        string expectedDenominator,
        string context)
    {
        Assert.AreEqual(expectedWhole, environment.ConvertToString(actual.Whole), $"{context}: whole");
        Assert.AreEqual(expectedNegative, actual.IsNegative, $"{context}: sign");

        if (string.IsNullOrWhiteSpace(expectedNumerator) || string.IsNullOrWhiteSpace(expectedDenominator))
        {
            Assert.IsNull(actual.Fragment, $"{context}: fragment should be null");
            return;
        }

        Assert.IsNotNull(actual.Fragment, $"{context}: fragment should not be null");
        Assert.AreEqual(expectedNumerator, environment.ConvertToString(actual.Fragment!.Numerator.Whole), $"{context}: numerator");
        Assert.AreEqual(expectedDenominator, environment.ConvertToString(actual.Fragment.Denominator.Whole), $"{context}: denominator");
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
