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

app.UseStaticFiles();
var calculatorOutputContentTypes = new FileExtensionContentTypeProvider();
calculatorOutputContentTypes.Mappings[".pdb"] = "application/octet-stream";
calculatorOutputContentTypes.Mappings[".dat"] = "application/octet-stream";

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(deploymentRoot),
    RequestPath = "/_framework",
    ContentTypeProvider = calculatorOutputContentTypes
});

MapHtmlPage("/", Path.Combine(siteRoot, "index.html"));
MapHtmlPage("/Fibonacci", Path.Combine(siteRoot, "Fibonacci", "index.html"));
MapHtmlPage("/Prime", Path.Combine(siteRoot, "Prime", "index.html"));
MapHtmlPage("/Home", Path.Combine(siteRoot, "Home", "index.html"));
MapHtmlPage("/Home/About", Path.Combine(siteRoot, "Home", "About", "index.html"));
MapHtmlPage("/Home/Links", Path.Combine(siteRoot, "Home", "Links", "index.html"));
MapHtmlPage("/Home/Contact", Path.Combine(siteRoot, "Home", "Contact", "index.html"));
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




