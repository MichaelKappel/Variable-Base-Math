using Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Marth.Tests
{
    [TestClass]
    public class MathNumberBase0123456789Tests
    {
        [TestMethod]
        public void Base10_0123456789_9_8_Multiply()
        {
            var environment = new MathEnvironmentInfo("0123456789");

            Number expected = new Number(environment, "72");
            
            
            Number actual = Number.Multiply(new Number(environment, "9"), new Number(environment, "8"));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Base10_0123456789_55_9_Multiply()
        {
            var expected = "495";


            String actual = Number.Multiply("0123456789", "55", "9");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Base10_0123456789_98765_9_Multiply()
        {
            var environment = new MathEnvironmentInfo("0123456789");

            var expected = new Number(environment,"888885");


            Number actual = Number.Multiply(new Number(environment, "98765"), '9');

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Base10_0123456789_1_0_Compare()
        {
            Int32 expected = 0.CompareTo(1);
            
            var environment = new MathEnvironmentInfo("0123456789");
            var zero = new Number(environment, new Char[] { '0' });
            var one = new Number(environment, new Char[] { '1' });

            Int32 actual = zero.CompareTo(one);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Base10_0123456789_9_PLUS_8()
        {
            var expected = "17";

            
            String actual = Number.Add("0123456789", "9", "8");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Base10_0123456789_51_PLUS_42()
        {
            var expected = "93";
            
            
            String actual = Number.Add("0123456789", "51", "42");

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void Base10_0123456789_99_PLUS_88()
        {
            var expected = "187";
            
            
            String actual = Number.Add("0123456789", "99", "88");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Base10_0123456789_999_PLUS_88()
        {
            var expected = "1087";
            
            
            String actual = Number.Add("0123456789", "999", "88");

            Assert.AreEqual(expected, actual);
        }
 
    }
}
