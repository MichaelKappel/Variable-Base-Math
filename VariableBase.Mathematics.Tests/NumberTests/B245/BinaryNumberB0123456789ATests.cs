using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VariableBase.Mathematics.Tests.NumberTests.B0123456789A
{
    [TestClass]
    public class BinaryNumberB245Tests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("ToAndFromBinary")]
        [TestCategory("B245")]
        public void ToAndFromBinary_B245()
        {
            var env = new MathEnvironment();
            
            var a = env.GetNumber("A6780123450123456789A601234560123456789A789A789A01234567890123456789AA012345601234560123456789A789A789A");
            var b = a.AsBinary();
            var c = env.AsNumber(b.Segments.Select((x) => x == 1).ToArray());
            
            Assert.AreEqual(a, c);
        }
    }
}
