using VariableBase.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace VariableBase.Mathematics.Tests.NumberTests.B0123456789
{
    [TestClass]
    public class Constructor0123456789Tests
    {
       
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Constructor")]
        [TestCategory("0123456789")]
        public void Constructor_String_00000()
        {
            var env = new MathEnvironment("0123456789");


            Number actual = env.GetNumber("00000");

            Assert.AreEqual(1, actual.Segments.Count);
        }
    }
}
