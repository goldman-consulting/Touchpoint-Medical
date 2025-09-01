using Newtonsoft.Json;

using TouchpointMedical.Logging;

namespace TouchpointMedical.Models
{
    public class Allergy
    {
        // -------------------------
        // PHI — Clinical details (protected when tied to a patient)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("allergen")]
        public required string Allergen { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("allergenCode")]
        public string? AllergenCode { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("allergenDisplay")]
        public string? AllergenDisplay { get; set; }

        // -------------------------
        // PHI — Dates (#3)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("onsetDate")]
        public DateTime? OnsetDate { get; set; }
    }
}
