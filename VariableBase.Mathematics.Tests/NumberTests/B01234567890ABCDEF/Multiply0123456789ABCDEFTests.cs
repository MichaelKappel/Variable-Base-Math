using VariableBase.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace VariableBase.Mathematics.Tests.NumberTests.B0123456789ABCDEF
{
    [TestClass]
    public class Multiply0123456789ABCDEFTests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Multiply")]
        [TestCategory("0123456789ABCDEF")]
        public void Multiply_15_9()
        {
            var env = new DecimalMathEnvironment("0123456789ABCDEF");

            var expected = env.GetNumber("BD");

            Number a = env.GetNumber("15");
            Number b = env.GetNumber("9");

            Number actual = a * b;

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Multiply")]
        [TestCategory("0123456789ABCDEF")]
        public void Multiply_15_1_2_X_9_1_2()
        {
            var env = new DecimalMathEnvironment("0123456789ABCDEF");

            Number expected = env.GetNumber("CC","1", "4");

            Number a = env.GetNumber("15","1", "2");

            Number b = env.GetNumber("9","1", "2");

            Number actual = a * b;

            Assert.AreEqual(expected, actual);
        }

    }
} 