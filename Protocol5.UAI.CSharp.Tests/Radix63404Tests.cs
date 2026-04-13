using System.Numerics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Protocol5.UAI;

namespace Protocol5.UAI.CSharp.Tests;

[TestClass]
public sealed class Radix63404Tests
{
    [TestMethod]
    public void Encode_UsesKnownReferenceValues()
    {
        Assert.AreEqual("J", Radix63404.Encode(new BigInteger(41)));
        Assert.AreEqual("ᙖ", Radix63404.Encode(new BigInteger(5651)));
        Assert.AreEqual("Ⴤ绠", Radix63404.Encode(new BigInteger(267914296)));
    }

    [TestMethod]
    public void Decode_RoundTripsEncodedValue()
    {
        var originalValue = BigInteger.Parse("123456789012345678901234567890");
        var encoded = Radix63404.Encode(originalValue);
        var decoded = Radix63404.Decode(encoded);

        Assert.AreEqual(originalValue, decoded);
    }

    [TestMethod]
    public void Zero_EncodesToFirstLegalDigit()
    {
        Assert.AreEqual("!", Radix63404.Encode(BigInteger.Zero));
    }
}
