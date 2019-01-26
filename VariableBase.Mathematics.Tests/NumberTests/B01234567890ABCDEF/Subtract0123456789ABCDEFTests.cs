using VariableBase.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace VariableBase.Mathematics.Tests.NumberTests.B0123456789ABCDEF
{
    [TestClass]
    public class Subtract0123456789ABCDEFTests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Subtract")]
        [TestCategory("0123456789ABCDEF")]
        public void Subtract_999_88()
        {
            var env = new CharMathEnvironment("0123456789ABCDEF");

            Number expected = env.GetNumber("‭96EE‬");

            Number a = env.GetNumber("9876");
            Number b = env.GetNumber("188");

            Number actual = a - b;

            Assert.AreEqual(expected, actual);
        }

    }
}