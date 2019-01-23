using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using VariableBase.Mathematics;

namespace VariableBase.Math.Tests.NumberTests.B0123456789A
{
    [TestClass]
    public class DivideAndMultiplyTests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Add")]
        [TestCategory("B0123456789A")]
        public void AB_DE_Test_1()
        {
            var env = new DecimalMathEnvironment("0123456789A");

            Number cNumber = env.GetNumber("267266182275726727A518564458464A597284017657A2011477837285953209496727A2530");

            Number a = env.GetNumber("89A2757323413245474677645A93273240218983245474677645A2109A980");
            Number b = env.GetNumber("3245474677645A");

            Number c = a * b;

            Assert.IsTrue(c == cNumber);

            Number d = c / b;
            Assert.IsTrue(a == d);

            Number e = c / a;
            Assert.IsTrue(b == e);
        }


        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Add")]
        [TestCategory("B0123456789A")]
        public void AB_DE_Test_2()
        {
            var env = new DecimalMathEnvironment("0123456789A");

            Number a = env.GetNumber("89A275732341324");
            Number b = env.GetNumber("5845474677645A");

            Number c = a * b;

            Number d = c / b;
            Assert.IsTrue(a == d);

            Number e = c / a;
            Assert.IsTrue(b == e);
        }
    }
}
