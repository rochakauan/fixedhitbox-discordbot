using System.Diagnostics;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.Localization;
using fixedhitbox.DiscordBot.Commands.Translations;
using fixedhitbox.DiscordBot.Utils;

namespace fixedhitbox.DiscordBot.Commands.Modules.Diagnostics;

public sealed class PingCommand
{

    [Command("ping"), InteractionLocalizer<PingTranslator>]
    [System.ComponentModel.Description("Pings the bot to check its latency.")]
    public static async ValueTask ExecuteAsync(CommandContext ctx)
    {
        var locale = ctx.As<SlashCommandContext>()?.Interaction.Locale;
        
        var sw = Stopwatch.StartNew();
        await ctx.RespondAsync(BotLocalizer.Get("Ping_Measuring", locale));
        sw.Stop();
        
        var gatewayMs = ctx.Client.GetConnectionLatency(0).TotalMilliseconds;
        var responseMs = sw.ElapsedMilliseconds;
        
        var content = gatewayMs > 0
            ? BotLocalizer.Get("Ping_Response", locale, gatewayMs, responseMs)
            : BotLocalizer.Get("Ping_Pong", locale, responseMs);
        
        await ctx.EditResponseAsync(content);
    }
}