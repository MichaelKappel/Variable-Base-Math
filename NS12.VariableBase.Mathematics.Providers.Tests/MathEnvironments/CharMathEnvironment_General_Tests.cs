using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS12.VariableBase.Mathematics.Common.Interfaces;
using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Providers.MathEnvironments;

namespace NS12.VariableBase.Mathematics.Providers.Tests;

[TestClass]
[TestCategory("CharMathEnvironment")]
[TestCategory("General")]
public class CharMathEnvironment_General_Tests
{
    [TestMethod]
    public void Equals_ReturnsTrue_WhenKeyDefinitionsMatch()
    {
        IMathEnvironment<Number> first = new CharMathEnvironment("0123456789");
        IMathEnvironment<Number> second = new CharMathEnvironment("0123456789");

        Assert.IsTrue(first.Equals(second));
        Assert.IsTrue(second.Equals(first));
    }

    [TestMethod]
    public void Equals_ReturnsFalse_WhenKeyDefinitionsDiffer()
    {
        IMathEnvironment<Number> first = new CharMathEnvironment("0123");
        IMathEnvironment<Number> second = new CharMathEnvironment("0124");

        Assert.IsFalse(first.Equals(second));
        Assert.IsFalse(second.Equals(first));
    }

    [TestMethod]
    public void GetDefinition_CanRoundTripIntoEquivalentEnvironment()
    {
        IMathEnvironment<Number> original = new CharMathEnvironment("abcde");
        string definition = original.GetDefinition();
        IMathEnvironment<Number> roundTripped = new CharMathEnvironment(definition);

        Assert.IsTrue(original.Equals(roundTripped));
    }

    [TestMethod]
    public void ParseNumberSegments_CanRoundTripThroughConvertToString()
    {
        IMathEnvironment<Number> environment = new CharMathEnvironment("0123456789");
        string raw = "1234567890";

        NumberSegments segments = environment.ParseNumberSegments(raw);
        string converted = environment.ConvertToString(segments);

        Assert.AreEqual(raw, converted);
    }
}
