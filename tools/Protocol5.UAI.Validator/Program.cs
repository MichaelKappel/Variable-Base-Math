using Protocol5.UAI;

var options = CliOptions.Parse(args);
if (options is null)
{
    CliOptions.WriteUsage(Console.Out);
    return 2;
}

if (!options.HasWork)
{
    CliOptions.WriteUsage(Console.Out);
    return 2;
}

var failures = new List<string>();
var successes = 0;

try
{
    if (options.ValidateEmbeddedExamples)
    {
        foreach (var exampleFileName in UaiConstants.GetEmbeddedExampleFileNames())
        {
            var label = $"embedded:{exampleFileName}";
            var json = UaiConstants.GetEmbeddedExampleText(exampleFileName);
            if (ValidateJson(label, json, options.RoundTrip, failures))
            {
                successes++;
            }
        }
    }

    foreach (var inputPath in ExpandInputPaths(options.InputPaths))
    {
        var json = File.ReadAllText(inputPath);
        if (ValidateJson(inputPath, json, options.RoundTrip, failures))
        {
            successes++;
        }
    }
}
catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or FormatException)
{
    Console.Error.WriteLine(exception.Message);
    return 2;
}

if (failures.Count > 0)
{
    foreach (var failure in failures)
    {
        Console.Error.WriteLine(failure);
    }

    Console.Error.WriteLine($"Validation failed. {successes} item(s) passed, {failures.Count} item(s) failed.");
    return 1;
}

Console.Out.WriteLine($"Validation succeeded. {successes} item(s) passed.");
return 0;

static IEnumerable<string> ExpandInputPaths(IEnumerable<string> inputPaths)
{
    var expanded = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    foreach (var inputPath in inputPaths)
    {
        var fullPath = Path.GetFullPath(inputPath);

        if (File.Exists(fullPath))
        {
            expanded.Add(fullPath);
            continue;
        }

        if (Directory.Exists(fullPath))
        {
            foreach (var file in Directory.GetFiles(fullPath, "*.uai.json", SearchOption.AllDirectories))
            {
                expanded.Add(file);
            }

            continue;
        }

        throw new IOException($"Input path was not found: {fullPath}");
    }

    return expanded.OrderBy(path => path, StringComparer.OrdinalIgnoreCase);
}

static bool ValidateJson(string label, string json, bool roundTrip, List<string> failures)
{
    var parser = new UaiDocumentParser();
    if (!parser.TryParse(json, out var document, out var validation) || document is null)
    {
        failures.Add($"{label}: {string.Join("; ", validation.Errors.Select(error => $"{error.Code}:{error.Path} {error.Message}"))}");
        return false;
    }

    if (roundTrip)
    {
        var serialized = UaiDocumentSerializer.Serialize(document);
        if (!parser.TryParse(serialized, out var roundTripped, out var roundTripValidation) || roundTripped is null)
        {
            failures.Add($"{label}: round-trip failed: {string.Join("; ", roundTripValidation.Errors.Select(error => $"{error.Code}:{error.Path} {error.Message}"))}");
            return false;
        }

        if (!string.Equals(document.DocumentId, roundTripped.DocumentId, StringComparison.Ordinal))
        {
            failures.Add($"{label}: round-trip changed documentId from '{document.DocumentId}' to '{roundTripped.DocumentId}'.");
            return false;
        }
    }

    Console.Out.WriteLine($"{label}: valid");
    return true;
}

internal sealed class CliOptions
{
    public bool RoundTrip { get; private set; }

    public bool ValidateEmbeddedExamples { get; private set; }

    public List<string> InputPaths { get; } = new();

    public bool HasWork => ValidateEmbeddedExamples || InputPaths.Count > 0;

    public static CliOptions? Parse(string[] args)
    {
        var options = new CliOptions();

        foreach (var arg in args)
        {
            switch (arg)
            {
                case "--roundtrip":
                    options.RoundTrip = true;
                    break;
                case "--embedded-examples":
                    options.ValidateEmbeddedExamples = true;
                    break;
                case "--help":
                case "-h":
                case "/?":
                    return null;
                default:
                    options.InputPaths.Add(arg);
                    break;
            }
        }

        return options;
    }

    public static void WriteUsage(TextWriter writer)
    {
        writer.WriteLine("Usage: Protocol5.UAI.Validator [--embedded-examples] [--roundtrip] <file-or-directory> [more paths]");
        writer.WriteLine();
        writer.WriteLine("Options:");
        writer.WriteLine("  --embedded-examples  Validate the canonical example corpus embedded in Protocol5.UAI.CSharp.");
        writer.WriteLine("  --roundtrip          Re-serialize and re-parse each valid document.");
    }
}