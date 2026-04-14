#if NET8_0_OR_GREATER
namespace Protocol5.UAI;

public sealed class Protocol5UaiWebsiteRequestContext
{
    public string? HtmlLanguageTag { get; set; }

    public bool WantsUaiJson { get; set; }

    public string? LegacyHeaderVersion { get; set; }
}
#endif
