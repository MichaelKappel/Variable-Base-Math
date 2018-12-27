using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Marth.Tests
{
    [TestClass]
    public class MathNumberBase01Tests
    {
        [TestMethod]
        public void Test_01Tests_9_PLUS_8()
        {
            var expected = "10001";
            
            
            String actual = Number.Add("01", "1001", "1000");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Test_01Tests_51_PLUS_42()
        {
            var expected = "1011101";
            
            
            String actual = Number.Add("01", "110011", "101010");

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void Test_01Tests_99_PLUS_88()
        {
            var expected = "10111011";
            
            
            String actual = Number.Add("01", "1100011", "1011000");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Test_01Tests_0123456789_999_PLUS_88()
        {
            var expected = "10000111111";
            
            
            String actual = Number.Add("01", "1111100111", "1011000");

            Assert.AreEqual(expected, actual);
        }
    }
}
