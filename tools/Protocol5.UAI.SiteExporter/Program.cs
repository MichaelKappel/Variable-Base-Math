using System.Text;
using System.Text.Json;

using Protocol5.UAI;

try
{
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

    var manifestDirectory = Path.GetDirectoryName(manifestPath)
        ?? throw new InvalidOperationException($"Manifest path did not have a parent directory: {manifestPath}");
    var exporter = new UaiHtmlExporter();

    foreach (var page in manifest.Pages)
    {
        ValidatePage(page);

        var inputHtmlPath = ResolveManifestRelativePath(manifestDirectory, page.InputHtmlPath);
        var outputJsonPath = ResolveManifestRelativePath(manifestDirectory, page.OutputJsonPath);

        var retrievedAt = manifest.GeneratedAt ?? new DateTimeOffset(File.GetLastWriteTimeUtc(inputHtmlPath), TimeSpan.Zero);

        exporter.ExportToFile(inputHtmlPath, outputJsonPath, new UaiHtmlTranslationOptions
        {
            SourceUri = page.SourceUri,
            DocumentId = page.DocumentId,
            RetrievedAt = retrievedAt,
            Language = page.Language,
            SiteName = page.SiteName ?? "Protocol5",
            PageType = page.PageType,
            CaptureNotes = page.CaptureNotes ?? $"Published machine endpoint generated from '{page.SourceUri}'."
        });

        Console.Out.WriteLine($"{page.SourceUri} -> {outputJsonPath}");
    }

    Console.Out.WriteLine($"Exported {manifest.Pages.Count} page(s).");
    return 0;
}
catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or InvalidOperationException or ArgumentException or JsonException)
{
    Console.Error.WriteLine(exception.Message);
    return 1;
}

static string ResolveManifestRelativePath(string manifestDirectory, string path)
{
    if (string.IsNullOrWhiteSpace(path))
    {
        throw new ArgumentException("Path values cannot be null or whitespace.", nameof(path));
    }

    return Path.IsPathRooted(path)
        ? Path.GetFullPath(path)
        : Path.GetFullPath(Path.Combine(manifestDirectory, path));
}

static void ValidatePage(ExportPage page)
{
    if (string.IsNullOrWhiteSpace(page.InputHtmlPath) ||
        string.IsNullOrWhiteSpace(page.OutputJsonPath) ||
        string.IsNullOrWhiteSpace(page.SourceUri) ||
        string.IsNullOrWhiteSpace(page.DocumentId) ||
        string.IsNullOrWhiteSpace(page.PageType))
    {
        throw new InvalidOperationException("Each export page must define inputHtmlPath, outputJsonPath, sourceUri, documentId, and pageType.");
    }
}

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