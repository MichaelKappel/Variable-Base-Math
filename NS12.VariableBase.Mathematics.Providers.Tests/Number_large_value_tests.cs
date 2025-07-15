using NS12.VariableBase.Mathematics.Common.Interfaces;
using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Providers.MathEnvironments;
using System.Collections.Generic;

namespace NS12.VariableBase.Mathematics.Providers.Tests
{
    [TestClass]
    public class Number_Large_Value_Tests
    {
        public static IEnumerable<object[]> Environments()
        {
            yield return new object[] { new CharMathEnvironment("01") };
            yield return new object[] { new CharMathEnvironment("012") };
            yield return new object[] { new CharMathEnvironment("01234") };
            yield return new object[] { new CharMathEnvironment("012345") };
            yield return new object[] { new CharMathEnvironment("01234567") };
            yield return new object[] { new CharMathEnvironment("0123456789") };
            yield return new object[] { new CharMathEnvironment("0123456789ABCDEF") };
            yield return new object[] { new CharMathEnvironment("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ") };
            yield return new object[] { new CharMathEnvironment((char)63404) };
        }

        [DataTestMethod]
        [DynamicData(nameof(Environments), DynamicDataSourceType.Method)]
        public void Large_Add_Test(IMathEnvironment<Number> env)
        {
            Number result = env.GetNumber(123456789012345678m) + env.GetNumber(987654321098765432m);
            Assert.AreEqual(env.GetNumber(1111111110111111110m), result);
        }

        [DataTestMethod]
        [DynamicData(nameof(Environments), DynamicDataSourceType.Method)]
        public void Large_Subtract_Test(IMathEnvironment<Number> env)
        {
            Number result = env.GetNumber(5000000000000000000m) - env.GetNumber(3000000000000000000m);
            Assert.AreEqual(env.GetNumber(2000000000000000000m), result);
        }

        [DataTestMethod]
        [DynamicData(nameof(Environments), DynamicDataSourceType.Method)]
        public void Large_Multiply_Test(IMathEnvironment<Number> env)
        {
            Number result = env.GetNumber(12345678m) * env.GetNumber(98765432m);
            Assert.AreEqual(env.GetNumber(1219326221002896m), result);
        }

        [DataTestMethod]
        [DynamicData(nameof(Environments), DynamicDataSourceType.Method)]
        public void Large_Divide_Test(IMathEnvironment<Number> env)
        {
            Number result = env.GetNumber(999999999999999999m) / env.GetNumber(3m);
            Assert.AreEqual(env.GetNumber(333333333333333333m), result);
        }

        [DataTestMethod]
        [DynamicData(nameof(Environments), DynamicDataSourceType.Method)]
        public void Large_GreaterThan_Test(IMathEnvironment<Number> env)
        {
            Assert.IsTrue(env.GetNumber(2000000000000000000m) > env.GetNumber(1000000000000000000m));
        }

        [DataTestMethod]
        [DynamicData(nameof(Environments), DynamicDataSourceType.Method)]
        public void Large_LessThan_Test(IMathEnvironment<Number> env)
        {
            Assert.IsTrue(env.GetNumber(1000000000000000000m) < env.GetNumber(2000000000000000000m));
        }

        [DataTestMethod]
        [DynamicData(nameof(Environments), DynamicDataSourceType.Method)]
        public void Large_Equal_Test(IMathEnvironment<Number> env)
        {
            Assert.IsTrue(env.GetNumber(4000000000000000000m) == env.GetNumber(4000000000000000000m));
        }
    }
}
