using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Math.Tests.WholeNumberTests.B0123456789ABCDEF
{
    [TestClass]
    public class Constructor0123456789ABCDEFTests
    {
       
        [TestMethod]
        [TestCategory("WholeNumber")]
        [TestCategory("Constructor")]
        [TestCategory("0123456789ABCDEF")]
        public void Constructor_String_00000()
        {
            var env = new MathEnvironment("0123456789ABCDEF");
           

            var actual = env.GetNumber("00000");

            Assert.AreEqual(1, actual.Segments.Count);
        }
    }
}
