namespace fixedhitbox.Infrastructure.External.Aredl.DTOs;

public sealed record AredlRecordResponse
{
    public Guid Id { get; }
    public AredllevelResponse? Level { get; }

    public bool? Mobile { get; }
    public string? VideoUrl { get; }
    public bool? IsVerification { get; }
    public bool? HideVideo { get; }
    public DateTime? CreatedAt { get; }
    public DateTime? UpdatedAt { get; }
}