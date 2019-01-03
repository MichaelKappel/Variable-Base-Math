using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Math.Tests.B0123456789
{
    [TestClass]
    public class Divide0123456789Tests
    {
        [TestMethod]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void Divide_160_40()
        {
            var env = new MathEnvironment("0123456789");

            var expected = env.GetNumber("4");

            var a = env.GetNumber("160");
            var b = env.GetNumber("40");

            Number actual = a / b;

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void Divide_9876_99()
        {
            var env = new MathEnvironment("0123456789");

            var expected = env.GetNumber("99","75", "99");

            var a = env.GetNumber("9876");
            var b = env.GetNumber("99");

            Number actual = a / b;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void Divide_34798249_40()
        {
            var env = new MathEnvironment("0123456789");

            var expected = env.GetNumber("869956","9", "40");

            var a = env.GetNumber("34798249");
            var b = env.GetNumber("40");

            Number actual = a / b;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void Divide_15789_2_3_D_9_7_8()
        {
            var env = new MathEnvironment("0123456789");

            var expected = env.GetNumber("1754","1", "3");

            var a = env.GetNumber("15789","2", "3");
            var b = env.GetNumber("9","7", "8");

            Number actual = a / b;

            Assert.AreEqual(expected, actual);
        }

    }
} 