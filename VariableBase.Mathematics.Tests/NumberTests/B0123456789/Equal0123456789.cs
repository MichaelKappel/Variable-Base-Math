using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VariableBase.Mathematics.Tests.NumberTests.B0123456789
{
    [TestClass]
    public class Equal0123456789
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Equal")]
        [TestCategory("0123456789")]
        public void Equal_0_default()
        {
            var env = new CharMathEnvironment("0123456789");

            var expected = true;

            Number a = env.GetNumber(0);
            Number b = default(Number);
            
            Boolean actual = (a == b);

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Equal")]
        [TestCategory("0123456789")]
        public void Equal_default_0()
        {
            var env = new CharMathEnvironment("0123456789");

            var expected = true;

            Number a = default(Number); 
            Number b = env.GetNumber(0);

            Boolean actual = (a == b);

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Equal")]
        [TestCategory("0123456789")]
        public void Equal_ENV1_1()
        {
            var env = new CharMathEnvironment("0123456789");

            var expected = true;

            Number a = env.GetNumber(1);
            Number b = env.GetNumber(env.GetNumber(1).ToString());

            Boolean actual = (a == b);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Equal")]
        [TestCategory("0123456789")]
        public void Equal_1_ENV1()
        {
            var env = new CharMathEnvironment("0123456789");

            var expected = true;

            Number a = env.GetNumber(env.GetNumber(1).ToString());
       
            Number b = env.GetNumber(1);

            Boolean actual = (a == b);

            Assert.AreEqual(expected, actual);
        }
    }
}
