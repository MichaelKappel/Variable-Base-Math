using System.Text.Json.Nodes;
using System.Xml.Linq;

using Json.Schema;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Protocol5.UAI.CSharp.Tests;

[TestClass]
public sealed class CanonicalArtifactAuditTests
{
    [TestMethod]
    public void Schema_UsesCanonicalProtocol5IdentityAndVocabulary()
    {
        var schema = JsonNode.Parse(File.ReadAllText(TestPaths.GetSchemaPath()))!.AsObject();

        Assert.AreEqual(UaiConstants.CanonicalSchemaId, schema["$id"]?.GetValue<string>());

        var requiredFields = schema["required"]!.AsArray()
            .Select(node => node!.GetValue<string>())
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
        var expectedFields = UaiConstants.TopLevelFieldOrder
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
        CollectionAssert.AreEqual(expectedFields, requiredFields);

        var pageTypes = schema["$defs"]!["metadata"]!["properties"]!["pageType"]!["enum"]!.AsArray()
            .Select(node => node!.GetValue<string>())
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
        CollectionAssert.AreEqual(UaiConstants.PageTypes.OrderBy(name => name, StringComparer.Ordinal).ToArray(), pageTypes);

        var nodeTypes = schema["$defs"]!["node"]!["properties"]!["type"]!["enum"]!.AsArray()
            .Select(node => node!.GetValue<string>())
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
        CollectionAssert.AreEqual(UaiConstants.NodeTypes.OrderBy(name => name, StringComparer.Ordinal).ToArray(), nodeTypes);
    }

    [TestMethod]
    public void DiscoveryRegistryAndPackageProject_AgreeOnAuthorityAndVersion()
    {
        var discovery = JsonNode.Parse(File.ReadAllText(TestPaths.GetProtocolDiscoveryPath()))!.AsObject();
        var examplesIndex = JsonNode.Parse(File.ReadAllText(TestPaths.GetExamplesIndexPath()))!.AsObject();
        var registry = JsonNode.Parse(File.ReadAllText(TestPaths.GetRegistryPath()))!.AsObject();
        var symbolRegistry = JsonNode.Parse(File.ReadAllText(TestPaths.GetSymbolRegistryPath()))!.AsObject();
        var packageVersion = XDocument.Load(TestPaths.GetPackageProjectPath())
            .Descendants("Version")
            .Select(element => element.Value.Trim())
            .First(value => !string.IsNullOrWhiteSpace(value));

        Assert.AreEqual(UaiConstants.SpecName, discovery["spec"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.SpecName, examplesIndex["spec"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.SpecName, registry["spec"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.SpecName, symbolRegistry["spec"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalPublicOrigin, registry["canonicalPublicOrigin"]?.GetValue<string>());

        Assert.AreEqual(UaiConstants.CurrentDocumentVersion, discovery["version"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CurrentDocumentVersion, examplesIndex["version"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CurrentRegistryVersion, registry["version"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CurrentRegistryVersion, symbolRegistry["version"]?.GetValue<string>());
        Assert.AreEqual(packageVersion, discovery["package"]!["version"]?.GetValue<string>());
        Assert.AreEqual(packageVersion, registry["package"]!["version"]?.GetValue<string>());

        Assert.AreEqual(UaiConstants.CanonicalMachineSpecPublicPath, discovery["machineEndpoints"]!["protocol"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalExamplesIndexPublicPath, discovery["machineEndpoints"]!["examplesIndex"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalRegistryIndexPublicPath, discovery["machineEndpoints"]!["registry"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalSymbolsRegistryPublicPath, discovery["machineEndpoints"]!["symbols"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalSchemaIndexPublicPath, discovery["machineEndpoints"]!["schema"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalRegistryPublicPath, discovery["canonicalArtifacts"]!["registry"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalSchemaPublicPath, discovery["canonicalArtifacts"]!["schema"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalTypesPublicPath, discovery["canonicalArtifacts"]!["types"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalExamplesPublicPath, discovery["canonicalArtifacts"]!["examplesDirectory"]?.GetValue<string>());

        Assert.AreEqual(UaiConstants.CanonicalExamplesIndexPublicPath, registry["canonicalPaths"]!["machineExamplesFile"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalExamplesPublicPath, registry["canonicalPaths"]!["examplesDirectory"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalRegistryIndexPublicPath, registry["canonicalPaths"]!["registryAliasFile"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalSymbolsRegistryPublicPath, registry["canonicalPaths"]!["symbolRegistryFile"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalRegistryPublicPath, registry["canonicalPaths"]!["registryFile"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalSchemaIndexPublicPath, registry["canonicalPaths"]!["schemaAliasFile"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalSchemaPublicPath, registry["canonicalPaths"]!["schemaFile"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalTypesPublicPath, registry["canonicalPaths"]!["typesFile"]?.GetValue<string>());

        Assert.AreEqual(UaiConstants.CanonicalRegistryPublicPath, examplesIndex["registry"]?.GetValue<string>());
        Assert.AreEqual(UaiConstants.CanonicalRegistryPublicPath, symbolRegistry["registry"]?.GetValue<string>());
    }

    [TestMethod]
    public void RepositoryExamplesEmbeddedExamplesRegistryAndExamplesIndex_AgreeExactly()
    {
        var repoFileNames = Directory.GetFiles(TestPaths.GetExamplesDirectory(), "*.uai.json")
            .Select(Path.GetFileName)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray()!;
        var embeddedFileNames = UaiConstants.GetEmbeddedExampleFileNames()
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
        CollectionAssert.AreEqual(repoFileNames, embeddedFileNames);

        var parser = new UaiDocumentParser();
        var validator = new UaiDocumentValidator();
        var registryExamples = JsonNode.Parse(File.ReadAllText(TestPaths.GetRegistryPath()))!["examples"]!.AsArray()
            .Select(node => ParseRegistryExample(node!.AsObject()))
            .OrderBy(example => example.Id, StringComparer.Ordinal)
            .ToArray();
        var indexExamples = JsonNode.Parse(File.ReadAllText(TestPaths.GetExamplesIndexPath()))!["examples"]!.AsArray()
            .Select(node => ParseExamplesIndexEntry(node!.AsObject()))
            .OrderBy(example => example.Id, StringComparer.Ordinal)
            .ToArray();

        Assert.AreEqual(repoFileNames.Length, registryExamples.Length);
        Assert.AreEqual(repoFileNames.Length, indexExamples.Length);
        CollectionAssert.AreEqual(repoFileNames, registryExamples.Select(example => Path.GetFileName(example.RepoPath)).OrderBy(name => name, StringComparer.Ordinal).ToArray());
        CollectionAssert.AreEqual(repoFileNames, indexExamples.Select(example => Path.GetFileName(example.PublicPath)).OrderBy(name => name, StringComparer.Ordinal).ToArray());

        var indexById = indexExamples.ToDictionary(example => example.Id, StringComparer.Ordinal);
        foreach (var registryExample in registryExamples)
        {
            Assert.IsTrue(indexById.TryGetValue(registryExample.Id, out var indexExample), $"Missing examples-index entry for '{registryExample.Id}'.");

            var fileName = Path.GetFileName(registryExample.RepoPath);
            Assert.AreEqual($"{UaiConstants.CanonicalExamplesPublicPath}/{fileName}", registryExample.PublicPath);
            Assert.AreEqual(registryExample.PublicPath, indexExample.PublicPath);
            Assert.AreEqual(registryExample.Title, indexExample.Title);
            Assert.AreEqual(registryExample.PageType, indexExample.PageType);

            var document = parser.Parse(File.ReadAllText(Path.Combine(TestPaths.GetExamplesDirectory(), fileName)));
            var validation = validator.Validate(document);
            Assert.IsTrue(validation.IsValid, $"Canonical example '{fileName}' failed validation: {string.Join("; ", validation.Errors.Select(error => $"{error.Code}:{error.Path}"))}");
            Assert.AreEqual(registryExample.DocumentId, document.DocumentId);
            Assert.AreEqual(registryExample.Title, document.Metadata.Title);
            Assert.AreEqual(registryExample.PageType, document.Metadata.PageType);
        }
    }

    [TestMethod]
    public void CanonicalExamples_RoundTripThroughParserAndSerializer()
    {
        var parser = new UaiDocumentParser();
        var validator = new UaiDocumentValidator();
        var schema = JsonSchema.FromText(File.ReadAllText(TestPaths.GetSchemaPath()));

        foreach (var file in Directory.GetFiles(TestPaths.GetExamplesDirectory(), "*.uai.json").OrderBy(path => path, StringComparer.Ordinal))
        {
            var originalJson = File.ReadAllText(file);
            var evaluation = schema.Evaluate(JsonNode.Parse(originalJson), new EvaluationOptions { OutputFormat = OutputFormat.List });
            Assert.IsTrue(evaluation.IsValid, $"Schema evaluation failed for '{Path.GetFileName(file)}'.");

            var document = parser.Parse(originalJson);
            var validation = validator.Validate(document);
            Assert.IsTrue(validation.IsValid, $"Validation failed for '{Path.GetFileName(file)}': {string.Join("; ", validation.Errors.Select(error => $"{error.Code}:{error.Path}"))}");

            var roundTrippedJson = UaiDocumentSerializer.Serialize(document);
            var roundTrippedDocument = parser.Parse(roundTrippedJson);
            var roundTripValidation = validator.Validate(roundTrippedDocument);
            Assert.IsTrue(roundTripValidation.IsValid, $"Round-trip validation failed for '{Path.GetFileName(file)}': {string.Join("; ", roundTripValidation.Errors.Select(error => $"{error.Code}:{error.Path}"))}");
            Assert.AreEqual(document.DocumentId, roundTrippedDocument.DocumentId);
            Assert.AreEqual(document.Metadata.Title, roundTrippedDocument.Metadata.Title);
            Assert.AreEqual(document.Metadata.PageType, roundTrippedDocument.Metadata.PageType);
        }
    }

    [TestMethod]
    public void SymbolRegistry_IsBackedByCanonicalExampleDefinitions()
    {
        var parser = new UaiDocumentParser();
        var exampleDocuments = Directory.GetFiles(TestPaths.GetExamplesDirectory(), "*.uai.json")
            .OrderBy(path => path, StringComparer.Ordinal)
            .Select(path => parser.Parse(File.ReadAllText(path)))
            .ToArray();
        var exampleDefinitions = exampleDocuments
            .SelectMany(document => document.Symbols)
            .GroupBy(symbol => symbol.Id, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group =>
                {
                    var first = group.First();
                    foreach (var duplicate in group.Skip(1))
                    {
                        Assert.AreEqual(first.Label, duplicate.Label, $"Symbol definition '{group.Key}' drifted across canonical examples.");
                        Assert.AreEqual(first.VisualForm, duplicate.VisualForm, $"Symbol definition '{group.Key}' drifted across canonical examples.");
                    }

                    return first;
                },
                StringComparer.Ordinal);
        var examplePublicPaths = Directory.GetFiles(TestPaths.GetExamplesDirectory(), "*.uai.json")
            .Select(path => $"{UaiConstants.CanonicalExamplesPublicPath}/{Path.GetFileName(path)}")
            .ToHashSet(StringComparer.Ordinal);

        var symbolRegistry = JsonNode.Parse(File.ReadAllText(TestPaths.GetSymbolRegistryPath()))!.AsObject();
        var registrySymbols = symbolRegistry["symbols"]!.AsArray()
            .Select(node => node!.AsObject())
            .ToArray();
        var registrySymbolIds = registrySymbols
            .Select(symbol => symbol["id"]!.GetValue<string>())
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        CollectionAssert.AreEqual(exampleDefinitions.Keys.OrderBy(id => id, StringComparer.Ordinal).ToArray(), registrySymbolIds);

        foreach (var symbol in registrySymbols)
        {
            var symbolId = symbol["id"]!.GetValue<string>();
            Assert.IsTrue(exampleDefinitions.TryGetValue(symbolId, out var exampleDefinition), $"Symbol registry entry '{symbolId}' is not backed by a canonical example definition.");
            Assert.AreEqual(exampleDefinition.Label, symbol["label"]?.GetValue<string>());
            Assert.AreEqual(exampleDefinition.VisualForm, symbol["visualForm"]?.GetValue<string>());

            var evidence = symbol["sourceEvidence"]!.AsArray();
            Assert.IsTrue(evidence.Count > 0, $"Symbol registry entry '{symbolId}' does not declare source evidence.");
            foreach (var evidenceNode in evidence)
            {
                var publicPath = evidenceNode!["value"]!.GetValue<string>();
                Assert.IsTrue(examplePublicPaths.Contains(publicPath), $"Symbol registry entry '{symbolId}' points at unknown example evidence '{publicPath}'.");
            }
        }
    }

    [TestMethod]
    public void PublishedSiteCanonicalArtifactCopies_MatchRepositorySources()
    {
        var siteRoot = TestPaths.GetSiteContentDirectory();

        AssertFileTextMatches(TestPaths.GetProtocolDiscoveryPath(), Path.Combine(siteRoot, "UAI-1.json"));
        AssertFileTextMatches(TestPaths.GetExamplesIndexPath(), Path.Combine(siteRoot, "UAI-1-examples.json"));
        AssertFileTextMatches(TestPaths.GetRegistryPath(), Path.Combine(siteRoot, "registry", "uai-1.json"));
        AssertFileTextMatches(TestPaths.GetRegistryPath(), Path.Combine(siteRoot, "UAI-1", "registry", "uai-1.registry.json"));
        AssertFileTextMatches(TestPaths.GetSymbolRegistryPath(), Path.Combine(siteRoot, "registry", "symbols.json"));
        AssertFileTextMatches(TestPaths.GetSchemaPath(), Path.Combine(siteRoot, "schema", "uai-1.schema.json"));
        AssertFileTextMatches(TestPaths.GetSchemaPath(), Path.Combine(siteRoot, "UAI-1", "schema", "uai-1.schema.json"));
        AssertFileTextMatches(TestPaths.GetTypesPath(), Path.Combine(siteRoot, "UAI-1", "schema", "uai-1.types.ts"));

        foreach (var examplePath in Directory.GetFiles(TestPaths.GetExamplesDirectory(), "*.uai.json").OrderBy(path => path, StringComparer.Ordinal))
        {
            AssertFileTextMatches(examplePath, Path.Combine(siteRoot, "UAI-1", "examples", Path.GetFileName(examplePath)));
        }
    }

    [TestMethod]
    public void PublishedSiteDownloadAliases_IncludeCanonicalAndLegacyBundleNames()
    {
        var siteRoot = TestPaths.GetSiteContentDirectory();
        var canonicalZipPath = Path.Combine(siteRoot, "downloads", "UAI-1-Package.zip");
        var legacyZipPath = Path.Combine(siteRoot, "downloads", "protocol5-uai-1-csharp-web-starter.zip");

        Assert.IsTrue(File.Exists(canonicalZipPath), "The canonical UAI package ZIP is missing from SiteContent/downloads.");
        Assert.IsTrue(File.Exists(legacyZipPath), "The legacy starter ZIP compatibility copy is missing from SiteContent/downloads.");
        Assert.AreEqual(
            GetFileHash(canonicalZipPath),
            GetFileHash(legacyZipPath),
            "The canonical and legacy ZIP downloads should publish the same bundle bytes.");
    }

    private static string GetFileHash(string path)
    {
        return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(path)));
    }

    private static RegistryExample ParseRegistryExample(JsonObject example)
    {
        return new RegistryExample(
            example["id"]!.GetValue<string>(),
            example["documentId"]!.GetValue<string>(),
            example["title"]!.GetValue<string>(),
            example["pageType"]!.GetValue<string>(),
            example["repoPath"]!.GetValue<string>(),
            example["publicPath"]!.GetValue<string>());
    }

    private static ExamplesIndexEntry ParseExamplesIndexEntry(JsonObject example)
    {
        return new ExamplesIndexEntry(
            example["id"]!.GetValue<string>(),
            example["title"]!.GetValue<string>(),
            example["pageType"]!.GetValue<string>(),
            example["publicPath"]!.GetValue<string>());
    }

    private static void AssertFileTextMatches(string expectedPath, string actualPath)
    {
        Assert.IsTrue(File.Exists(expectedPath), $"Expected source artifact was not found: {expectedPath}");
        Assert.IsTrue(File.Exists(actualPath), $"Expected published artifact was not found: {actualPath}");

        var expected = NormalizeFileText(File.ReadAllText(expectedPath));
        var actual = NormalizeFileText(File.ReadAllText(actualPath));
        Assert.AreEqual(expected, actual, $"Artifact drift detected between '{expectedPath}' and '{actualPath}'.");
    }

    private static string NormalizeFileText(string value)
    {
        return value.Replace("\r\n", "\n", StringComparison.Ordinal).Trim();
    }

    private sealed record RegistryExample(string Id, string DocumentId, string Title, string PageType, string RepoPath, string PublicPath);

    private sealed record ExamplesIndexEntry(string Id, string Title, string PageType, string PublicPath);
}