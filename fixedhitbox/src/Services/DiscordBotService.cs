using DSharpPlus;
using fixedhitbox.BotCommands;
using fixedhitbox.BotEvents;
using fixedhitbox.Options;
using fixedhitbox.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace fixedhitbox.Services;

public sealed class DiscordBotService(
    IOptions<DiscordOptions> options,
    ILogger<DiscordBotService> logger,
    IServiceProvider serviceProvider) : BackgroundService
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

        builder.ConfigureServices(services =>
        {
            services.AddSingleton(serviceProvider.GetRequiredService<ILinkAredlService>());
            services.AddSingleton(serviceProvider.GetRequiredService<IAredlApiService>());
        });

        try
        {
            CommandMap.RegisterAllCommands(builder, _options.DebugGuildId);
            await DiscordEvents.RegisterAll(builder);
            logger.LogInformation("All commands and events registered.");
        }
        catch
        {
            logger.LogWarning("All commands and events could not be registered.");
        }

        _client = builder.Build();

        await _client.ConnectAsync();
        logger.LogInformation(
            "Connected to Discord! Debug server id: {debugServerId}", _options.DebugGuildId);

        var commands = await _client.GetGlobalApplicationCommandsAsync();
        var ping = commands.FirstOrDefault(x => x.Name == "ping");

        if (ping is not null)
        {
            await _client.DeleteGlobalApplicationCommandAsync(ping.Id);
            logger.LogInformation("Ping command deleted.");
        }

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
        try
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
                        logger.LogInformation("Guild download completed. Bot is ready to operate.");
                        return Task.CompletedTask;
                    });
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while configuring Discord event handlers.");
        }
    }
}