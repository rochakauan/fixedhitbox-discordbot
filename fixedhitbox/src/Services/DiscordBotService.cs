using DSharpPlus;
using fixedhitbox.Commands;
using fixedhitbox.Data;
using fixedhitbox.Events;
using fixedhitbox.Options;
using fixedhitbox.Services.Apis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace fixedhitbox.Services;

public sealed class DiscordBotService(
    IOptions<DiscordOptions> options,
    ILogger<DiscordBotService> logger,
    AppDbContext db) : BackgroundService
{

    private readonly AppDbContext _db = db;
    private readonly DiscordOptions _options = options.Value;
    private readonly ILogger<DiscordBotService> _logger = logger;
    private DiscordClient? _client;

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

        builder.ConfigureServices(services =>
        {
            services.AddHttpClient();
            services.AddHttpClient<AredlApiService>(client =>
            {
                client.BaseAddress = new Uri("https://api.aredl.net/");
                client.Timeout = TimeSpan.FromSeconds(5);
            });

            //EF SQLite db
            //LinkAredlUserService
            //...TODO
        });

        CommandMap.RegisterAllCommands(builder, _options.DebugGuildId);
        await DiscordEvents.RegisterAll(builder);

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