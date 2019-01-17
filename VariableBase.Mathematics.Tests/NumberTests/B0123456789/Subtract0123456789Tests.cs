using VariableBase.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace VariableBase.Mathematics.Tests.NumberTests.B0123456789
{
    [TestClass]
    public class Subtract0123456789Tests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Subtract")]
        [TestCategory("0123456789")]
        public void Subtract_999_88()
        {
            var env = new MathEnvironment("0123456789");

            Number expected = env.GetNumber("9688");

            Number a = env.GetNumber("9876");
            Number b = env.GetNumber("188");

            Number actual = a - b;

            Assert.AreEqual(expected, actual);
        }

    }
} 