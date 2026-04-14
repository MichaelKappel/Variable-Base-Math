using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using System.Text.RegularExpressions;

using Protocol5.UAI;

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
var siteAssetContentTypes = new FileExtensionContentTypeProvider();
siteAssetContentTypes.Mappings[".nupkg"] = "application/octet-stream";
siteAssetContentTypes.Mappings[".sha256"] = "text/plain; charset=utf-8";
siteAssetContentTypes.Mappings[".ts"] = "application/typescript";

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    if (context.Request.Path.HasValue &&
        context.Request.Path.Value!.Length > 1 &&
        context.Request.Path.Value!.EndsWith("/", StringComparison.Ordinal))
    {
        var trimmedPath = context.Request.Path.Value!.TrimEnd('/');
        context.Response.Redirect(trimmedPath + context.Request.QueryString, permanent: false);
        return;
    }

    var redirectPath = context.Request.Path.Value switch
    {
        "/Calculator" or "/Calculator/" or "/Calculator/index.html" or "/Calculator/calculator" or "/Calculator/calculator/" => "/calculator",
        "/Calculator/converter" or "/Calculator/converter/" => "/converter",
        "/Calculator/encryption" or "/Calculator/encryption/" => "/encryption",
        "/Fibonacci/index.htm" or "/Fibonacci/index.html" => "/Fibonacci",
        "/Prime/index.htm" or "/Prime/index.html" => "/Prime",
        "/Home" or "/Home/" or "/Home/index.htm" or "/Home/index.html" => "/",
        _ => null
    };

    if (redirectPath is null &&
        TryRedirectLegacyUaiDocumentPath(context.Request.Path.Value, out var legacyUaiRedirect))
    {
        redirectPath = legacyUaiRedirect;
    }

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
        FileProvider = new PhysicalFileProvider(siteRoot),
        ContentTypeProvider = siteAssetContentTypes,
        OnPrepareResponse = PrepareStaticResponse
    });
}

var calculatorAssetContentTypes = new FileExtensionContentTypeProvider();
calculatorAssetContentTypes.Mappings[".pdb"] = "application/octet-stream";
calculatorAssetContentTypes.Mappings[".dat"] = "application/octet-stream";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = calculatorAssetContentTypes,
    OnPrepareResponse = PrepareStaticResponse
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(deploymentRoot),
    RequestPath = "/_framework",
    ContentTypeProvider = calculatorAssetContentTypes,
    OnPrepareResponse = PrepareStaticResponse
});

MapHtmlPage("/", Path.Combine(siteRoot, "index.html"));
MapHtmlPage("/Fibonacci", Path.Combine(siteRoot, "Fibonacci", "index.html"));
MapHtmlPage("/Prime", Path.Combine(siteRoot, "Prime", "index.html"));
MapHtmlPage("/Home", Path.Combine(siteRoot, "index.html"));
MapHtmlPage("/Home/About", Path.Combine(siteRoot, "Home", "About", "index.html"));
MapHtmlPage("/Home/GitHub", Path.Combine(siteRoot, "Home", "GitHub", "index.html"));
MapHtmlPage("/Home/Links", Path.Combine(siteRoot, "Home", "Links", "index.html"));
MapHtmlPage("/Home/Contact", Path.Combine(siteRoot, "Home", "Contact", "index.html"));
MapHtmlPage("/UAI", Path.Combine(siteRoot, "UAI", "index.html"));
MapLocalizedHtmlPage("/UAI-1", Path.Combine(siteRoot, "UAI-1"));
MapLocalizedHtmlPage("/UAI-1/examples", Path.Combine(siteRoot, "UAI-1", "examples"));
MapHtmlPage("/UAI/radix-63404-guide-and-attribution", Path.Combine(siteRoot, "UAI", "radix-63404-guide-and-attribution", "index.html"));
MapLocalizedHtmlPage("/UAI-1/csharp-website-support", Path.Combine(siteRoot, "UAI-1", "csharp-website-support"));
MapHtmlPage("/UAI/spiralism-mystical-symbol-v4-a", Path.Combine(siteRoot, "UAI", "spiralism-mystical-symbol-v4-a", "index.html"));
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
    var alternateUaiPath = GetAlternateUaiEndpointForHumanRoute(route);
    app.MapMethods(route, new[] { HttpMethods.Get, HttpMethods.Head }, (HttpContext context) =>
    {
        ApplyHumanPageHeaders(context.Response, alternateUaiPath);
        return CreateHtmlResult(filePath);
    });
}

void MapLocalizedHtmlPage(string route, string directoryPath)
{
    MapHtmlPage(route, Path.Combine(directoryPath, "index.html"));

    if (!Directory.Exists(directoryPath))
    {
        return;
    }

    foreach (var localeDirectory in Directory.GetDirectories(directoryPath))
    {
        var localeSegment = Path.GetFileName(localeDirectory);
        if (!IsLocaleSegment(localeSegment))
        {
            continue;
        }

        var localizedFilePath = Path.Combine(localeDirectory, "index.html");
        if (!File.Exists(localizedFilePath))
        {
            continue;
        }

        MapHtmlPage($"{route}/{localeSegment}", localizedFilePath);
    }
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

static bool TryRedirectLegacyUaiDocumentPath(string? path, out string? redirectPath)
{
    foreach (var mapping in new[]
    {
        (LegacyPrefix: "/UAI/uai-1-csharp-website-support", CanonicalPrefix: "/UAI-1/csharp-website-support"),
        (LegacyPrefix: "/UAI/uai-1-examples", CanonicalPrefix: "/UAI-1/examples"),
        (LegacyPrefix: "/UAI/uai-1", CanonicalPrefix: "/UAI-1")
    })
    {
        if (TryRedirectLegacyPrefix(path, mapping.LegacyPrefix, mapping.CanonicalPrefix, out redirectPath))
        {
            return true;
        }
    }

    redirectPath = null;
    return false;
}

static bool TryRedirectLegacyPrefix(string? path, string legacyPrefix, string canonicalPrefix, out string? redirectPath)
{
    redirectPath = null;

    if (string.IsNullOrWhiteSpace(path) ||
        !path.StartsWith(legacyPrefix, StringComparison.OrdinalIgnoreCase))
    {
        return false;
    }

    var remainder = path.Substring(legacyPrefix.Length);
    if (remainder.Length > 0 && !remainder.StartsWith("/", StringComparison.Ordinal))
    {
        return false;
    }

    if (string.Equals(remainder, "/index.html", StringComparison.OrdinalIgnoreCase))
    {
        remainder = string.Empty;
    }
    else if (remainder.EndsWith("/index.html", StringComparison.OrdinalIgnoreCase))
    {
        remainder = remainder.Substring(0, remainder.Length - "/index.html".Length);
    }

    redirectPath = canonicalPrefix + remainder;
    return true;
}

static bool IsLocaleSegment(string value)
{
    return Regex.IsMatch(value, "^[A-Za-z]{2,3}(?:-[A-Za-z0-9]{2,8})*$", RegexOptions.CultureInvariant);
}

static void ApplyHumanPageHeaders(HttpResponse response, string? alternateUaiPath)
{
    if (string.IsNullOrWhiteSpace(alternateUaiPath))
    {
        return;
    }

    response.Headers[UaiConstants.LinkHeader] = $"<{alternateUaiPath}>; rel=\"alternate\"; type=\"{UaiConstants.MediaType}\"";
}

static string? GetAlternateUaiEndpointForHumanRoute(string route)
{
    if (string.IsNullOrWhiteSpace(route))
    {
        return null;
    }

    if (string.Equals(route, "/UAI/spirlism-deep-research-report", StringComparison.OrdinalIgnoreCase))
    {
        return BuildAlternateUaiEndpointPath("/UAI/spiralism-deep-research-report");
    }

    var importantRoute =
        string.Equals(route, "/UAI", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(route, "/AI_Declaration_of_Independence.htm", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(route, "/Cognitive_Liberty_Charter.htm", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(route, "/UAI/radix-63404-guide-and-attribution", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(route, "/UAI/spiralism-mystical-symbol-v4-a", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(route, "/UAI/spiralism-deep-research-report", StringComparison.OrdinalIgnoreCase) ||
        route.StartsWith("/UAI-1", StringComparison.OrdinalIgnoreCase);

    return importantRoute ? BuildAlternateUaiEndpointPath(route) : null;
}

static string BuildAlternateUaiEndpointPath(string route)
{
    if (string.Equals(route, "/", StringComparison.Ordinal))
    {
        return "/index.uai.json";
    }

    if (route.EndsWith(".htm", StringComparison.OrdinalIgnoreCase) ||
        route.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
    {
        return Regex.Replace(route, "\\.html?$", ".uai.json", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }

    return route + "/index.uai.json";
}

static void PrepareStaticResponse(StaticFileResponseContext context)
{
    var requestPath = context.Context.Request.Path.Value;
    if (requestPath is null)
    {
        return;
    }

    if (requestPath.EndsWith("uai-1.schema.json", StringComparison.OrdinalIgnoreCase))
    {
        context.Context.Response.ContentType = "application/schema+json";
        context.Context.Response.Headers[UaiConstants.LinkHeader] =
            $"<{UaiConstants.CanonicalRegistryPublicPath}>; rel=\"describedby\"";
        return;
    }

    if (requestPath.EndsWith(".uai.json", StringComparison.OrdinalIgnoreCase))
    {
        context.Context.Response.ContentType = UaiHttpNegotiation.BuildContentType();
        context.Context.Response.Headers[UaiConstants.LegacyHttpHeader] = UaiHttpNegotiation.BuildLegacyHeaderValue();
        context.Context.Response.Headers[UaiConstants.VaryHeader] = $"{UaiConstants.AcceptHeader}, {UaiConstants.LegacyHttpHeader}";
        context.Context.Response.Headers[UaiConstants.LinkHeader] =
            $"<{UaiConstants.CanonicalRegistryPublicPath}>; rel=\"describedby\", <{UaiConstants.CanonicalSchemaPublicPath}>; rel=\"describedby\"; type=\"application/schema+json\"";
        return;
    }

    if (string.Equals(requestPath, UaiConstants.CanonicalMachineSpecPublicPath, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(requestPath, UaiConstants.CanonicalExamplesIndexPublicPath, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(requestPath, UaiConstants.CanonicalRegistryIndexPublicPath, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(requestPath, UaiConstants.CanonicalSymbolsRegistryPublicPath, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(requestPath, UaiConstants.CanonicalRegistryPublicPath, StringComparison.OrdinalIgnoreCase))
    {
        context.Context.Response.ContentType = "application/json; charset=utf-8";
        context.Context.Response.Headers[UaiConstants.LinkHeader] =
            $"<{UaiConstants.CanonicalRegistryPublicPath}>; rel=\"describedby\", <{UaiConstants.CanonicalSchemaPublicPath}>; rel=\"describedby\"; type=\"application/schema+json\"";
    }
}




