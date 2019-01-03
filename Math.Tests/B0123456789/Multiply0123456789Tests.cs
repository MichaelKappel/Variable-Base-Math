using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Math.Tests.B0123456789
{
    [TestClass]
    public class Multiply0123456789Tests
    {
        [TestMethod]
        [TestCategory("Multiply")]
        [TestCategory("0123456789")]
        public void Multiply_15_9()
        {
            var env = new MathEnvironment("0123456789");

            var expected = env.GetNumber("135");

            var a = env.GetNumber("15");
            var b = env.GetNumber("9");

            Number actual = a * b;

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("Multiply")]
        [TestCategory("0123456789")]
        public void Multiply_15_1_2_X_9_1_2()
        {
            var env = new MathEnvironment("0123456789");

            var expected = env.GetNumber("147","1", "4");

            var a = env.GetNumber("15","1", "2");

            var b = env.GetNumber("9","1", "2");

            Number actual = a * b;

            Assert.AreEqual(expected, actual);
        }

    }
} 