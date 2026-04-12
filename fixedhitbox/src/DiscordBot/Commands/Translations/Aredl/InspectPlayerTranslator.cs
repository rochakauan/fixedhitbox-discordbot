using DSharpPlus.Commands.Processors.SlashCommands.Localization;

namespace fixedhitbox.DiscordBot.Commands.Translations.Aredl;

public class InspectPlayerTranslator : IInteractionLocalizer
{

    public ValueTask<IReadOnlyDictionary<DiscordLocale, string>> TranslateAsync(string fullSymbolName)
        => fullSymbolName switch
        {
            "inspect.name" => ValueTask.FromResult<IReadOnlyDictionary<DiscordLocale, string>>(
                new Dictionary<DiscordLocale, string>
                {
                    { DiscordLocale.en_US, "inspect" },
                    { DiscordLocale.pt_BR, "inspecionar" }
                }),
            "inspect.description" => ValueTask.FromResult<IReadOnlyDictionary<DiscordLocale, string>>(
                new Dictionary<DiscordLocale, string>
                {
                    { DiscordLocale.en_US, "Take a peek at a player's information.." },
                    { DiscordLocale.pt_BR, "Dá uma espiada nas informações de um jogador." }
                }),
            _ => ValueTask.FromResult<IReadOnlyDictionary<DiscordLocale, string>>(
                new Dictionary<DiscordLocale, string>())
        };
}