using TouchpointMedical.Logging;

namespace TouchpointMedical.Integration
{
    public class HttpClientOptions
    {
        public WebApiCallLoggingType CallLogging { get; set; } = WebApiCallLoggingType.None;
        public int DefaultPageSize = 50;
    }
}
