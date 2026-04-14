using System.Runtime.CompilerServices;

namespace Protocol5.UAI.CSharp.Tests;

internal static class TestPaths
{
    public static string GetRepoRoot([CallerFilePath] string filePath = "")
    {
        return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(filePath)!, ".."));
    }

    public static string GetExamplesDirectory([CallerFilePath] string filePath = "")
    {
        return Path.Combine(GetRepoRoot(filePath), "examples");
    }

    public static string GetSchemaPath([CallerFilePath] string filePath = "")
    {
        return Path.Combine(GetRepoRoot(filePath), "spec", "schema", "uai-1.schema.json");
    }

    public static string GetTypesPath([CallerFilePath] string filePath = "")
    {
        return Path.Combine(GetRepoRoot(filePath), "spec", "schema", "uai-1.types.ts");
    }

    public static string GetRegistryPath([CallerFilePath] string filePath = "")
    {
        return Path.Combine(GetRepoRoot(filePath), "spec", "registry", "uai-1.registry.json");
    }

    public static string GetSymbolRegistryPath([CallerFilePath] string filePath = "")
    {
        return Path.Combine(GetRepoRoot(filePath), "spec", "registry", "symbols.json");
    }

    public static string GetProtocolDiscoveryPath([CallerFilePath] string filePath = "")
    {
        return Path.Combine(GetRepoRoot(filePath), "spec", "discovery", "uai-1.json");
    }

    public static string GetExamplesIndexPath([CallerFilePath] string filePath = "")
    {
        return Path.Combine(GetRepoRoot(filePath), "spec", "discovery", "uai-1-examples.json");
    }

    public static string GetSiteContentDirectory([CallerFilePath] string filePath = "")
    {
        return Path.Combine(GetRepoRoot(filePath), "Protocol5.com", "SiteContent");
    }

    public static string GetPackageProjectPath([CallerFilePath] string filePath = "")
    {
        return Path.Combine(GetRepoRoot(filePath), "Protocol5.UAI.CSharp", "Protocol5.UAI.CSharp.csproj");
    }

    public static string GetSiteExporterProjectPath([CallerFilePath] string filePath = "")
    {
        return Path.Combine(GetRepoRoot(filePath), "tools", "Protocol5.UAI.SiteExporter", "Protocol5.UAI.SiteExporter.csproj");
    }

    public static string GetValidatorProjectPath([CallerFilePath] string filePath = "")
    {
        return Path.Combine(GetRepoRoot(filePath), "tools", "Protocol5.UAI.Validator", "Protocol5.UAI.Validator.csproj");
    }
}