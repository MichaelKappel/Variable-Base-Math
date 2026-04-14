using System.Diagnostics;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Protocol5.UAI.CSharp.Tests;

[TestClass]
public sealed class SiteExporterCliTests
{
    [TestMethod]
    public void SiteExporterCli_RelativeManifestPaths_ExportsValidDocument()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "Protocol5.UAI.SiteExporter.Tests", Guid.NewGuid().ToString("N"));
        var inputRoot = Path.Combine(tempRoot, "input");
        var manifestRoot = Path.Combine(tempRoot, "manifests");
        var outputRoot = Path.Combine(tempRoot, "output");
        Directory.CreateDirectory(inputRoot);
        Directory.CreateDirectory(manifestRoot);

        var htmlPath = Path.Combine(inputRoot, "hello.html");
        var manifestPath = Path.Combine(manifestRoot, "export.json");
        var outputPath = Path.Combine(outputRoot, "hello.uai.json");

        File.WriteAllText(htmlPath, "<html lang=\"en\"><body><h1>Hello CLI</h1><p>Export me.</p></body></html>", Encoding.UTF8);
        File.WriteAllText(manifestPath, """
{
  "generatedAt": "2026-04-13T00:00:00Z",
  "pages": [
    {
      "inputHtmlPath": "../input/hello.html",
      "outputJsonPath": "../output/hello.uai.json",
      "sourceUri": "https://example.org/cli-export",
      "documentId": "cli-export",
      "pageType": "article",
      "language": "en",
      "siteName": "Example Site"
    }
  ]
}
""", Encoding.UTF8);

        try
        {
            var result = RunSiteExporter($"\"{manifestPath}\"");

            Assert.AreEqual(0, result.ExitCode, result.Output);
            StringAssert.Contains(result.Output, "https://example.org/cli-export ->");
            Assert.IsTrue(File.Exists(outputPath), "The exporter did not create the expected output file.");

            var document = new UaiDocumentParser().Parse(File.ReadAllText(outputPath));
            var validation = new UaiDocumentValidator().Validate(document);
            Assert.IsTrue(validation.IsValid, result.Output + Environment.NewLine + string.Join("; ", validation.Errors.Select(error => $"{error.Code}:{error.Path}")));
            Assert.AreEqual("cli-export", document.DocumentId);
            Assert.AreEqual("article", document.Metadata.PageType);
            Assert.AreEqual("2026-04-13T00:00:00.0000000+00:00", document.Source.RetrievedAt);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }
    [TestMethod]
    public void SiteExporterCli_WithoutGeneratedAt_UsesStableSourceFileTimestamp()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "Protocol5.UAI.SiteExporter.Tests", Guid.NewGuid().ToString("N"));
        var inputRoot = Path.Combine(tempRoot, "input");
        var manifestRoot = Path.Combine(tempRoot, "manifests");
        var outputRoot = Path.Combine(tempRoot, "output");
        Directory.CreateDirectory(inputRoot);
        Directory.CreateDirectory(manifestRoot);

        var htmlPath = Path.Combine(inputRoot, "hello.html");
        var manifestPath = Path.Combine(manifestRoot, "export.json");
        var outputPath = Path.Combine(outputRoot, "hello.uai.json");
        var stableTimestamp = new DateTimeOffset(2026, 04, 13, 00, 00, 00, TimeSpan.Zero);

        File.WriteAllText(htmlPath, "<html lang=\"en\"><body><h1>Hello CLI</h1><p>Export me.</p></body></html>", Encoding.UTF8);
        File.SetLastWriteTimeUtc(htmlPath, stableTimestamp.UtcDateTime);
        File.WriteAllText(manifestPath, """
{
  "pages": [
    {
      "inputHtmlPath": "../input/hello.html",
      "outputJsonPath": "../output/hello.uai.json",
      "sourceUri": "https://example.org/cli-export-stable",
      "documentId": "cli-export-stable",
      "pageType": "article",
      "language": "en",
      "siteName": "Example Site"
    }
  ]
}
""", Encoding.UTF8);

        try
        {
            var first = RunSiteExporter($"\"{manifestPath}\"");
            Assert.AreEqual(0, first.ExitCode, first.Output);
            var firstDocument = new UaiDocumentParser().Parse(File.ReadAllText(outputPath));

            var second = RunSiteExporter($"\"{manifestPath}\"");
            Assert.AreEqual(0, second.ExitCode, second.Output);
            var secondDocument = new UaiDocumentParser().Parse(File.ReadAllText(outputPath));

            Assert.AreEqual(stableTimestamp.ToString("O"), firstDocument.Source.RetrievedAt);
            Assert.AreEqual(firstDocument.Source.RetrievedAt, secondDocument.Source.RetrievedAt);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }

    [TestMethod]
    public void SiteExporterCli_InvalidManifest_ReturnsFailure()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "Protocol5.UAI.SiteExporter.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);
        var manifestPath = Path.Combine(tempRoot, "invalid-export.json");
        File.WriteAllText(manifestPath, """
{
  "pages": [
    {
      "inputHtmlPath": "page.html",
      "outputJsonPath": "page.uai.json",
      "sourceUri": "https://example.org/invalid",
      "documentId": "invalid"
    }
  ]
}
""", Encoding.UTF8);

        try
        {
            var result = RunSiteExporter($"\"{manifestPath}\"");

            Assert.AreEqual(1, result.ExitCode, result.Output);
            StringAssert.Contains(result.Output, "Each export page must define inputHtmlPath, outputJsonPath, sourceUri, documentId, and pageType.");
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }

    private static (int ExitCode, string Output) RunSiteExporter(string arguments)
    {
        var projectPath = TestPaths.GetSiteExporterProjectPath();
        var repoRoot = TestPaths.GetRepoRoot();
        var startInfo = new ProcessStartInfo("dotnet", $"run --project \"{projectPath}\" -- {arguments}")
        {
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(startInfo)!;
        process.WaitForExit();

        var output = process.StandardOutput.ReadToEnd() + Environment.NewLine + process.StandardError.ReadToEnd();
        return (process.ExitCode, output);
    }
}