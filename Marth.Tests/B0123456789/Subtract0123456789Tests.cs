using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Marth.Tests
{
    [TestClass]
    public class Subtract0123456789Tests
    {
        [TestMethod]
        [TestCategory("Subtract")]
        [TestCategory("0123456789")]
        public void Subtract_999_88()
        {
            var env = new MathEnvironment("0123456789");

            var expected = new Number(env, "9688");

            var a = new Number(env, "9876");
            var b = new Number(env, "188");

            Number actual = a - b;

            Assert.AreEqual(expected, actual);
        }

    }
} 