using System.Net.Http.Headers;

namespace TouchpointMedical.Integration
{
    public class HttpCallOptions
    {
        public required string Uri { get; set; }
        public AuthenticationHeaderValue? AuthorizationHeader { get; set; }
    }
}
