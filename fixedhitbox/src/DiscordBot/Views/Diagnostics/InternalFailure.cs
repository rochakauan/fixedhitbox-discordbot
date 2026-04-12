using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using fixedhitbox.DiscordBot.Utils;

namespace fixedhitbox.DiscordBot.Views.Diagnostics;

internal static class InternalFailure
{
    private const string GitHubEmoji = "<:githubicon:1492835919210741790>";
        
    public static DiscordFollowupMessageBuilder CreateResponseView(CommandContext ctx)
    {
        var locale = ctx.As<SlashCommandContext>().Interaction.Locale;
        
        return new DiscordFollowupMessageBuilder()
            .AsEphemeral()
            .WithContent(BotLocalizer.Get("InternalFailure_Content", locale))
            .AddEmbed(new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Red)
                .WithTitle(BotLocalizer.Get("InternalFailure_Title", locale))
                .WithDescription(BotLocalizer.Get("InternalFailure_Description", locale))
                .AddField(
                    BotLocalizer.Get("InternalFailure_Field_Repository", locale), 
                        
                    $"[{BotLocalizer.Get("InternalFailure_Field_Repository_Param", locale, GitHubEmoji)}]" +
                    $"(https://github.com/rochakauan/fixedhitbox-discordbot)",
                    inline: true)
                    
                .WithFooter(BotLocalizer.Get("Diagnostics_Embeds_Footer", locale))
                .WithTimestamp(DateTime.UtcNow)
            );
    }
}