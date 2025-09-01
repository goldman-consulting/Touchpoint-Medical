using Newtonsoft.Json;

using TouchpointMedical.Logging;

namespace TouchpointMedical.Integration.PointClickCare.Models
{
    public class Allergy
    {
        public class ReactionSubTypeCodeItem
        {
            [JsonProperty("coding")]
            public CodingItem[]? Coding { get; set; }
        }

        public class AllergenCodeItem
        {
            [JsonProperty("codings")]
            public required CodingItem[] Codings { get; set; }
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

        public class ReactionItem
        {
            [JsonProperty("manifestations")]
            public ManifestationItem[]? Manifestations { get; set; }
        }

        public class ManifestationItem
        {
            [TouchpointLogMasked]
            [JsonProperty("systemName")]
            public string? SystemName { get; set; }

            [JsonProperty("systemNameSpace")]
            public string? SystemNameSpace { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("code")]
            public required string Code { get; set; }

            [TouchpointLogMasked]
            [JsonProperty("displayName")]
            public required string DisplayName { get; set; }
        }

        public class ReactionTypeCodeItem
        {
            [JsonProperty("codint")]
            public CodingItem[]? Coding { get; set; }
        }

        // -------------------------
        // PHI — Unique identifiers (#18)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("allergyIntoleranceId")]
        public int AllergyIntoleranceId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("patientId")]
        public int PatientId { get; set; }

        // -------------------------
        // PHI — Clinical details (protected when tied to a patient)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("allergen")]
        public required string Allergen { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("allergenCode")]
        public required AllergenCodeItem AllergenCode { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("category")]
        public string? Category { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("clinicalStatus")]
        public string? ClinicalStatus { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("reactionNote")]
        public string? ReactionNote { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("reaction")]
        public ReactionItem? Reaction { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("reactionSubType")]
        public string? ReactionSubType { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("reactionSubTypeCode")]
        public ReactionSubTypeCodeItem? ReactionSubTypeCode { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("reactionType")]
        public string? ReactionType { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("reactionTypeCode")]
        public ReactionTypeCodeItem? ReactionTypeCode { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("severity")]
        public string? Severity { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("type")]
        public string? Type { get; set; }

        // -------------------------
        // PHI — Dates (#3)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("onsetDate")]
        public required DateTime OnsetDate { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("revisionDate")]
        public DateTime RevisionDate { get; set; }

        // -------------------------
        // Non-PHI (system/operational context)
        // -------------------------
        [JsonProperty("createdBy")]
        public string? CreatedBy { get; set; }

        [JsonProperty("revisionBy")]
        public string? RevisionBy { get; set; }
    }
}
