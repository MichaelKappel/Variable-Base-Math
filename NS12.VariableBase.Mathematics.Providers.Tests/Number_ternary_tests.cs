using NS12.VariableBase.Mathematics.Common.Interfaces;
using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Providers.MathEnvironments;

namespace NS12.VariableBase.Mathematics.Providers.Tests
{
    [TestClass]
    public class Number_Ternary_Tests
    {
        [TestMethod]
        public void Ternary_Add_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("012");
            Number result = env.GetNumber(2m) + env.GetNumber(1m);
            Assert.AreEqual(env.GetNumber(3m), result);
        }

        [TestMethod]
        public void Ternary_Subtract_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("012");
            Number result = env.GetNumber(5m) - env.GetNumber(3m);
            Assert.AreEqual(env.GetNumber(2m), result);
        }

        [TestMethod]
        public void Ternary_Multiply_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("012");
            Number result = env.GetNumber(3m) * env.GetNumber(2m);
            Assert.AreEqual(env.GetNumber(6m), result);
        }

        [TestMethod]
        public void Ternary_Divide_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("012");
            Number result = env.GetNumber(6m) / env.GetNumber(2m);
            Assert.AreEqual(env.GetNumber(3m), result);
        }

        [TestMethod]
        public void Ternary_GreaterThan_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("012");
            Assert.IsTrue(env.GetNumber(3m) > env.GetNumber(2m));
        }

        [TestMethod]
        public void Ternary_LessThan_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("012");
            Assert.IsTrue(env.GetNumber(2m) < env.GetNumber(3m));
        }

        [TestMethod]
        public void Ternary_Equal_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("012");
            Assert.IsTrue(env.GetNumber(4m) == env.GetNumber(4m));
        }
    }
}
