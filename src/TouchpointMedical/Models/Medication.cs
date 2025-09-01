using Newtonsoft.Json;

using TouchpointMedical.Logging;

namespace TouchpointMedical.Models
{
    public class Medication
    {
        public class ScheduleItem
        {
            [TouchpointLogMasked]
            [JsonProperty("orderScheduleId")]
            public required string Id { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("directions")]
            public string? Directions { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("dose")]
            public double? DoseQuantity { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("doseUOM")]
            public string? DoseUnit { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("frequency")]
            public string? Frequency { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("startDateTime")]
            public DateTime? StartDate { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("endDateTime")]
            public DateTime? EndDate { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("scheduleType")]
            public string? ScheduleType { get; set; }

        }

        // -------------------------
        // PHI — Unique identifiers (#18)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("orderId")]
        public required string Id { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("patientId")]
        public required string ResidentId { get; set; }

        // -------------------------
        // PHI — Dates (#3)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("startDate")]
        public required DateTime StartDate { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("createdDate")]
        public required DateTime CreatedDate { get; set; }

        // -------------------------
        // PHI — Clinical details (protected when tied to a patient)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("status")]
        public required string Status { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("medicationCode")]
        public required string MedicationCode { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("medicationName")]
        public required string MedicationName { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("route")]
        public string? Route { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("strength")]
        public string? Strength { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("strengthUOM")]
        public string? StrengthUOM { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("notes")]
        public string? Notes { get; set; }

        // -------------------------
        // Non-PHI (system/operational context)
        // -------------------------
        [JsonProperty("createdBy")]
        public string? CreatedBy { get; set; }

        [JsonProperty("verifiedBy")]
        public string? VerifiedBy { get; set; }

        [JsonProperty("schedules")]
        public List<ScheduleItem> Schedules { get; set; } = [];
    
    }
}
