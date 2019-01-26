using VariableBase.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace VariableBase.Mathematics.Tests.NumberTests.B0123456789
{
    [TestClass]
    public class LerssThan0123456789Tests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("LessThan")]
        [TestCategory("0123456789")]
        public void GreaterThan_5_15()
        {
            var env = new CharMathEnvironment("0123456789");

            var expected = true;

            Number a = env.GetNumber("5");
            Number b = env.GetNumber("15");

            Boolean actual = a < b;

            Assert.AreEqual(expected, actual);
        }
    }
} 