using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using TouchpointMedical.Logging;

namespace TouchpointMedical.Integration.PointClickCare.Models
{
    public class Medication
    {
        public class AdministrationItem
        {
            [JsonProperty("route")]
            public required RouteItem Route { get; set; }
        }

        public class RouteItem
        {
            [JsonProperty("coding")]
            public required List<CodingItem> Coding { get; set; }
        }

        public class CodingItem
        {
            [TouchpointLogMasked]
            [JsonProperty("code")]
            public required string Code { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("display")]
            public required string Display { get; set; }

            [JsonProperty("system")]
            public required string System { get; set; }
        }

        public class ScheduleItem
        {
            [TouchpointLogMasked]
            [JsonProperty("directions")]
            public required string Directions { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("dose")]
            public required string Dose { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("doseUOM")]
            public required string DoseUOM { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("codeableDoseUOM")]
            public required CodeableDoseUOMItem CodeableDoseUOM { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("endDateTime")]
            public DateTime? EndDateTime { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("frequency")]
            public required string Frequency { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("indicationsForUse")]
            public required string IndicationsForUse { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("lastAdministration")]
            public LastAdministrationItem? LastAdministration { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("orderScheduleId")]
            public required int OrderScheduleId { get; set; }

            [JsonProperty("scheduleType")]
            public required string ScheduleType { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("startDateTime")]
            public required DateTime StartDateTime { get; set; }
        }

        public class CodeableDoseUOMItem
        {
            [JsonProperty("coding")]
            public required List<CodingItem> Coding { get; set; }
        }

        public class LastAdministrationItem
        {
            [JsonProperty("administeredAmount")]
            public required string AdministeredAmount { get; set; }

            [JsonProperty("administeredBy")]
            public required string AdministeredBy { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("administeredDateTime")]
            public required DateTime AdministeredDateTime { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("code")]
            public required string Code { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("codeDescription")]
            public required string CodeDescription { get; set; }

            [JsonProperty("unableToAdminister")]
            public required bool WasUnableToAdminister { get; set; }
        }

        // -------------------------
        // PHI — Unique identifiers (#18)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("clientId")]
        public required int ClientId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("ddId")]
        public required int DdId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("orderId")]
        public required int Id { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("pnId")]
        public required int PnId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("residentNumber")]
        public required string ResidentNumber { get; set; }

        // Patient’s name = HIPAA identifier (#1)
        [TouchpointLogMasked]
        [JsonProperty("residentName")]
        public required string ResidentName { get; set; }

        // -------------------------
        // PHI — Dates (#3)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("createdDate")]
        public required DateTime CreatedDate { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("orderDate")]
        public required DateTime OrderDate { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("revisionDate")]
        public DateTime? RevisionDate { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("startDate")]
        public required DateTime StartDate { get; set; }

        // -------------------------
        // PHI — Clinical details (protected when tied to a patient)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("administration")]
        public required AdministrationItem Administration { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("description")]
        public required string Description { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("directions")]
        public required string Directions { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("generic")]
        public string? Generic { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("importedData")]
        public string? ImportedData { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("rxNormId")]
        public string? RxNormId { get; set; }

        [JsonProperty("schedules")]
        public required List<ScheduleItem> Schedules { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("status")]
        public required string Status { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("strength")]
        public string? Strength { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("strengthUOM")]
        public string? StrengthUOM { get; set; }

        // -------------------------
        // Non-PHI (system context)
        // -------------------------
        [JsonProperty("createdBy")]
        public required string CreatedBy { get; set; }

        [JsonProperty("revisionBy")]
        public required string RevisionBy { get; set; }
    }
}
