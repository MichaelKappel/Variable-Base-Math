using System.Globalization;

namespace Protocol5.UAI;

public static class UaiCultureInfo
{
    public const string CanonicalVersion = UaiConstants.SpecName;
    public const string CanonicalLanguageTag = UaiConstants.LegacyLanguageTag;

    private static readonly string[] SupportedTags =
    {
        CanonicalLanguageTag,
        "uai-1",
        "x-uai",
        "uai"
    };

    private static readonly Lazy<CultureInfo> WebsiteCulture = new(CreateWebsiteCultureCore);

    public static IReadOnlyList<string> AcceptedLanguageTags => SupportedTags;

    public static CultureInfo CanonicalSerializationCulture => CultureInfo.InvariantCulture;

    public static CultureInfo CreateWebsiteCulture()
    {
        return WebsiteCulture.Value;
    }

    public static bool IsCanonicalCultureAvailable()
    {
        return string.Equals(CreateWebsiteCulture().Name, CanonicalLanguageTag, StringComparison.OrdinalIgnoreCase);
    }

    public static bool MatchesLanguageTag(string? languageTag)
    {
        return NormalizeLanguageTag(languageTag) is not null;
    }

    public static string? NormalizeLanguageTag(string? languageTag)
    {
        if (string.IsNullOrWhiteSpace(languageTag))
        {
            return null;
        }

        var candidate = languageTag.Trim();

        var separatorIndex = candidate.IndexOfAny(new[] { ',', ';', '|' });
        if (separatorIndex >= 0)
        {
            candidate = candidate.Substring(0, separatorIndex).Trim();
        }

        var equalsIndex = candidate.IndexOf('=');
        if (equalsIndex >= 0 && equalsIndex < candidate.Length - 1)
        {
            candidate = candidate.Substring(equalsIndex + 1).Trim();
        }

        foreach (var supportedTag in SupportedTags)
        {
            if (string.Equals(candidate, supportedTag, StringComparison.OrdinalIgnoreCase))
            {
                return CanonicalLanguageTag;
            }
        }

        return null;
    }

    private static CultureInfo CreateWebsiteCultureCore()
    {
        try
        {
            return CultureInfo.ReadOnly(CultureInfo.GetCultureInfo(CanonicalLanguageTag));
        }
        catch (CultureNotFoundException)
        {
            return CultureInfo.InvariantCulture;
        }
    }
}
