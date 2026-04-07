using DotNetEnv;
using fixedhitbox.Application;
using fixedhitbox.Application.Interfaces.Aredl;
using fixedhitbox.Application.UseCases.LinkAredl;
using fixedhitbox.DiscordBot.Options;
using fixedhitbox.HostedService;
using fixedhitbox.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace fixedhitbox;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            Env.Load();

            var builder = Host.CreateApplicationBuilder(args);

            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile("botsettings.json", true, true)
                .AddEnvironmentVariables();
            
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            builder.Services.AddHostedService<DiscordBotService>();
            
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

            builder.Logging.ClearProviders();
            builder.Logging.SetMinimumLevel(LogLevel.Information);
            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
            builder.Logging.AddSimpleConsole();
            
            builder.Logging.AddFilter("fixedhitbox", LogLevel.Information);
            builder.Logging.AddFilter("System", LogLevel.Warning);
            builder.Logging.AddFilter("Microsoft.Host", LogLevel.Warning); 
            //TODO: Let the app settings decide which filters to apply.

            using var host = builder.Build();
            
            BotServices.Provider = host.Services;
            
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
