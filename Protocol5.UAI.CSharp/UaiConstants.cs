using System.Reflection;

namespace Protocol5.UAI;

public static class UaiConstants
{
    public const string SpecName = "UAI-1";
    public const string CurrentDocumentVersion = "1.0.0";
    public const string CurrentSchemaVersion = "1.0.0";
    public const string CurrentRegistryVersion = "1.0.0";
    public const string CurrentTranslatorContractVersion = "1.0.0";
    public const string LegacyCompatibilityVersion = "1.0";
    public const string MediaType = "application/uai+json";
    public const string LegacyHttpHeader = "X-UAI-1";
    public const string LegacyLanguageTag = "x-uai-1";
    public const string AcceptHeader = "Accept";
    public const string ContentTypeHeader = "Content-Type";
    public const string ContentLanguageHeader = "Content-Language";
    public const string LinkHeader = "Link";
    public const string VaryHeader = "Vary";
    public const string CanonicalPublicOrigin = "https://protocol5.com";
    public const string CanonicalMachineSpecPublicPath = "/UAI-1.json";
    public const string CanonicalExamplesIndexPublicPath = "/UAI-1-examples.json";
    public const string CanonicalRegistryIndexPublicPath = "/registry/uai-1.json";
    public const string CanonicalExamplesRegistryPublicPath = "/registry/uai-1-examples.json";
    public const string CanonicalSymbolsRegistryPublicPath = "/registry/symbols.json";
    public const string CanonicalSchemaIndexPublicPath = "/schema/uai-1.schema.json";
    public const string CanonicalRegistryPublicPath = "/UAI-1/registry/uai-1.registry.json";
    public const string CanonicalSchemaPublicPath = "/UAI-1/schema/uai-1.schema.json";
    public const string CanonicalSchemaId = CanonicalPublicOrigin + CanonicalSchemaPublicPath;
    public const string CanonicalTypesPublicPath = "/UAI-1/schema/uai-1.types.ts";
    public const string CanonicalExamplesPublicPath = "/UAI-1/examples";

    public static IReadOnlyCollection<string> TopLevelFieldOrder { get; } = new[]
    {
        "spec",
        "version",
        "schemaVersion",
        "documentId",
        "source",
        "metadata",
        "structure",
        "semantics",
        "symbols",
        "assets",
        "relationships",
        "annotations",
        "provenance",
        "extensions"
    };

    public static IReadOnlyCollection<string> PageTypes { get; } = new[]
    {
        "generic",
        "homepage",
        "article",
        "landing-page",
        "navigation",
        "symbolic-manuscript",
        "wordpress-page",
        "gallery",
        "glossary",
        "reference"
    };

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

    public static string GetEmbeddedRegistryText()
    {
        return GetEmbeddedText("uai-1.registry.json");
    }

    public static string GetEmbeddedProtocolDiscoveryText()
    {
        return GetEmbeddedText("uai-1.json");
    }

    public static string GetEmbeddedExamplesIndexText()
    {
        return GetEmbeddedText("uai-1-examples.json");
    }

    public static string GetEmbeddedSymbolRegistryText()
    {
        return GetEmbeddedText("symbols.json");
    }

    public static string GetEmbeddedTypesText()
    {
        return GetEmbeddedText("uai-1.types.ts");
    }

    public static string GetEmbeddedArtifactText(string fileName)
    {
        Guard.NotNull(fileName, nameof(fileName));
        return GetEmbeddedText(fileName);
    }

    public static string GetEmbeddedExampleText(string fileName)
    {
        return GetEmbeddedText(fileName);
    }

    public static IReadOnlyCollection<string> GetEmbeddedExampleFileNames()
    {
        var assembly = typeof(UaiConstants).Assembly;
        return assembly.GetManifestResourceNames()
            .Where(name => name.Contains(".EmbeddedExamples.", StringComparison.Ordinal) &&
                name.EndsWith(".uai.json", StringComparison.OrdinalIgnoreCase))
            .Select(GetEmbeddedExampleFileName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
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

    private static string GetEmbeddedExampleFileName(string resourceName)
    {
        var marker = ".EmbeddedExamples.";
        var markerIndex = resourceName.IndexOf(marker, StringComparison.Ordinal);
        if (markerIndex < 0)
        {
            throw new InvalidOperationException($"Embedded example resource '{resourceName}' did not include the expected marker '{marker}'.");
        }

        var fileName = resourceName.Substring(markerIndex + marker.Length);
        var segments = fileName.Split('.');
        return segments.Length >= 3
            ? string.Join(".", segments.Skip(segments.Length - 3))
            : fileName;
    }
}