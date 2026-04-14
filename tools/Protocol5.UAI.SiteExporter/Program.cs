using System.Text;
using System.Text.Json;

using Protocol5.UAI;

if (args.Length != 1)
{
    Console.Error.WriteLine("Usage: Protocol5.UAI.SiteExporter <manifest.json>");
    return 1;
}

var manifestPath = Path.GetFullPath(args[0]);
if (!File.Exists(manifestPath))
{
    Console.Error.WriteLine($"Manifest file was not found: {manifestPath}");
    return 1;
}

var manifest = JsonSerializer.Deserialize<ExportManifest>(
    File.ReadAllText(manifestPath, Encoding.UTF8),
    new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });

if (manifest is null || manifest.Pages.Count == 0)
{
    Console.Error.WriteLine("Manifest did not contain any pages.");
    return 1;
}

var exporter = new UaiHtmlExporter();
var generatedAt = manifest.GeneratedAt ?? DateTimeOffset.UtcNow;

foreach (var page in manifest.Pages)
{
    if (string.IsNullOrWhiteSpace(page.InputHtmlPath) ||
        string.IsNullOrWhiteSpace(page.OutputJsonPath) ||
        string.IsNullOrWhiteSpace(page.SourceUri) ||
        string.IsNullOrWhiteSpace(page.DocumentId) ||
        string.IsNullOrWhiteSpace(page.PageType))
    {
        throw new InvalidOperationException("Each export page must define inputHtmlPath, outputJsonPath, sourceUri, documentId, and pageType.");
    }

    exporter.ExportToFile(page.InputHtmlPath, page.OutputJsonPath, new UaiHtmlTranslationOptions
    {
        SourceUri = page.SourceUri,
        DocumentId = page.DocumentId,
        RetrievedAt = generatedAt,
        Language = page.Language,
        SiteName = page.SiteName ?? "Protocol5",
        PageType = page.PageType,
        CaptureNotes = page.CaptureNotes ?? $"Published machine endpoint generated from '{page.SourceUri}'."
    });
}

return 0;

internal sealed class ExportManifest
{
    public DateTimeOffset? GeneratedAt { get; set; }

    public List<ExportPage> Pages { get; set; } = new();
}

internal sealed class ExportPage
{
    public string InputHtmlPath { get; set; } = string.Empty;

    public string OutputJsonPath { get; set; } = string.Empty;

    public string SourceUri { get; set; } = string.Empty;

    public string DocumentId { get; set; } = string.Empty;

    public string PageType { get; set; } = string.Empty;

    public string? Language { get; set; }

    public string? SiteName { get; set; }

    public string? CaptureNotes { get; set; }
}