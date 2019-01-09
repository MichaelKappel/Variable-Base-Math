using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Math.Tests.WholeNumberTests.B0123456789
{
    [TestClass]
    public class GreaterThan0123456789Tests
    {
        [TestMethod]
        [TestCategory("WholeNumber")]
        [TestCategory("GreaterThan")]
        [TestCategory("0123456789")]
        public void GreaterThan_15_5()
        {
            var env = new MathEnvironment("0123456789");

            var expected = true;

            var a = env.GetWholeNumber("15");
            var b = env.GetWholeNumber("5");

            Boolean actual = a > b;

            Assert.AreEqual(expected, actual);
        }
    }
}