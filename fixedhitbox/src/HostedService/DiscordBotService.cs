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
        logger.LogInformation("Starting Discord bot...");
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
            //await _client!.BulkOverwriteGlobalApplicationCommandsAsync([]);
            
            CommandMap.RegisterAllCommands(builder, _options.DebugGuildId);
            await DiscordEvents.RegisterAll(builder);

            logger.LogInformation("All commands and events registered.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Commands/events registration failed.");
        }

        _client = builder.Build();

        await _client.ConnectAsync();
        
        logger.LogInformation(
            "Connected to Discord! Debug server id: {debugServerId}", _options.DebugGuildId);
        logger.LogInformation("Press Ctrl+C to shut down Discord bot.");
        
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Shutdown signal received.");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_client is not null)
        {
            logger.LogInformation("Disconnecting from Discord...");
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