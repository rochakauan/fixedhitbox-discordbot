using fixedhitbox.Application.Interfaces.Application;
using fixedhitbox.Application.Interfaces.Application.Aredl;
using fixedhitbox.Application.UseCases.LinkAredl;
using Microsoft.Extensions.DependencyInjection;

namespace fixedhitbox.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IStartLinkAredl, StartLinkAredl>();
        services.AddTransient<ICancelLinkAredl, CancelLinkAredl>();
        services.AddTransient<IConfirmLinkAredl, ConfirmLinkUseCase>();

        return services;
    }
}