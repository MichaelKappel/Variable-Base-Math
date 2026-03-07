using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS12.VariableBase.Mathematics.Common.Interfaces;
using NS12.VariableBase.Mathematics.Providers.MathEnvironments;
using NS12.VariableBase.Mathematics.Providers.Utilities;

namespace NS12.VariableBase.Mathematics.Providers.Tests;

[TestClass]
[TestCategory("CharMathEnvironment")]
[TestCategory("Radix63404")]
public class CharMathEnvironment_Radix63404_Tests
{
    private static readonly IMathEnvironment<Number> Environment = new CharMathEnvironment(UnicodeRadixKeyProvider.Radix63404Key);

    [TestMethod]
    public void Definition_ContainsExpectedNumberOfSingleCharacterDigits()
    {
        Assert.AreEqual(UnicodeRadixKeyProvider.Radix63404, UnicodeRadixKeyProvider.Radix63404Key.Length);
        Assert.AreEqual(UnicodeRadixKeyProvider.Radix63404, Environment.Key.Count);
    }

    [TestMethod]
    public void Definition_ExcludesWhitespace_ControlCharacters_AndSurrogates()
    {
        Assert.IsFalse(UnicodeRadixKeyProvider.Radix63404Key.Any(char.IsWhiteSpace));
        Assert.IsFalse(UnicodeRadixKeyProvider.Radix63404Key.Any(char.IsControl));
        Assert.IsFalse(UnicodeRadixKeyProvider.Radix63404Key.Any(char.IsSurrogate));
    }

    [TestMethod]
    public void HistoricalExamples_MatchGeneratedProtocol5Pages()
    {
        Assert.AreEqual("\u0022", Environment.ConvertToString(Environment.GetNumber(1m).Whole));
        Assert.AreEqual("#", Environment.ConvertToString(Environment.GetNumber(2m).Whole));
        Assert.AreEqual(">", Environment.ConvertToString(Environment.GetNumber(29m).Whole));
        Assert.AreEqual("\u0260", Environment.ConvertToString(Environment.GetNumber(541m).Whole));
        Assert.AreEqual("\u1F33", Environment.ConvertToString(Environment.GetNumber(7919m).Whole));
    }

    [TestMethod]
    public void ParseNumberSegments_CanRoundTrip_Radix63404Digits()
    {
        string raw = "\u0022#>\u0260\u1F33";

        var segments = Environment.ParseNumberSegments(raw);
        string converted = Environment.ConvertToString(segments);

        Assert.AreEqual(raw, converted);
    }
}
