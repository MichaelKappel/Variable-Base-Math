using NS12.VariableBase.Mathematics.Common.Interfaces;
using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Providers.MathEnvironments;

namespace NS12.VariableBase.Mathematics.Providers.Tests
{
    [TestClass]
    public class Number_Equals_Tests
    {
        [TestMethod]
        public void Equals_Number_Null_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("0123456789");
            Number one = env.GetNumber(1m);
            Assert.IsFalse(one.Equals((Number)null));
        }

        [TestMethod]
        public void Equals_Object_Null_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("0123456789");
            Number one = env.GetNumber(1m);
            Assert.IsFalse(one.Equals((object?)null));
        }

        [TestMethod]
        public void Equals_Object_InvalidType_Test()
        {
            IMathEnvironment<Number> env = new CharMathEnvironment("0123456789");
            Number one = env.GetNumber(1m);
            Assert.IsFalse(one.Equals("1"));
        }
    }
}
