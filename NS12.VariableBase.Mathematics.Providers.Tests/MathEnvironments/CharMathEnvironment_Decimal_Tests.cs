using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS12.VariableBase.Mathematics.Common.Interfaces;
using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Providers.MathEnvironments;

namespace NS12.VariableBase.Mathematics.Providers.Tests;

[TestClass]
[TestCategory("CharMathEnvironment")]
[TestCategory("Decimal")]
public class CharMathEnvironment_Decimal_Tests
{
    private static IMathEnvironment<Number> CreateEnvironment() => new CharMathEnvironment("0123456789");

    [TestMethod]
    public void GetNumber_FromDecimalValue_RoundTripsToString()
    {
        Number number = CreateEnvironment().GetNumber(987654321m);

        Assert.AreEqual("987654321", number.ToString());
    }

    [TestMethod]
    public void Multiply_ReturnsExpectedDecimalProduct()
    {
        IMathEnvironment<Number> environment = CreateEnvironment();
        Number seven = environment.GetNumber("7");
        Number six = environment.GetNumber("6");

        Number result = seven * six;

        Assert.AreEqual("42", result.ToString());
    }

    [TestMethod]
    public void Divide_WithRemainder_ReturnsMixedFraction()
    {
        IMathEnvironment<Number> environment = CreateEnvironment();
        Number seven = environment.GetNumber("7");
        Number two = environment.GetNumber("2");

        Number result = seven / two;

        Assert.AreEqual("3 1/2", result.ToString());
    }

    [TestMethod]
    public void Modulo_ReturnsFractionalRemainderRepresentation()
    {
        IMathEnvironment<Number> environment = CreateEnvironment();
        Number seven = environment.GetNumber("7");
        Number five = environment.GetNumber("5");

        Number result = seven % five;

        Assert.AreEqual("0 2/5", result.ToString());
    }

    [TestMethod]
    public void ComparisonOperators_OrderValuesCorrectly()
    {
        IMathEnvironment<Number> environment = CreateEnvironment();
        Number two = environment.GetNumber("2");
        Number three = environment.GetNumber("3");

        Assert.IsTrue(two < three);
        Assert.IsTrue(three > two);
        Assert.IsTrue(three >= two);
        Assert.IsTrue(two <= three);
    }
}
