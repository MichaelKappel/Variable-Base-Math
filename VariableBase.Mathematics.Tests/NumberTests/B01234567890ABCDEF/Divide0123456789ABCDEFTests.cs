using VariableBase.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace VariableBase.Mathematics.Tests.NumberTests.B0123456789ABCDEF
{
    [TestClass]
    public class Divide0123456789ABCDEFTests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Divide")]
        [TestCategory("0123456789ABCDEF")]
        public void Divide_160_40()
        {
            var env = new CharMathEnvironment("0123456789ABCDEF");

            Number expected = env.GetNumber("5");

            Number a = env.GetNumber("160");
            Number b = env.GetNumber("40");

            Number actual = a / b;

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Divide")]
        [TestCategory("0123456789ABCDEF")]
        public void Divide_9876_99()
        {
            var env = new CharMathEnvironment("0123456789ABCDEF");

            var expected = env.GetNumber("FF", "F", "99");

            var a = env.GetNumber("9876");
            var b = env.GetNumber("99");

            Number actual = a / b;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Divide")]
        [TestCategory("0123456789ABCDEF")]
        public void Divide_34798249_40()
        {
            var env = new CharMathEnvironment("0123456789ABCDEF");

            var expected = env.GetNumber("20D","B", "1F");

            var a = env.GetNumber("3F9E");
            var b = env.GetNumber("1F");

            Number actual = a / b;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Divide")]
        [TestCategory("0123456789ABCDEF")]
        public void Divide_15789_2_3_D_9_7_8()
        {
            var env = new CharMathEnvironment("0123456789ABCDEF");

            var a = env.GetNumber("3DAD","2", "3");
            var b = env.GetNumber("9","7", "8");

            Number c = a / b;


            Number d = c * b;

            Assert.AreEqual(a, d);
        }

    }
} 