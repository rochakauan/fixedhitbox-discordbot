using DSharpPlus;
using fixedhitbox.Commands;
using fixedhitbox.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace fixedhitbox.Services;

public sealed class DiscordBotService : BackgroundService
{
    private readonly DiscordOptions _options;
    private readonly ILogger<DiscordBotService> _logger;
    private DiscordClient? _client;

    public DiscordBotService(
        IOptions<DiscordOptions> options,
        ILogger<DiscordBotService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var builder = DiscordClientBuilder.CreateDefault(
            _options.Token,
            DiscordIntents.Guilds);
        
        builder.ConfigureEventHandlers(events =>
        {
            events
                .HandleSessionCreated((_, _) =>
                {
                    _logger.LogInformation("[FixedHitbox] Session created with the gateway.");
                    return Task.CompletedTask;
                })
                .HandleGuildDownloadCompleted((_, _) =>
                {
                    _logger.LogInformation("[FixedHitbox] Guild download completed. Bot is ready to operate.");
                    return Task.CompletedTask;
                });
        });
        
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSimpleConsole(options =>
            {
                options.TimestampFormat = "[HH:mm:ss] ";
                options.SingleLine = true;
            });

            logging.SetMinimumLevel(LogLevel.Warning);
            logging.AddFilter("DSharpPlus", LogLevel.Warning);
        });
        
        CommandMap.RegisterAllCommands(builder, _options.DebugGuildId);
        
        _client = builder.Build();
        
        await _client.ConnectAsync();
        _logger.LogInformation(
            "[FixedHitbox] Connected to Discord! Debug server id: {debugServerId}", _options.DebugGuildId);
      
        var commands = await _client.GetGlobalApplicationCommandsAsync();
        var ping = commands.FirstOrDefault(x => x.Name == "ping");

        if (ping is not null)
        {
            await _client.DeleteGlobalApplicationCommandAsync(ping.Id);
            _logger.LogInformation("[FixedHitbox] Ping command deleted.");
        }
        
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("[FixedHitbox] Disconnected from Discord!");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_client is not null)
        {
            _logger.LogInformation("[FixedHitbox] Disconnecting...");
            await _client.DisconnectAsync();
            _logger.LogInformation("[FixedHitbox] Disconnected. Bot is now offline.");
        }

        await base.StopAsync(cancellationToken);
    }
}