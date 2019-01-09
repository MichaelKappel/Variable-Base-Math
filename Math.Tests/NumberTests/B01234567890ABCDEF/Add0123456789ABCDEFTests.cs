using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Math.Tests.NumberTests.B0123456789ABCDEF
{
    [TestClass]
    public class Add0123456789ABCDEFTests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Add")]
        [TestCategory("0123456789ABCDEF")]
        public void Add_999_88()
        {
            var env = new MathEnvironment("0123456789ABCDEF");

            Number expected = env.GetNumber("A21");

            Number a = env.GetNumber("999");
            Number b = env.GetNumber("88");

            Number actual = a + b;

            Assert.AreEqual(expected, actual);
        }
    }
} 