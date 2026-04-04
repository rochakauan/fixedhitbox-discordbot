namespace fixedhitbox.Dtos.Aredl;

public sealed record PendingAredlLinkDto
{
    public required ulong DiscordId { get; init; }
    public required string Username { get; init; }
    public string? GlobalName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid AredlUserId { get; init; }
    public int? Country {get; init;}
}