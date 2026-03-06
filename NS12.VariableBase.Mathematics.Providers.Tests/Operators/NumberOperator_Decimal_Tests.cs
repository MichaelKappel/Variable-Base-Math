using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS12.VariableBase.Mathematics.Common.Interfaces;
using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Providers.MathEnvironments;

namespace NS12.VariableBase.Mathematics.Providers.Tests;

[TestClass]
[TestCategory("NumberOperator")]
[TestCategory("Decimal")]
public class NumberOperator_Decimal_Tests
{
    private static IMathEnvironment<Number> CreateEnvironment() => new CharMathEnvironment("0123456789");

    [TestMethod]
    public void Add_ReturnsExpectedSum_ForDifferentLengthWholeNumbers()
    {
        IMathEnvironment<Number> environment = CreateEnvironment();
        Number first = environment.GetNumber("102003");
        Number second = environment.GetNumber("10330");

        Number result = first + second;

        Assert.AreEqual(environment.GetNumber("112333"), result);
    }

    [TestMethod]
    public void Add_ReturnsExpectedSum_ForLargeWholeNumbers()
    {
        IMathEnvironment<Number> environment = CreateEnvironment();
        Number first = environment.GetNumber("102003576575");
        Number second = environment.GetNumber("103303454535");

        Number result = first + second;

        Assert.AreEqual(environment.GetNumber("205307031110"), result);
    }

    [TestMethod]
    public void Divide_ThenReduce_ReturnsExpectedMixedFraction()
    {
        IMathEnvironment<Number> environment = CreateEnvironment();
        Number first = environment.GetNumber("100");
        Number second = environment.GetNumber("60");

        Number reduced = Number.Reduce(first / second);

        Assert.AreEqual(environment.GetNumber("1", "40", "60", false), reduced);
    }

    [TestMethod]
    public void Subtract_ReturnsNegativeNumber_WhenSubtrahendIsGreater()
    {
        IMathEnvironment<Number> environment = CreateEnvironment();
        Number first = environment.GetNumber("4");
        Number second = environment.GetNumber("10");

        Number result = first - second;

        Assert.AreEqual(environment.GetNumber("6", isNegative: true), result);
    }
}
