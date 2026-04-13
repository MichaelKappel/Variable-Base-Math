#if NET8_0_OR_GREATER
namespace Protocol5.UAI;

public sealed class Protocol5UaiWebsiteSupportOptions
{
    public bool CheckAcceptLanguageHeader { get; set; } = true;

    public bool SetContentLanguageHeader { get; set; } = true;

    public IList<string> QueryKeys { get; } = new List<string>
    {
        "lang",
        "culture",
        "ui-culture"
    };

    public IList<string> CookieNames { get; } = new List<string>
    {
        "Protocol5.UAI",
        ".AspNetCore.Culture"
    };
}
#endif
