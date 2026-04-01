using System.ComponentModel.DataAnnotations;

namespace fixedhitbox.Options;

public sealed class DiscordOptions
{
    
    [Required(AllowEmptyStrings = false)]
    public string Token { get; init; } = string.Empty;
    
    [Range(1, ulong.MaxValue, ErrorMessage = "DebugGuildId must be greater than zero.")]
    public ulong DebugGuildId { get; init; }
}