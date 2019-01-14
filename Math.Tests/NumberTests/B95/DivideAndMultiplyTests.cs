using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Math.Tests.NumberTests.B234
{
    [TestClass]
    public class B95DivideAndMultiplyTests
    {
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Add")]
        [TestCategory("B95")]
        public void AB_DE_Test_1()
        {
            var env = new MathEnvironment("{}[]0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&*()-=_+ ,.<>~`:;'\"?|\\/");

            Number cNumber = env.GetNumber("cDL#lQ>fb^aefL3K)z?EE [.Uj(a{m27G=qXH5qFW_UK+n?/$gK409WmN$<sC{JpnDewD,=f<{x_q--rFt>F=!1C$|R?kqAH02N4dX;j1AWJ&')Q%<~dnFzV4B.D6?3BK]iz2fMEpb&m=zA3fSyf^g!Vip`CD/I;],%z/FQRG\\*/T,gI4]2~1}(Rw^rEjoR)\")OeM$=oW\"JK/amU8[>!c6!Dn_??=i[UidDz?/*Mid5bdbmtvrgM\\p1YsypP.HWR0k*&f;3'g$");

            Number a = env.GetNumber("nopqrstuvwxyz!@#$%^&*()-=_456789ABCDEFGHIJKLMNOPQRSTUVW456789ABCDEFGHIJKLMNOPQRSTUVW456789ABCDEFGHIJKLMNOPQRSTUVW");
            Number b = env.GetNumber("()-=_45()-=_45()-=_45456789ABCDEFGHIJKLMNOP()-=_45QRSTUVW456789ABCDEFGHIJKLMNOPQRSTUVWJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&*()-=_+ ,.<>~`:;'");

            Number c = a * b;

            Assert.IsTrue(c == cNumber);

            Number d = c / b;
            Assert.IsTrue(a == d);
            
            Number e = c / a;
            Assert.IsTrue(b == e);
        }
    }
}


