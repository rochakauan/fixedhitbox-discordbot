using DSharpPlus;
using fixedhitbox.DiscordBot.Commands;
using fixedhitbox.DiscordBot.Events;
using fixedhitbox.DiscordBot.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace fixedhitbox.HostedService;

public sealed class DiscordBotService(
    IOptions<DiscordOptions> options,
    ILogger<DiscordBotService> logger) : BackgroundService
{

    private readonly DiscordOptions _options = options.Value;
    private DiscordClient? _client;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var builder = DiscordClientBuilder.CreateDefault(
            _options.Token,
            DiscordIntents.Guilds);

        ConfigureBotEventHandlers(builder);

        builder.ConfigureLogging(logging =>
        {
            logging.AddFilter("DSharpPlus", LogLevel.Warning);
        });

        try
        {
            CommandMap.RegisterAllCommands(builder, _options.DebugGuildId);
            await DiscordEvents.RegisterAll(builder);

            logger.LogInformation("All commands and events registered.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "All commands/events registration failed.");
        }

        _client = builder.Build();

        await _client.ConnectAsync();

        logger.LogInformation(
            "Connected to Discord! Debug server id: {debugServerId}", _options.DebugGuildId);

        await base.StartAsync(cancellationToken);

        logger.LogInformation("Press Ctrl+C to shut down the bot service.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Disconnected from Discord!");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_client is not null)
        {
            logger.LogInformation("Disconnecting...");
            await _client.DisconnectAsync();
            logger.LogInformation("Disconnected. Bot is now offline.");
        }

        await base.StopAsync(cancellationToken);
    }

    private void ConfigureBotEventHandlers(DiscordClientBuilder clientBuilder)
    {
        clientBuilder.ConfigureEventHandlers(events =>
            {
                events
                    .HandleSessionCreated((_, _) =>
                    {
                        logger.LogInformation("Session created with the gateway.");
                        return Task.CompletedTask;
                    })
                    .HandleGuildDownloadCompleted((_, _) =>
                    {
                        logger.LogInformation("Guild download completed. Bot is now ready to operate.");
                        return Task.CompletedTask;
                    });
            });
    }
}