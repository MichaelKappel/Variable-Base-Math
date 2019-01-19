using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VariableBase.Mathematics.Tests.NumberTests.B0123456789A
{
    [TestClass]
    public class BinaryNumberB0123456789ATests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("ToAndFromBinary")]
        [TestCategory("0123456789A")]
        public void ToAndFromBinary_1()
        {
            var env = new MathEnvironment("0123456789A");
            
            var a = env.GetNumber("A6780123450123456789A601234560123456789A789A789A01234567890123456789AA012345601234560123456789A789A789A");
            var b = a.AsBinary();
            var c = env.AsNumber(b.Segments.Select((x) => x == 1).ToArray());
            
            Assert.AreEqual(a, c);
        }
    }
}
