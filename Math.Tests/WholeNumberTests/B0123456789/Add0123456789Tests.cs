using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Math.Tests.WholeNumberTests.B0123456789
{
    [TestClass]
    public class Add0123456789Tests
    {
        [TestMethod]
        [TestCategory("WholeNumber")]
        [TestCategory("Add")]
        [TestCategory("0123456789")]
        public void Add_999_88()
        {
            var env = new MathEnvironment("0123456789");

            var expected = env.GetNumber("1087");

            WholeNumber a = env.GetWholeNumber("999");
            WholeNumber b = env.GetWholeNumber("88");

            WholeNumber actual = a + b;

            Assert.AreEqual(expected, actual);
        }
    }
} 