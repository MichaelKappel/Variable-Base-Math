using System.Diagnostics;
using System.IO.Compression;
using System.Security;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Protocol5.UAI.CSharp.Tests;

[TestClass]
public sealed class PackageSmokeTests
{
    [TestMethod]
    public void DotnetPack_ProducesPackageWithRegistrySchemaExamplesAndSymbols()
    {
        var outputDirectory = PackPackage();

        try
        {
            var packagePath = Directory.GetFiles(outputDirectory, "Protocol5.UAI.CSharp.1.0.0.nupkg").Single();
            var symbolPackagePath = Directory.GetFiles(outputDirectory, "Protocol5.UAI.CSharp.1.0.0.snupkg").Single();
            Assert.IsTrue(File.Exists(symbolPackagePath), "The symbol package was not produced.");

            using var archive = ZipFile.OpenRead(packagePath);
            var entryNames = archive.Entries.Select(entry => entry.FullName).ToList();

            CollectionAssert.Contains(entryNames, "README.md");
            CollectionAssert.Contains(entryNames, "contentFiles/any/any/Protocol5.UAI/spec/discovery/uai-1.json");
            CollectionAssert.Contains(entryNames, "contentFiles/any/any/Protocol5.UAI/spec/discovery/uai-1-examples.json");
            CollectionAssert.Contains(entryNames, "contentFiles/any/any/Protocol5.UAI/spec/registry/uai-1.registry.json");
            CollectionAssert.Contains(entryNames, "contentFiles/any/any/Protocol5.UAI/spec/registry/symbols.json");
            CollectionAssert.Contains(entryNames, "contentFiles/any/any/Protocol5.UAI/spec/integration-contracts.md");
            CollectionAssert.Contains(entryNames, "contentFiles/any/any/Protocol5.UAI/spec/translator-contract.md");
            CollectionAssert.Contains(entryNames, "contentFiles/any/any/Protocol5.UAI/spec/website-export-contract.md");
            CollectionAssert.Contains(entryNames, "contentFiles/any/any/Protocol5.UAI/spec/registry-resolution-contract.md");
            CollectionAssert.Contains(entryNames, "contentFiles/any/any/Protocol5.UAI/spec/radix-63404-contract.md");
            CollectionAssert.Contains(entryNames, "contentFiles/any/any/Protocol5.UAI/spec/schema/uai-1.schema.json");
            CollectionAssert.Contains(entryNames, "contentFiles/any/any/Protocol5.UAI/spec/schema/uai-1.types.ts");
            CollectionAssert.Contains(entryNames, "contentFiles/any/any/Protocol5.UAI/examples/homepage.uai.json");
            CollectionAssert.Contains(entryNames, "contentFiles/any/any/Protocol5.UAI/docs/package-usage.md");
            Assert.IsTrue(entryNames.Any(name => name.EndsWith("lib/net8.0/Protocol5.UAI.CSharp.dll", StringComparison.Ordinal)));
            Assert.IsTrue(entryNames.Any(name => name.EndsWith("lib/netstandard2.1/Protocol5.UAI.CSharp.dll", StringComparison.Ordinal)));
        }
        finally
        {
            DeleteDirectory(outputDirectory);
        }
    }

    [TestMethod]
    public void PackedPackage_BuildsMinimalReferenceImplementationProject()
    {
        var outputDirectory = PackPackage();
        var tempRoot = Path.Combine(Path.GetTempPath(), "Protocol5.UAI.CSharp.PackageProject", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var restoreSources = SecurityElement.Escape(outputDirectory) ?? outputDirectory;
            var projectPath = Path.Combine(tempRoot, "ReferenceSite.csproj");
            var programPath = Path.Combine(tempRoot, "Program.cs");

            File.WriteAllText(projectPath, $$"""
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <RestoreSources>$(RestoreSources);{{restoreSources}}</RestoreSources>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Protocol5.UAI.CSharp" Version="1.0.0" />
  </ItemGroup>
</Project>
""", Encoding.UTF8);

            File.WriteAllText(programPath, """
using Protocol5.UAI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProtocol5UaiWebsiteSupport();

var app = builder.Build();
app.UseProtocol5UaiWebsiteSupport();
app.MapProtocol5UaiCanonicalArtifacts();
app.MapProtocol5UaiHtmlEndpoint(
    "/demo/index.uai.json",
    static () => "<html><body><h1>Hello package</h1><p>Ready</p></body></html>",
    new UaiHtmlTranslationOptions
    {
        SourceUri = "https://example.org/demo",
        DocumentId = "demo-package",
        PageType = "generic"
    });

app.Run();
""", Encoding.UTF8);

            var build = RunProcess("dotnet", $"build \"{projectPath}\" -nologo -v:m", tempRoot);
            Assert.AreEqual(0, build.ExitCode, build.Output);
        }
        finally
        {
            DeleteDirectory(tempRoot);
            DeleteDirectory(outputDirectory);
        }
    }

    private static string PackPackage()
    {
        var repoRoot = TestPaths.GetRepoRoot();
        var projectPath = Path.Combine(repoRoot, "Protocol5.UAI.CSharp", "Protocol5.UAI.CSharp.csproj");
        var outputDirectory = Path.Combine(Path.GetTempPath(), "Protocol5.UAI.CSharp.PackageSmoke", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputDirectory);

        var pack = RunProcess("dotnet", $"pack \"{projectPath}\" -c Release -o \"{outputDirectory}\"", repoRoot);
        Assert.AreEqual(0, pack.ExitCode, pack.Output);
        return outputDirectory;
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

    private static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}