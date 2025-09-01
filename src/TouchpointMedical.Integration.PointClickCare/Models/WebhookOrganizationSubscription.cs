using Newtonsoft.Json;

namespace TouchpointMedical.Integration.PointClickCare.Models
{
    public class WebhookOrganizationSubscription
    {
        [JsonProperty("action")]
        public required string Action { get; set; }

        [JsonProperty("orgUuid")]
        public required string OrgUuid { get; set; }

        [JsonProperty("status")]
        public required string Status { get; set; }
    }
}
