using System.Text.Json;
using System.Text.Json.Nodes;

using Json.Schema;

namespace Protocol5.UAI;

public sealed class UaiSchemaValidator
{
    private readonly JsonSchema _schema;
    private readonly UaiDocumentParser _parser = new();
    private readonly UaiDocumentValidator _validator = new();

    public UaiSchemaValidator()
        : this(UaiConstants.GetEmbeddedSchemaText())
    {
    }

    public UaiSchemaValidator(string schemaJson)
    {
        if (string.IsNullOrWhiteSpace(schemaJson))
        {
            throw new ArgumentException("Schema JSON cannot be null or whitespace.", nameof(schemaJson));
        }

        _schema = JsonSchema.FromText(schemaJson);
    }

    public UaiSchemaValidationResult ValidateJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new UaiSchemaValidationResult(new[]
            {
                new UaiSchemaValidationError
                {
                    Keyword = "json.parse",
                    Message = "Input JSON cannot be null or whitespace.",
                    InstanceLocation = "$"
                }
            });
        }

        try
        {
            var node = JsonNode.Parse(json) ?? JsonValue.Create((string?)null)!;
            var evaluation = _schema.Evaluate(node, new EvaluationOptions { OutputFormat = OutputFormat.List });
            var errors = new List<UaiSchemaValidationError>();
            CollectErrors(evaluation, errors);

            if (!evaluation.IsValid && errors.Count == 0)
            {
                errors.Add(new UaiSchemaValidationError
                {
                    Keyword = "schema.invalid",
                    Message = "Schema evaluation failed without emitting a detailed error.",
                    InstanceLocation = evaluation.InstanceLocation.ToString(),
                    EvaluationPath = evaluation.EvaluationPath.ToString(),
                    SchemaLocation = evaluation.SchemaLocation?.ToString() ?? string.Empty
                });
            }

            return new UaiSchemaValidationResult(errors);
        }
        catch (Exception exception) when (exception is System.Text.Json.JsonException or FormatException)
        {
            return new UaiSchemaValidationResult(new[]
            {
                new UaiSchemaValidationError
                {
                    Keyword = "json.parse",
                    Message = exception.Message,
                    InstanceLocation = "$"
                }
            });
        }
    }

    public UaiSchemaValidationResult Validate(UaiDocument document)
    {
        Guard.NotNull(document, nameof(document));
        return ValidateJson(UaiDocumentSerializer.Serialize(document));
    }

    public UaiCanonicalValidationResult ValidateCanonicalJson(string json, bool normalize = true)
    {
        var schemaValidation = ValidateJson(json);
        _parser.TryParse(json, out var document, out var semanticValidation);

        if (document is not null && normalize)
        {
            semanticValidation = _validator.Validate(document);
        }

        return new UaiCanonicalValidationResult(schemaValidation, semanticValidation);
    }

    public UaiCanonicalValidationResult ValidateCanonical(UaiDocument document)
    {
        Guard.NotNull(document, nameof(document));
        var schemaValidation = Validate(document);
        var semanticValidation = _validator.Validate(document);
        return new UaiCanonicalValidationResult(schemaValidation, semanticValidation);
    }

    private static void CollectErrors(EvaluationResults evaluation, ICollection<UaiSchemaValidationError> errors)
    {
        if (evaluation.Errors is not null)
        {
            foreach (var error in evaluation.Errors)
            {
                errors.Add(new UaiSchemaValidationError
                {
                    Keyword = error.Key,
                    Message = error.Value,
                    InstanceLocation = evaluation.InstanceLocation.ToString(),
                    EvaluationPath = evaluation.EvaluationPath.ToString(),
                    SchemaLocation = evaluation.SchemaLocation?.ToString() ?? string.Empty
                });
            }
        }

        if (!evaluation.HasDetails)
        {
            return;
        }

        foreach (var detail in evaluation.Details)
        {
            CollectErrors(detail, errors);
        }
    }
}

public sealed class UaiSchemaValidationResult
{
    public UaiSchemaValidationResult(IReadOnlyList<UaiSchemaValidationError> errors)
    {
        Errors = errors ?? throw new ArgumentNullException(nameof(errors));
    }

    public bool IsValid => Errors.Count == 0;

    public IReadOnlyList<UaiSchemaValidationError> Errors { get; }
}

public sealed class UaiSchemaValidationError
{
    public string Keyword { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string InstanceLocation { get; set; } = string.Empty;

    public string EvaluationPath { get; set; } = string.Empty;

    public string SchemaLocation { get; set; } = string.Empty;
}

public sealed class UaiCanonicalValidationResult
{
    public UaiCanonicalValidationResult(UaiSchemaValidationResult schema, UaiValidationResult semantic)
    {
        Schema = schema ?? throw new ArgumentNullException(nameof(schema));
        Semantic = semantic ?? throw new ArgumentNullException(nameof(semantic));
    }

    public bool IsValid => Schema.IsValid && Semantic.IsValid;

    public UaiSchemaValidationResult Schema { get; }

    public UaiValidationResult Semantic { get; }
}