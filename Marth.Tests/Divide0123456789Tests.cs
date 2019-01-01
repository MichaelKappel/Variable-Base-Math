using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Marth.Tests
{
    [TestClass]
    public class Divide0123456789Tests
    {
        [TestMethod]
        [TestCategory("Divide")]
        public void Divide_160_40()
        {
            var env = new Math.MathEnvironment("0123456789");

            var expected = new Number(env, "4");

            var a = new Number(env, "160");
            var b = new Number(env, "40");

            Number actual = a / b;

            Assert.AreEqual(expected, actual);
        }

    }
} 