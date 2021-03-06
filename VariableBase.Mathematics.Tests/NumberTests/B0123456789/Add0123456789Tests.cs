using VariableBase.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace VariableBase.Mathematics.Tests.NumberTests.B0123456789
{
    [TestClass]
    public class Add0123456789Tests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Add")]
        [TestCategory("0123456789")]
        public void Add_999_88()
        {
            var env = new CharMathEnvironment("0123456789");

            var expected = env.GetNumber("1087");

            var a = env.GetNumber("999");
            var b = env.GetNumber("88");

            Number actual = a + b;

            Assert.AreEqual(expected, actual);
        }
    }
} 