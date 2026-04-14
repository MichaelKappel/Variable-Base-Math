using System.Diagnostics;
using System.IO.Compression;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Protocol5.UAI.CSharp.Tests;

[TestClass]
public sealed class PackageSmokeTests
{
    [TestMethod]
    public void DotnetPack_ProducesPackageWithSchemaExamplesAndSymbols()
    {
        var repoRoot = TestPaths.GetRepoRoot();
        var projectPath = Path.Combine(repoRoot, "Protocol5.UAI.CSharp", "Protocol5.UAI.CSharp.csproj");
        var outputDirectory = Path.Combine(Path.GetTempPath(), "Protocol5.UAI.CSharp.PackageSmoke", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputDirectory);

        try
        {
            var startInfo = new ProcessStartInfo("dotnet", $"pack \"{projectPath}\" -c Release -o \"{outputDirectory}\"")
            {
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var process = Process.Start(startInfo)!;
            process.WaitForExit();

            var output = process.StandardOutput.ReadToEnd() + Environment.NewLine + process.StandardError.ReadToEnd();
            Assert.AreEqual(0, process.ExitCode, output);

            var packagePath = Directory.GetFiles(outputDirectory, "Protocol5.UAI.CSharp.1.0.0.nupkg").Single();
            var symbolPackagePath = Directory.GetFiles(outputDirectory, "Protocol5.UAI.CSharp.1.0.0.snupkg").Single();
            Assert.IsTrue(File.Exists(symbolPackagePath), "The symbol package was not produced.");

            using var archive = ZipFile.OpenRead(packagePath);
            var entryNames = archive.Entries.Select(entry => entry.FullName).ToList();

            CollectionAssert.Contains(entryNames, "README.md");
            CollectionAssert.Contains(entryNames, "contentFiles/any/any/Protocol5.UAI/spec/schema/uai-1.schema.json");
            CollectionAssert.Contains(entryNames, "contentFiles/any/any/Protocol5.UAI/spec/schema/uai-1.types.ts");
            CollectionAssert.Contains(entryNames, "contentFiles/any/any/Protocol5.UAI/examples/homepage.uai.json");
            CollectionAssert.Contains(entryNames, "contentFiles/any/any/Protocol5.UAI/docs/package-usage.md");
            Assert.IsTrue(entryNames.Any(name => name.EndsWith("lib/net8.0/Protocol5.UAI.CSharp.dll", StringComparison.Ordinal)));
            Assert.IsTrue(entryNames.Any(name => name.EndsWith("lib/netstandard2.1/Protocol5.UAI.CSharp.dll", StringComparison.Ordinal)));
        }
        finally
        {
            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, recursive: true);
            }
        }
    }
}
