using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Marth.Tests
{
    [TestClass]
    public class GreaterThan0123456789Tests
    {
        [TestMethod]
        [TestCategory("GreaterThan")]
        public void GreaterThan_15_5()
        {
            var env = new Math.MathEnvironment("0123456789");

            var expected = true;

            var a = new Number(env, "15");
            var b = new Number(env, "5");

            Boolean actual = a > b;

            Assert.AreEqual(expected, actual);
        }
    }
}