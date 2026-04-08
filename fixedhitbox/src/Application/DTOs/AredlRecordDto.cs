namespace fixedhitbox.Application.DTOs;

public sealed record AredlRecordDto(
    Guid Id,
    bool Mobile,
    string? VideoUrl,
    bool IsVerification,
    bool? HideVideo,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    AredlLevelDto Level
);