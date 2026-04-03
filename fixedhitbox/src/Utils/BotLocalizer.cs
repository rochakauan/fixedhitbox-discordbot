using System.Globalization;
using fixedhitbox.Resources;

namespace fixedhitbox.Utils;

public static class BotLocalizer
{

    public static string Get(string key, string? discordLocale, params object[] args)
    {
        var culture = ParseLocale(discordLocale);
        var value = Langs.ResourceManager.GetString(key, culture)
                    ?? Langs.ResourceManager.GetString(key, CultureInfo.InvariantCulture)
                    ?? key;

        return args.Length > 0 ? string.Format(value, args) : value;
    }

    private static CultureInfo ParseLocale(string? locale)
    {
        if (string.IsNullOrEmpty(locale))
            return CultureInfo.InvariantCulture;
        
        try { return CultureInfo.GetCultureInfo(locale); }
        catch { return CultureInfo.InvariantCulture; }
    }
}