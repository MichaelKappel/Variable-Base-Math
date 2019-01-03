using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Marth.Tests
{
    [TestClass]
    public class Add0123456789Tests
    {
        [TestMethod]
        [TestCategory("Add")]
        [TestCategory("0123456789")]
        public void Add_999_88()
        {
            var env = new MathEnvironment("0123456789");

            var expected = new Number(env, "1087");

            var a = new Number(env, "999");
            var b = new Number(env, "88");

            Number actual = a + b;

            Assert.AreEqual(expected, actual);
        }
    }
} 