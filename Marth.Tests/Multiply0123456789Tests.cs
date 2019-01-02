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
        [TestCategory("0123456789")]
        public void Multiply_15_9()
        {
            var env = new MathEnvironment("0123456789");

            var expected = new Number(env, "135");

            var a = new Number(env, "15");
            var b = new Number(env, "9");

            Number actual = a * b;

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("Multiply")]
        [TestCategory("0123456789")]
        public void Multiply_15_1_2_X_9_1_2()
        {
            var env = new MathEnvironment("0123456789");

            var expected = new Number(env, "147", new Fraction(env, '1', '4'));

            var a = new Number(env, "15", new Fraction(env, '1', '2'));

            var b = new Number(env, "9", new Fraction(env, '1', '2'));

            Number actual = a * b;

            Assert.AreEqual(expected, actual);
        }

    }
} 