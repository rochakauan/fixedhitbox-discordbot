namespace fixedhitbox.Domain.Enums;

public enum EResultStatus
{
    Success,
    NotFound,
    AlreadyLinked,
    PendingConfirmation,
    CacheExpired,
    ConnectionError,
    ValidationError,
    Conflict,
    UnexpectedError
}