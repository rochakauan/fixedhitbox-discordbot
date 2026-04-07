using fixedhitbox.Infrastructure.Config.Options.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace fixedhitbox.Infrastructure.Extensions;

public static class HttpClientExtensions
{
    public static IHttpClientBuilder AddConfiguredHttpClient<TClient, TImplementation, TOptions>
        (this IServiceCollection services)
        where TClient : class
        where TImplementation : class, TClient
        where TOptions : class, IHttpClientOptions
    {
        return services.AddHttpClient<TClient, TImplementation>()
            .ConfigureHttpClient((provider, client) =>
            {
                var options = provider
                    .GetRequiredService<IOptions<TOptions>>()
                    .Value;

                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            })
            .AddStandardPolicies();
    }
}