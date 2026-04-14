#if NET8_0_OR_GREATER
using System.Globalization;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Protocol5.UAI;

public static class Protocol5UaiWebsiteSupportExtensions
{
    public static IServiceCollection AddProtocol5UaiWebsiteSupport(
        this IServiceCollection services,
        Action<Protocol5UaiWebsiteSupportOptions>? configure = null)
    {
        services.AddOptions<Protocol5UaiWebsiteSupportOptions>();

        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.TryAddSingleton<UaiDocumentParser>();
        services.TryAddSingleton<UaiDocumentValidator>();
        services.TryAddSingleton<UaiHtmlExporter>();
        services.TryAddSingleton<UaiHtmlRenderer>();
        services.TryAddSingleton<UaiHtmlTranslator>();

        return services;
    }

    public static IApplicationBuilder UseProtocol5UaiWebsiteSupport(this IApplicationBuilder app)
    {
        return app.UseMiddleware<Protocol5UaiWebsiteSupportMiddleware>();
    }

    public static bool IsProtocol5UaiRequest(this HttpContext context)
    {
        return context.GetProtocol5UaiRequestContext() is not null;
    }

    public static Protocol5UaiWebsiteRequestContext? GetProtocol5UaiRequestContext(this HttpContext context)
    {
        if (context.Items.TryGetValue(Protocol5UaiWebsiteSupportMiddleware.HttpContextItemKey, out var value) &&
            value is Protocol5UaiWebsiteRequestContext requestContext)
        {
            return requestContext;
        }

        return null;
    }

    public static bool WantsProtocol5UaiJson(this HttpContext context)
    {
        return context.GetProtocol5UaiRequestContext()?.WantsUaiJson == true;
    }

    public static string GetProtocol5HtmlLanguage(this HttpContext context)
    {
        var requestContext = context.GetProtocol5UaiRequestContext();
        if (!string.IsNullOrWhiteSpace(requestContext?.HtmlLanguageTag))
        {
            return requestContext.HtmlLanguageTag!;
        }

        return string.IsNullOrWhiteSpace(CultureInfo.CurrentUICulture.Name)
            ? UaiCultureInfo.CanonicalLanguageTag
            : CultureInfo.CurrentUICulture.Name;
    }
}
#endif