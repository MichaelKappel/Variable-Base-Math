using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using VariableBase.Mathematics;

namespace VariableBase.Math.Tests.NumberTests.B245
{
    [TestClass]
    public class B245DivideAndMultiplyTests
    {

        [TestMethod]
        [TestCategory("Number")]
        [TestCategory("Add")]
        [TestCategory("B245")]
        public void AB_DE_Test_1()
        {
            var env = new MathEnvironment("電电電買买買車车車紅红紅無无無東东東馬马馬風风風時时時鳥鸟鳥語语語頭头頭魚鱼魚園园園長长長島岛島愛爱愛紙纸紙書书書見见見假假仮佛佛仏德德徳拜拜拝黑黑黒冰冰氷兔兔兎妒妒妬每每毎壤壤壌步步歩巢巢巣惠惠恵鞋鞋靴莓莓苺圓圆円聽听聴實实実證证証龍龙竜賣卖売龜龟亀藝艺芸戰战戦繩绳縄繪绘絵鐵铁鉄圖图図團团団轉转転廣广広惡恶悪豐丰豊腦脑脳雜杂雑壓压圧雞鸡鶏價价価樂乐楽氣气気廳厅庁發发発勞劳労劍剑剣歲岁歳權权権燒烧焼贊赞賛兩两両譯译訳觀观観營营営處处処齒齿歯驛驿駅櫻樱桜產产産藥药薬讀读読顏颜顔畫画画聲声声學学学體体体點点点麥麦麦蟲虫虫舊旧旧會会会萬万万盜盗盗寶宝宝國国国醫医医雙双双晝昼昼觸触触來来来黃黄黄區区区");

            Number cNumber = env.GetNumber("黄廣広广来車馬厅賛桜绳樂齒转営紅学巢赞權馬萬麦步歩药拝區证豐風书時藝黑畫體繩處団焼两鸟");

            Number a = env.GetNumber("電買买買車车車紅红紅無无無東东東馬马馬風风風時时時");
            Number b = env.GetNumber("悪豐丰豊腦脑脳雜杂雑壓压圧雞鸡鶏價价価樂");

            Number c = a * b;

            Assert.IsTrue(c == cNumber);

            Number d = c / b;
            Assert.IsTrue(a == d);

            Number e = c / a;
            Assert.IsTrue(b == e);
        }
    }
}


