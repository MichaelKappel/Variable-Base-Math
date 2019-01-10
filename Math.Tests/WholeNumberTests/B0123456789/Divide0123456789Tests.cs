using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Math.Tests.WholeNumberTests.B0123456789
{
    [TestClass]
    public class Divide0123456789Tests
    {
        [TestMethod]
        [TestCategory("WholeNumber")]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void Divide_160_40()
        {
            var env = new MathEnvironment("0123456789");

            var expected = env.GetWholeNumber("4");

            WholeNumber a = env.GetWholeNumber("160");
            WholeNumber b = env.GetWholeNumber("40");

            WholeNumber actual = a / b;

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("WholeNumber")]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void Divide_161_40()
        {
            var env = new MathEnvironment("0123456789");

            var expected = env.GetWholeNumber("4");

            WholeNumber a = env.GetWholeNumber("161");
            WholeNumber b = env.GetWholeNumber("40");

            WholeNumber actual = a / b;

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("WholeNumber")]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void Divide_9876_99()
        {
            var env = new MathEnvironment("0123456789");

            var expected = env.GetWholeNumber("99");

            WholeNumber a = env.GetWholeNumber("9876");
            WholeNumber b = env.GetWholeNumber("99");

            WholeNumber actual = a / b;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("WholeNumber")]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void Divide_34798249_40()
        {
            var env = new MathEnvironment("0123456789");

            var expected = env.GetWholeNumber("869956");

            WholeNumber a = env.GetWholeNumber("34798249");
            WholeNumber b = env.GetWholeNumber("40");

            WholeNumber actual = a / b;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("WholeNumber")]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void Divide_15789_D_9()
        {
            var env = new MathEnvironment("0123456789");

            var expected = env.GetNumber("1754", "1", "3");

            WholeNumber a = env.GetWholeNumber("15789");
            WholeNumber b = env.GetWholeNumber("9");

            Number actual = a / b;

            Assert.AreEqual(expected, actual);
        }

    }
} 