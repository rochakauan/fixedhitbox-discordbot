using DSharpPlus.Commands.Processors.SlashCommands.Localization;

namespace fixedhitbox.DiscordBot.Commands.Translations.Aredl;

public sealed class UnlinkTranslator : IInteractionLocalizer
{

    public ValueTask<IReadOnlyDictionary<DiscordLocale, string>> TranslateAsync(string fullSymbolName)
        => fullSymbolName switch
        {
            "unlink.name" => ValueTask.FromResult<IReadOnlyDictionary<DiscordLocale, string>>(
                new Dictionary<DiscordLocale, string>
                {
                    { DiscordLocale.en_US, "unlink" },
                    { DiscordLocale.pt_BR, "desvincular" }
                }),
            "unlink.description" => ValueTask.FromResult<IReadOnlyDictionary<DiscordLocale, string>>(
                new Dictionary<DiscordLocale, string>
                {
                    { DiscordLocale.en_US, "Unlink your discord account from any external service (ex: Aredl API)." },
                    { DiscordLocale.pt_BR, "Desvincula sua conta do discord de qualquer serviço externo (ex.: API da Aredl)." }
                }),
            _ => ValueTask.FromResult<IReadOnlyDictionary<DiscordLocale, string>>(
                new Dictionary<DiscordLocale, string>())
        };
}