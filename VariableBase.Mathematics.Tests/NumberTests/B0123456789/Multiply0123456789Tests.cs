using VariableBase.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace VariableBase.Mathematics.Tests.NumberTests.B0123456789
{
    [TestClass]
    public class Multiply0123456789Tests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Multiply")]
        [TestCategory("0123456789")]
        public void Multiply_15_9()
        {
            var env = new DecimalMathEnvironment("0123456789");

            Number expected = env.GetNumber("135");

            Number a = env.GetNumber("15");
            Number b = env.GetNumber("9");

            Number actual = a * b;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Multiply")]
        [TestCategory("0123456789")]
        public void Multiply_98234567890876_3456789001234()
        {
            var env = new DecimalMathEnvironment("0123456789");

            Number expected = env.GetNumber("3395752623168");

            Number a = env.GetNumber("9823456");
            Number b = env.GetNumber("345678");

            Number actual = a * b;

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("Multiply")]
        [TestCategory("0123456789")]
        public void Multiply_15_1_2_X_9_1_2()
        {
            var env = new DecimalMathEnvironment("0123456789");

            var expected = env.GetNumber("147","1", "4");

            var a = env.GetNumber("15","1", "2");

            var b = env.GetNumber("9","1", "2");

            Number actual = a * b;

            Assert.AreEqual(expected, actual);
        }

    }
} 