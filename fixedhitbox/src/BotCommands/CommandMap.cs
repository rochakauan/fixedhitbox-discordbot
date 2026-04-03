using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using fixedhitbox.BotCommands.Modules.Aredl;
using fixedhitbox.BotCommands.Modules.Diagnostics;
using fixedhitbox.BotCommands.Processors;

namespace fixedhitbox.BotCommands;

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

            extension.AddCommands([
                typeof(PingCommand),
                typeof(LinkAredlCommand)]);
            extension.AddProcessor(slashCommandProcessor);

        }, new CommandsConfiguration
        {
            RegisterDefaultCommandProcessors = false,
            DebugGuildId = debugGuildId
        });
    }
}