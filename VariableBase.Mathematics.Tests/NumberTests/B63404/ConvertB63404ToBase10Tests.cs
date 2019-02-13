using FileRepositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using VariableBase.Mathematics;

namespace VariableBase.Math.Tests.NumberTests.BUInt16
{
    [TestClass]
    public class ConvertB63404ToBase10Tests
    {

        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Add")]
        [TestCategory("B63404")]
        public void ConvertB63404ToBase10Tests_9()
        {
            var env = new CharMathEnvironment(true, true, true, true, true, true, false, false, false, false);

            Number a = env.GetNumber("Ȧ촣ḡ窌㻱곱㽻꨾㷏謼딝邦䄬⼞뭺鑒藝闤臭齧⩗신儕ᘖᙅ疯噩쪏쁉몵﹘仏쳻䚐ᵚ㟱뾣悇딀␊㕯靨헫웜ꕣ쀠ᓟ珘遫㜂쑗ˠ劢棍ﮅ懒훗㫁ꊞ톟ꍩ䑃");


            Number result = a.ConvertToBase10();
            Assert.AreEqual(result.Environment.GetNumber("10733451489189611103121609043038710477166925241925645413424099370355605456852169736033991876014762808340865848447476173426115162172818890323837138136782951865054538417494035229785971002587932638902311416018904156170269354720460896363558168129004231138415225204738582550720791061581463934092726107458349298577292984375276210232582438075"), result);
        }
        
        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Add")]
        [TestCategory("B63404")]
        public void ConvertB63404ToBase10Tests_10()
        {
            var env = new CharMathEnvironment(true, true, true, true, true, true, false, false, false, false);

            Number a = env.GetNumber("ѵ䃵乄ன㵉塹Ỏ镜숩");
            

            Number result = a.ConvertToBase10();
            Assert.AreEqual(result.Environment.GetNumber("280571172992510140037611932413038677189525"), result);
        }


        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Add")]
        [TestCategory("B63404")]
        public void ConvertB63404ToBase10Tests_11()
        {
            var env = new CharMathEnvironment(true, true, true, true, true, true, false, false, false, false);

            Number a = env.GetNumber("Ĝ咉몺턊揋콉炙﹃猀章犑╖㱔쳪븀㊛做ꕚ굁℘㉉᭓⃦툃會郀ㆶ恇鋤羮殡䖁듓ࡼ⥽ᴄ裍腕ꀴ뱴杏賜ʜ㫮䁇醻赫羛ᛵ蛽ﴦꇝ⍐㜏뢐郠蠝伿퓘疑铣ㄢ戚춭薐⹑岦衁ꥺ햦鏚醙娻蝵⾐ǖ앃ຮ螯挚Ꮀӭ钳魭뺉覾힇ޠ烂䇕ꊚ뮂廯徙퀡咬짴ⅽᎊ朊ꏝ㬭㫹矟填엟걇埙굜쬂캟ᄝ悂驁尕糁ൣ誸隌璞羲؛");


            Number result = a.ConvertToBase10();
            Assert.AreEqual(result.Environment.GetNumber("410615886307971260333568378719267105220125108637369252408885430926905584274113403731330491660850044560830036835706942274588569362145476502674373045446852160486606292497360503469773453733196887405847255290082049086907512622059054542195889758031109222670849274793859539133318371244795543147611073276240066737934085191731810993201706776838934766764778739502174470268627820918553842225858306408301661862900358266857238210235802504351951472997919676524004784236376453347268364152648346245840573214241419937917242918602639810097866942392015404620153818671425739835074851396421139982713640679581178458198658692285968043243656709796000"), result);
        }




        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Add")]
        [TestCategory("B63404")]
        public void ConvertB63404ToBase10Tests_12()
        {
            var env = new CharMathEnvironment(true, true, true, true, true, true, false, false, false, false);

            Number a = env.GetNumber("ⵈ詑");


            Number result = a.ConvertToBase10();
            Assert.AreEqual(result.Environment.GetNumber("729751961"), result);
        }




    }
}




