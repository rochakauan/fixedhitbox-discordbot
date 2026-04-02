using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.Localization;
using DSharpPlus.Entities;
using fixedhitbox.Commands.Translations;
using fixedhitbox.Utils;

namespace fixedhitbox.Commands.Modules.Aredl;

public sealed class LinkAredlCommand
{

    [Command("link-with-demonlist"), InteractionLocalizer<LinkAredlTranslator>]
    [System.ComponentModel.Description("Link your Discord account to your AREDL and POINTERCRATE profile.")]
    public async ValueTask ExecuteAsync(CommandContext ctx)
    {
        var locale = ctx.As<SlashCommandContext>()?.Interaction.Locale;

        await ctx.RespondAsync(new DiscordInteractionResponseBuilder()
            .WithContent(BotLocalizer.Get("Aredl_ConnectingApi", locale))
            .AsEphemeral());
        
        
    }
}