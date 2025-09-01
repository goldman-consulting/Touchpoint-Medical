using Newtonsoft.Json;

using TouchpointMedical.Logging;

namespace TouchpointMedical.Integration.PointClickCare.Models
{
    public class Observation
    {
        public class UnitCodeItem
        {
            [TouchpointLogMasked]
            [JsonProperty("code")]
            public required string Code { get; set; }

            [JsonProperty("system")]
            public required string System { get; set; }
        }

        public class MethodCodeItem
        {
            [JsonProperty("codings")]
            public required List<CodingItem> Codings { get; set; }
        }

        public class CodingItem
        {
            [JsonProperty("system")]
            public required string System { get; set; }

            [JsonProperty("version")]
            public required string Version { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("code")]
            public required string Code { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("display")]
            public required string Display { get; set; }
        }

        // -------------------------
        // PHI — Unique identifiers (#18)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("observationId")]
        public required int Id { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("patientId")]
        public required int ResidentId { get; set; }

        // -------------------------
        // PHI — Clinical details (protected when tied to a patient)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("type")]
        public required string Type { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("value")]
        public double? Value { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("diastolicValue")]
        public double? DiastolicValue { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("systolicValue")]
        public double? SystolicValue { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("unit")]
        public string? Unit { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("unitCode")]
        public UnitCodeItem? UnitCode { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("method")]
        public string? Method { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("methodCode")]
        public MethodCodeItem? MethodCode { get; set; }

        // -------------------------
        // PHI — Dates (#3)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("recordedDate")]
        public required DateTime RecordedDate { get; set; }

        // -------------------------
        // Non-PHI (system/operational context)
        // -------------------------
        [JsonProperty("recordedBy")]
        public required string RecordedBy { get; set; }
    }


}
