using NS12.VariableBase.Mathematics.Common.Interfaces;
using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Providers.MathEnvironments;

namespace NS12.VariableBase.Mathematics.Providers.Tests
{
    [TestClass]
    public class Number_Radix63404_Tests
    {
        [TestMethod]
        public void Radix63404_Add_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment((char)63404);
            Number result = env.GetNumber(2m) + env.GetNumber(1m);
            Assert.AreEqual(env.GetNumber(3m), result);
        }

        [TestMethod]
        public void Radix63404_Subtract_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment((char)63404);
            Number result = env.GetNumber(5m) - env.GetNumber(3m);
            Assert.AreEqual(env.GetNumber(2m), result);
        }

        [TestMethod]
        public void Radix63404_Multiply_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment((char)63404);
            Number result = env.GetNumber(3m) * env.GetNumber(2m);
            Assert.AreEqual(env.GetNumber(6m), result);
        }

        [TestMethod]
        public void Radix63404_Divide_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment((char)63404);
            Number result = env.GetNumber(6m) / env.GetNumber(2m);
            Assert.AreEqual(env.GetNumber(3m), result);
        }

        [TestMethod]
        public void Radix63404_GreaterThan_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment((char)63404);
            Assert.IsTrue(env.GetNumber(3m) > env.GetNumber(2m));
        }

        [TestMethod]
        public void Radix63404_LessThan_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment((char)63404);
            Assert.IsTrue(env.GetNumber(2m) < env.GetNumber(3m));
        }

        [TestMethod]
        public void Radix63404_Equal_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment((char)63404);
            Assert.IsTrue(env.GetNumber(4m) == env.GetNumber(4m));
        }
    }
}
