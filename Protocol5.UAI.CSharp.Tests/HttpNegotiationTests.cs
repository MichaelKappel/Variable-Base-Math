using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Protocol5.UAI.CSharp.Tests;

[TestClass]
public sealed class HttpNegotiationTests
{
    [TestMethod]
    public void AcceptsUaiJson_DetectsCanonicalMediaType()
    {
        Assert.IsTrue(UaiHttpNegotiation.AcceptsUaiJson("application/uai+json; version=1.0.0, application/json"));
        Assert.IsFalse(UaiHttpNegotiation.AcceptsUaiJson("text/html, application/json"));
    }

    [TestMethod]
    public void LegacyHeader_ParsesKnownValues()
    {
        Assert.AreEqual("1.0.0", UaiHttpNegotiation.TryParseLegacyVersion("version=1.0.0"));
        Assert.AreEqual("1.0.0", UaiHttpNegotiation.TryParseLegacyVersion("1.0"));
        Assert.IsNull(UaiHttpNegotiation.TryParseLegacyVersion("off"));
    }
}
