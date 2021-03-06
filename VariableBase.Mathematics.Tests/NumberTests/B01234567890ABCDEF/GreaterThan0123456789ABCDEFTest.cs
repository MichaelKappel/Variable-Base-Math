using VariableBase.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace VariableBase.Mathematics.Tests.NumberTests.B0123456789ABCDEF
{
    [TestClass]
    public class GreaterThan0123456789ABCDEFTests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("GreaterThan")]
        [TestCategory("0123456789ABCDEF")]
        public void GreaterThan_15_5()
        {
            var env = new CharMathEnvironment("0123456789ABCDEF");

            var expected = true;

            var a = env.GetNumber("15");
            var b = env.GetNumber("5");

            Boolean actual = a > b;

            Assert.AreEqual(expected, actual);
        }
    }
}