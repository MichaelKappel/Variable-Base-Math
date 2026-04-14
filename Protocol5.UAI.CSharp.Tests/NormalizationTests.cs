using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Protocol5.UAI.CSharp.Tests;

[TestClass]
public sealed class NormalizationTests
{
    [TestMethod]
    public void Normalize_ComputesNormalizedTextAndSortsTopLevelCollections()
    {
        var document = new UaiDocument
        {
            DocumentId = "doc-1",
            Source = new UaiSourceDescriptor
            {
                Uri = "https://example.org",
                RetrievedAt = "2026-04-13T21:00:00Z"
            },
            Metadata = new UaiMetadata
            {
                Title = " Test ",
                Language = "en",
                PageType = "generic"
            },
            Structure =
            [
                new UaiNode
                {
                    Type = "document",
                    Id = "doc-1",
                    Children =
                    [
                        new UaiNode
                        {
                            Type = "paragraph",
                            Id = "p-1",
                            Text = new UaiTextValue
                            {
                                Literal = "  Hello   world  "
                            }
                        }
                    ]
                }
            ],
            Semantics =
            [
                new UaiSemanticRecord
                {
                    Id = "b",
                    Targets = ["p-1"],
                    Kind = "topic",
                    Value = "beta",
                    Source = "source"
                },
                new UaiSemanticRecord
                {
                    Id = "a",
                    Targets = ["p-1"],
                    Kind = "topic",
                    Value = "alpha",
                    Source = "source"
                }
            ],
            Symbols = [],
            Assets = [],
            Relationships = [],
            Annotations = [],
            Provenance = new UaiProvenance
            {
                GeneratedAt = "2026-04-13T21:00:00Z"
            },
            Extensions = new SortedDictionary<string, UaiExtensionValue>(StringComparer.Ordinal)
        };

        UaiDocumentNormalizer.Normalize(document);

        Assert.AreEqual("Test", document.Metadata.Title);
        Assert.AreEqual("Hello world", document.Structure[0].Children![0].Text!.Normalized);
        Assert.AreEqual("a", document.Semantics[0].Id);
        Assert.AreEqual("b", document.Semantics[1].Id);
    }
}
