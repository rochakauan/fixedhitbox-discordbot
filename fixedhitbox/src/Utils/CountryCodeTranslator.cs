namespace fixedhitbox.Utils;

public static class CountryCodeTranslator
{

    public static string IsoCodeToDiscordEmojiFlag(int? countryNumeric)
    {
        if (countryNumeric is not int numericCode)
            return "🏳️";

        var country = ISO3166.Country.List
            .FirstOrDefault(c => c.NumericCode == numericCode.ToString("D3"));

        return country is null
            ? "🏳️"
            : string.Concat(country.TwoLetterCode.ToUpper().Select(c =>
                char.ConvertFromUtf32(c - 'A' + 0x1F1E6)));
    }
}