using fixedhitbox.Enums;

namespace fixedhitbox.Services.Results;

public class AredlApiResult<T>
{

    public EAredlApiStatus Status { get; set; }
    public T? Data { get; }
    public string? ErrorMessage { get; }

    private AredlApiResult(
        EAredlApiStatus status,
        T? data = default,
        string? errorMessage = null)
    {
        Status = status;
        Data = data;
        ErrorMessage = errorMessage;
    }

    public static AredlApiResult<T> Success(T data)
        => new(EAredlApiStatus.Success, data);

    public static AredlApiResult<T> NotFound(string? errorMessage = null)
        => new(EAredlApiStatus.NotFound, default, errorMessage);

    public static AredlApiResult<T> ConnectionError(string? errorMessage = null)
        => new(EAredlApiStatus.ConnectionError, default, errorMessage);

    public static AredlApiResult<T> InvalidResponse(string errorMessage)
        => new(EAredlApiStatus.InvalidResponse, default, errorMessage);

    public static AredlApiResult<T> UnexpectedError(string errorMessage)
        => new(EAredlApiStatus.UnexpectedError, default, errorMessage);
}