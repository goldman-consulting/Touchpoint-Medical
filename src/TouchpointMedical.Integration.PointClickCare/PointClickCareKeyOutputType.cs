using System.Runtime.Serialization;

namespace TouchpointMedical.Integration.PointClickCare
{
    public enum PointClickCareKeyOutputType
    {
        [EnumMember(Value = "Default")]
        Default,

        [EnumMember(Value = "Organization Key")]
        OrganizationKey,

        [EnumMember(Value = "Facility Key")]
        FacilityKey,

        [EnumMember(Value = "Resident Key")]
        ResidentKey,

        [EnumMember(Value = "File Safe")]
        FileSafe
    }
}
