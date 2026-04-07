namespace fixedhitbox.Infrastructure.Config.Options.Abstractions;

public interface IHttpClientOptions
{
    string BaseUrl { get; }
    int TimeoutSeconds { get; }
}