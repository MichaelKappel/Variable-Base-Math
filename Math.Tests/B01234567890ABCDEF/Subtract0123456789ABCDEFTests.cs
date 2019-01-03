using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Math.Tests.B0123456789ABCDEF
{
    [TestClass]
    public class Subtract0123456789ABCDEFTests
    {
        [TestMethod]
        [TestCategory("Subtract")]
        [TestCategory("0123456789ABCDEF")]
        public void Subtract_999_88()
        {
            var env = new MathEnvironment("0123456789ABCDEF");

            var expected = env.GetNumber("‭96EE‬");

            var a = env.GetNumber("9876");
            var b = env.GetNumber("188");

            Number actual = a - b;

            Assert.AreEqual(expected, actual);
        }

    }
}