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
}
