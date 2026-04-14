using System.Text.Json;

namespace Protocol5.UAI;

public sealed class UaiCanonicalAssetLoader
{
    private readonly UaiDocumentParser _parser;

    public UaiCanonicalAssetLoader()
        : this(new UaiDocumentParser())
    {
    }

    public UaiCanonicalAssetLoader(UaiDocumentParser parser)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }

    public string LoadProtocolDiscoveryText() => UaiConstants.GetEmbeddedProtocolDiscoveryText();

    public JsonDocument LoadProtocolDiscoveryJson() => ParseJson(LoadProtocolDiscoveryText());

    public string LoadExamplesIndexText() => UaiConstants.GetEmbeddedExamplesIndexText();

    public JsonDocument LoadExamplesIndexJson() => ParseJson(LoadExamplesIndexText());

    public string LoadRegistryText() => UaiConstants.GetEmbeddedRegistryText();

    public JsonDocument LoadRegistryJson() => ParseJson(LoadRegistryText());

    public string LoadSymbolRegistryText() => UaiConstants.GetEmbeddedSymbolRegistryText();

    public JsonDocument LoadSymbolRegistryJson() => ParseJson(LoadSymbolRegistryText());

    public string LoadSchemaText() => UaiConstants.GetEmbeddedSchemaText();

    public JsonDocument LoadSchemaJson() => ParseJson(LoadSchemaText());

    public string LoadTypesText() => UaiConstants.GetEmbeddedTypesText();

    public IReadOnlyCollection<string> GetExampleFileNames() => UaiConstants.GetEmbeddedExampleFileNames();

    public string LoadExampleText(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("Example file name cannot be null or whitespace.", nameof(fileName));
        }

        return UaiConstants.GetEmbeddedExampleText(fileName);
    }

    public JsonDocument LoadExampleJson(string fileName) => ParseJson(LoadExampleText(fileName));

    public UaiDocument LoadExampleDocument(string fileName, bool normalize = true) => _parser.Parse(LoadExampleText(fileName), normalize);

    public IReadOnlyList<UaiDocument> LoadAllExampleDocuments(bool normalize = true)
    {
        return GetExampleFileNames()
            .OrderBy(name => name, StringComparer.Ordinal)
            .Select(name => LoadExampleDocument(name, normalize))
            .ToArray();
    }

    private static JsonDocument ParseJson(string json)
    {
        return JsonDocument.Parse(json);
    }
}