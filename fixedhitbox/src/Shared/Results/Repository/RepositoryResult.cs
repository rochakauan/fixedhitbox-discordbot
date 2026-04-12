using fixedhitbox.Domain.Enums;

namespace fixedhitbox.Shared.Results.Repository;

public sealed class RepositoryResult<T>
{
    public bool Success { get; }
    public T? Value { get; }
    public string? Error { get; }
    public ERepositoryStatus Status { get; }

    private RepositoryResult(T value)
    {
        Success = true;
        Status = ERepositoryStatus.Success;
        Value = value;
        Error = null;
    }

    private RepositoryResult(ERepositoryStatus status, string error)
    {
        Success = false;
        Status = status;
        Error = error;
        Value = default;
    }

    public static RepositoryResult<T> Ok(T value) => new(value);
    public static RepositoryResult<T> Fail(ERepositoryStatus status, string error)
    => new(status, error);
}