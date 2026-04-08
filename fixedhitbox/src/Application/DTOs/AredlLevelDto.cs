namespace fixedhitbox.Application.DTOs;

public sealed record AredlLevelDto(
    Guid Id,
    string Name,
    ulong LevelId,
    bool TwoPlayer,
    int Position,
    int Points,
    bool Legacy
);