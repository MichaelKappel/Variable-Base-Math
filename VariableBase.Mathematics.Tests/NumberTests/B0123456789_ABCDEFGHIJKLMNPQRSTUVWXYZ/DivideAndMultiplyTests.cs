using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using VariableBase.Mathematics;

namespace VariableBase.Math.Tests.NumberTests.B0123456789_ABCDEFGHIJKLMNPQRSTUVWXYZ
{
    [TestClass]
    public class DivideAndMultiplyTests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Add")]
        [TestCategory("B0123456789_ABCDEFGHIJKLMNPQRSTUVWXYZ")]
        public void AB_DE_Test_1()
        {
            var env = new MathEnvironment("0123456789_ABCDEFGHIJKLMNPQRSTUVWXYZ");

            Number cNumber = env.GetNumber("00YICXCDSHI3_MJ9FAZ0NU1EPWIZX8HVHR938F35ZQ973UTRLPZL7I61V3A2J3V26GJF61W815A");

            Number a = env.GetNumber("89A2757323413245474677645A93273240218983245474677645A2109A980");
            Number b = env.GetNumber("3245474677645A");

            Number c = a * b;

            Assert.IsTrue(c == cNumber);

            Number d = c / b;
            Assert.IsTrue(a == d);

            Number e = c / a;
            Assert.IsTrue(b == e);
        }
    }
}
