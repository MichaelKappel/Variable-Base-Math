using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VariableBase.Mathematics.Tests.NumberTests.B0123456789_ABCDEFGHIJKLMNPQRSTUVWXYZ
{
    [TestClass]
    public class BinaryNumberB0123456789ATests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("ToAndFromBinary")]
        [TestCategory("01234567890ABCDEF")]
        public void ToAndFromBinary_1()
        {
            var env = new MathEnvironment("01234567890ABCDEF");
            
            var a = env.GetNumber("58012345801234590ABCDEF67890A5801234590ABCDEF67890ABCDEFBCDEF590ABCDE5801234590ABCDEF678905801234590ABCDEF67890ABCDEFABCDEFF67890ABC5801234590ABCDEF67890ABCDEFDEF");
            var b = a.AsBinary();
            var c = env.AsNumber(b.Segments.Select((x) => x == 1).ToArray());
            
            Assert.AreEqual(a, c);
        }
    }
}
