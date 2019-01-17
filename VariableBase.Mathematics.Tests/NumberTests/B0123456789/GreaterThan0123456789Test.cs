using VariableBase.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace VariableBase.Mathematics.Tests.NumberTests.B0123456789
{
    [TestClass]
    public class GreaterThan0123456789Tests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("GreaterThan")]
        [TestCategory("0123456789")]
        public void GreaterThan_15_5()
        {
            var env = new MathEnvironment("0123456789");

            var expected = true;

            Number a = env.GetNumber("15");
            Number b = env.GetNumber("5");

            Boolean actual = a > b;

            Assert.AreEqual(expected, actual);
        }
    }
}