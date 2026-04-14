using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Protocol5.UAI.CSharp.Tests;

[TestClass]
public sealed class UaiHtmlExporterTests
{
    [TestMethod]
    public void Export_ProducesValidatedJsonWithComputedContentHash()
    {
        const string html = "<html lang=\"en\"><body><h1>Hello</h1><p>World</p></body></html>";
        var exporter = new UaiHtmlExporter();

        var export = exporter.Export(html, new UaiHtmlTranslationOptions
        {
            SourceUri = "https://example.org/hello",
            DocumentId = "hello-doc",
            PageType = "article",
            SiteName = "Example"
        });

        Assert.IsTrue(export.Validation.IsValid);
        Assert.IsTrue(export.Document.Source.ContentHash!.StartsWith("sha256:", StringComparison.Ordinal));

        var roundTripped = new UaiDocumentParser().Parse(export.Json);
        var validation = new UaiDocumentValidator().Validate(roundTripped);
        Assert.IsTrue(validation.IsValid, string.Join("; ", validation.Errors.Select(error => $"{error.Code}:{error.Path}")));
        Assert.AreEqual("hello-doc", roundTripped.DocumentId);
        Assert.AreEqual("article", roundTripped.Metadata.PageType);
    }

    [TestMethod]
    public void ExportToFile_WritesJsonDocumentToDisk()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "Protocol5.UAI.CSharp.Export", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        var inputPath = Path.Combine(tempRoot, "page.html");
        var outputPath = Path.Combine(tempRoot, "page.uai.json");
        File.WriteAllText(inputPath, "<html><body><h1>Disk export</h1><p>Ready</p></body></html>");

        try
        {
            var exporter = new UaiHtmlExporter();
            var export = exporter.ExportToFile(inputPath, outputPath, new UaiHtmlTranslationOptions
            {
                SourceUri = "https://example.org/disk-export",
                DocumentId = "disk-export",
                PageType = "generic"
            });

            Assert.IsTrue(File.Exists(outputPath));
            Assert.IsTrue(export.Validation.IsValid);

            var json = File.ReadAllText(outputPath);
            var document = new UaiDocumentParser().Parse(json);
            Assert.AreEqual("disk-export", document.DocumentId);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }
}