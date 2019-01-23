using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using VariableBase.Mathematics;

namespace VariableBase.Math.Tests.NumberTests.B0123456789A
{
    [TestClass]
    public class GetDisplayValueTests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("GetDisplayValue")]
        [TestCategory("B0123456789A")]
        public void GetDisplayValue_B11()
        {
            var env = new DecimalMathEnvironment("0123456789A");

            String expected = "89A2757323413245474677645A93273240218983245474677645A2109A980";
            Number a = env.GetNumber(expected);

            String actual = a.GetDisplayValue();

            Assert.AreEqual(expected, actual);
        }
    }
}
