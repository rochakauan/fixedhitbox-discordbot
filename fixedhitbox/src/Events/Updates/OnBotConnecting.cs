using DSharpPlus;
using DSharpPlus.Entities;

namespace fixedhitbox.Events.Updates;

public abstract class OnBotConnecting
{

    public static Task UpdateAsync(DiscordClientBuilder builder)
    {
        builder.ConfigureEventHandlers(bot =>
        {
            bot.HandleGuildDownloadCompleted(async (sender, _) =>
            {
                await sender.UpdateStatusAsync(new DiscordActivity("🎈 Hello World!",
                    DiscordActivityType.Custom));
                
            });
        });
        
        return Task.CompletedTask;
    }
}