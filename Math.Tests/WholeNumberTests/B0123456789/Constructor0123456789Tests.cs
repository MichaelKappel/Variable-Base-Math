using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Math.Tests.WholeNumberTests.B0123456789
{
    [TestClass]
    [TestCategory("WholeNumber")]
    public class Constructor0123456789Tests
    {
       
        [TestMethod]
        [TestCategory("Constructor")]
        [TestCategory("0123456789")]
        public void Constructor_String_00000()
        {
            var env = new MathEnvironment("0123456789");


            WholeNumber actual = env.GetWholeNumber("00000");

            Assert.AreEqual(1, actual.Segments.Count);
        }
    }
}
