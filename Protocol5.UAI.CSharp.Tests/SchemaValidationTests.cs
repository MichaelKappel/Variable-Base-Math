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
}
