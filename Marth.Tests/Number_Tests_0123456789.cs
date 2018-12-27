using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Math
{
    [TestClass]
    public class Number_Tests_0123456789
    {
        [TestMethod]
        public void Test_987_Multiply_456()
        {
            var expected = "450072";

            
            String actual = Number.Multiply("0123456789", "987", "456");


            Assert.AreEqual(expected, actual);
        }

    }
}