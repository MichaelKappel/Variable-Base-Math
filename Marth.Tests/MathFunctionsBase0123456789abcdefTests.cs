using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Marth.Tests
{
    [TestClass]
    public class MathNumberBase0123456789abcdefTests
    {
        [TestMethod]
        public void Test_0123456789abcdefTests_9_PLUS_8()
        {
            var expected = "11";
            
            
            String actual = Number.Add("0123456789abcdef", "9", "8");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Test_0123456789abcdefTests_51_PLUS_42()
        {
            var expected = "93";
            
            
            String actual = Number.Add("0123456789abcdef", "51", "42");

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void Test_0123456789abcdefTests_99_PLUS_88()
        {
            var expected = "121";
            
            
            String actual = Number.Add("0123456789abcdef", "99", "88");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Test_0123456789abcdefTests_0123456789_999_PLUS_88()
        {
            var expected = "a21";
            
            
            String actual = Number.Add("0123456789abcdef", "999", "88");

            Assert.AreEqual(expected, actual);
        }
    }
}
