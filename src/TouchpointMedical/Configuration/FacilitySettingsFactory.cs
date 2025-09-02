using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TouchpointMedical.Configuration
{
    public sealed class FacilitySettingsFactory(
        IOptionsMonitor<FacilitiesSettingsMap> facilitySettingsMap,
        ILogger<FacilitySettingsFactory> logger) : IFacilitySettingsFactory
    {
        private readonly IOptionsMonitor<FacilitiesSettingsMap> _facilitySettingsMap = facilitySettingsMap;
        private readonly ILogger<FacilitySettingsFactory> _logger = logger;

        public async Task<FacilitySettings> Get(string facilitySettingsKey)
        {
            if (!_facilitySettingsMap.CurrentValue.Facilities.TryGetValue(
                    facilitySettingsKey, out var facilitySettings))
            {
                _logger.LogCritical("Facility Settings not found for {FacilityKey}", facilitySettingsKey);

                throw new TouchpointException($"No facility settings configured for {facilitySettingsKey}"));
            }
            
            return await Task.FromResult(facilitySettings);
        }
    }
}
