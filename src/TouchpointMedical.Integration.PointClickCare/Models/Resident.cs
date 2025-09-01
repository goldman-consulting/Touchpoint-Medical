using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using TouchpointMedical.Logging;

namespace TouchpointMedical.Integration.PointClickCare.Models
{
    public class Resident
    {
        // -------------------------
        // Non-PHI (org/facility context)
        // -------------------------
        [JsonProperty("orgId")]
        public long OrgId { get; set; }

        [JsonProperty("orgUuid")]
        public required string OrgUuid { get; set; }

        [JsonProperty("facId")]
        public long FacilityId { get; set; }

        // -------------------------
        // PHI — Addresses are protected at the Address class level
        // -------------------------
        [JsonProperty("previousAddress")]
        public Address? PreviousAddress { get; set; }

        [JsonProperty("legalMailingAddress")]
        public Address? MailingAddress { get; set; }

        // -------------------------
        // PHI — Unique identifiers (#18) / MRN (#8) / Health plan (#9)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("patientId")]
        public long Id { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("patientExternalId", NullValueHandling = NullValueHandling.Ignore)]
        public string? ExternalId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("ompId")]
        public long OmpId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("medicalRecordNumber", NullValueHandling = NullValueHandling.Ignore)]
        public string? MedicalRecordNumber { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("medicareNumber", NullValueHandling = NullValueHandling.Ignore)]
        public string? MedicareNumber { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("medicaidNumber", NullValueHandling = NullValueHandling.Ignore)]
        public string? MedicaidNumber { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("medicareBeneficiaryIdentifier", NullValueHandling = NullValueHandling.Ignore)]
        public string? MedicareBeneficiaryIdentifier { get; set; }

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
        [JsonProperty("middleName", NullValueHandling = NullValueHandling.Ignore)]
        public string? MiddleName { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("prefix", NullValueHandling = NullValueHandling.Ignore)]
        public string? Prefix { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("suffix", NullValueHandling = NullValueHandling.Ignore)]
        public string? Suffix { get; set; }

        // Computed name forms are also identifiers
        [TouchpointLogMasked]
        public string FullName => $"{FirstName} {LastName}";

        [TouchpointLogMasked]
        public string FullNameAsSalesforce => $"{LastName}, {FirstName}";

        // -------------------------
        // PHI — Contact identifiers (#4 Phone, #6 Email, #7 SSN)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("homePhone")]
        public string? HomePhone { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("email")]
        public string? Email { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("socialBeneficiaryIdentifier")]
        public string? SocialBeneficiaryIdentifier { get; set; }

        // -------------------------
        // PHI — Dates (#3)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("birthDate")]
        public DateTime? BirthDate { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("admissionDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? AdmissionDate { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("dischargeDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DischargeDate { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("deathDateTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DeathDateTime { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        // -------------------------
        // PHI — Facility location (Unit/Floor/Room/Bed) → Identifier (#18)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("bedId", NullValueHandling = NullValueHandling.Ignore)]
        public long? BedId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("bedDesc", NullValueHandling = NullValueHandling.Ignore)]
        public string? BedDesc { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("roomDesc", NamingStrategyType = typeof(DefaultNamingStrategy), NullValueHandling = NullValueHandling.Ignore)]
        public string? RoomDesc { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("roomId", NullValueHandling = NullValueHandling.Ignore)]
        public long? RoomId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("floorDesc", NullValueHandling = NullValueHandling.Ignore)]
        public string? FloorDesc { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("floorId", NullValueHandling = NullValueHandling.Ignore)]
        public long? FloorId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("fullRoomDesc")]
        public string FullRoomDescription
        {
            get
            {
                return string.IsNullOrEmpty(BedDesc) ? $"{RoomDesc}" : $"{RoomDesc}-{BedDesc}";
            }
        }

        [TouchpointLogMasked]
        [JsonProperty("unitDesc", NullValueHandling = NullValueHandling.Ignore)]
        public string? UnitDesc { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("unitId", NullValueHandling = NullValueHandling.Ignore)]
        public long? UnitId { get; set; }

        // -------------------------
        // PHI — Demographic/clinical/status (protected when linked)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("patientStatus")]
        public required string Status { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("gender")]
        public required string Gender { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("languageCode", NullValueHandling = NullValueHandling.Ignore)]
        public string? LanguageCode { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("languageDesc", NullValueHandling = NullValueHandling.Ignore)]
        public string? LanguageDesc { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("religion", NullValueHandling = NullValueHandling.Ignore)]
        public string? Religion { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("citizenship", NullValueHandling = NullValueHandling.Ignore)]
        public string? Citizenship { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("maritalStatus", NullValueHandling = NullValueHandling.Ignore)]
        public string? MaritalStatus { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("deceased")]
        public bool IsDeceased { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("outpatient")]
        public bool IsOutpatient { get; set; }

        [JsonProperty("hasPhoto")]
        public bool HasPhoto { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("waitingList")]
        public bool IsWaitingList { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("isOnLeave", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsOnLeave { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("smokingStatusCode")]
        public string? SmokingStatusCode { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("smokingStatusDesc")]
        public string? SmokingStatusDesc { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("ethnicityDesc", NullValueHandling = NullValueHandling.Ignore)]
        public string? EthnicityDesc { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("dischargeReason")]
        public string? DischargeReason
        {
            get
            {
                string? dischargeReason = null;
                if (IsDeceased && DeathDateTime.HasValue)
                {
                    dischargeReason = $"Death: {DeathDateTime.Value:MMMM dd, yyyy}";
                }
                else if (IsDeceased)
                {
                    dischargeReason = $"Death: Date Missing";
                }
                else if (IsOutpatient)
                {
                    dischargeReason = "Outpatient";
                }
                else if (DischargeDate.HasValue)
                {
                    dischargeReason = $"Discharged: {DischargeDate.Value:MMMM dd, yyyy}";
                }
                else if (!string.IsNullOrWhiteSpace(Status)
                         && !Status.Equals("Current", StringComparison.InvariantCultureIgnoreCase))
                {
                    dischargeReason = $"Not Current: {Status}";
                }

                return dischargeReason;
            }
        }

        // Derived convenience flag is still tied to the individual → treat as PHI in logs
        [TouchpointLogMasked]
        [JsonProperty("active")]
        public bool IsActive
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Status)
                    && Status.Equals("Current", StringComparison.InvariantCultureIgnoreCase)
                    && !IsOutpatient;
            }
        }

        //[JsonProperty("orgId")]
        //public long OrgId { get; set; }

        //[JsonProperty("orgUuid")]
        //public required string OrgUuid { get; set; }

        //[JsonProperty("facId")]
        //public long FacilityId { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("patientId")]
        //public long Id { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("medicalRecordNumber", NullValueHandling = NullValueHandling.Ignore)]
        //public string? MedicalRecordNumber { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("medicareNumber", NullValueHandling = NullValueHandling.Ignore)]
        //public string? MedicareNumber { get; set; }

        //[JsonProperty("patientExternalId", NullValueHandling = NullValueHandling.Ignore)]
        //public string? ExternalId { get; set; }

        //[JsonProperty("patientStatus")]
        //public required string Status { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("lastName")]
        //public required string LastName { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("firstName")]
        //public required string FirstName { get; set; }

        //[JsonProperty("prefix", NullValueHandling = NullValueHandling.Ignore)]
        //public string? Prefix { get; set; }

        //[JsonProperty("outpatient")]
        //public bool IsOutpatient { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("ompId")]
        //public long OmpId { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("birthDate")]
        //public DateTime? BirthDate { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("admissionDate", NullValueHandling = NullValueHandling.Ignore)]
        //public DateTime? AdmissionDate { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("dischargeDate", NullValueHandling = NullValueHandling.Ignore)]
        //public DateTime? DischargeDate { get; set; }

        //[JsonProperty("waitingList")]
        //public bool IsWaitingList { get; set; }

        //[JsonProperty("hasPhoto")]
        //public bool HasPhoto { get; set; }

        //[JsonProperty("gender")]
        //public required string Gender { get; set; }

        //[JsonProperty("languageCode", NullValueHandling = NullValueHandling.Ignore)]
        //public string? LanguageCode { get; set; }

        //[JsonProperty("languageDesc", NullValueHandling = NullValueHandling.Ignore)]
        //public string? LanguageDesc { get; set; }

        //[JsonProperty("religion", NullValueHandling = NullValueHandling.Ignore)]
        //public string? Religion { get; set; }

        //[JsonProperty("citizenship", NullValueHandling = NullValueHandling.Ignore)]
        //public string? Citizenship { get; set; }

        //[JsonProperty("maritalStatus", NullValueHandling = NullValueHandling.Ignore)]
        //public string? MaritalStatus { get; set; }

        //[JsonProperty("deceased")]
        //public bool IsDeceased { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("medicaidNumber", NullValueHandling = NullValueHandling.Ignore)]
        //public string? MedicaidNumber { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("deathDateTime", NullValueHandling = NullValueHandling.Ignore)]
        //public DateTime? DeathDateTime { get; set; }

        //[JsonProperty("ethnicityDesc", NullValueHandling = NullValueHandling.Ignore)]
        //public string? EthnicityDesc { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("bedId", NullValueHandling = NullValueHandling.Ignore)]
        //public long? BedId { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("bedDesc", NullValueHandling = NullValueHandling.Ignore)]
        //public string? BedDesc { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("roomDesc", NamingStrategyType = typeof(DefaultNamingStrategy), NullValueHandling = NullValueHandling.Ignore)]
        //public string? RoomDesc { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("roomId", NullValueHandling = NullValueHandling.Ignore)]
        //public long? RoomId { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("floorDesc", NullValueHandling = NullValueHandling.Ignore)]
        //public string? FloorDesc { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("floorId", NullValueHandling = NullValueHandling.Ignore)]
        //public long? FloorId { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("unitDesc", NullValueHandling = NullValueHandling.Ignore)]
        //public string? UnitDesc { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("unitId", NullValueHandling = NullValueHandling.Ignore)]
        //public long? UnitId { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("middleName", NullValueHandling = NullValueHandling.Ignore)]
        //public string? MiddleName { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("medicareBeneficiaryIdentifier", NullValueHandling = NullValueHandling.Ignore)]
        //public string? MedicareBeneficiaryIdentifier { get; set; }

        //[JsonProperty("isOnLeave", NullValueHandling = NullValueHandling.Ignore)]
        //public bool? IsOnLeave { get; set; }

        //[JsonProperty("suffix", NullValueHandling = NullValueHandling.Ignore)]
        //public string? Suffix { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("email")]
        //public string? Email { get; set; }

        //[JsonProperty("previousAddress")]
        //public Address? PreviousAddress { get; set; }

        //[JsonProperty("legalMailingAddress")]
        //public Address? MailingAddress { get; set; }

        //[JsonProperty("smokingStatusCode")]
        //public string? SmokingStatusCode { get; set; }

        //[JsonProperty("smokingStatusDesc")]
        //public string? SmokingStatusDesc { get; set; }

        //[JsonProperty("socialBeneficiaryIdentifier")]
        //public string? SocialBeneficiaryIdentifier { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("homePhone")]
        //public string? HomePhone { get; set; }

        //[TouchpointLogMasked]
        //[JsonProperty("createdDate")]
        //public DateTime CreatedDate { get; set; }

        //[TouchpointLogMasked]
        //public string FullName { get { return $"{FirstName} {LastName}"; } }

        //[TouchpointLogMasked]
        //[JsonProperty("fullRoomDesc")]
        //public string FullRoomDescription
        //{
        //    get
        //    {
        //        return string.IsNullOrEmpty(BedDesc) ? $"{RoomDesc}" : $"{RoomDesc}-{BedDesc}";
        //    }
        //}


        //[TouchpointLogMasked]
        //[JsonProperty("dischargeReason")]
        //public string? DischargeReason
        //{
        //    get
        //    {
        //        string? dischargeReason = null;
        //        if (IsDeceased && DeathDateTime.HasValue)
        //        {
        //            dischargeReason = $"Death: {DeathDateTime.Value:MMMM dd, yyyy}";
        //        }
        //        else if (IsDeceased)
        //        {
        //            dischargeReason = $"Death: Date Missing";
        //        }
        //        else if (IsOutpatient)
        //        {
        //            dischargeReason = "Outpatient";
        //        }
        //        else if (DischargeDate.HasValue)
        //        {
        //            dischargeReason = $"Discharged: {DischargeDate.Value:MMMM dd, yyyy}";
        //        }
        //        else if (
        //            !string.IsNullOrWhiteSpace(Status) 
        //                && !Status.Equals("Current", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            dischargeReason = $"Not Current: {Status}";
        //        }

        //        return dischargeReason;
        //    }
        //}

        //[TouchpointLogMasked]
        //public string FullNameAsSalesforce { get { return $"{LastName}, {FirstName}"; } }

        //[JsonProperty("active")]
        //public bool IsActive
        //{
        //    get
        //    {
        //        return !string.IsNullOrWhiteSpace(Status) 
        //            && Status.Equals("Current", StringComparison.InvariantCultureIgnoreCase) 
        //            && !IsOutpatient;
        //    }
        //}
    }
}
