using Newtonsoft.Json;

using TouchpointMedical.Logging;

namespace TouchpointMedical.Models
{
    public class Patient
    {
        // -------------------------
        // Non-PHI (system/org context)
        // -------------------------
        [JsonProperty("orgUuid")]
        public required string OrgUuid { get; set; }

        [JsonProperty("facId")]
        public required string FacilityId { get; set; }

        [JsonProperty("standardActionType")]
        public required string StandardActionType { get; set; }

        [JsonProperty("isCancelledRecord")]
        public bool IsCancelledRecord { get; set; }

        [JsonProperty("enteredBy")]
        public string? EnteredBy { get; set; }

        // -------------------------
        // PHI — Unique identifiers (#18)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("patientId")]
        public required string Id { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("encounterId")]
        public required string EncounterId { get; set; }

        // Facility location info → identifiers when tied to a patient
        [TouchpointLogMasked]
        [JsonProperty("unitId")]
        public required string UnitId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("unitDesc")]
        public required string UnitDesc { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("floorId")]
        public string? FloorId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("floorDesc")]
        public string? FloorDesc { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("roomId")]
        public string? RoomId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("roomDesc")]
        public string? RoomDesc { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("bedId")]
        public string? BedId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("bedDesc")]
        public string? BedDesc { get; set; }

        // -------------------------
        // PHI — Names (#1)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("lastName")]
        public required string LastName { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("firstName")]
        public required string FirstName { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("middleName")]
        public string? MiddleName { get; set; }

        // -------------------------
        // PHI — Dates (#3)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("birthDate")]
        public DateTime? BirthDate { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("enteredDate")]
        public DateTime EnteredDate { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("admissionDateTime")]
        public DateTime? AdmissionDateTime { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("dischargeDateTime")]
        public DateTime? DischargeDateTime { get; set; }

        // -------------------------
        // PHI — Demographics / Clinical details
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("gender")]
        public string? Gender { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("patientStatus")]
        public required string PatientStatus { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("height")]
        public string? Height { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("weight")]
        public string? Weight { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("actionCode")]
        public required string ActionCode { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("actionType")]
        public required string ActionType { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("origin")]
        public string? Origin { get; set; }

        // -------------------------
        // PHI — Health plan identifiers (#9)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("medicalRecordNumber")]
        public required string MedicalRecordNumber { get; set; }
    }
}
