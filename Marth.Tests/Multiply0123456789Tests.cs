using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Marth.Tests
{
    [TestClass]
    public class Multiply0123456789Tests
    {
        [TestMethod]
        [TestCategory("Multiply")]
        public void Multiply_15_9()
        {
            var env = new Math.MathEnvironment("0123456789");

            var expected = new Number(env, "135");

            var a = new Number(env, "15");
            var b = new Number(env, "9");

            Number actual = a * b;

            Assert.AreEqual(expected, actual);
        }

    }
} 