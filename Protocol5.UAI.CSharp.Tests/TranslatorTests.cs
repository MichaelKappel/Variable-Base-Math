using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Protocol5.UAI.CSharp.Tests;

[TestClass]
public sealed class TranslatorTests
{
    [TestMethod]
    public void Translator_ConvertsHomePageIntoValidUai()
    {
        var repoRoot = TestPaths.GetRepoRoot();
        var htmlPath = Path.Combine(repoRoot, "Protocol5.com", "SiteContent", "index.html");
        var html = File.ReadAllText(htmlPath);

        var translator = new UaiHtmlTranslator();
        var document = translator.Translate(html, new UaiHtmlTranslationOptions
        {
            SourceUri = "https://protocol5.example/",
            PageType = "homepage"
        });

        var validation = new UaiDocumentValidator().Validate(document);

        Assert.IsTrue(validation.IsValid, string.Join("; ", validation.Errors.Select(error => $"{error.Code}:{error.Path}")));
        Assert.AreEqual("homepage", document.Metadata.PageType);
        Assert.IsTrue(document.Structure[0].Children!.Any(node => node.Type == "header"));
        Assert.IsTrue(document.Structure[0].Children!.Any(node => node.Type == "section"));
        Assert.IsTrue(document.Structure[0].Children!.Any(node => node.Type == "footer"));
        Assert.IsTrue(document.Structure[0].Children!
            .SelectMany(Flatten)
            .Any(node => node.Type == "button"));
    }

    [TestMethod]
    public void Translator_UsesSymbolDefinitionsAndOccurrencesWhenHintsArePresent()
    {
        const string html = """
        <!DOCTYPE html>
        <html lang="en">
        <body>
          <section>
            <img src="/images/spiral.png"
                 alt="Flow Spiral"
                 class="symbol"
                 data-uai-symbol-id="symbol.flow-spiral"
                 data-uai-symbol-label="Flow Spiral" />
          </section>
        </body>
        </html>
        """;

        var translator = new UaiHtmlTranslator();
        var document = translator.Translate(html, new UaiHtmlTranslationOptions
        {
            SourceUri = "https://example.org/symbol",
            PageType = "reference"
        });

        Assert.AreEqual(1, document.Symbols.Count);
        Assert.IsTrue(document.Structure[0].Children!
            .SelectMany(Flatten)
            .Any(node => node.Type == "symbol" && node.SymbolRef == "symbol.flow-spiral"));
    }

    [TestMethod]
    public void Renderer_ProducesHtmlFromTranslatedDocument()
    {
        const string html = "<html><body><h1>Hello</h1><p>World</p></body></html>";
        var document = new UaiHtmlTranslator().Translate(html, new UaiHtmlTranslationOptions
        {
            SourceUri = "https://example.org/hello",
            PageType = "generic"
        });

        var rendered = new UaiHtmlRenderer().Render(document);

        StringAssert.Contains(rendered, "<h1>Hello</h1>");
        StringAssert.Contains(rendered, "<p>World</p>");
    }

    [TestMethod]
    public void Translator_UsesImageAltTextForImageOnlyLinks()
    {
        const string html = """
        <!DOCTYPE html>
        <html lang="en">
        <body>
          <section>
            <a href="/images/full.png">
              <img src="/images/thumb.png" alt="Open full-size manuscript panel" />
            </a>
          </section>
        </body>
        </html>
        """;

        var document = new UaiHtmlTranslator().Translate(html, new UaiHtmlTranslationOptions
        {
            SourceUri = "https://example.org/image-link",
            PageType = "reference"
        });

        var link = document.Structure[0].Children!
            .SelectMany(Flatten)
            .Single(node => node.Type == "link");

        Assert.AreEqual("Open full-size manuscript panel", link.Text?.Literal);
        Assert.AreEqual("Open full-size manuscript panel", link.Text?.Normalized);
    }

    private static IEnumerable<UaiNode> Flatten(UaiNode node)
    {
        yield return node;

        if (node.Children is null)
        {
            yield break;
        }

        foreach (var child in node.Children.SelectMany(Flatten))
        {
            yield return child;
        }
    }
}
