using NS12.VariableBase.Mathematics.Common.Interfaces;
using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Providers.MathEnvironments;

namespace NS12.VariableBase.Mathematics.Providers.Tests
{
    [TestClass]
    public class Number_decimal_tests
    {
        [TestMethod]
        public void TestMethod1()
        {
            IMathEnvironment<Number> mathEnvironment = new CharMathEnvironment("0123456789");

            var firstWholeNumber = "102003";
            var secondtWholeNumber = "10330";

            var firstNumber = mathEnvironment.GetNumber(firstWholeNumber);
            var secondNumber = mathEnvironment.GetNumber(secondtWholeNumber);

            Number result = firstNumber + secondNumber;

            Assert.AreEqual(result, "112333");
        }
    }
}