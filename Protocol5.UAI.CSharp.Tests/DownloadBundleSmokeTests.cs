using System.Diagnostics;
using System.IO.Compression;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Protocol5.UAI.CSharp.Tests;

[TestClass]
public sealed class DownloadBundleSmokeTests
{
    [TestMethod]
    public void BuildDownloadBundleScript_ProducesStarterZipWithExporterValidatorAndPackage()
    {
        var repoRoot = TestPaths.GetRepoRoot();
        var scriptPath = Path.Combine(repoRoot, "tools", "Build-Protocol5UaiCSharpWebsiteSupport.ps1");
        var outputRoot = Path.Combine(Path.GetTempPath(), "Protocol5.UAI.CSharp.DownloadBundle", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputRoot);

        try
        {
            var result = RunProcess(
                "powershell",
                $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\" -Configuration Release -OutputRoot \"{outputRoot}\"",
                repoRoot);
            Assert.AreEqual(0, result.ExitCode, result.Output);

            var zipPath = Path.Combine(outputRoot, "protocol5-uai-1-csharp-web-starter.zip");
            var packagePath = Path.Combine(outputRoot, "Protocol5.UAI.CSharp.1.0.0.nupkg");
            Assert.IsTrue(File.Exists(zipPath), "The starter ZIP was not produced.");
            Assert.IsTrue(File.Exists(packagePath), "The NuGet package was not produced.");
            Assert.IsTrue(File.Exists(zipPath + ".sha256"), "The starter ZIP checksum was not produced.");
            Assert.IsTrue(File.Exists(packagePath + ".sha256"), "The NuGet package checksum was not produced.");

            using var zip = ZipFile.OpenRead(zipPath);
            var entryNames = zip.Entries.Select(entry => entry.FullName).ToArray();
            CollectionAssert.Contains(entryNames, "downloads\\Protocol5.UAI.CSharp.1.0.0.nupkg");
            CollectionAssert.Contains(entryNames, "tools\\Protocol5.UAI.SiteExporter\\Program.cs");
            CollectionAssert.Contains(entryNames, "tools\\Protocol5.UAI.SiteExporter\\samples\\export-manifest.sample.json");
            CollectionAssert.Contains(entryNames, "tools\\Protocol5.UAI.Validator\\Program.cs");
            CollectionAssert.Contains(entryNames, "spec\\website-export-contract.md");
            CollectionAssert.Contains(entryNames, "docs\\package-usage.md");
        }
        finally
        {
            if (Directory.Exists(outputRoot))
            {
                Directory.Delete(outputRoot, recursive: true);
            }
        }
    }

    private static (int ExitCode, string Output) RunProcess(string fileName, string arguments, string workingDirectory)
    {
        var startInfo = new ProcessStartInfo(fileName, arguments)
        {
            WorkingDirectory = workingDirectory,
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