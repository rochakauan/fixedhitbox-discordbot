using fixedhitbox.Enums;

namespace fixedhitbox.Services.Results;

public abstract class Result(EResultStatus status, string? errorMessage)
{
    public EResultStatus Status { get; } = status;
    public string? ErrorMessage { get; } = errorMessage;
}