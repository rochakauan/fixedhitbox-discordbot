namespace fixedhitbox.Application.DTOs;

public sealed record PendingAredlLinkDto
{
    public ulong DiscordId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string GlobalName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid Id { get; init; }
    public int? Country { get; init; }
    public DateTime CreatedAt { get; init; }
}