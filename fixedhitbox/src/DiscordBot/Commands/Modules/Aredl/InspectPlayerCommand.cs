using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.Localization;
using DSharpPlus.Entities;
using fixedhitbox.DiscordBot.Commands.Translations;
using fixedhitbox.DiscordBot.Commands.Translations.Aredl;

namespace fixedhitbox.DiscordBot.Commands.Modules.Aredl;

public sealed class InspectPlayerCommand
{
    
    [Command("inspect"), InteractionLocalizer<InspectPlayerTranslator>]
    [Description("Take a peek at a player's information.")]
    public static async ValueTask ExecuteAsync(CommandContext ctx)
    {
        var locale = ctx.As<SlashCommandContext>().Interaction.Locale;
        
        await ctx.RespondAsync(new DiscordWebhookBuilder()
            .WithContent("Testando"));
    }
}