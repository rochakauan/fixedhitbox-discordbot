using fixedhitbox.DiscordBot.Options.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace fixedhitbox.DiscordBot.Options;

public static class OptionsExtensions
{
    private static OptionsBuilder<T> AddConfiguredOptions<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<OptionsBuilder<T>>? configure = null) 
        where T : class, IDcOptions
    { 
        var builder = services
            .AddOptions<T>()
            .Bind(configuration.GetSection(T.SectionName))
            .ValidateOnStart();
        
        configure?.Invoke(builder);

        return builder;
    }

    public static IServiceCollection AddDiscordOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConfiguredOptions<DiscordOptions>(configuration, options =>
        {
            options.Validate(o => !string.IsNullOrWhiteSpace(o.Token),
                "Discord token is required.");

            options.Validate(o => o.Token.Length <= 180,
                "The Discord token is too long. Ensure you have provided a valid token.");

            options.Validate(o => o.DebugGuildId >= 1,
                "Discord debug guild id must be between 1 and 100 characters.");
        });

        return services;
    }
}