using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using fixedhitbox.Commands.Processors;

namespace fixedhitbox.Commands;

public static class CommandMap
{

    public static void RegisterAllCommands(DiscordClientBuilder builder, ulong debugGuildId)
    {
        builder.UseCommands((_, extension) =>
        {
            var slashCommandProcessor = new SlashCommandProcessor(new SlashCommandConfiguration
            {
                NamingPolicy = new OrdinalKebabCaseInteractionNamingPolicy()
            });

            extension.AddCommands([typeof(PingCommand)]);
            extension.AddProcessor(slashCommandProcessor);

        }, new CommandsConfiguration
        {
            RegisterDefaultCommandProcessors = false,
            DebugGuildId = debugGuildId
        });
    }
}