using NS12.VariableBase.Mathematics.Common.Interfaces;
using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Providers.MathEnvironments;

namespace NS12.VariableBase.Mathematics.Providers.Tests
{
    [TestClass]
    public class Add_Decimal_Tests
    {
        [TestMethod]
        public void Add_Decimal_1_Test()
        {
            IMathEnvironment<Number> mathEnvironment = new CharMathEnvironment("0123456789");

            var firstWholeNumber = "102003";
            var secondtWholeNumber = "10330";

            var firstNumber = mathEnvironment.GetNumber(firstWholeNumber);
            var secondNumber = mathEnvironment.GetNumber(secondtWholeNumber);

            Number result = firstNumber + secondNumber;

            Assert.AreEqual(result, mathEnvironment.GetNumber("112333"));
        }

        [TestMethod]
        public void Add_Decimal_2_Test()
        {
            IMathEnvironment<Number> mathEnvironment = new CharMathEnvironment("0123456789");

            var firstWholeNumber = "102003576575";
            var secondtWholeNumber = "103303454535";

            var firstNumber = mathEnvironment.GetNumber(firstWholeNumber);
            var secondNumber = mathEnvironment.GetNumber(secondtWholeNumber);

            Number result = firstNumber + secondNumber;

            Assert.AreEqual(result, mathEnvironment.GetNumber("205307031110"));
        }
    }
}