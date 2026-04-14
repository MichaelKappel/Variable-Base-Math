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
        var requestPreferences = UaiHttpNegotiation.FromHttpRequest(context.Request);
        var wantsUaiJson = _options.CheckAcceptHeader && requestPreferences.WantsUaiJson;
        var legacyHeaderVersion = _options.CheckLegacyHeader
            ? requestPreferences.LegacyVersion
            : null;

        if (languageTag is null && !wantsUaiJson && legacyHeaderVersion is null)
        {
            await _next(context);
            return;
        }

        context.Items[HttpContextItemKey] = new Protocol5UaiWebsiteRequestContext
        {
            HtmlLanguageTag = languageTag,
            WantsUaiJson = wantsUaiJson,
            LegacyHeaderVersion = legacyHeaderVersion
        };

        var previousCulture = CultureInfo.CurrentCulture;
        var previousUiCulture = CultureInfo.CurrentUICulture;
        var cultureWasChanged = false;

        if (languageTag is not null)
        {
            var websiteCulture = UaiCultureInfo.CreateWebsiteCulture();
            CultureInfo.CurrentCulture = websiteCulture;
            CultureInfo.CurrentUICulture = websiteCulture;
            cultureWasChanged = true;
        }

        if (_options.SetContentLanguageHeader || _options.SetVaryHeader || (_options.EmitLegacyHeader && (wantsUaiJson || legacyHeaderVersion is not null)))
        {
            context.Response.OnStarting(() =>
            {
                if (_options.SetContentLanguageHeader && languageTag is not null)
                {
                    context.Response.Headers["Content-Language"] = languageTag;
                }

                if (_options.SetVaryHeader)
                {
                    context.Response.Headers[UaiConstants.VaryHeader] = $"{UaiConstants.AcceptHeader}, {UaiConstants.LegacyHttpHeader}, Accept-Language";
                }

                if (_options.EmitLegacyHeader && (wantsUaiJson || legacyHeaderVersion is not null))
                {
                    context.Response.Headers[UaiConstants.LegacyHttpHeader] = UaiHttpNegotiation.BuildLegacyHeaderValue();
                }

                return Task.CompletedTask;
            });
        }

        try
        {
            await _next(context);
        }
        finally
        {
            if (cultureWasChanged)
            {
                CultureInfo.CurrentCulture = previousCulture;
                CultureInfo.CurrentUICulture = previousUiCulture;
            }
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
            foreach (var candidate in Guard.SplitAndTrim(acceptLanguage, ','))
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
        foreach (var segment in Guard.SplitAndTrim(decodedValue, '|'))
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
