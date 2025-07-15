using NS12.VariableBase.Mathematics.Common.Interfaces;
using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Providers.MathEnvironments;

namespace NS12.VariableBase.Mathematics.Providers.Tests
{
    [TestClass]
    public class CharMathEnvironment_Additional_Tests
    {
        [TestMethod]
        public void Environment_Equals_SameKey_Test()
        {
            IMathEnvironment<Number> env1 = new CharMathEnvironment("0123456789");
            IMathEnvironment<Number> env2 = new CharMathEnvironment("0123456789");

            Assert.IsTrue(env1.Equals(env2));
            Assert.IsTrue(env2.Equals(env1));
        }

        [TestMethod]
        public void Environment_Definition_RoundTrip_Test()
        {
            IMathEnvironment<Number> env1 = new CharMathEnvironment("abcde");
            string def = env1.GetDefinition();
            IMathEnvironment<Number> env2 = new CharMathEnvironment(def);

            Assert.IsTrue(env1.Equals(env2));
        }

        [TestMethod]
        public void Parse_Convert_RoundTrip_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("0123456789");
            string raw = "1234567890";
            var segments = env.ParseNumberSegments(raw);
            string converted = env.ConvertToString(segments);
            Assert.AreEqual(raw, converted);
        }

        [TestMethod]
        public void GetNumber_Decimal_RoundTrip_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("0123456789");
            Number number = env.GetNumber(987654321m);
            Assert.AreEqual("987654321", number.ToString());
        }

        [TestMethod]
        public void IsZero_EmptyString_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("01");
            Assert.IsTrue(env.IsZero(""));
        }

        [TestMethod]
        public void IsZero_AllZeros_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("01");
            Assert.IsTrue(env.IsZero("0000"));
        }

        [TestMethod]
        public void IsZero_NotZero_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("01");
            Assert.IsFalse(env.IsZero("010"));
        }

        [TestMethod]
        public void IsOne_SingleOne_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("01");
            Assert.IsTrue(env.IsOne("1"));
        }

        [TestMethod]
        public void IsOne_WithLeadingZeros_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("01");
            Assert.IsTrue(env.IsOne("0001"));
        }

        [TestMethod]
        public void IsOne_False_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("01");
            Assert.IsFalse(env.IsOne("11"));
        }

        [TestMethod]
        public void Add_Binary_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("01");
            Number a = env.GetNumber("101");
            Number b = env.GetNumber("1");
            Number result = a + b;
            Assert.AreEqual("110", result.ToString());
        }

        [TestMethod]
        public void Multiply_Decimal_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("0123456789");
            Number seven = env.GetNumber("7");
            Number six = env.GetNumber("6");
            Number result = seven * six;
            Assert.AreEqual("42", result.ToString());
        }

        [TestMethod]
        public void Divide_Decimal_WithRemainder_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("0123456789");
            Number seven = env.GetNumber("7");
            Number two = env.GetNumber("2");
            Number result = seven / two;
            Assert.AreEqual("3 1/2", result.ToString());
        }

        [TestMethod]
        public void Modulo_Decimal_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("0123456789");
            Number a = env.GetNumber("7");
            Number b = env.GetNumber("5");
            Number result = a % b;
            Assert.AreEqual("2", result.ToString());
        }

        [TestMethod]
        public void Number_Comparison_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("0123456789");
            Number two = env.GetNumber("2");
            Number three = env.GetNumber("3");
            Assert.IsTrue(two < three);
            Assert.IsTrue(three > two);
            Assert.IsTrue(three >= three);
            Assert.IsTrue(two <= two);
        }
    }
}
