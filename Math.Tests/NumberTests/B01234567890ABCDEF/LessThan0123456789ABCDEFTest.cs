using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Math.Tests.NumberTests.B0123456789ABCDEF
{
    [TestClass]
    public class LerssThan0123456789ABCDEFTests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("LessThan")]
        [TestCategory("0123456789ABCDEF")]
        public void GreaterThan_5_15()
        {
            var env = new MathEnvironment("0123456789ABCDEF");

            var expected = true;

            var a = env.GetNumber("5");
            var b = env.GetNumber("15");

            Boolean actual = a < b;

            Assert.AreEqual(expected, actual);
        }
    }
} 