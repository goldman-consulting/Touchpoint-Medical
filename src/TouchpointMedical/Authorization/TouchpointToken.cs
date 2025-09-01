using Newtonsoft.Json;

using TouchpointMedical.Authorization;

namespace TouchpointMedical.Integration
{
    public record TouchpointToken : AuthToken
    {
        [JsonProperty("token_type")]
        public required string TokenType { get; set; }
    }
}
