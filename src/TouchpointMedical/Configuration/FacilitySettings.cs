using Newtonsoft.Json;

using TouchpointMedical.Http.Interfaces;
using TouchpointMedical.Logging;

namespace TouchpointMedical.Configuration
{
    public record FacilitySettings : ITokenServiceInstanceKey
    {
        [JsonProperty("facilityKey")]
        public required string FacilityKey { get; set; }

        [JsonProperty("name")]
        public required string Name { get; init; }

        [JsonProperty("appName")]
        public required string AppName { get; init; }

        [JsonProperty("appKey")]
        [TouchpointLogMasked]
        public required string AppKey { get; init; }

        [JsonProperty("appKeySecret")]
        [TouchpointLogMasked]
        public required string AppKeySecret { get; init; }

        public string InstanceKey => Name;
    }
}
