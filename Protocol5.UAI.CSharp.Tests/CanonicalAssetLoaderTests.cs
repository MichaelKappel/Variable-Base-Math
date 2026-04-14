using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Protocol5.UAI.CSharp.Tests;

[TestClass]
public sealed class CanonicalAssetLoaderTests
{
    [TestMethod]
    public void CanonicalAssetLoader_LoadsEmbeddedArtifactsAndExamples()
    {
        var loader = new UaiCanonicalAssetLoader();

        using var discovery = loader.LoadProtocolDiscoveryJson();
        using var examplesIndex = loader.LoadExamplesIndexJson();
        using var registry = loader.LoadRegistryJson();
        using var symbolRegistry = loader.LoadSymbolRegistryJson();
        using var schema = loader.LoadSchemaJson();

        Assert.AreEqual(UaiConstants.SpecName, discovery.RootElement.GetProperty("spec").GetString());
        Assert.AreEqual(UaiConstants.SpecName, examplesIndex.RootElement.GetProperty("spec").GetString());
        Assert.AreEqual(UaiConstants.SpecName, registry.RootElement.GetProperty("spec").GetString());
        Assert.AreEqual(UaiConstants.SpecName, symbolRegistry.RootElement.GetProperty("spec").GetString());
        Assert.AreEqual(UaiConstants.CanonicalSchemaId, schema.RootElement.GetProperty("$id").GetString());

        var exampleNames = loader.GetExampleFileNames().OrderBy(name => name, StringComparer.Ordinal).ToArray();
        CollectionAssert.AreEqual(UaiConstants.GetEmbeddedExampleFileNames().OrderBy(name => name, StringComparer.Ordinal).ToArray(), exampleNames);

        var homepage = loader.LoadExampleDocument("homepage.uai.json");
        Assert.AreEqual("homepage", homepage.DocumentId);
        Assert.AreEqual("homepage", homepage.Structure[0].Id);
    }

    [TestMethod]
    public void SchemaValidator_ValidatesCanonicalExampleAndReturnsCombinedResult()
    {
        var loader = new UaiCanonicalAssetLoader();
        var validator = new UaiSchemaValidator();
        var json = loader.LoadExampleText("homepage.uai.json");

        var schemaValidation = validator.ValidateJson(json);
        var canonicalValidation = validator.ValidateCanonicalJson(json);

        Assert.IsTrue(schemaValidation.IsValid, string.Join("; ", schemaValidation.Errors.Select(error => $"{error.Keyword}:{error.InstanceLocation}")));
        Assert.IsTrue(canonicalValidation.IsValid, string.Join("; ", canonicalValidation.Semantic.Errors.Select(error => $"{error.Code}:{error.Path}")));
    }

    [TestMethod]
    public void SchemaValidator_InvalidJson_ReturnsSchemaErrors()
    {
        var validator = new UaiSchemaValidator();

        var validation = validator.ValidateJson("{}");

        Assert.IsFalse(validation.IsValid);
        Assert.IsTrue(validation.Errors.Count > 0);
    }
}