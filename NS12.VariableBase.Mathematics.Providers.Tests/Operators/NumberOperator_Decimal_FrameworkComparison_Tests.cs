using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS12.VariableBase.Mathematics.Common.Interfaces;
using NS12.VariableBase.Mathematics.Providers.MathEnvironments;

namespace NS12.VariableBase.Mathematics.Providers.Tests;

[TestClass]
[TestCategory("NumberOperator")]
[TestCategory("Decimal")]
public class NumberOperator_Decimal_FrameworkComparison_Tests
{
    private static readonly IMathEnvironment<Number> Environment = new CharMathEnvironment("0123456789");

    [DataTestMethod]
    [DynamicData(nameof(GetInt64WholeCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetInt64WholeCaseDisplayName))]
    public void WholeNumberOperators_MatchInt64Arithmetic(long leftValue, long rightValue)
    {
        AssertWholeNumberOperators(
            BigInteger.Parse(leftValue.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture),
            BigInteger.Parse(rightValue.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture),
            $"int64:{leftValue}:{rightValue}");
    }

    [DataTestMethod]
    [DynamicData(nameof(GetBigIntegerWholeCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetBigIntegerWholeCaseDisplayName))]
    public void WholeNumberOperators_MatchBigIntegerArithmetic(string leftRaw, string rightRaw)
    {
        AssertWholeNumberOperators(
            BigInteger.Parse(leftRaw, CultureInfo.InvariantCulture),
            BigInteger.Parse(rightRaw, CultureInfo.InvariantCulture),
            $"bigint:{leftRaw}:{rightRaw}");
    }

    [DataTestMethod]
    [DynamicData(nameof(GetExactDecimalAdditionCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetExactDecimalCaseDisplayName))]
    public void Add_WithExactDecimalFractions_MatchesDecimal(
        string leftWhole,
        string leftNumerator,
        string leftDenominator,
        string rightWhole,
        string rightNumerator,
        string rightDenominator,
        string expectedDecimal)
    {
        Number left = CreateNumber(leftWhole, leftNumerator, leftDenominator);
        Number right = CreateNumber(rightWhole, rightNumerator, rightDenominator);

        Number actual = left + right;

        Assert.AreEqual(ParseDecimal(expectedDecimal), ToDecimal(actual), $"addition actual={DescribeNumber(actual)}");
    }

    [DataTestMethod]
    [DynamicData(nameof(GetExactDecimalSubtractionCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetExactDecimalCaseDisplayName))]
    public void Subtract_WithExactDecimalFractions_MatchesDecimal(
        string leftWhole,
        string leftNumerator,
        string leftDenominator,
        string rightWhole,
        string rightNumerator,
        string rightDenominator,
        string expectedDecimal)
    {
        Number left = CreateNumber(leftWhole, leftNumerator, leftDenominator);
        Number right = CreateNumber(rightWhole, rightNumerator, rightDenominator);

        Number actual = left - right;

        Assert.AreEqual(ParseDecimal(expectedDecimal), ToDecimal(actual), $"subtraction actual={DescribeNumber(actual)}");
    }

    [DataTestMethod]
    [DynamicData(nameof(GetExactDecimalMultiplicationCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetExactDecimalCaseDisplayName))]
    public void Multiply_WithExactDecimalFractions_MatchesDecimal(
        string leftWhole,
        string leftNumerator,
        string leftDenominator,
        string rightWhole,
        string rightNumerator,
        string rightDenominator,
        string expectedDecimal)
    {
        Number left = CreateNumber(leftWhole, leftNumerator, leftDenominator);
        Number right = CreateNumber(rightWhole, rightNumerator, rightDenominator);

        Number actual = left * right;

        Assert.AreEqual(ParseDecimal(expectedDecimal), ToDecimal(actual), $"multiplication actual={DescribeNumber(actual)}");
    }

    [DataTestMethod]
    [DynamicData(nameof(GetExactDecimalDivisionCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetExactDecimalCaseDisplayName))]
    public void Divide_WithExactDecimalFractions_MatchesDecimal(
        string leftWhole,
        string leftNumerator,
        string leftDenominator,
        string rightWhole,
        string rightNumerator,
        string rightDenominator,
        string expectedDecimal)
    {
        Number left = CreateNumber(leftWhole, leftNumerator, leftDenominator);
        Number right = CreateNumber(rightWhole, rightNumerator, rightDenominator);

        Number actual = left / right;

        Assert.AreEqual(ParseDecimal(expectedDecimal), ToDecimal(actual), $"division actual={DescribeNumber(actual)}");
    }

    public static IEnumerable<object[]> GetInt64WholeCases()
    {
        long[] leftValues =
        {
            0L,
            1L,
            2L,
            7L,
            10L,
            99L,
            1234L,
            123456789L,
            9876543210123L,
            900719925474099L
        };

        long[] rightValues =
        {
            1L,
            2L,
            3L,
            5L,
            9L,
            10L,
            37L,
            4567L,
            7654321L,
            123456789L
        };

        for (int index = 0; index < leftValues.Length; index++)
        {
            yield return new object[] { leftValues[index], rightValues[index] };
        }

        yield return new object[] { 42L, 999L };
        yield return new object[] { 5000L, 5000L };
        yield return new object[] { 1000000L, 64L };
        yield return new object[] { 9223372036854775L, 97L };
        yield return new object[] { 3141592653589793L, 2718281L };
    }

    public static string GetInt64WholeCaseDisplayName(MethodInfo _, object[] data)
    {
        return $"Decimal_int64_{data[0]}_{data[1]}";
    }

    public static IEnumerable<object[]> GetBigIntegerWholeCases()
    {
        yield return new object[] { "1234567890123456789012345678901234567890", "9" };
        yield return new object[] { "999999999999999999999999999999999999999", "12345678901234567890" };
        yield return new object[] { "10000000000000000000000000000000000000000000012345", "54321" };
        yield return new object[] { "31415926535897932384626433832795028841971693993751", "2718281828459045235360287" };
        yield return new object[] { "1844674407370955161618446744073709551618", "4294967297" };
        yield return new object[] { "88888888888888888888888888888888888888888888", "7777777777777777777" };
        yield return new object[] { "10000000000000000000000000000000000000000001", "1000000000000003" };
        yield return new object[] { "12345678901234567890123456789012345678901234567890", "98765432109876543210987654321" };
        yield return new object[] { "99999999999999999999999999999999999999999999999999", "11111111111111111111111111111111111111" };
        yield return new object[] { "1000000000000000000000000000000000000000000000000000001", "25" };
    }

    public static string GetBigIntegerWholeCaseDisplayName(MethodInfo _, object[] data)
    {
        return $"Decimal_bigint_{data[0]}_{data[1]}";
    }

    public static IEnumerable<object[]> GetExactDecimalAdditionCases()
    {
        yield return new object[] { "0", "1", "2", "0", "1", "4", "0.75" };
        yield return new object[] { "12", "3", "10", "7", "7", "20", "19.65" };
        yield return new object[] { "5", "1", "8", "2", "7", "8", "8" };
        yield return new object[] { "100", "5", "100", "200", "95", "100", "301" };
    }

    public static IEnumerable<object[]> GetExactDecimalSubtractionCases()
    {
        yield return new object[] { "5", "3", "8", "2", "1", "8", "3.25" };
        yield return new object[] { "9", "9", "10", "4", "4", "10", "5.5" };

        yield return new object[] { "20", "5", "20", "3", "10", "20", "16.75" };
    }

    public static IEnumerable<object[]> GetExactDecimalMultiplicationCases()
    {
        yield return new object[] { "0", "1", "5", "0", "3", "8", "0.075" };
        yield return new object[] { "7", "1", "2", "2", "2", "5", "18" };
        yield return new object[] { "1", "25", "100", "0", "4", "5", "1" };
        yield return new object[] { "3", "3", "20", "4", "5", "10", "14.175" };
    }

    public static IEnumerable<object[]> GetExactDecimalDivisionCases()
    {
        yield return new object[] { "0", "3", "4", "0", "1", "8", "6" };
        yield return new object[] { "2", "1", "2", "0", "4", "5", "3.125" };
        yield return new object[] { "9", "9", "10", "3", "3", "10", "3" };
        yield return new object[] { "1", "1", "4", "0", "5", "10", "2.5" };
    }

    public static string GetExactDecimalCaseDisplayName(MethodInfo _, object[] data)
    {
        return $"Decimal_fraction_{data[0]}_{data[1]}_{data[2]}__{data[3]}_{data[4]}_{data[5]}";
    }

    private static void AssertWholeNumberOperators(BigInteger leftValue, BigInteger rightValue, string context)
    {
        Number left = Environment.GetNumber(leftValue.ToString(CultureInfo.InvariantCulture));
        Number right = Environment.GetNumber(rightValue.ToString(CultureInfo.InvariantCulture));

        AssertWholeNumber(left + right, leftValue + rightValue, $"{context}:add");
        AssertWholeNumber(left - right, leftValue - rightValue, $"{context}:subtract");
        AssertWholeNumber(left * right, leftValue * rightValue, $"{context}:multiply");

        BigInteger quotient = BigInteger.DivRem(leftValue, rightValue, out BigInteger remainder);
        AssertDivisionNumber(left / right, quotient, remainder, rightValue, $"{context}:divide");
        AssertModuloNumber(left % right, remainder, rightValue, $"{context}:modulo");
    }

    private static Number CreateNumber(string whole, string numerator, string denominator)
    {
        if (string.IsNullOrWhiteSpace(numerator) || string.IsNullOrWhiteSpace(denominator))
        {
            return Environment.GetNumber(whole);
        }

        return Environment.GetNumber(whole, numerator, denominator);
    }

    private static void AssertWholeNumber(Number actual, BigInteger expected, string context)
    {
        BigInteger expectedMagnitude = BigInteger.Abs(expected);
        string actualWhole = Environment.ConvertToString(actual.Whole);

        Assert.AreEqual(expectedMagnitude.ToString(CultureInfo.InvariantCulture), actualWhole, $"{context}: whole");
        Assert.AreEqual(expected.Sign < 0, actual.IsNegative, $"{context}: sign");
        Assert.IsNull(actual.Fragment, $"{context}: fragment");
    }

    private static void AssertDivisionNumber(Number actual, BigInteger expectedWhole, BigInteger expectedRemainder, BigInteger divisor, string context)
    {
        string actualWhole = Environment.ConvertToString(actual.Whole);
        Assert.AreEqual(expectedWhole.ToString(CultureInfo.InvariantCulture), actualWhole, $"{context}: whole");
        Assert.IsFalse(actual.IsNegative, $"{context}: sign");

        if (expectedRemainder == BigInteger.Zero)
        {
            Assert.IsNull(actual.Fragment, $"{context}: fragment");
            return;
        }

        Assert.IsNotNull(actual.Fragment, $"{context}: fragment");
        Assert.AreEqual(expectedRemainder.ToString(CultureInfo.InvariantCulture), Environment.ConvertToString(actual.Fragment!.Numerator.Whole), $"{context}: numerator");
        Assert.AreEqual(divisor.ToString(CultureInfo.InvariantCulture), Environment.ConvertToString(actual.Fragment.Denominator.Whole), $"{context}: denominator");
    }

    private static void AssertModuloNumber(Number actual, BigInteger expectedRemainder, BigInteger divisor, string context)
    {
        Assert.AreEqual("0", Environment.ConvertToString(actual.Whole), $"{context}: whole");
        Assert.IsFalse(actual.IsNegative, $"{context}: sign");

        if (expectedRemainder == BigInteger.Zero)
        {
            Assert.IsNull(actual.Fragment, $"{context}: fragment");
            return;
        }

        Assert.IsNotNull(actual.Fragment, $"{context}: fragment");
        Assert.AreEqual(expectedRemainder.ToString(CultureInfo.InvariantCulture), Environment.ConvertToString(actual.Fragment!.Numerator.Whole), $"{context}: numerator");
        Assert.AreEqual(divisor.ToString(CultureInfo.InvariantCulture), Environment.ConvertToString(actual.Fragment.Denominator.Whole), $"{context}: denominator");
    }

    private static decimal ToDecimal(Number number)
    {
        decimal whole = ParseDecimal(Environment.ConvertToString(number.Whole));
        decimal value = whole;

        if (number.Fragment != null)
        {
            decimal numerator = ParseDecimal(Environment.ConvertToString(number.Fragment.Numerator.Whole));
            decimal denominator = ParseDecimal(Environment.ConvertToString(number.Fragment.Denominator.Whole));
            value += numerator / denominator;
        }

        return number.IsNegative ? -value : value;
    }

    private static decimal ParseDecimal(string raw)
    {
        return decimal.Parse(raw, NumberStyles.Number, CultureInfo.InvariantCulture);
    }

    private static string DescribeNumber(Number number)
    {
        string whole = Environment.ConvertToString(number.Whole);
        string fragment = number.Fragment == null
            ? "null"
            : $"{Environment.ConvertToString(number.Fragment.Numerator.Whole)}/{Environment.ConvertToString(number.Fragment.Denominator.Whole)}";

        return $"whole={whole}, fragment={fragment}, negative={number.IsNegative}";
    }
}