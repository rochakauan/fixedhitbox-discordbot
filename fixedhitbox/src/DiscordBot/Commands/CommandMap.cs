using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using fixedhitbox.DiscordBot.Commands.Modules.Aredl;
using fixedhitbox.DiscordBot.Commands.Modules.Diagnostics;
using fixedhitbox.DiscordBot.Commands.Processors;

namespace fixedhitbox.DiscordBot.Commands;

public static class CommandMap
{

    public static void RegisterAllCommands(
        DiscordClientBuilder builder, ulong debugGuildId)
    {
        builder.UseCommands((_, extension) =>
        {
            var slashCommandProcessor = new SlashCommandProcessor(new SlashCommandConfiguration
            {
                NamingPolicy = new OrdinalKebabCaseInteractionNamingPolicy()
            });

            extension.AddCommands([
                typeof(PingCommand),
                typeof(LinkAredlCommand),
                typeof(UnlinkCommand),
                typeof(InspectPlayerCommand)]);
            
            extension.AddProcessor(slashCommandProcessor);

        }, new CommandsConfiguration
        {
            RegisterDefaultCommandProcessors = false,
            DebugGuildId = debugGuildId
        });
    }
}