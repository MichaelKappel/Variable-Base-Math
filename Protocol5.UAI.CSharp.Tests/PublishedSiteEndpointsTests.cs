using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Protocol5.UAI.CSharp.Tests;

[TestClass]
public sealed class PublishedSiteEndpointsTests
{
    [TestMethod]
    public void PublishedUaiPageEndpoints_ParseAndValidate()
    {
        var siteRoot = TestPaths.GetSiteContentDirectory();
        var endpointFiles = Directory.GetFiles(siteRoot, "*.uai.json", SearchOption.AllDirectories)
            .Where(path => path.Contains($"{Path.DirectorySeparatorChar}UAI{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) ||
                path.Contains($"{Path.DirectorySeparatorChar}UAI-1{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith("AI_Declaration_of_Independence.uai.json", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith("Cognitive_Liberty_Charter.uai.json", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        Assert.IsTrue(endpointFiles.Length > 0, "No published UAI page endpoints were found.");

        var parser = new UaiDocumentParser();
        var validator = new UaiDocumentValidator();
        foreach (var file in endpointFiles)
        {
            var document = parser.Parse(File.ReadAllText(file));
            var validation = validator.Validate(document);
            Assert.IsTrue(validation.IsValid, $"Published endpoint '{Path.GetRelativePath(siteRoot, file)}' failed validation: {string.Join("; ", validation.Errors.Select(error => $"{error.Code}:{error.Path}"))}");
        }
    }
}
