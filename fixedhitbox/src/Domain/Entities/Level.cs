using fixedhitbox.Domain.Abstractions;

namespace fixedhitbox.Domain.Entities;

public sealed class Level : Entity
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    
    public ulong LevelId { get; init; }
    public bool TwoPlayer { get; init; }
    public int Position { get; init; }
    public int Points { get; init; }
    public bool Legacy { get; init; }
}