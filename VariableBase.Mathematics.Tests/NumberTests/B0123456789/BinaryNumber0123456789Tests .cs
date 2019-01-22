using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VariableBase.Mathematics.Tests.NumberTests.B0123456789
{
    [TestClass]
    public class AsBinaryNumber0123456789Tests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("AsBinary")]
        [TestCategory("0123456789")]
        public void AsBinary_9840()
        {
            var env = new MathEnvironment("0123456789");

            var expectedEnvironment = new MathEnvironment("01");
            var expected = expectedEnvironment.GetNumber("10011001110000");

            var a = env.GetNumber("9840");

            Number actual = a.AsBinary();

            Assert.AreEqual(expected, actual);
        }



        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("AsBinary")]
        [TestCategory("0123456789")]
        public void AsBinary_5862652587024684796()
        {
            var env = new MathEnvironment("0123456789");

            var expectedEnvironment = new MathEnvironment("01");
            var expected = expectedEnvironment.GetNumber("‭101000101011100010100110111011101011011001000011001011011111100‬");

            var a = env.GetNumber("5862652587024684796");

            Number actual = a.AsBinary();

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("AsEnvironmentNumber")]
        [TestCategory("0123456789")]
        public void AsEnvironmentNumber_9840()
        {
            var env = new MathEnvironment("0123456789");

            var expected = env.GetNumber("9840");

            Boolean[] binary = "10011001110000".ToCharArray().ToList().Select((x) => x == '1').Reverse().ToArray();

            Number actual = env.AsNumber(binary);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("AsEnvironmentNumber")]
        [TestCategory("0123456789")]
        public void AsEnvironmentNumber_‭9223372036854775807‬()
        {
            var env = new MathEnvironment("0123456789");

            var expected = env.GetNumber("‭9223372036854775807‬");

            Boolean[] binary = "111111111111111111111111111111111111111111111111111111111111111".ToCharArray().ToList().Select((x) => x == '1').Reverse().ToArray();

            Number actual = env.AsNumber(binary);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("AsEnvironmentNumber")]
        [TestCategory("0123456789")]
        public void AsEnvironmentNumber_5862652587024684796()
        {
            var env = new MathEnvironment("0123456789");

            var expectedEnvironment = new MathEnvironment("01");
            var expected = expectedEnvironment.GetNumber("‭101000101011100010100110111011101011011001000011001011011111100‬");

            var a = env.GetNumber("5862652587024684796");

            Number actual = a.AsBinary();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("ToAndFromBinary")]
        [TestCategory("0123456789")]
        public void ToAndFromBinary_B10()
        {
            var env = new MathEnvironment("0123456789");
            
            var a = env.GetNumber("586265258702468479623456789009876542134567890987654234567890987654321234567890987654234567");
            var b = a.AsBinary();
            var c = env.AsNumber(b.Segments.Select((x) => x == 1).ToArray());
            
            Assert.AreEqual(a, c);
        }

        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("ToAndFromBinary")]
        [TestCategory("0123456789")]
        public void ToAndFromBinary_B10_182()
        {
            var env = new MathEnvironment("0123456789");

            var a = env.GetNumber("95862652587024684796234567890098765421345678909876542345678909876543212345678909876542345675862652587024684796234567890098765421345678909876542345678909876543212345678909876542345679");
            var b = a.AsBinary();
            var c = env.AsNumber(b.Segments.Select((x) => x == 1).ToArray());

            Assert.AreEqual(a, c);
        }
    }
}
