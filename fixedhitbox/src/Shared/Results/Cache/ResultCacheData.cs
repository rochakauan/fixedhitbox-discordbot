using fixedhitbox.Domain.Enums;

namespace fixedhitbox.Shared.Results.Cache;

public sealed class ResultCacheData<T>
{
    private ResultCacheData(
        ECacheResultStatus status = ECacheResultStatus.NotFound,
        T? data = default,
        string? failMessage = null)
    {
        Status = status;
        Data = data;
        FailMessage = failMessage;
    }
    public ECacheResultStatus Status { get; }
    public T? Data { get; }
    public string? FailMessage { get; }
    public bool IsExpired => Status == ECacheResultStatus.CacheExpired;
    
    public static ResultCacheData<T> PendingConfirmation(T data)
        => new(ECacheResultStatus.PendingConfirmation, data);
    
    public static ResultCacheData<T> AlreadyLinked(T data)
        => new(ECacheResultStatus.AlreadyLinked, data);
    
    public static ResultCacheData<T> NotFound(string? failMessage = null)
        => new(ECacheResultStatus.NotFound, default, failMessage ?? "Cache not found.");
    
    public static ResultCacheData<T> ProfileFound(T data)
        => new(ECacheResultStatus.ProfileFound, data);
    
    public static ResultCacheData<T> CacheExpired(string? failMessage = null)
        => new(ECacheResultStatus.CacheExpired, default, failMessage);
}