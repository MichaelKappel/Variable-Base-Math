using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Math.Tests.WholeNumberTests.B0123456789
{
    [TestClass]
    public class Multiply0123456789Tests
    {
        [TestMethod]
        [TestCategory("WholeNumber")]
        [TestCategory("Multiply")]
        [TestCategory("0123456789")]
        public void Multiply_15_9()
        {
            var env = new MathEnvironment("0123456789");

            var expected = env.GetNumber("135");

            var a = env.GetWholeNumber("15");
            var b = env.GetWholeNumber("9");

            WholeNumber actual = a * b;

            Assert.AreEqual(expected, actual);
        }


    }
} 