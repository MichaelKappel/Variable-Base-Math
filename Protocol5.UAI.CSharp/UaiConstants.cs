using System.Reflection;

namespace Protocol5.UAI;

public static class UaiConstants
{
    public const string SpecName = "UAI-1";
    public const string CurrentDocumentVersion = "1.0.0";
    public const string CurrentSchemaVersion = "1.0.0";
    public const string CurrentTranslatorContractVersion = "1.0.0";
    public const string MediaType = "application/uai+json";
    public const string LegacyHttpHeader = "X-UAI-1";
    public const string AcceptHeader = "Accept";
    public const string ContentTypeHeader = "Content-Type";
    public const string ContentLanguageHeader = "Content-Language";
    public const string LinkHeader = "Link";
    public const string VaryHeader = "Vary";

    public static IReadOnlyCollection<string> NodeTypes { get; } = new[]
    {
        "document",
        "section",
        "heading",
        "paragraph",
        "quote",
        "list",
        "listItem",
        "table",
        "image",
        "figure",
        "caption",
        "button",
        "link",
        "navigation",
        "form",
        "input",
        "glossaryEntry",
        "symbol",
        "glyphCluster",
        "diagram",
        "manuscriptPanel",
        "callout",
        "metadataBlock",
        "footer",
        "header",
        "unknown"
    };

    public static string GetEmbeddedSchemaText()
    {
        return GetEmbeddedText("uai-1.schema.json");
    }

    public static string GetEmbeddedExampleText(string fileName)
    {
        return GetEmbeddedText(fileName);
    }

    private static string GetEmbeddedText(string fileName)
    {
        var assembly = typeof(UaiConstants).Assembly;
        var resourceName = assembly
            .GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
        {
            throw new InvalidOperationException($"Embedded resource '{fileName}' was not found in assembly '{assembly.GetName().Name}'.");
        }

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource stream '{resourceName}' could not be opened.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
