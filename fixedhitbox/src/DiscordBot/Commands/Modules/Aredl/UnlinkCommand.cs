using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.Localization;
using fixedhitbox.Application.Interfaces.Application.Aredl;
using fixedhitbox.DiscordBot.Commands.Translations.Aredl;
using fixedhitbox.DiscordBot.Views.Diagnostics;
using fixedhitbox.HostedService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace fixedhitbox.DiscordBot.Commands.Modules.Aredl;

public sealed class UnlinkCommand
{

    [Command("unlink"), InteractionLocalizer<UnlinkTranslator>]
    [Description("Unlink your discord account from any external service (ex: Aredl API).")]
    public static async ValueTask ExecuteAsync(CommandContext ctx)
    {
        var locale = ctx.As<SlashCommandContext>().Interaction.Locale;

        using var scope = BotServices.Provider.CreateScope();
        if (scope.ServiceProvider.GetService<IUnlink>() is null)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<UnlinkCommand>>();
            logger.LogCritical("The dependency injection for the IUnlink service has not been resolved. " +
                               "To preserve the Bot's state, the /unlink command is disabled.");
            
            await ctx.RespondAsync(InternalFailure.CreateResponseView(ctx));
            return;
        }

        await ctx.RespondAsync("Testando...");
    }
}