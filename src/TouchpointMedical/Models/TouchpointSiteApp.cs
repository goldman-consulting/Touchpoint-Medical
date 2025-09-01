using Newtonsoft.Json;

using TouchpointMedical.Http.Interfaces;

namespace TouchpointMedical.Models
{
    public class TouchpointSiteApp : IHttpContentable
    {
        [JsonProperty("client_id")]
        public required string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public required string ClientSecret { get; set; }
    }
}
