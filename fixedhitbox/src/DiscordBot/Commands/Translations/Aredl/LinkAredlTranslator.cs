using DSharpPlus.Commands.Processors.SlashCommands.Localization;

namespace fixedhitbox.DiscordBot.Commands.Translations.Aredl;

public class LinkAredlTranslator : IInteractionLocalizer
{

    public ValueTask<IReadOnlyDictionary<DiscordLocale, string>> TranslateAsync(string fullSymbolName)
        => fullSymbolName switch
        {
            "link-aredl.name" => ValueTask.FromResult<IReadOnlyDictionary<DiscordLocale, string>>(
                new Dictionary<DiscordLocale, string>
                {
                    { DiscordLocale.en_US, "link-aredl" },
                    { DiscordLocale.pt_BR, "vincular-aredl" }
                }),
            "link-aredl.description" => ValueTask.FromResult<IReadOnlyDictionary<DiscordLocale, string>>(
                new Dictionary<DiscordLocale, string>
                {
                    { DiscordLocale.en_US, "Link your Discord account to your AREDL profile." },
                    { DiscordLocale.pt_BR, "Vincula sua conta Discord com seu perfil da AREDL." }
                }),
            _ => ValueTask.FromResult<IReadOnlyDictionary<DiscordLocale, string>>(
                new Dictionary<DiscordLocale, string>())
        };
}