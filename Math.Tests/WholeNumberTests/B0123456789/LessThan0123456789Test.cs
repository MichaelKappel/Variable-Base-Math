using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Math.Tests.WholeNumberTests.B0123456789
{
    [TestClass]
    public class LerssThan0123456789Tests
    {
        [TestMethod]
        [TestCategory("WholeNumber")]
        [TestCategory("LessThan")]
        [TestCategory("0123456789")]
        public void GreaterThan_5_15()
        {
            var env = new MathEnvironment("0123456789");

            var expected = true;

            var a = env.GetWholeNumber("5");
            var b = env.GetWholeNumber("15");

            Boolean actual = a < b;

            Assert.AreEqual(expected, actual);
        }
    }
} 