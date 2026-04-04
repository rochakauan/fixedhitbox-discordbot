using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.Localization;
using DSharpPlus.Entities;
using fixedhitbox.BotCommands.Translations;
using fixedhitbox.Enums;
using fixedhitbox.Services.Interfaces;
using fixedhitbox.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace fixedhitbox.BotCommands.Modules.Aredl;

public sealed class LinkAredlCommand(IServiceProvider sp)
{
    private readonly ILinkAredlService _linkAredlService = sp.GetRequiredService<ILinkAredlService>();
    
    
    [Command("link-with-demonlist"), InteractionLocalizer<LinkAredlTranslator>]
    [System.ComponentModel.Description("Link your Discord account to your AREDL and POINTERCRATE profile.")]
    public async ValueTask ExecuteAsync(CommandContext ctx)
    {
        var locale = ctx.As<SlashCommandContext>()?.Interaction.Locale;

        await ctx.RespondAsync(new DiscordInteractionResponseBuilder()
            .WithContent(BotLocalizer.Get("Aredl_ConnectingApi", locale))
            .AsEphemeral());

        var result = await _linkAredlService.StartLinkAsync(ctx.User.Id);
        var response = result.Data;

        switch (result.Status)
        {
            case EResultStatus.ConnectionError:
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent(BotLocalizer.Get("Aredl_ConnectionError", locale)));
                break;
            
            case EResultStatus.NotFound:
                var errorEmbed = new DiscordEmbedBuilder()
                    .WithTitle(BotLocalizer.Get("Aredl_NotFound_Title", locale))
                    .WithDescription(BotLocalizer.Get("Aredl_NotFound_Description", locale))
                    .WithColor(DiscordColor.Red)
                    .AddField(
                        BotLocalizer.Get("Aredl_NotFound_Field_AredlLink", locale),
                        "https://aredl.net/",
                        inline: true)
                    .WithFooter(BotLocalizer.Get("Aredl_Embeds_Footer", locale), ctx.Client.CurrentUser.AvatarUrl)
                    .WithTimestamp(DateTime.UtcNow.ToLocalTime());

                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(errorEmbed));
                break;
            
            case EResultStatus.AlreadyLinked:
                if (response is null)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                        .WithContent(BotLocalizer.Get("Aredl_AlreadyLinked_ShortMessage", locale)));
                    break;
                }

                var alreadyEmbed = new DiscordEmbedBuilder()
                    .WithTitle(BotLocalizer.Get("Aredl_AlreadyLinked_Title", locale))
                    .WithDescription(BotLocalizer.Get("Aredl_AlreadyLinked_Description", locale))
                    .WithColor(DiscordColor.Red)
                    .AddField(BotLocalizer.Get("Aredl_AlreadyLinked_Field_UserProfile", locale),
                        $"https://aredl.net/profile/user{response.Username}",
                        inline: true)
                    .WithFooter(BotLocalizer.Get("Aredl_Embeds_Footer", locale), ctx.Client.CurrentUser.AvatarUrl)
                    .WithTimestamp(DateTime.UtcNow.ToLocalTime());
                
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(alreadyEmbed));
                break;
            
            case EResultStatus.Success:
            case EResultStatus.PendingConfirmation:
                if (response is null)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                        .WithContent(BotLocalizer.Get("Aredl_PendingConfirmation_When_NullApiResponse", locale)));
                    break;
                }

                var userMention = ctx.User.Mention;
                var embed = new DiscordEmbedBuilder()
                    .WithTitle(BotLocalizer.Get("Aredl_ProfileFound_Title", locale))
                    .WithDescription(BotLocalizer.Get("Aredl_ProfileFound_Description", locale, userMention))
                    .WithColor(DiscordColor.Yellow)
                    .AddField(BotLocalizer.Get(
                        "Aredl_ProfileFound_Field_Player", locale),
                        response.GlobalName ?? response.Username,
                        inline: true)
                    .AddField(BotLocalizer.Get(
                        "Aredl_ProfileFound_Field_Country", locale),
                        CountryCodeTranslator.IsoCodeToDiscordEmojiFlag(response.Country),
                        inline: true)
                    .AddField(BotLocalizer.Get(
                        "Aredl_ProfileFound_Field_Records", locale),
                        "Not implemented yet",
                        inline: true)
                    .WithFooter(BotLocalizer.Get("Aredl_Embeds_Footer", locale),
                        ctx.Client.CurrentUser.AvatarUrl)
                    .WithTimestamp(DateTime.UtcNow.ToLocalTime());

                var btnYes = new DiscordButtonComponent(
                    DiscordButtonStyle.Success,
                    "confirm_link",
                    BotLocalizer.Get("Aredl_ProfileFound_Interaction_ConfirmLink", locale),
                    false,
                    new DiscordComponentEmoji("✅"));

                var btnNo = new DiscordButtonComponent(
                    DiscordButtonStyle.Danger,
                    "cancel_link",
                    BotLocalizer.Get("Aredl_ProfileFound_Interaction_CancelLink", locale),
                    false,
                    new DiscordComponentEmoji("✖️"));

                var finalBotResponse = new DiscordWebhookBuilder()
                    .AddEmbed(embed)
                    .AddActionRowComponent(btnYes, btnNo);
                
                await ctx.EditResponseAsync(finalBotResponse);
                break;
            
            default:
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(new DiscordEmbedBuilder().WithColor(DiscordColor.Red))
                    .WithContent(BotLocalizer.Get("Aredl_UnexpectedError", locale)));
                break;
        }
    }
}