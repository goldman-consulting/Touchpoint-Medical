using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using TouchpointMedical.Logging;

namespace TouchpointMedical.Integration.PointClickCare.Models
{
    public class ADTRecord
    {
        // -------------------------
        // Non-PHI (system/org context)
        // -------------------------
        [JsonProperty("accessingEntityId")]
        public int? AccessingEntityId { get; set; }

        [JsonProperty("enteredBy")]
        public required string EnteredBy { get; set; }

        [JsonProperty("enteredByPositionId")]
        public int? EnteredByPositionId { get; set; }

        [JsonProperty("isCancelledRecord")]
        public required bool IsCancelledRecord { get; set; }

        [JsonProperty("standardActionType")]
        public required string StandardActionType { get; set; }

        // -------------------------
        // PHI — Unique identifiers (#18)
        // (patient or in-facility locating ids/codes)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("adtRecordId")]
        public required int Id { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("patientId")]
        public required int PatientId { get; set; }

        // Location (Unit/Floor/Room/Bed) → identifier when tied to an individual
        [TouchpointLogMasked]
        [JsonProperty("bedId")]
        public required int BedId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("bedDesc")]
        public required string BedDesc { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("roomId")]
        public required int RoomId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("roomDesc")]
        public required string RoomDesc { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("floorId")]
        public required int FloorId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("floorDesc")]
        public required string FloorDesc { get; set; }

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
        [JsonProperty("unitId")]
        public required int UnitId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("unitDesc")]
        public required string UnitDesc { get; set; }

        // Additional (secondary/transfer) location details
        [TouchpointLogMasked]
        [JsonProperty("additionalBedId")]
        public int? AdditionalBedId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("additionalBedDesc")]
        public string? AdditionalBedDesc { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("additionalRoomId")]
        public int? AdditionalRoomId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("additionalRoomDesc")]
        public string? AdditionalRoomDesc { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("additionalFloorId")]
        public int? AdditionalFloorId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("additionalFloorDesc")]
        public string? AdditionalFloorDesc { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("fullAdditionalRoomDesc")]
        public string FullAdditionalRoomDescription
        {
            get
            {
                return string.IsNullOrEmpty(AdditionalBedDesc) ? $"{AdditionalRoomDesc}" : $"{AdditionalRoomDesc}-{AdditionalBedDesc}";
            }
        }

        [TouchpointLogMasked]
        [JsonProperty("additionalUnitId")]
        public int? AdditionalUnitId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("additionalUnitDesc")]
        public string? AdditionalUnitDesc { get; set; }

        // -------------------------
        // PHI — Dates (#3)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("effectiveDateTime")]
        public required DateTime EffectiveDateTime { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("enteredDate")]
        public required DateTime EnteredDate { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("modifiedDateTime")]
        public required DateTime ModifiedDateTime { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("skilledEffectiveFromDate")]
        public DateTime? SkilledEffectiveFromDate { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("skilledEffectiveToDate")]
        public DateTime? SkilledEffectiveToDate { get; set; }

        // -------------------------
        // PHI — Payer/financial identifiers (#9 health plan beneficiary / payer context)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("payerCode")]
        public required string PayerCode { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("payerName")]
        public required string PayerName { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("payerType")]
        public required string PayerType { get; set; }

        // -------------------------
        // PHI — Demographic/clinical/status (protected when linked to a person)
        // -------------------------
        [TouchpointLogMasked]
        [JsonProperty("actionCode")]
        public required string ActionCode { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("actionType")]
        public required string ActionType { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("origin")]
        public string? Origin { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("originType")]
        public string? OriginType { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("outpatient")]
        public required bool IsOutpatient { get; set; } 

        [TouchpointLogMasked]
        [JsonProperty("skilledCare")]
        public bool? IsSkilledCare { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("qhsWaiver")]
        public string? QhsWaiver { get; set; }
    }
}
