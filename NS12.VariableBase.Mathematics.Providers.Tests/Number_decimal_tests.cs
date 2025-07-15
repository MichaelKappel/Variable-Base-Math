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



        [TestMethod]
        public void Divide_Decimal_1_Reduce_Test()
        {
            IMathEnvironment<Number> mathEnvironment = new CharMathEnvironment("0123456789");

            var firstWholeNumber = "100";
            var secondtWholeNumber = "60";

            var firstNumber = mathEnvironment.GetNumber(firstWholeNumber);
            var secondNumber = mathEnvironment.GetNumber(secondtWholeNumber);

            Number result = firstNumber / secondNumber;
            result = Number.Reduce(result);

            Assert.AreEqual(result, mathEnvironment.GetNumber("1", "40", "60", false));
        }

        [TestMethod]
        public void Subtract_Decimal_1_Negative_Test()
        {
            IMathEnvironment<Number> mathEnvironment = new CharMathEnvironment("0123456789");

            var firstWholeNumber = "4";
            var secondtWholeNumber = "10";

            var firstNumber = mathEnvironment.GetNumber(firstWholeNumber);
            var secondNumber = mathEnvironment.GetNumber(secondtWholeNumber);

            Number result = firstNumber - secondNumber;

            Assert.AreEqual(result, mathEnvironment.GetNumber("6", null, null, true));
        }

        [TestMethod]
        public void Environment_Equals_KeyMismatch_Test()
        {
            IMathEnvironment<Number> env1 = new CharMathEnvironment("0123");
            IMathEnvironment<Number> env2 = new CharMathEnvironment("0124");

            Assert.IsFalse(env1.Equals(env2));
            Assert.IsFalse(env2.Equals(env1));
        }

    }
}