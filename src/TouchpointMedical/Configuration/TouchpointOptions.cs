using TouchpointMedical.Integration;

namespace TouchpointMedical.Configuration
{
    public class TouchpointOptions
    {
        public required string Connection { get; set; }
        public bool Enabled { get; set; } = false;
        public string KeyPrefix { get; set; } = "cache";
        public string FacilitySettingsDataPath { get; set; } = "App_Data/FacilitySettings.json";
        public long WindowMs { get; set; } = 30_000;
        public long TimeToLiveHr { get; set; } = 24;
        public long ProcessorDelayMs { get; set; } = 2_000;
        public HttpClientOptions HttpClientOptions { get; init; } = new HttpClientOptions();

    }
}
