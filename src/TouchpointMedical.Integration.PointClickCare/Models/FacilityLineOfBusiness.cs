using Newtonsoft.Json;

namespace TouchpointMedical.Integration.PointClickCare.Models
{
    public class FacilityLineOfBusiness
    {
        [JsonProperty("shortDesc")]
        public required string ShortDesc { get; set; }

        [JsonProperty("longDesc")]
        public required string LongDesc { get; set; }
    }

}
