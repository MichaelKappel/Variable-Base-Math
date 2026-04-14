using System.Diagnostics;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Protocol5.UAI.CSharp.Tests;

[TestClass]
public sealed class ValidatorCliTests
{
    [TestMethod]
    public void ValidatorCli_ValidFile_ReturnsSuccess()
    {
        var examplePath = Path.Combine(TestPaths.GetExamplesDirectory(), "homepage.uai.json");

        var result = RunValidator($"\"{examplePath}\"");

        Assert.AreEqual(0, result.ExitCode, result.Output);
        StringAssert.Contains(result.Output, "homepage.uai.json: valid");
        StringAssert.Contains(result.Output, "Validation succeeded.");
    }

    [TestMethod]
    public void ValidatorCli_InvalidFile_ReturnsValidationFailure()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "Protocol5.UAI.Validator.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);
        var invalidPath = Path.Combine(tempRoot, "invalid.uai.json");
        File.WriteAllText(invalidPath, "{}", Encoding.UTF8);

        try
        {
            var result = RunValidator($"\"{invalidPath}\"");

            Assert.AreEqual(1, result.ExitCode, result.Output);
            StringAssert.Contains(result.Output, "Validation failed.");
            StringAssert.Contains(result.Output, "uai.documentId.required");
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }

    [TestMethod]
    public void ValidatorCli_EmbeddedExamplesWithRoundTrip_ReturnsSuccess()
    {
        var result = RunValidator("--embedded-examples --roundtrip");

        Assert.AreEqual(0, result.ExitCode, result.Output);
        StringAssert.Contains(result.Output, "embedded:homepage.uai.json: valid");
        StringAssert.Contains(result.Output, "Validation succeeded. 10 item(s) passed.");
    }

    private static (int ExitCode, string Output) RunValidator(string arguments)
    {
        var projectPath = TestPaths.GetValidatorProjectPath();
        var repoRoot = TestPaths.GetRepoRoot();
        var startInfo = new ProcessStartInfo("dotnet", $"run --project \"{projectPath}\" -- {arguments}")
        {
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(startInfo)!;
        process.WaitForExit();

        var output = process.StandardOutput.ReadToEnd() + Environment.NewLine + process.StandardError.ReadToEnd();
        return (process.ExitCode, output);
    }
}