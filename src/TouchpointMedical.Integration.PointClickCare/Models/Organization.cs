using Newtonsoft.Json;

namespace TouchpointMedical.Integration.PointClickCare.Models
{
    public class Organization
    {
        [JsonProperty("applicationName")]
        public required string ApplicationName { get; set; }

        [JsonProperty("orgId")]
        public int OrgId { get; set; }

        [JsonProperty("scope")]
        public int Scope { get; set; }

        [JsonProperty("orgUuid")]
        public required string OrgUuid { get; set; }
    }
}
