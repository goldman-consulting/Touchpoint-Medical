using TouchpointMedical.Integration.PointClickCare.Models;

using tpmPatient = TouchpointMedical.Models.Patient;
using tpmAllergy = TouchpointMedical.Models.Allergy;
using tpmMedication = TouchpointMedical.Models.Medication;

namespace TouchpointMedical.Integration.PointClickCare.Extensions
{
    internal static class PointClickCareExtensions
    {
        public static tpmPatient MapTouchpointResident(
            this Resident resident,
            List<Observation> observations)
        {
            return new tpmPatient
            {
                OrgUuid = resident.OrgUuid,
                FacilityId = resident.FacilityId.ToString(),
                Id = resident.Id.ToString(),
                MedicalRecordNumber = resident.MedicalRecordNumber ?? resident.ExternalId ?? "UNKNOWN",
                LastName = resident.LastName,
                FirstName = resident.FirstName,
                MiddleName = resident.MiddleName,
                BirthDate = resident.BirthDate,
                Gender = resident.Gender,
                Height = observations
                    .Where(o => o.Type == "height")
                    .Select(o => $"{o.Value} {o.Unit}").FirstOrDefault(),
                Weight = observations
                    .Where(o => o.Type == "weight")
                    .Select(o => $"{o.Value} {o.Unit}").FirstOrDefault(),
                PatientStatus = resident.Status,
                ActionCode = "UNKNOWN",
                ActionType = "UNKNOWN",
                StandardActionType = "UNKNOWN",
                EncounterId = resident.ExternalId ?? "UNKNOWN",
                UnitDesc = resident.UnitDesc ?? "UNKNOWN",
                UnitId = resident.UnitId?.ToString() ?? "UNKNOWN",
                IsCancelledRecord = false,
                Origin = "UNKNOWN",
                FloorDesc = resident.FloorDesc ?? "UNKNOWN",
                FloorId = resident.FloorId?.ToString() ?? "UNKNOWN",
                BedDesc = resident.BedDesc ?? "UNKNOWN",
                BedId = resident.BedId?.ToString() ?? "UNKNOWN",
                RoomDesc = resident.RoomDesc ?? "UNKNOWN",
                RoomId = resident.RoomId?.ToString() ?? "UNKNOWN",
                AdmissionDateTime = resident.AdmissionDate
            };
        }

        public static tpmPatient MapTouchpointResident(
            this ADTRecord adtRecord, 
            Resident resident,
            List<Observation> observations)
        {
            return new tpmPatient
            { 
                OrgUuid = resident.OrgUuid,
                FacilityId = resident.FacilityId.ToString(),
                Id = adtRecord.Id.ToString(),
                MedicalRecordNumber = resident.MedicalRecordNumber ?? resident.ExternalId ?? "UNKNOWN",
                LastName = resident.LastName,
                FirstName = resident.FirstName,
                MiddleName = resident.MiddleName,
                BirthDate = resident.BirthDate,
                Gender = resident.Gender,
                Height = observations
                    .Where(o=>o.Type == "height")
                    .Select(o=>$"{o.Value} {o.Unit}").First(),
                Weight = observations
                    .Where(o => o.Type == "weight")
                    .Select(o => $"{o.Value} {o.Unit}").First(),
                PatientStatus = resident.Status,
                ActionCode = adtRecord.ActionCode,
                ActionType = adtRecord.ActionType,
                StandardActionType = adtRecord.StandardActionType,
                EncounterId = adtRecord.Id.ToString(),
                UnitDesc = adtRecord.UnitDesc,
                UnitId = adtRecord.UnitId.ToString(),
                IsCancelledRecord = adtRecord.IsCancelledRecord,
                Origin = adtRecord.Origin,
                FloorDesc = adtRecord.FloorDesc,
                FloorId =  adtRecord.FloorId.ToString(),
                BedDesc = adtRecord.BedDesc,
                BedId = adtRecord.BedId.ToString(),
                RoomDesc = adtRecord.RoomDesc,
                RoomId = adtRecord.RoomId.ToString(),
                AdmissionDateTime = resident.AdmissionDate
            };
        }

        public static tpmMedication MapTouchpointMedication(this Medication medication)
        {
            var result =  new tpmMedication
            {
                Id = medication.Id.ToString(),
                ResidentId = medication.ClientId.ToString(),
                Status = medication.Status,
                MedicationCode = medication.DdId > 0 ? medication.DdId.ToString() : medication.DdId.ToString(),
                MedicationName = medication.Generic ?? "UNKNOWN",
                StartDate = medication.StartDate,
                EndDate = medication.EndDate,
                Route = medication.Administration.Route.Coding.FirstOrDefault()?.Display,
                Strength = medication.Strength,
                StrengthUOM = medication.StrengthUOM,
                Notes = "Pharmacy Notes not available in API",
                CreatedBy = medication.CreatedBy,
                CreatedDate = medication.CreatedDate,
                VerifiedBy = medication.Status.Equals("ACTIVE", StringComparison.OrdinalIgnoreCase) 
                    ? medication.RevisionBy : default
            };

            foreach(var schedule in medication.Schedules)
            {
                var tpmSchedule = new tpmMedication.ScheduleItem 
                {
                    Id = schedule.OrderScheduleId.ToString(),
                    Directions = schedule.Directions,
                    DoseQuantity = double.TryParse(schedule.Dose, out double doseValue) ? doseValue : default,
                    DoseUnit = schedule.DoseUOM,
                    StartDate = schedule.StartDateTime,
                    EndDate = schedule.EndDateTime,
                    Frequency = schedule.Frequency,
                    ScheduleType = schedule.ScheduleType
                };

                result.Schedules.Add(tpmSchedule);
            }

            return result;
        }

        public static List<tpmAllergy> MapTouchpointAllergy(this List<Allergy> allergy)
        {
           return [.. allergy.Select(m => new tpmAllergy
           {
               Allergen = m.Allergen,
               AllergenCode = m.AllergenCode?.Codings?.FirstOrDefault()?.Code ?? "UNKNOWN",
               AllergenDisplay = m.AllergenCode?.Codings?.FirstOrDefault()?.Display ?? "UNKNOWN",
               OnsetDate = m.OnsetDate
           })];


        }

    }
}
