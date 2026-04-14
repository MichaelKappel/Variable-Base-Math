using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Protocol5.UAI;

public static class UaiDocumentSerializer
{
    private static readonly JsonSerializerOptions ReadOptions = CreateOptions(writeIndented: false);
    private static readonly JsonSerializerOptions WriteOptions = CreateOptions(writeIndented: true);

    public static UaiDocument Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("UAI JSON cannot be null or whitespace.", nameof(json));
        }

        return JsonSerializer.Deserialize<UaiDocument>(json, ReadOptions)
            ?? throw new FormatException("UAI JSON did not deserialize into a document.");
    }

    public static UaiDocument Deserialize(Stream stream)
    {
        Guard.NotNull(stream, nameof(stream));

        return JsonSerializer.Deserialize<UaiDocument>(stream, ReadOptions)
            ?? throw new FormatException("UAI JSON stream did not deserialize into a document.");
    }

    public static string Serialize(UaiDocument document)
    {
        Guard.NotNull(document, nameof(document));

        return JsonSerializer.Serialize(document, WriteOptions);
    }

    public static void Serialize(Stream stream, UaiDocument document)
    {
        Guard.NotNull(stream, nameof(stream));
        Guard.NotNull(document, nameof(document));

        JsonSerializer.Serialize(stream, document, WriteOptions);
    }

    private static JsonSerializerOptions CreateOptions(bool writeIndented)
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = writeIndented
        };
    }
}
