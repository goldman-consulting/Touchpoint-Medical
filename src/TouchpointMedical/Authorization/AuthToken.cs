using Newtonsoft.Json;

using TouchpointMedical.Logging;

namespace TouchpointMedical.Authorization
{
    public record AuthToken
    {
        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

        [TouchpointLogMasked(ShowFirst = 2, ShowLast = 2)]
        [JsonProperty("access_token")]
        public required string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public required int ExpiresIn { get; set; }

        [JsonProperty("expires_at")]
        public DateTimeOffset ExpiresAt => CreatedAt.AddSeconds(ExpiresIn - 60);

        [JsonIgnore]
        public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;

        [JsonProperty("needs_refresh")]
        public bool NeedsRefresh => DateTimeOffset.UtcNow > ExpiresAt.AddMinutes(5);
    }
}
