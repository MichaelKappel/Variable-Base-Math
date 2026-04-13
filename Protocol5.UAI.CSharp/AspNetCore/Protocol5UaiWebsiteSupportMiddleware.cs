#if NET8_0_OR_GREATER
using System.Globalization;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Protocol5.UAI;

public sealed class Protocol5UaiWebsiteSupportMiddleware
{
    internal const string HttpContextItemKey = "Protocol5.UAI.LanguageTag";

    private readonly RequestDelegate _next;
    private readonly Protocol5UaiWebsiteSupportOptions _options;

    public Protocol5UaiWebsiteSupportMiddleware(
        RequestDelegate next,
        IOptions<Protocol5UaiWebsiteSupportOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var languageTag = TryResolveLanguageTag(context);
        if (languageTag is null)
        {
            await _next(context);
            return;
        }

        context.Items[HttpContextItemKey] = languageTag;

        var websiteCulture = UaiCultureInfo.CreateWebsiteCulture();
        var previousCulture = CultureInfo.CurrentCulture;
        var previousUiCulture = CultureInfo.CurrentUICulture;

        CultureInfo.CurrentCulture = websiteCulture;
        CultureInfo.CurrentUICulture = websiteCulture;

        if (_options.SetContentLanguageHeader)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers["Content-Language"] = languageTag;
                return Task.CompletedTask;
            });
        }

        try
        {
            await _next(context);
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
            CultureInfo.CurrentUICulture = previousUiCulture;
        }
    }

    private string? TryResolveLanguageTag(HttpContext context)
    {
        foreach (var queryKey in _options.QueryKeys)
        {
            if (!context.Request.Query.TryGetValue(queryKey, out var values))
            {
                continue;
            }

            foreach (var value in values)
            {
                var normalized = UaiCultureInfo.NormalizeLanguageTag(value);
                if (normalized is not null)
                {
                    return normalized;
                }
            }
        }

        foreach (var cookieName in _options.CookieNames)
        {
            if (!context.Request.Cookies.TryGetValue(cookieName, out var cookieValue))
            {
                continue;
            }

            var normalized = NormalizeCookieLanguageTag(cookieValue);
            if (normalized is not null)
            {
                return normalized;
            }
        }

        if (_options.CheckAcceptLanguageHeader)
        {
            var acceptLanguage = context.Request.Headers.AcceptLanguage.ToString();
            foreach (var candidate in acceptLanguage.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var normalized = UaiCultureInfo.NormalizeLanguageTag(candidate);
                if (normalized is not null)
                {
                    return normalized;
                }
            }
        }

        return null;
    }

    private static string? NormalizeCookieLanguageTag(string? cookieValue)
    {
        if (string.IsNullOrWhiteSpace(cookieValue))
        {
            return null;
        }

        var decodedValue = Uri.UnescapeDataString(cookieValue);
        foreach (var segment in decodedValue.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var normalized = UaiCultureInfo.NormalizeLanguageTag(segment);
            if (normalized is not null)
            {
                return normalized;
            }
        }

        return UaiCultureInfo.NormalizeLanguageTag(decodedValue);
    }
}
#endif
