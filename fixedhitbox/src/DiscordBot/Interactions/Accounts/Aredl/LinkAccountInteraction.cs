using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using fixedhitbox.Application.DTOs;
using fixedhitbox.Application.Interfaces.Application;
using fixedhitbox.Application.Interfaces.Application.Aredl;
using fixedhitbox.DiscordBot.Utils;
using fixedhitbox.Domain.Enums;
using fixedhitbox.HostedService;
using fixedhitbox.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace fixedhitbox.DiscordBot.Interactions.Accounts.Aredl;

internal class LinkAccountInteraction
{
    internal Task RegisterAsync(DiscordClientBuilder builder)
    {
        builder.ConfigureEventHandlers(bot =>
        {
            bot.HandleComponentInteractionCreated(async (_, ev) =>
            {
                var confirmLinkAredl = BotServices.Provider
                    .GetRequiredService<IConfirmLinkAredl>();
                var cancelLinkAredl = BotServices.Provider
                    .GetRequiredService<ICancelLinkAredl>();

                var locale = ev.Interaction.Locale;

                switch (ev.Id)
                {
                    case "confirm_link":
                        {
                            var result = await confirmLinkAredl
                                .ConfirmLinkAsync(ev.User.Id);

                            await HandleConfirmationKeys(result, locale, ev);
                        }
                        break;

                    case "cancel_link":
                        {
                            var result = await cancelLinkAredl
                                .CancelLinkAsync(ev.User.Id);

                            await HandleCancellationKeys(result, locale, ev);
                        }

                        break;
                }
            });
        });

        return Task.CompletedTask;
    }

    private static async Task HandleConfirmationKeys(
        ResultData<PendingAredlLinkDto> result,
        string? locale,
        ComponentInteractionCreatedEventArgs ev)
    {
        switch (result.Status)
        {
            case EResultStatus.CacheExpired:
                await ev.Interaction.CreateResponseAsync(
                    DiscordInteractionResponseType.UpdateMessage,
                    new DiscordInteractionResponseBuilder()
                        .WithContent(BotLocalizer.Get("Aredl_Interaction_CacheExpired",
                            locale)));
                break;

            case EResultStatus.AlreadyLinked:
                await ev.Interaction.CreateResponseAsync(
                    DiscordInteractionResponseType.UpdateMessage,
                    new DiscordInteractionResponseBuilder()
                        .WithContent(BotLocalizer.Get("Aredl_Interaction_ConfirmLink_AlreadyLinked", locale)));
                break;

            case EResultStatus.Success:
                await ev.Interaction.CreateResponseAsync(
                    DiscordInteractionResponseType.UpdateMessage,
                    new DiscordInteractionResponseBuilder()
                        .WithContent(BotLocalizer.Get("Aredl_Interaction_ConfirmLink_Success", locale)));
                break;

            case EResultStatus.UnexpectedError:
            default:
                await ev.Interaction.CreateResponseAsync(
                    DiscordInteractionResponseType.UpdateMessage,
                    new DiscordInteractionResponseBuilder()
                        .WithContent(BotLocalizer.Get("Aredl_UnexpectedError_Title", locale))
                        .WithContent(BotLocalizer.Get("Aredl_UnexpectedError_Description", locale)));
                break;
        }
    }
    private static async Task HandleCancellationKeys(
        ResultData<PendingAredlLinkDto> result,
        string? locale,
        ComponentInteractionCreatedEventArgs ev)
    {
        switch (result.Status)
        {
            case EResultStatus.CacheExpired:
                await ev.Interaction.CreateResponseAsync(
                    DiscordInteractionResponseType.UpdateMessage,
                    new DiscordInteractionResponseBuilder()
                        .WithContent(BotLocalizer.Get("Aredl_Interaction_CacheExpired",
                            locale)));
                break;

            case EResultStatus.AlreadyLinked:
                await ev.Interaction.CreateResponseAsync(
                    DiscordInteractionResponseType.UpdateMessage,
                    new DiscordInteractionResponseBuilder()
                        .WithContent(BotLocalizer.Get("Aredl_Interaction_CancelLink_AlreadyLinked",
                            locale)));
                break;

            case EResultStatus.PendingConfirmation:
                await ev.Interaction.CreateResponseAsync(
                    DiscordInteractionResponseType.UpdateMessage,
                    new DiscordInteractionResponseBuilder()
                        .WithContent(BotLocalizer.Get("Aredl_Interaction_CancelLink_Success",
                            locale)));
                break;

            case EResultStatus.UnexpectedError:
            default:
                await ev.Interaction.CreateResponseAsync(
                    DiscordInteractionResponseType.UpdateMessage,
                    new DiscordInteractionResponseBuilder()
                        .WithContent(BotLocalizer.Get("Aredl_UnexpectedError_Title", locale))
                        .WithContent(BotLocalizer.Get("Aredl_UnexpectedError_Description", locale)));
                break;
        }
    }
}