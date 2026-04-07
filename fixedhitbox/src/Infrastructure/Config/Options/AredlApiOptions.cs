using fixedhitbox.Infrastructure.Config.Options.Abstractions;

namespace fixedhitbox.Infrastructure.Config.Options;

public sealed class AredlApiOptions : IHttpClientOptions, IAppOptions
{
    public string Uri { get; init; } = string.Empty;
    public int TimeoutFromSeconds { get; init; } = 5;

    public string BaseUrl => Uri;
    public int TimeoutSeconds => TimeoutFromSeconds;
    public static string SectionName => "AredlApi";
}