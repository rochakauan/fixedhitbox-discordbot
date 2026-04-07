using fixedhitbox.Domain.Enums;

namespace fixedhitbox.Shared;

public abstract class Result(EResultStatus status, string? errorMessage)
{
    public EResultStatus Status { get; } = status;
    public string? ErrorMessage { get; } = errorMessage;
}