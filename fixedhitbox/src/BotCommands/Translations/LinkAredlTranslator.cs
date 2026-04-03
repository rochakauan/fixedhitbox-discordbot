using DSharpPlus.Commands.Processors.SlashCommands.Localization;

namespace fixedhitbox.BotCommands.Translations;

public class LinkAredlTranslator : IInteractionLocalizer
{

    public ValueTask<IReadOnlyDictionary<DiscordLocale, string>> TranslateAsync(string fullSymbolName)
        => fullSymbolName switch
        {
            "link-with-demonlist.name" => ValueTask.FromResult<IReadOnlyDictionary<DiscordLocale, string>>(
                new Dictionary<DiscordLocale, string>
                {
                    { DiscordLocale.en_US, "link-with-demonlist" },
                    { DiscordLocale.pt_BR, "vincular-a-demonlist" }
                }),
            "link-with-demonlist.description" => ValueTask.FromResult<IReadOnlyDictionary<DiscordLocale, string>>(
                new Dictionary<DiscordLocale, string>
                {
                    { DiscordLocale.en_US, "Link your Discord account to your AREDL and POINTERCRATE profie." },
                    { DiscordLocale.pt_BR, "Vincula sua conta Discord com seu perfil da AREDL e POINTERCRATE." }
                }),
            _ => ValueTask.FromResult<IReadOnlyDictionary<DiscordLocale, string>>(
                new Dictionary<DiscordLocale, string>())
        };
}