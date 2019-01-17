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
            var env = new MathEnvironment("0123456789");

            var expected = true;

            Number a = env.BottomNumber;
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
            var env = new MathEnvironment("0123456789");

            var expected = true;

            Number a = default(Number); 
            Number b = env.BottomNumber;

            Boolean actual = (a == b);

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Equal")]
        [TestCategory("0123456789")]
        public void Equal_ENV1_1()
        {
            var env = new MathEnvironment("0123456789");

            var expected = true;

            Number a = env.FirstNumber;
            Number b = env.GetNumber(env.FirstNumber.ToString());

            Boolean actual = (a == b);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Equal")]
        [TestCategory("0123456789")]
        public void Equal_1_ENV1()
        {
            var env = new MathEnvironment("0123456789");

            var expected = true;

            Number a = env.GetNumber(env.FirstNumber.ToString());
       
            Number b = env.FirstNumber;

            Boolean actual = (a == b);

            Assert.AreEqual(expected, actual);
        }
    }
}
