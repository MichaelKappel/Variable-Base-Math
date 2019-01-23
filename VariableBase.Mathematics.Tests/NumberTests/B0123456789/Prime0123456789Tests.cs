using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace VariableBase.Mathematics.Tests.NumberTests.B0123456789
{
    [TestClass]
    public class Prime0123456789Tests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Subtract")]
        [TestCategory("0123456789")]
        public void IsPrime_7()
        {
            var env = new MathEnvironment("0123456789");
            

            Number a = env.GetNumber("7");
            
            Assert.IsTrue(a.IsPrime());
        }


        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Subtract")]
        [TestCategory("0123456789")]
        public void IsPrime_9()
        {
            var env = new MathEnvironment("0123456789");


            Number a = env.GetNumber("9");

            Assert.IsFalse(a.IsPrime());
        }
    }
}
