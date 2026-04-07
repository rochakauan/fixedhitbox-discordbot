using DSharpPlus.Commands.Processors.SlashCommands.Localization;

namespace fixedhitbox.DiscordBot.Commands.Translations;

public abstract class PingTranslator : IInteractionLocalizer
{

    public ValueTask<IReadOnlyDictionary<DiscordLocale, string>> TranslateAsync(string fullSymbolName)
        => fullSymbolName switch
        {
            "ping.name" => ValueTask.FromResult<IReadOnlyDictionary<DiscordLocale, string>>(
                new Dictionary<DiscordLocale, string>
                {
                    { DiscordLocale.en_US, "ping" },
                    { DiscordLocale.pt_BR, "ping" }
                }),
            "ping.description" => ValueTask.FromResult<IReadOnlyDictionary<DiscordLocale, string>>(
                new Dictionary<DiscordLocale, string>
                {
                    { DiscordLocale.en_US, "Pings the bot to check its latency." },
                    { DiscordLocale.pt_BR, "Verifica a latência do bot." }
                }),
            _ => ValueTask.FromResult<IReadOnlyDictionary<DiscordLocale, string>>(
                new Dictionary<DiscordLocale, string>())
        };
}