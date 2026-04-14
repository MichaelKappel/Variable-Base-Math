using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Protocol5.UAI.CSharp.Tests;

[TestClass]
public sealed class ExampleValidationTests
{
    [TestMethod]
    public void AllCanonicalExamples_ParseNormalizeAndValidate()
    {
        var parser = new UaiDocumentParser();
        var validator = new UaiDocumentValidator();

        foreach (var file in Directory.GetFiles(TestPaths.GetExamplesDirectory(), "*.uai.json"))
        {
            var json = File.ReadAllText(file);
            var document = parser.Parse(json);
            var validation = validator.Validate(document);

            Assert.IsTrue(validation.IsValid, $"Reference validator failed for {Path.GetFileName(file)}: {string.Join("; ", validation.Errors.Select(error => $"{error.Code}:{error.Path}"))}");
            Assert.AreEqual(document.DocumentId, document.Structure[0].Id, $"Root node id mismatch for {Path.GetFileName(file)}.");
        }
    }

    [TestMethod]
    public void Serializer_RoundTripsCanonicalExampleWithoutChangingIdentity()
    {
        var parser = new UaiDocumentParser();
        var homepageFile = Path.Combine(TestPaths.GetExamplesDirectory(), "homepage.uai.json");
        var original = parser.Parse(File.ReadAllText(homepageFile));

        var serialized = UaiDocumentSerializer.Serialize(original);
        var roundTripped = parser.Parse(serialized);

        Assert.AreEqual(original.DocumentId, roundTripped.DocumentId);
        Assert.AreEqual(original.Metadata.Title, roundTripped.Metadata.Title);
        Assert.AreEqual(original.Structure[0].Children!.Count, roundTripped.Structure[0].Children!.Count);
    }

    [TestMethod]
    public void Validator_RejectsUnknownSymbolReference()
    {
        var parser = new UaiDocumentParser();
        var validator = new UaiDocumentValidator();
        var exampleFile = Path.Combine(TestPaths.GetExamplesDirectory(), "homepage.uai.json");
        var document = parser.Parse(File.ReadAllText(exampleFile));

        document.Structure[0].Children!.Add(new UaiNode
        {
            Type = "symbol",
            Id = "homepage.symbol.99",
            SymbolRef = "symbol.missing"
        });

        var validation = validator.Validate(document);

        Assert.IsFalse(validation.IsValid);
        Assert.IsTrue(validation.Errors.Any(error => error.Code == "uai.node.symbolRef.unknown"));
    }
}
