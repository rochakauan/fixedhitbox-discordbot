using fixedhitbox.Application.Interfaces;
using fixedhitbox.Domain.Repositories;
using fixedhitbox.Infrastructure.Cache;
using fixedhitbox.Infrastructure.Config;
using fixedhitbox.Infrastructure.Config.Options;
using fixedhitbox.Infrastructure.Data;
using fixedhitbox.Infrastructure.Data.Repositories;
using fixedhitbox.Infrastructure.Extensions;
using fixedhitbox.Infrastructure.External.Aredl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace fixedhitbox.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.AddSingleton<IAredlCache, AredlCache>();

        //Repositories and EF Contexts.
        services.AddDbContextFactory<AppDbContext>(options =>
            options.UseSqlite(DbConfig.ResolveConnectionString(configuration))
        );
        services.AddSingleton<IUserRepository, UserRepository>();
        
        //External API services.
        services.AddAppOptions<AredlApiOptions>(configuration, options =>
        {
            options.Validate(o => !string.IsNullOrWhiteSpace(o.Uri),
                "AredlApi Uri is required");

            options.Validate(o => o.TimeoutFromSeconds is >= 1 and <= 30,
                "AredlApi Timeout must be between 1 and 30 seconds");
        });

        services.AddConfiguredHttpClient<
            IAredlApiService,
            AredlApiService,
            AredlApiOptions>();
        
        return services;
    }
}