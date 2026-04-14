#if NET8_0_OR_GREATER
using System.Text;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Protocol5.UAI;

public static class Protocol5UaiEndpointRouteExtensions
{
    public static IEndpointRouteBuilder MapProtocol5UaiCanonicalArtifacts(this IEndpointRouteBuilder endpoints)
    {
        Guard.NotNull(endpoints, nameof(endpoints));

        MapJsonArtifact(endpoints, UaiConstants.CanonicalMachineSpecPublicPath, UaiConstants.GetEmbeddedProtocolDiscoveryText());
        MapJsonArtifact(endpoints, UaiConstants.CanonicalExamplesIndexPublicPath, UaiConstants.GetEmbeddedExamplesIndexText());
        MapJsonArtifact(endpoints, UaiConstants.CanonicalExamplesRegistryPublicPath, UaiConstants.GetEmbeddedExamplesIndexText());
        MapJsonArtifact(endpoints, UaiConstants.CanonicalRegistryIndexPublicPath, UaiConstants.GetEmbeddedRegistryText());
        MapJsonArtifact(endpoints, UaiConstants.CanonicalSymbolsRegistryPublicPath, UaiConstants.GetEmbeddedSymbolRegistryText());
        MapJsonArtifact(endpoints, UaiConstants.CanonicalRegistryPublicPath, UaiConstants.GetEmbeddedRegistryText());
        MapSchemaArtifact(endpoints, UaiConstants.CanonicalSchemaIndexPublicPath, UaiConstants.GetEmbeddedSchemaText());
        MapSchemaArtifact(endpoints, UaiConstants.CanonicalSchemaPublicPath, UaiConstants.GetEmbeddedSchemaText());
        MapTextArtifact(endpoints, UaiConstants.CanonicalTypesPublicPath, UaiConstants.GetEmbeddedTypesText(), "application/typescript");

        foreach (var fileName in UaiConstants.GetEmbeddedExampleFileNames())
        {
            MapUaiJsonArtifact(endpoints, $"{UaiConstants.CanonicalExamplesPublicPath}/{fileName}", UaiConstants.GetEmbeddedExampleText(fileName), CreateEndpointOptions(null));
        }

        return endpoints;
    }

    public static RouteHandlerBuilder MapProtocol5UaiDocumentEndpoint(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Func<HttpContext, CancellationToken, ValueTask<UaiDocument>> documentFactory,
        Action<Protocol5UaiEndpointOptions>? configure = null)
    {
        Guard.NotNull(endpoints, nameof(endpoints));
        Guard.NotNull(documentFactory, nameof(documentFactory));

        var endpointOptions = CreateEndpointOptions(configure);
        return endpoints.MapMethods(pattern, new[] { HttpMethods.Get, HttpMethods.Head }, async (HttpContext context, CancellationToken cancellationToken) =>
        {
            var document = await documentFactory(context, cancellationToken);
            Guard.NotNull(document, nameof(documentFactory));

            UaiDocumentNormalizer.Normalize(document);

            var validator = context.RequestServices.GetService<UaiDocumentValidator>() ?? new UaiDocumentValidator();
            var validation = validator.Validate(document);
            if (!validation.IsValid)
            {
                throw BuildValidationException(pattern, validation);
            }

            var json = UaiDocumentSerializer.Serialize(document);
            ApplyUaiJsonHeaders(context.Response, endpointOptions);
            return Results.Text(json, UaiHttpNegotiation.BuildContentType(endpointOptions.DocumentVersion), Encoding.UTF8);
        });
    }

    public static RouteHandlerBuilder MapProtocol5UaiHtmlEndpoint(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Func<string> htmlFactory,
        UaiHtmlTranslationOptions options,
        Action<Protocol5UaiEndpointOptions>? configure = null)
    {
        Guard.NotNull(htmlFactory, nameof(htmlFactory));
        Guard.NotNull(options, nameof(options));

        return endpoints.MapProtocol5UaiHtmlEndpoint(
            pattern,
            (_, _) => ValueTask.FromResult(htmlFactory()),
            _ => options,
            configure);
    }

    public static RouteHandlerBuilder MapProtocol5UaiHtmlEndpoint(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Func<HttpContext, CancellationToken, ValueTask<string>> htmlFactory,
        Func<HttpContext, UaiHtmlTranslationOptions> optionsFactory,
        Action<Protocol5UaiEndpointOptions>? configure = null)
    {
        Guard.NotNull(endpoints, nameof(endpoints));
        Guard.NotNull(htmlFactory, nameof(htmlFactory));
        Guard.NotNull(optionsFactory, nameof(optionsFactory));

        var endpointOptions = CreateEndpointOptions(configure);
        return endpoints.MapMethods(pattern, new[] { HttpMethods.Get, HttpMethods.Head }, async (HttpContext context, CancellationToken cancellationToken) =>
        {
            var html = await htmlFactory(context, cancellationToken);
            var translationOptions = optionsFactory(context) ?? throw new InvalidOperationException("UAI HTML endpoint options cannot be null.");
            var exporter = context.RequestServices.GetService<UaiHtmlExporter>() ?? new UaiHtmlExporter();
            var export = exporter.Export(html, translationOptions);

            ApplyUaiJsonHeaders(context.Response, endpointOptions);
            return Results.Text(export.Json, UaiHttpNegotiation.BuildContentType(endpointOptions.DocumentVersion), Encoding.UTF8);
        });
    }

    private static Protocol5UaiEndpointOptions CreateEndpointOptions(Action<Protocol5UaiEndpointOptions>? configure)
    {
        var options = new Protocol5UaiEndpointOptions();
        configure?.Invoke(options);
        return options;
    }

    private static void MapJsonArtifact(IEndpointRouteBuilder endpoints, string path, string json)
    {
        endpoints.MapMethods(path, new[] { HttpMethods.Get, HttpMethods.Head }, (HttpContext context) =>
        {
            ApplyJsonArtifactHeaders(context.Response);
            return Results.Text(json, "application/json", Encoding.UTF8);
        });
    }

    private static void MapSchemaArtifact(IEndpointRouteBuilder endpoints, string path, string json)
    {
        endpoints.MapMethods(path, new[] { HttpMethods.Get, HttpMethods.Head }, (HttpContext context) =>
        {
            ApplySchemaArtifactHeaders(context.Response);
            return Results.Text(json, "application/schema+json", Encoding.UTF8);
        });
    }

    private static void MapTextArtifact(IEndpointRouteBuilder endpoints, string path, string content, string contentType)
    {
        endpoints.MapMethods(path, new[] { HttpMethods.Get, HttpMethods.Head }, () => Results.Text(content, contentType, Encoding.UTF8));
    }

    private static void MapUaiJsonArtifact(IEndpointRouteBuilder endpoints, string path, string json, Protocol5UaiEndpointOptions options)
    {
        endpoints.MapMethods(path, new[] { HttpMethods.Get, HttpMethods.Head }, (HttpContext context) =>
        {
            ApplyUaiJsonHeaders(context.Response, options);
            return Results.Text(json, UaiHttpNegotiation.BuildContentType(options.DocumentVersion), Encoding.UTF8);
        });
    }

    private static void ApplyJsonArtifactHeaders(HttpResponse response)
    {
        response.Headers[UaiConstants.LinkHeader] = BuildDescribedByHeaderValue(UaiConstants.CanonicalRegistryPublicPath, UaiConstants.CanonicalSchemaPublicPath);
    }

    private static void ApplySchemaArtifactHeaders(HttpResponse response)
    {
        response.Headers[UaiConstants.LinkHeader] = $"<{UaiConstants.CanonicalRegistryPublicPath}>; rel=\"describedby\"";
    }

    private static void ApplyUaiJsonHeaders(HttpResponse response, Protocol5UaiEndpointOptions options)
    {
        if (options.EmitLegacyHeader)
        {
            response.Headers[UaiConstants.LegacyHttpHeader] = UaiHttpNegotiation.BuildLegacyHeaderValue();
        }

        if (options.EmitVaryHeader)
        {
            response.Headers[UaiConstants.VaryHeader] = $"{UaiConstants.AcceptHeader}, {UaiConstants.LegacyHttpHeader}, Accept-Language";
        }

        if (options.EmitDescribedByHeaders)
        {
            response.Headers[UaiConstants.LinkHeader] = BuildDescribedByHeaderValue(options.RegistryPath, options.SchemaPath);
        }
    }

    private static string BuildDescribedByHeaderValue(string registryPath, string schemaPath)
    {
        return $"<{registryPath}>; rel=\"describedby\", <{schemaPath}>; rel=\"describedby\"; type=\"application/schema+json\"";
    }

    private static Exception BuildValidationException(string pattern, UaiValidationResult validation)
    {
        var message = string.Join(Environment.NewLine, validation.Errors.Select(error => $"{error.Code} {error.Path}: {error.Message}"));
        return new InvalidOperationException($"UAI endpoint '{pattern}' failed validation.{Environment.NewLine}{message}");
    }
}
#endif