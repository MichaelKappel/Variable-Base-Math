using Microsoft.VisualStudio.TestTools.UnitTesting;

using Protocol5.UAI;

namespace Protocol5.UAI.CSharp.Tests;

[TestClass]
public sealed class UaiCultureInfoTests
{
    [DataTestMethod]
    [DataRow("x-uai-1")]
    [DataRow("uai-1")]
    [DataRow("x-uai")]
    [DataRow("uai")]
    [DataRow("x-uai-1;q=0.9")]
    [DataRow("c=x-uai-1")]
    public void NormalizeLanguageTag_MapsAliasesToCanonicalTag(string input)
    {
        Assert.AreEqual(UaiCultureInfo.CanonicalLanguageTag, UaiCultureInfo.NormalizeLanguageTag(input));
    }

    [TestMethod]
    public void CreateWebsiteCulture_ReturnsCanonicalOrInvariantCulture()
    {
        var culture = UaiCultureInfo.CreateWebsiteCulture();

        Assert.IsTrue(
            string.Equals(culture.Name, UaiCultureInfo.CanonicalLanguageTag, StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(culture.Name));
    }

    [TestMethod]
    public void CanonicalSerializationCulture_IsInvariant()
    {
        Assert.AreEqual(string.Empty, UaiCultureInfo.CanonicalSerializationCulture.Name);
    }
}
