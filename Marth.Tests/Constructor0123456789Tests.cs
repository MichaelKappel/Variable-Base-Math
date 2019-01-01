using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marth.Tests
{
    [TestClass]
    public class Constructor0123456789Tests
    {
       
        [TestMethod]
        [TestCategory("Constructor")]
        public void Constructor_String_00000()
        {
            var env = new Math.MathEnvironment("0123456789");
           

            var actual = new Number(env, "00000");

            Assert.AreEqual(1, actual.Segments.Count);
        }


        [TestMethod]
        [TestCategory("Constructor")]
        public void Constructor_Array_00000()
        {
            var env = new Math.MathEnvironment("0123456789");


            var actual = new Number(env, new Char[] { '0', '0', '0', '0', '0', '0', '0' });

            Assert.AreEqual(1, actual.Segments.Count);
        }
    }
}
