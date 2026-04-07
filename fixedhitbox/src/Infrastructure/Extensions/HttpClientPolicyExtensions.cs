using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace fixedhitbox.Infrastructure.Extensions;

public static class HttpClientPolicyExtensions
{
    public static IHttpClientBuilder AddStandardPolicies(this IHttpClientBuilder builder)
        => builder.AddPolicyHandler(GetRetryPolicy());

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retry =>
                TimeSpan.FromMilliseconds(200 * retry));
    }
}