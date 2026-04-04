using System.Text.Json.Serialization;

namespace fixedhitbox.Models;

public sealed class LinkedUser
{
    [JsonIgnore]
    public int Id { get; init; }
    public ulong DiscordId { get; init; }

    public string Username { get; set; } = string.Empty;
    public string? GlobalName { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid AredlUserId { get; init; }
    public int? Country { get; set; }
    public byte BanLevel { get; set; }

    public DateTime LinkedAtUtc { get; private set; }
    public DateTime LastUpdateAtUtc { get; set; }

    private LinkedUser() { }

    public LinkedUser(
        ulong discordId, string username,
        string? globalName, string? description,
        Guid aredlUserId, int? country)
    {
        DiscordId = discordId;
        Username = username;
        GlobalName = globalName;
        Description = description;
        AredlUserId = aredlUserId;
        Country = country;
    }
}