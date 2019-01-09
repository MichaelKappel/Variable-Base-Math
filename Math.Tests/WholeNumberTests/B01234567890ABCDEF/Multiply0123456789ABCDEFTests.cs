using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Math.Tests.WholeNumberTests.B0123456789ABCDEF
{
    [TestClass]
    public class Multiply0123456789ABCDEFTests
    {
        [TestMethod]
        [TestCategory("WholeNumber")]
        [TestCategory("Multiply")]
        [TestCategory("0123456789ABCDEF")]
        public void Multiply_15_9()
        {
            var env = new MathEnvironment("0123456789ABCDEF");

            var expected = env.GetNumber("BD");

            var a = env.GetNumber("15");
            var b = env.GetNumber("9");

            Number actual = a * b;

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("Multiply")]
        [TestCategory("0123456789ABCDEF")]
        public void Multiply_15_1_2_X_9_1_2()
        {
            var env = new MathEnvironment("0123456789ABCDEF");

            var expected = env.GetNumber("CC","1", "4");

            var a = env.GetNumber("15","1", "2");

            var b = env.GetNumber("9","1", "2");

            Number actual = a * b;

            Assert.AreEqual(expected, actual);
        }

    }
} 