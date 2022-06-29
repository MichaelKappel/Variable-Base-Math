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
            var mathEnvironment = new CharMathEnvironment("0123456789");
            var firstWholeNumber = new NumberSegments("102003");
            var secondtWholeNumber = new NumberSegments("10330");

            var firstNumber = new Number(mathEnvironment, firstWholeNumber);
            var secondNumber = new Number(mathEnvironment, secondtWholeNumber);

            Number result = firstNumber + secondNumber;

            Assert.AreEqual(result, "112333");

        }
    }
}