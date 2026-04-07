using fixedhitbox.Infrastructure.Config.Options.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace fixedhitbox.Infrastructure.Extensions;

public static class OptionsExtensions
{
    public static OptionsBuilder<T> AddAppOptions<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<OptionsBuilder<T>>? configure = null)
        where T : class, IAppOptions
    {
        var builder = services.AddOptions<T>()
            .Bind(configuration.GetSection(T.SectionName))
            .ValidateOnStart();
        
        configure?.Invoke(builder);
        
        return builder;
    }
}