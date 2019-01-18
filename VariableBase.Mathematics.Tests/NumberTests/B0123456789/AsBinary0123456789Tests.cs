using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace VariableBase.Mathematics.Tests.NumberTests.B0123456789
{
    [TestClass]
    public class AsBinary0123456789Tests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("AsBinary")]
        [TestCategory("0123456789")]
        public void AsBinary_9840()
        {
            var env = new MathEnvironment("0123456789");

            var expectedEnvironment = new MathEnvironment("01");
            var expected = expectedEnvironment.GetNumber("10011001110000");

            var a = env.GetNumber("9840");

            Number actual = a.AsBinary();

            Assert.AreEqual(expected, actual);
        }
    }
}
