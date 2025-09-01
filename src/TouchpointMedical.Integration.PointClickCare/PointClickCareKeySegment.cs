using System.Runtime.Serialization;

namespace TouchpointMedical.Integration.PointClickCare
{
    public enum PointClickCareKeySegment
    {
        [EnumMember(Value = "Organization")]
        Organization,

        [EnumMember(Value = "Facility")]
        Facility,

        [EnumMember(Value = "Resident")]
        Resident,

        [EnumMember(Value = "Contact")]
        Contact,

        [EnumMember(Value = "ResidentContact")]
        ResidentContact
    }
}
