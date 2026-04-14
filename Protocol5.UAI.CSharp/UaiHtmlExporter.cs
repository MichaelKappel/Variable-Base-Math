using System.Security.Cryptography;
using System.Text;

namespace Protocol5.UAI;

public sealed class UaiHtmlExporter
{
    private readonly UaiHtmlTranslator _translator;
    private readonly UaiDocumentValidator _validator;

    public UaiHtmlExporter()
        : this(new UaiHtmlTranslator(), new UaiDocumentValidator())
    {
    }

    public UaiHtmlExporter(UaiHtmlTranslator translator, UaiDocumentValidator validator)
    {
        _translator = translator ?? throw new ArgumentNullException(nameof(translator));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public UaiHtmlExportResult Export(string html, UaiHtmlTranslationOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            throw new ArgumentException("HTML input cannot be null or whitespace.", nameof(html));
        }

        var effectiveOptions = CloneOptions(options);
        if (string.IsNullOrWhiteSpace(effectiveOptions.ContentHash))
        {
            effectiveOptions.ContentHash = BuildContentHash(html);
        }

        var document = _translator.Translate(html, effectiveOptions);
        var validation = _validator.Validate(document);
        if (!validation.IsValid)
        {
            throw BuildValidationException(effectiveOptions.SourceUri ?? effectiveOptions.DocumentId ?? "document", validation);
        }

        return new UaiHtmlExportResult(document, UaiDocumentSerializer.Serialize(document), validation);
    }

    public UaiHtmlExportResult ExportFile(string inputHtmlPath, UaiHtmlTranslationOptions? options = null)
    {
        Guard.NotNull(inputHtmlPath, nameof(inputHtmlPath));

        var fullInputPath = Path.GetFullPath(inputHtmlPath);
        if (!File.Exists(fullInputPath))
        {
            throw new FileNotFoundException($"Input HTML file was not found: {fullInputPath}", fullInputPath);
        }

        var effectiveOptions = CloneOptions(options);
        if (string.IsNullOrWhiteSpace(effectiveOptions.SourceUri))
        {
            effectiveOptions.SourceUri = new Uri(fullInputPath).AbsoluteUri;
        }

        var html = File.ReadAllText(fullInputPath, Encoding.UTF8);
        return Export(html, effectiveOptions);
    }

    public UaiHtmlExportResult ExportToFile(string inputHtmlPath, string outputJsonPath, UaiHtmlTranslationOptions? options = null)
    {
        Guard.NotNull(outputJsonPath, nameof(outputJsonPath));

        var export = ExportFile(inputHtmlPath, options);
        var fullOutputPath = Path.GetFullPath(outputJsonPath);
        var directory = Path.GetDirectoryName(fullOutputPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(fullOutputPath, export.Json, new UTF8Encoding(false));
        return export;
    }

    private static string BuildContentHash(string html)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(html));
        var hashHex = BitConverter.ToString(hashBytes).Replace("-", string.Empty, StringComparison.Ordinal);
        return "sha256:" + hashHex;
    }

    private static Exception BuildValidationException(string label, UaiValidationResult validation)
    {
        var messages = string.Join(Environment.NewLine, validation.Errors.Select(error => $"{error.Code} {error.Path}: {error.Message}"));
        return new InvalidOperationException($"UAI export failed validation for '{label}'.{Environment.NewLine}{messages}");
    }

    private static UaiHtmlTranslationOptions CloneOptions(UaiHtmlTranslationOptions? options)
    {
        options ??= new UaiHtmlTranslationOptions();

        return new UaiHtmlTranslationOptions
        {
            SourceUri = options.SourceUri,
            DocumentId = options.DocumentId,
            RetrievedAt = options.RetrievedAt,
            ContentHash = options.ContentHash,
            Language = options.Language,
            DefaultLanguage = options.DefaultLanguage,
            SiteName = options.SiteName,
            PageType = options.PageType,
            PreserveUnsupportedAsUnknown = options.PreserveUnsupportedAsUnknown,
            GeneratorName = options.GeneratorName,
            GeneratorVersion = options.GeneratorVersion,
            TranslatorName = options.TranslatorName,
            TranslatorVersion = options.TranslatorVersion,
            CaptureNotes = options.CaptureNotes
        };
    }
}

public sealed class UaiHtmlExportResult
{
    public UaiHtmlExportResult(UaiDocument document, string json, UaiValidationResult validation)
    {
        Document = document ?? throw new ArgumentNullException(nameof(document));
        Json = json ?? throw new ArgumentNullException(nameof(json));
        Validation = validation ?? throw new ArgumentNullException(nameof(validation));
    }

    public UaiDocument Document { get; }

    public string Json { get; }

    public UaiValidationResult Validation { get; }
}