using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.Localization;
using DSharpPlus.Entities;
using fixedhitbox.Application.Interfaces.Aredl;
using fixedhitbox.DiscordBot.Commands.Translations;
using fixedhitbox.DiscordBot.Utils;
using fixedhitbox.DiscordBot.Views.Aredl;
using fixedhitbox.Domain.Enums;
using fixedhitbox.HostedService;
using Microsoft.Extensions.DependencyInjection;

namespace fixedhitbox.DiscordBot.Commands.Modules.Aredl;

public sealed class LinkAredlCommand()
{

    [Command("link-aredl"), InteractionLocalizer<LinkAredlTranslator>]
    [Description("Link your Discord account to your AREDL profile.")]
    public async ValueTask ExecuteAsync(CommandContext ctx)
    {
        var locale = ctx.As<SlashCommandContext>().Interaction.Locale;
        
        using var scope = BotServices.Provider.CreateScope();
        var exists = scope.ServiceProvider
        .GetService<IStartLinkAredl>() ?? throw new
        NullReferenceException("IStartLinkAredl not registered");

        var useCase = scope.ServiceProvider
            .GetRequiredService<IStartLinkAredl>();

        await ctx.RespondAsync(AredlResponseBuilder.ConnectingToApi(ctx)
            .AsEphemeral());

        var result = await useCase.StartLinkAsync(ctx.User.Id);

        if (RequiresData(result.Status) && result.Data is null)
        {
            await ctx.EditResponseAsync(AredlResponseBuilder.UnexpectedError(ctx));
            return;
        }

        switch (result.Status)
        {
            case EResultStatus.ConnectionError:
                await ctx.EditResponseAsync(AredlResponseBuilder.ConnectionError(ctx));
                break;

            case EResultStatus.NotFound:
                await ctx.EditResponseAsync(AredlResponseBuilder.NotFound(ctx));
                break;

            case EResultStatus.AlreadyLinked:
                await ctx.EditResponseAsync(AredlResponseBuilder.AlreadyLinked(
                    result.Data!.Username, ctx));
                break;

            case EResultStatus.Success:
            case EResultStatus.PendingConfirmation:
                await ctx.EditResponseAsync(AredlResponseBuilder.Pending(result.Data!, ctx));
                break;

            case EResultStatus.UnexpectedError:
                await ctx.EditResponseAsync(AredlResponseBuilder.UnexpectedError(ctx));
                break;

            default:
                await SafeRespondAsync(ctx, locale!);
                break;
        }
    }

    private static bool RequiresData(EResultStatus status)
    {
        return status is EResultStatus.AlreadyLinked
            or EResultStatus.Success
            or EResultStatus.PendingConfirmation;
    }

    private static async Task SafeRespondAsync(CommandContext ctx, string locale)
    {
        try
        {
            await ctx.EditResponseAsync(AredlResponseBuilder.UnexpectedError(ctx));
        }
        catch
        {
            await ctx.FollowupAsync(new DiscordWebhookBuilder()
                .WithContent(BotLocalizer.Get("Aredl_UnexpectedError_Message", locale)));
        }
    }
}