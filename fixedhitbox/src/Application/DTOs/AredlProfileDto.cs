namespace fixedhitbox.Application.DTOs;

public sealed record AredlProfileDto(
    Guid Id,
    string Username,
    string GlobalName,
    ulong DiscordId,
    string? Description,
    int? Country,
    DateTime CreatedInAredlAt,
    IReadOnlyList<AredlRecordDto> Records
);