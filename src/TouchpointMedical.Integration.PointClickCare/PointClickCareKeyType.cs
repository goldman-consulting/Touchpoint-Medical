using System.Runtime.Serialization;

namespace TouchpointMedical.Integration.PointClickCare
{
    public enum PointClickCareKeyType
    {
        [EnumMember(Value = "Not Set")]
        NotSet,

        [EnumMember(Value = "Organization")]
        Organization,

        [EnumMember(Value = "Facility")]
        Facility,

        [EnumMember(Value = "Resident")]
        Resident,

        [EnumMember(Value = "Resident Contact")]
        ResidentContact
    }
}
