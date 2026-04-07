using fixedhitbox.Domain.Abstractions;

namespace fixedhitbox.Domain.Entities;

public sealed class Record : Entity
{
    public Guid Id { get; init; }
    public bool Mobile { get; init; }
    public string VideoUrl { get; init; } = string.Empty;
    public bool IsVerification { get; init; }
    public bool HideVideo { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    public Level Level { get; init; } = new();
}