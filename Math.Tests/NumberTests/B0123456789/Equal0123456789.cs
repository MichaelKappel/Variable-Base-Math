using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Math.Tests.NumberTests.B0123456789
{
    [TestClass]
    public class Equal0123456789
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("GreaterThan")]
        [TestCategory("0123456789")]
        public void Equal_0_null()
        {
            var env = new MathEnvironment("0123456789");

            var expected = true;

            Number a = env.BottomNumber;
            Number b = null;
            
            Boolean actual = (a == b);

            Assert.AreEqual(expected, actual);
        }
    }
}
