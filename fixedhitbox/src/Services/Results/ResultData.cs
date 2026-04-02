using fixedhitbox.Enums;

namespace fixedhitbox.Services.Results;

public sealed class ResultData<T> : Result
{

    private ResultData(
        EResultStatus status = EResultStatus.Success,
        T? data = default,
        string? errorMessage = null,
        ulong? discordId = null)

        : base(status, errorMessage) => Data = data;

    public T? Data { get; }
    public bool IsSuccess => Status == EResultStatus.Success;

    public static ResultData<T> Success(T data)
        => new(EResultStatus.Success, data);

    public static ResultData<T> PendingConfirmation(T data)
        => new(EResultStatus.PendingConfirmation, data);
    public static ResultData<T> PendingConfirmation()
        => new(EResultStatus.PendingConfirmation);

    public static ResultData<T> AlreadyLinked(T data)
        => new(EResultStatus.AlreadyLinked, data);
    public static ResultData<T> AlreadyLinked()
        => new(EResultStatus.AlreadyLinked);

    public static ResultData<T> NotFound(string? errorMessage = null)
        => new(EResultStatus.NotFound, default, errorMessage);

    public static ResultData<T> CacheExpired(string? errorMessage = null)
        => new(EResultStatus.CacheExpired, default, errorMessage);

    public static ResultData<T> ConnectionError(string? errorMessage = null)
        => new(EResultStatus.ConnectionError, default, errorMessage);

    public static ResultData<T> ValidationError(string errorMessage)
        => new(EResultStatus.ValidationError, default, errorMessage);

    public static ResultData<T> Conflict(string errorMessage)
        => new(EResultStatus.Conflict, default, errorMessage);

    public static ResultData<T> UnexpectedError(string errorMessage)
        => new(EResultStatus.UnexpectedError, default, errorMessage);
}