using System.Globalization;

#if NET8_0_OR_GREATER
using Microsoft.AspNetCore.Http;
#endif

namespace Protocol5.UAI;

public static class UaiHttpNegotiation
{
    public static bool AcceptsUaiJson(string? acceptHeader)
    {
        if (string.IsNullOrWhiteSpace(acceptHeader))
        {
            return false;
        }

        return Guard.SplitAndTrim(acceptHeader, ',')
            .Any(segment => segment.StartsWith(UaiConstants.MediaType, StringComparison.OrdinalIgnoreCase));
    }

    public static bool HasLegacySignal(string? legacyHeaderValue)
    {
        return TryParseLegacyVersion(legacyHeaderValue) is not null;
    }

    public static string? TryParseLegacyVersion(string? legacyHeaderValue)
    {
        if (string.IsNullOrWhiteSpace(legacyHeaderValue))
        {
            return null;
        }

        foreach (var segment in Guard.SplitAndTrim(legacyHeaderValue, ';'))
        {
            if (segment.StartsWith("version=", StringComparison.OrdinalIgnoreCase))
            {
                return segment.Substring("version=".Length).Trim();
            }

            if (segment.Equals("1", StringComparison.OrdinalIgnoreCase) ||
                segment.Equals("1.0", StringComparison.OrdinalIgnoreCase) ||
                segment.Equals("1.0.0", StringComparison.OrdinalIgnoreCase))
            {
                return UaiConstants.CurrentDocumentVersion;
            }
        }

        return null;
    }

    public static string BuildLegacyHeaderValue(string? version = null)
    {
        return version ?? UaiConstants.LegacyCompatibilityVersion;
    }

    public static string BuildContentType(string? version = null)
    {
        return $"{UaiConstants.MediaType}; version={version ?? UaiConstants.CurrentDocumentVersion}";
    }

#if NET8_0_OR_GREATER
    public static UaiRequestPreferences FromHttpRequest(HttpRequest request)
    {
        Guard.NotNull(request, nameof(request));

        return new UaiRequestPreferences
        {
            WantsUaiJson = AcceptsUaiJson(request.Headers.Accept.ToString()),
            LegacyVersion = TryParseLegacyVersion(request.Headers[UaiConstants.LegacyHttpHeader].ToString()),
            HtmlLanguage = UaiCultureInfo.NormalizeLanguageTag(request.Headers.AcceptLanguage.ToString())
        };
    }

    public static void ApplyUaiResponseHeaders(HttpResponse response, string? version = null)
    {
        Guard.NotNull(response, nameof(response));

        response.Headers[UaiConstants.LegacyHttpHeader] = BuildLegacyHeaderValue(version);
        response.Headers[UaiConstants.VaryHeader] = $"{UaiConstants.AcceptHeader}, {UaiConstants.LegacyHttpHeader}, Accept-Language";
    }
#endif
}

public sealed class UaiRequestPreferences
{
    public bool WantsUaiJson { get; set; }

    public string? LegacyVersion { get; set; }

    public string? HtmlLanguage { get; set; }
}
