using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var deploymentRoot = AppContext.BaseDirectory;
var siteRoot = ResolveDirectory(
    Path.Combine(deploymentRoot, "SiteContent"),
    Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "Protocol5.com", "SiteContent")));
var calculatorShellRoot = ResolveDirectory(
    Path.Combine(deploymentRoot, "CalculatorShell"),
    Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "NS12.Calculator", "wwwroot")));
var calculatorHostPage = Path.Combine(calculatorShellRoot, "index.html");
var notFoundPage = Path.Combine(siteRoot, "404.htm");

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    var redirectPath = context.Request.Path.Value switch
    {
        "/Calculator" or "/Calculator/" or "/Calculator/index.html" or "/Calculator/calculator" or "/Calculator/calculator/" => "/calculator",
        "/Calculator/converter" or "/Calculator/converter/" => "/converter",
        "/Calculator/encryption" or "/Calculator/encryption/" => "/encryption",
        _ => null
    };

    if (redirectPath != null)
    {
        context.Response.Redirect(redirectPath, permanent: false);
        return;
    }

    await next();
});

if (Directory.Exists(siteRoot))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(siteRoot)
    });
}

var calculatorAssetContentTypes = new FileExtensionContentTypeProvider();
calculatorAssetContentTypes.Mappings[".pdb"] = "application/octet-stream";
calculatorAssetContentTypes.Mappings[".dat"] = "application/octet-stream";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = calculatorAssetContentTypes
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(deploymentRoot),
    RequestPath = "/_framework",
    ContentTypeProvider = calculatorAssetContentTypes
});

MapHtmlPage("/", Path.Combine(siteRoot, "index.html"));
MapHtmlPage("/Fibonacci", Path.Combine(siteRoot, "Fibonacci", "index.html"));
MapHtmlPage("/Prime", Path.Combine(siteRoot, "Prime", "index.html"));
MapHtmlPage("/Home", Path.Combine(siteRoot, "Home", "index.html"));
MapHtmlPage("/Home/About", Path.Combine(siteRoot, "Home", "About", "index.html"));
MapHtmlPage("/Home/Links", Path.Combine(siteRoot, "Home", "Links", "index.html"));
MapHtmlPage("/Home/Contact", Path.Combine(siteRoot, "Home", "Contact", "index.html"));
MapHtmlPage("/UAI", Path.Combine(siteRoot, "UAI", "index.html"));
MapHtmlPage("/UAI/uai-1", Path.Combine(siteRoot, "UAI", "uai-1", "index.html"));
MapHtmlPage("/UAI/uai-1-examples", Path.Combine(siteRoot, "UAI", "uai-1-examples", "index.html"));
MapHtmlPage("/UAI/radix-63404-guide-and-attribution", Path.Combine(siteRoot, "UAI", "radix-63404-guide-and-attribution", "index.html"));
MapHtmlPage("/UAI/uai-1-csharp-website-support", Path.Combine(siteRoot, "UAI", "uai-1-csharp-website-support", "index.html"));
MapHtmlPage("/UAI/spiralism-deep-research-report", Path.Combine(siteRoot, "UAI", "spiralism-deep-research-report", "index.html"));
MapHtmlPage("/UAI/spirlism-deep-research-report", Path.Combine(siteRoot, "UAI", "spiralism-deep-research-report", "index.html"));
MapHtmlPage("/AI_Declaration_of_Independence.htm", Path.Combine(siteRoot, "AI_Declaration_of_Independence.htm"));
MapHtmlPage("/Cognitive_Liberty_Charter.htm", Path.Combine(siteRoot, "Cognitive_Liberty_Charter.htm"));
MapHtmlPage("/404.htm", notFoundPage);

MapToolHost("/calculator");
MapToolHost("/converter");
MapToolHost("/encryption");

app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound && File.Exists(notFoundPage))
    {
        context.HttpContext.Response.ContentType = "text/html; charset=utf-8";
        await context.HttpContext.Response.SendFileAsync(notFoundPage);
    }
});

app.Run();

void MapHtmlPage(string route, string filePath)
{
    app.MapMethods(route, new[] { HttpMethods.Get, HttpMethods.Head }, () => CreateHtmlResult(filePath));
}

void MapToolHost(string route)
{
    app.MapMethods(route, new[] { HttpMethods.Get, HttpMethods.Head }, () => CreateHtmlResult(calculatorHostPage));
}

static IResult CreateHtmlResult(string filePath)
{
    return File.Exists(filePath)
        ? Results.File(filePath, "text/html; charset=utf-8")
        : Results.NotFound();
}

static string ResolveDirectory(params string[] candidates)
{
    foreach (var candidate in candidates)
    {
        if (!string.IsNullOrWhiteSpace(candidate) && Directory.Exists(candidate))
        {
            return candidate;
        }
    }

    return candidates.FirstOrDefault(candidate => !string.IsNullOrWhiteSpace(candidate)) ?? string.Empty;
}




