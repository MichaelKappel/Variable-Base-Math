using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS12.VariableBase.Mathematics.Common.Interfaces;
using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Providers.MathEnvironments;

namespace NS12.VariableBase.Mathematics.Providers.Tests;

[TestClass]
[TestCategory("CharMathEnvironment")]
[TestCategory("Binary")]
public class CharMathEnvironment_Binary_Tests
{
    private static IMathEnvironment<Number> CreateEnvironment() => new CharMathEnvironment("01");

    [TestMethod]
    public void IsZero_ReturnsTrue_ForEmptyInput()
    {
        Assert.IsTrue(CreateEnvironment().IsZero(string.Empty));
    }

    [TestMethod]
    public void IsZero_ReturnsTrue_WhenAllDigitsAreZero()
    {
        Assert.IsTrue(CreateEnvironment().IsZero("0000"));
    }

    [TestMethod]
    public void IsZero_ReturnsFalse_WhenAnyDigitIsNonZero()
    {
        Assert.IsFalse(CreateEnvironment().IsZero("010"));
    }

    [TestMethod]
    public void IsOne_ReturnsTrue_ForCanonicalOne()
    {
        Assert.IsTrue(CreateEnvironment().IsOne("1"));
    }

    [TestMethod]
    public void IsOne_ReturnsTrue_WhenOneHasLeadingZeros()
    {
        Assert.IsTrue(CreateEnvironment().IsOne("0001"));
    }

    [TestMethod]
    public void IsOne_ReturnsFalse_ForValuesGreaterThanOne()
    {
        Assert.IsFalse(CreateEnvironment().IsOne("11"));
    }

    [TestMethod]
    public void Add_ReturnsExpectedBinarySum()
    {
        IMathEnvironment<Number> environment = CreateEnvironment();
        Number first = environment.GetNumber("101");
        Number second = environment.GetNumber("1");

        Number result = first + second;

        Assert.AreEqual("110", result.ToString());
    }
}
