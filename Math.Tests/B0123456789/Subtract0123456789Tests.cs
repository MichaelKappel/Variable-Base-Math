using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Math.Tests.B0123456789
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

            var expected = env.GetNumber("9688");

            var a = env.GetNumber("9876");
            var b = env.GetNumber("188");

            Number actual = a - b;

            Assert.AreEqual(expected, actual);
        }

    }
} 