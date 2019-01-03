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
        [TestCategory("0123456789")]
        public void Divide_160_40()
        {
            var env = new MathEnvironment("0123456789");

            var expected = new Number(env, "4");

            var a = new Number(env, "160");
            var b = new Number(env, "40");

            Number actual = a / b;

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void Divide_9876_99()
        {
            var env = new MathEnvironment("0123456789");

            var expected = new Number(env, "99", new Fraction(new Number(env, "75"), new Number(env, "99")));

            var a = new Number(env, "9876");
            var b = new Number(env, "99");

            Number actual = a / b;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void Divide_34798249_40()
        {
            var env = new MathEnvironment("0123456789");

            var expected = new Number(env, "869956", new Fraction(new Number(env, "9"), new Number(env, "40")));

            var a = new Number(env, "34798249");
            var b = new Number(env, "40");

            Number actual = a / b;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void Divide_15789_2_3_D_9_7_8()
        {
            var env = new MathEnvironment("0123456789");

            var expected = new Number(env, "1754", new Fraction(new Number(env, "1"), new Number(env, "3")));

            var a = new Number(env, "15789", new Fraction(new Number(env, "2"), new Number(env, "3")));
            var b = new Number(env, "9", new Fraction(new Number(env, "7"), new Number(env, "8")));

            Number actual = a / b;

            Assert.AreEqual(expected, actual);
        }

    }
} 