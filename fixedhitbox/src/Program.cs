using DotNetEnv;
using fixedhitbox.config;
using fixedhitbox.Data;
using fixedhitbox.Options;
using fixedhitbox.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace fixedhitbox;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            Env.Load();

            var builder = Host.CreateApplicationBuilder(args);

            builder.Services
                .AddDbContext<AppDbContext>((sp, options) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.UseSqlite(DbConfig.ResolveConnectionString(config));
                });

            builder.Services
                .AddOptions<DiscordOptions>()
                .Bind(builder.Configuration.GetSection("Discord"))
                .Validate(
                    options => !string.IsNullOrWhiteSpace(options.Token),
                    "Discord token is required.")

                .Validate(
                    options => options.DebugGuildId > 0,
                    "Discord debug guild id must be greater than zero.")

                .ValidateOnStart();

            builder.Services.AddHostedService<DiscordBotService>();

            builder.Logging.ClearProviders();
            builder.Logging.AddSimpleConsole(options =>
            {
                options.TimestampFormat = "[HH:mm:ss] ";
                options.SingleLine = true;
            });

            builder.Logging.SetMinimumLevel(LogLevel.Information);
            builder.Logging.AddFilter("DSharpPlus", LogLevel.Warning);

            using var host = builder.Build();
            await host.RunAsync();

            return 0;
        }
        catch (DSharpPlus.Exceptions.UnauthorizedException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            await Console.Error.WriteLineAsync(
                "(!) The request was not authorized. Check if the token provided is valid.");
            Console.ResetColor();
            return 1;
        }
        catch (OptionsValidationException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var failure in ex.Failures)
                await Console.Error.WriteLineAsync("Configuration error: " + failure);

            Console.ResetColor();
            return 1;
        }

        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            await Console.Error.WriteLineAsync("(!) Fatal error: " + ex.Message);
            Console.ResetColor();
            return 1;
        }
    }
}
