namespace Protocol5.UAI;

public sealed class UaiDocumentParser
{
    private readonly UaiDocumentValidator _validator = new();

    public UaiDocument Parse(string json, bool normalize = true)
    {
        var document = UaiDocumentSerializer.Deserialize(json);
        if (normalize)
        {
            UaiDocumentNormalizer.Normalize(document);
        }

        var validation = _validator.Validate(document);
        if (!validation.IsValid)
        {
            throw new FormatException(string.Join(Environment.NewLine, validation.Errors.Select(error => $"{error.Code} at {error.Path}: {error.Message}")));
        }

        return document;
    }

    public bool TryParse(string json, out UaiDocument? document, out UaiValidationResult validation)
    {
        document = null;

        try
        {
            document = UaiDocumentSerializer.Deserialize(json);
            UaiDocumentNormalizer.Normalize(document);
            validation = _validator.Validate(document);
            return validation.IsValid;
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException)
        {
            validation = new UaiValidationResult(new[]
            {
                UaiValidationError.Error("$", "uai.parse.failed", exception.Message)
            });
            return false;
        }
    }
}
