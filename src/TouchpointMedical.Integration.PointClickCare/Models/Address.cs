using Newtonsoft.Json;

using TouchpointMedical.Logging;

namespace TouchpointMedical.Integration.PointClickCare.Models
{
    public class Address
    {
        [TouchpointLogMasked]
        [JsonIgnore]
        public string? Street
        {
            get
            {
                var street = AddressLine1;

                if (!string.IsNullOrEmpty(AddressLine2))
                {
                    street += street == null ? AddressLine2 : $"{Environment.NewLine}{AddressLine2}";
                }

                return street;
            }
        }

        [TouchpointLogMasked]
        [JsonProperty("addressLine1")]
        public string? AddressLine1 { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("addressLine2")]
        public string? AddressLine2 { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("city")]
        public string? City { get; set; }

        [JsonProperty("state")]
        public string? State { get; set; }

        [TouchpointLogMasked(ShowFirst = 2, Text ="***-****")]
        [JsonProperty("postalCode")]
        public string? PostalCode { get; set; }

        [JsonProperty("country")]
        public string? Country { get; set; }
    }
}
