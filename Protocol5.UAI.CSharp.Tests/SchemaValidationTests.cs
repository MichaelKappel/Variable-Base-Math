using System.Text.Json.Nodes;

using Json.Schema;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Protocol5.UAI.CSharp.Tests;

[TestClass]
public sealed class SchemaValidationTests
{
    [TestMethod]
    public void EmbeddedSchema_MatchesRepositorySchema()
    {
        var repoSchema = File.ReadAllText(TestPaths.GetSchemaPath());
        var embeddedSchema = UaiConstants.GetEmbeddedSchemaText();

        Assert.AreEqual(repoSchema.Replace("\r\n", "\n"), embeddedSchema.Replace("\r\n", "\n"));
    }

    [TestMethod]
    public void EmbeddedRegistry_MatchesRepositoryRegistry()
    {
        var repoRegistry = File.ReadAllText(TestPaths.GetRegistryPath());
        var embeddedRegistry = UaiConstants.GetEmbeddedRegistryText();

        Assert.AreEqual(repoRegistry.Replace("\r\n", "\n"), embeddedRegistry.Replace("\r\n", "\n"));
    }

    [TestMethod]
    public void EmbeddedDiscoveryDocuments_MatchRepositoryDocuments()
    {
        var repoDiscovery = File.ReadAllText(TestPaths.GetProtocolDiscoveryPath());
        var embeddedDiscovery = UaiConstants.GetEmbeddedProtocolDiscoveryText();
        Assert.AreEqual(repoDiscovery.Replace("\r\n", "\n"), embeddedDiscovery.Replace("\r\n", "\n"));

        var repoExamplesIndex = File.ReadAllText(TestPaths.GetExamplesIndexPath());
        var embeddedExamplesIndex = UaiConstants.GetEmbeddedExamplesIndexText();
        Assert.AreEqual(repoExamplesIndex.Replace("\r\n", "\n"), embeddedExamplesIndex.Replace("\r\n", "\n"));

        var repoSymbolRegistry = File.ReadAllText(TestPaths.GetSymbolRegistryPath());
        var embeddedSymbolRegistry = UaiConstants.GetEmbeddedSymbolRegistryText();
        Assert.AreEqual(repoSymbolRegistry.Replace("\r\n", "\n"), embeddedSymbolRegistry.Replace("\r\n", "\n"));
    }

    [TestMethod]
    public void AllCanonicalExamples_ValidateAgainstJsonSchema()
    {
        var schema = JsonSchema.FromText(File.ReadAllText(TestPaths.GetSchemaPath()));
        var exampleFiles = Directory.GetFiles(TestPaths.GetExamplesDirectory(), "*.uai.json");

        foreach (var file in exampleFiles)
        {
            var json = JsonNode.Parse(File.ReadAllText(file));
            var evaluation = schema.Evaluate(json, new EvaluationOptions { OutputFormat = OutputFormat.List });

            Assert.IsTrue(evaluation.IsValid, $"Schema validation failed for {Path.GetFileName(file)}.");
        }
    }

    [TestMethod]
    public void CanonicalRegistry_ReferencesExistingArtifacts()
    {
        var registry = JsonNode.Parse(File.ReadAllText(TestPaths.GetRegistryPath()))!.AsObject();
        Assert.AreEqual("UAI-1", registry["spec"]?.GetValue<string>());
        Assert.AreEqual("1.0.0", registry["version"]?.GetValue<string>());

        var canonicalArtifacts = registry["canonicalArtifacts"]!.AsObject();
        var canonicalArtifactNames = canonicalArtifacts.Select(artifact => artifact.Key).ToArray();
        CollectionAssert.Contains(canonicalArtifactNames, "translatorContract");
        CollectionAssert.Contains(canonicalArtifactNames, "integrationContracts");
        CollectionAssert.Contains(canonicalArtifactNames, "websiteExportContract");
        CollectionAssert.Contains(canonicalArtifactNames, "registryResolutionContract");
        CollectionAssert.Contains(canonicalArtifactNames, "radix63404Contract");

        foreach (var artifact in canonicalArtifacts)
        {
            var repoPath = artifact.Value!["repoPath"]?.GetValue<string>();
            if (!string.IsNullOrWhiteSpace(repoPath))
            {
                Assert.IsTrue(File.Exists(Path.Combine(TestPaths.GetRepoRoot(), repoPath)), $"Missing artifact '{repoPath}'.");
            }
        }

        var examples = registry["examples"]!.AsArray();
        Assert.AreEqual(10, examples.Count);
        foreach (var exampleNode in examples)
        {
            var example = exampleNode!.AsObject();
            var repoPath = example["repoPath"]!.GetValue<string>();
            Assert.IsTrue(File.Exists(Path.Combine(TestPaths.GetRepoRoot(), repoPath)), $"Missing example '{repoPath}'.");
        }

        var nodeTypes = registry["documentModel"]!["nodeTypes"]!.AsArray().Select(node => node!.GetValue<string>()).ToArray();
        CollectionAssert.AreEquivalent(UaiConstants.NodeTypes.ToArray(), nodeTypes);

        var pageTypes = registry["documentModel"]!["pageTypes"]!.AsArray().Select(node => node!.GetValue<string>()).ToArray();
        CollectionAssert.AreEquivalent(UaiConstants.PageTypes.ToArray(), pageTypes);
    }

    [TestMethod]
    public void DiscoveryDocuments_ReferenceCanonicalMachineEndpoints()
    {
        var discovery = JsonNode.Parse(File.ReadAllText(TestPaths.GetProtocolDiscoveryPath()))!.AsObject();
        Assert.AreEqual(UaiConstants.CanonicalMachineSpecPublicPath, discovery["machineEndpoints"]!["protocol"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalExamplesIndexPublicPath, discovery["machineEndpoints"]!["examplesIndex"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalRegistryIndexPublicPath, discovery["machineEndpoints"]!["registry"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalSymbolsRegistryPublicPath, discovery["machineEndpoints"]!["symbols"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalSchemaIndexPublicPath, discovery["machineEndpoints"]!["schema"]?.GetValue<string>());

        var examplesIndex = JsonNode.Parse(File.ReadAllText(TestPaths.GetExamplesIndexPath()))!.AsObject();
        var examples = examplesIndex["examples"]!.AsArray();
        Assert.AreEqual(10, examples.Count);

        foreach (var exampleNode in examples)
        {
            var publicPath = exampleNode!["publicPath"]!.GetValue<string>();
            Assert.IsTrue(publicPath.StartsWith(UaiConstants.CanonicalExamplesPublicPath + "/", StringComparison.Ordinal), $"Unexpected example path '{publicPath}'.");
        }

        var symbols = JsonNode.Parse(File.ReadAllText(TestPaths.GetSymbolRegistryPath()))!["symbols"]!.AsArray();
        Assert.AreEqual(2, symbols.Count);
    }
}
