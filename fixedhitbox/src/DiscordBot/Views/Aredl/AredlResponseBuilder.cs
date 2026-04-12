using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using fixedhitbox.Application.DTOs;
using fixedhitbox.DiscordBot.Utils;

namespace fixedhitbox.DiscordBot.Views.Aredl;

public static class AredlResponseBuilder
{
    public static DiscordInteractionResponseBuilder ConnectingToApi(CommandContext ctx)
    {
        var locale = ctx.As<SlashCommandContext>().Interaction.Locale;

        return new DiscordInteractionResponseBuilder()
            .WithContent(BotLocalizer.Get("Aredl_ConnectingApi", locale));
    }
    public static DiscordWebhookBuilder NotFound(CommandContext ctx)
    {
        var locale = ctx.As<SlashCommandContext>().Interaction.Locale;

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

        return new DiscordWebhookBuilder().AddEmbed(errorEmbed);
    }

    public static DiscordWebhookBuilder AlreadyLinked(
        string username,
        CommandContext ctx,
        bool shortSentence = false)
    {
        var locale = ctx.As<SlashCommandContext>().Interaction.Locale;

        var alreadyEmbed = new DiscordEmbedBuilder()
            .WithTitle(BotLocalizer.Get("Aredl_AlreadyLinked_Title", locale))
            .WithDescription(BotLocalizer.Get("Aredl_AlreadyLinked_Description", locale))
            .WithColor(DiscordColor.Red)
            .AddField(BotLocalizer.Get("Aredl_AlreadyLinked_Field_UserProfile", locale),
                $"https://aredl.net/profile/user/{username}",
                inline: true)
            .WithFooter(BotLocalizer.Get("Aredl_Embeds_Footer", locale), ctx.Client.CurrentUser.AvatarUrl)
            .WithTimestamp(DateTime.UtcNow.ToLocalTime());

        var webhook = !shortSentence
            ? new DiscordWebhookBuilder().AddEmbed(alreadyEmbed)
            : new DiscordWebhookBuilder()
                .WithContent(BotLocalizer.Get("Aredl_AlreadyLinked_ShortMessage", locale));

        return webhook;
    }

    public static DiscordWebhookBuilder Pending(
        PendingAredlLinkDto dto,
        CommandContext ctx,
        bool shortSentence = false)
    {
        var locale = ctx.As<SlashCommandContext>().Interaction.Locale;

        var hardestRecord = dto.Records?.FirstOrDefault();
        var hardestDemon = hardestRecord?.Level.Name;

        var userMention = ctx.User.Mention;
        var embed = new DiscordEmbedBuilder()
            .WithTitle(BotLocalizer.Get("Aredl_ProfileFound_Title", locale))
            .WithDescription(BotLocalizer.Get("Aredl_ProfileFound_Description", locale, userMention))
            .WithColor(DiscordColor.Yellow)

            .AddField(BotLocalizer.Get(
                    "Aredl_ProfileFound_Field_Player", locale),
                dto.GlobalName,
                inline: true)

            .AddField(BotLocalizer.Get(
                    "Aredl_ProfileFound_Field_Country", locale),
                CountryCodeTranslator.IsoCodeToDiscordEmojiFlag(dto.Country),
                inline: true)

            .AddField(BotLocalizer.Get(
                    "Aredl_ProfileFound_Field_Hardest", locale),
                $"{hardestDemon ?? "❓"}",
                inline: true)

            .WithThumbnail(ctx.User.AvatarUrl)
            .WithFooter(BotLocalizer.Get("Aredl_Embeds_Footer", locale),
                ctx.Client.CurrentUser.AvatarUrl)
            .WithTimestamp(DateTime.UtcNow.ToLocalTime());

        var buttons = PendingButtons(locale);

        var webhook = !shortSentence
            ? new DiscordWebhookBuilder().AddEmbed(embed)
                .AddActionRowComponent(buttons[0], buttons[1])
            : new DiscordWebhookBuilder()
                .WithContent(BotLocalizer.Get("Aredl_PendingConfirmation_When_NullApiResponse",
                    locale));

        return webhook;
    }

    public static DiscordWebhookBuilder ConnectionError(CommandContext ctx)
    {
        var locale = ctx.As<SlashCommandContext>().Interaction.Locale;

        return new DiscordWebhookBuilder()
            .WithContent(BotLocalizer.Get("Aredl_ConnectingApi", locale));
    }

    public static DiscordWebhookBuilder UnexpectedError(CommandContext ctx)
    {
        var locale = ctx.As<SlashCommandContext>().Interaction.Locale;

        return new DiscordWebhookBuilder()
            .WithContent(BotLocalizer.Get("Aredl_UnexpectedError_Message", locale))
            .AddEmbed(new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Red)
                .WithTitle(BotLocalizer.Get("Aredl_UnexpectedError_Title", locale))
                .WithDescription(BotLocalizer.Get("Aredl_UnexpectedError_Description", locale)));
    }
    private static DiscordButtonComponent[] PendingButtons(string? locale)
    {
        DiscordButtonComponent[] buttons =
        [
            new (
                DiscordButtonStyle.Success,
                "confirm_link",
                BotLocalizer.Get("Aredl_ProfileFound_Interaction_ConfirmLink", locale),
                false,
                new DiscordComponentEmoji("✅")),

            new (
            DiscordButtonStyle.Danger,
            "cancel_link",
            BotLocalizer.Get("Aredl_ProfileFound_Interaction_CancelLink", locale),
            false,
            new DiscordComponentEmoji("✖️"))
        ];

        return buttons;
    }
}