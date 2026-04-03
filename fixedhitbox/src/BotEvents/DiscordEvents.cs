using DSharpPlus;
using fixedhitbox.BotEvents.Updates;

namespace fixedhitbox.BotEvents;

internal static class DiscordEvents
{

    public static async Task RegisterAll(DiscordClientBuilder builder)
    {
        try
        {
            await OnBotConnecting.UpdateAsync(builder);
        }
        catch (Exception ex)
        {
            await Task.FromException(ex);
        }
    }
}