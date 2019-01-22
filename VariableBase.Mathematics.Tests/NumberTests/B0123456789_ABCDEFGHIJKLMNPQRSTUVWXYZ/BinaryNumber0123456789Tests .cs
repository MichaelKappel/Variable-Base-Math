using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VariableBase.Mathematics.Tests.NumberTests.B0123456789_ABCDEFGHIJKLMNPQRSTUVWXYZ
{
    [TestClass]
    public class AsBinaryNumber0123456789_ABCDEFGHIJKLMNPQRSTUVWXYZTests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("ToAndFromBinary")]
        [TestCategory("0123456789_ABCDEFGHIJKLMNPQRSTUVWXYZ")]
        public void ToAndFromBinary_B37()
        {
            var env = new MathEnvironment("0123456789_ABCDEFGHIJKLMNPQRSTUVWXYZ");
            
            var a = env.GetNumber("5862652580123456789_ABCDEFGHIJKLMNPQRSTUVWXYZ0123456789_ABCDEFGHIJKLMNPQRSTUVWXYZ90987654234567");
            var b = a.AsBinary();
            var c = env.AsNumber(b.Segments.Select((x) => x == 1).ToArray());
            
            Assert.AreEqual(a, c);
        }
    }
}
