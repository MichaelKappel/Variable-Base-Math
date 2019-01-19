using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using VariableBase.Mathematics;

namespace VariableBase.Math.Tests.NumberTests.BInt32
{
    [TestClass]
    public class BInt32DivideAndMultiplyTests
    {        
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Add")]
        [TestCategory("BInt32")]
        public void BInt32_Test_1()
        {
            var env = new MathEnvironment();

            Number a = env.GetNumber("1500");
            Number b = env.GetNumber("6508");

            Number c = a * b;
            
            Number d = c / b;
            Assert.IsTrue(a == d);

            Number e = c / a;
            Assert.IsTrue(b == e);
        }


        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Add")]
        [TestCategory("BInt32")]
        public void BInt32_Test_2()
        {
            var env = new MathEnvironment();

            Number a = env.GetNumber("150hglyfjhlflhlfg0");
            Number b = env.GetNumber("65ghdfiy;f;fyifyiyfpflyiifyfyl08");

            Number c = a * b;

            Number d = c / b;
            Assert.IsTrue(a == d);

            Number e = c / a;
            Assert.IsTrue(b == e);
        }
    }
}


