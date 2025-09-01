using Newtonsoft.Json;

namespace TouchpointMedical.Integration.PointClickCare.Models
{
    public class Facility
    {
        [JsonProperty("orgId")]
        public int OrgId { get; set; }

        [JsonProperty("orgUuid")]
        public required string OrgUuid { get; set; }

        [JsonProperty("facId")]
        public int FacId { get; set; }

        [JsonProperty("facilityName")]
        public required string FacilityName { get; set; }

        [JsonProperty("country")]
        public required string Country { get; set; }

        [JsonProperty("addressLine1")]
        public required string AddressLine1 { get; set; }

        [JsonProperty("postalCode")]
        public required string PostalCode { get; set; }

        [JsonProperty("phone")]
        public required string Phone { get; set; }

        [JsonProperty("city")]
        public required string City { get; set; }

        [JsonProperty("state")]
        public required string State { get; set; }

        [JsonProperty("fax")]
        public required string Fax { get; set; }

        [JsonProperty("bedCount")]
        public int BedCount { get; set; }

        [JsonProperty("lineOfBusiness")]
        public required FacilityLineOfBusiness LineOfBusiness { get; set; }

        [JsonProperty("healthType")]
        public required string HealthType { get; set; }

        [JsonProperty("facilityCode")]
        public required string FacilityCode { get; set; }

        [JsonProperty("orgName")]
        public required string OrgName { get; set; }

        [JsonProperty("environment")]
        public required string Environment { get; set; }

        [JsonProperty("facilityStatus")]
        public required string FacilityStatus { get; set; }

        [JsonProperty("countryId")]
        public int CountryId { get; set; }

        [JsonProperty("timeZoneOffset")]
        public int TimeZoneOffset { get; set; }

        [JsonProperty("orgDbType")]
        public required string OrgDbType { get; set; }

        [JsonProperty("billingStyleCountry")]
        public required string BillingStyleCountry { get; set; }

        [JsonProperty("timeZone")]
        public required string TimeZone { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("headOffice")]
        public bool HeadOffice { get; set; }
    }

}
