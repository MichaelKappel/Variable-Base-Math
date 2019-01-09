using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Math.Tests.WholeNumberTests.B0123456789ABCDEF
{
    [TestClass]
    public class Subtract0123456789ABCDEFTests
    {
        [TestMethod]
        [TestCategory("WholeNumber")]
        [TestCategory("Subtract")]
        [TestCategory("0123456789ABCDEF")]
        public void Subtract_999_88()
        {
            var env = new MathEnvironment("0123456789ABCDEF");

            var expected = env.GetNumber("‭96EE‬");

            WholeNumber a = env.GetWholeNumber("9876");
            WholeNumber b = env.GetWholeNumber("188");

            WholeNumber actual = a - b;

            Assert.AreEqual(expected, actual);
        }

    }
}