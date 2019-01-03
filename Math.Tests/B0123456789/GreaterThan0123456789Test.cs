using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Math.Tests.B0123456789
{
    [TestClass]
    public class GreaterThan0123456789Tests
    {
        [TestMethod]
        [TestCategory("GreaterThan")]
        [TestCategory("0123456789")]
        public void GreaterThan_15_5()
        {
            var env = new MathEnvironment("0123456789");

            var expected = true;

            var a = env.GetNumber("15");
            var b = env.GetNumber("5");

            Boolean actual = a > b;

            Assert.AreEqual(expected, actual);
        }
    }
}