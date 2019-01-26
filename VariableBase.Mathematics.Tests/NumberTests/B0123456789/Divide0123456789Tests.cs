using VariableBase.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using VariableBase.Mathematics;

namespace VariableBase.Math.Tests.NumberTests.B0123456789
{
    [TestClass]
    public class Divide0123456789Tests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void Divide_160_40()
        {
            var env = new CharMathEnvironment("0123456789");

            Number expected = env.GetNumber("4");

            Number a = env.GetNumber("160");
            Number b = env.GetNumber("40");

            Number actual = a / b;

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void Divide_9876_99()
        {
            var env = new CharMathEnvironment("0123456789");

            var expected = env.GetNumber("99","75", "99");

            var a = env.GetNumber("9876");
            var b = env.GetNumber("99");

            Number actual = a / b;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void Divide_34798249_40()
        {
            var env = new CharMathEnvironment("0123456789");

            var expected = env.GetNumber("869956","9", "40");

            var a = env.GetNumber("34798249");
            var b = env.GetNumber("40");

            Number actual = a / b;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void Divide_15789_2_3_D_9_7_8()
        {
            var env = new CharMathEnvironment("0123456789");

            var expected = env.GetNumber("1754","1", "3");

            var a = env.GetNumber("15789","2", "3");
            var b = env.GetNumber("9","7", "8");

            Number actual = a / b;

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void AB_DE_15789_78485789435878690790970952072_89573297589472827()
        {
            var env = new CharMathEnvironment("0123456789");

            var cNumber = env.GetNumber("7030230973684664604284953097930534576763347544");

            var a = env.GetNumber("78485789435878690790970952072");
            var b = env.GetNumber("89573297589472827");

            Number actual = a / b;


            Number c = a * b;

            Assert.IsTrue(c == cNumber);

            Number d = c / b;
            Assert.IsTrue(a == d);

            Number e = c / a;
            Assert.IsTrue(b == e);
        }

        [TestMethod]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void AB_DE_Test_1()
        {
            var env = new CharMathEnvironment("0123456789");

            var cNumber = env.GetNumber("7030230973684664674587262834847483632372822538517685854708233754041860488982766035728404973516310376696023789142517103192839438521360237891425170328905297016747134920187968225570920706569882172020557046279817435252472461961287852330534576763347544");

            var a = env.GetNumber("7848578943587869079097095207278485789435878690790970952072784857894358786907909709520727848578943587869079097095207278485789435878690790970952072");
            var b = env.GetNumber("895732975894728278957329758947282789573297589472827895732975894728278957329758947282789573297589472827");

            Number actual = a / b;


            Number c = a * b;

            Assert.IsTrue(c == cNumber);

            Number d = c / b;
            Assert.IsTrue(a == d);

            Number e = c / a;
            Assert.IsTrue(b == e);
        }


        [TestMethod]
        [TestCategory("Divide")]
        [TestCategory("0123456789")]
        public void AB_DE_Test_2()
        {
            var env = new CharMathEnvironment("0123456789");
            
            var a = env.GetNumber("7848578943587869078485789435878690790970952072784857894358786907909709520727848578943587869079097095207278485789435878690790970952072784857894358786907909709520727909709520727848578943587869079097095207278485789435878690790970952072784857894378485789435878690790970952072784857894358786907909709520727848578943587869079097095207278485789437848578943587869079097095207278485789435878690790970952072784857894358786907909709520727848578943587869079097095207278485789435878690790970952072587869079097095207278485789435878690790970952072587869079097095207278485789435878690790970952072");
            var b = env.GetNumber("895732975894728278957329758947282789573297589472827895732975894728278957329758947282789573297589472827");

            Number actual = a / b;
            
            Number c = a * b;
            
            Number d = c / b;
            Assert.IsTrue(a == d);

            Number e = c / a;
            Assert.IsTrue(b == e);
        }
    }
} 