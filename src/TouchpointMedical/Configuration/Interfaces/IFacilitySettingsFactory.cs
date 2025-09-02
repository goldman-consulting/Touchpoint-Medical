namespace TouchpointMedical.Configuration
{
    public interface IFacilitySettingsFactory
    {
        Task<FacilitySettings> Get(string name);
    }
}
