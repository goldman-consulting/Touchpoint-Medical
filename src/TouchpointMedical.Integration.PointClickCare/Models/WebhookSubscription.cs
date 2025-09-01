using Newtonsoft.Json;

using TouchpointMedical.Http.Interfaces;

namespace TouchpointMedical.Integration.PointClickCare.Models
{
    public record WebhookSubscription : IHttpContentable
    {
        private WebhookSubscription() { }

        [JsonProperty("applicationName")]
        public required string ApplicationName { get; set; }

        [JsonProperty("enableRoomReservationCancellation")]
        public bool HasRoomReservationCancellationEnabled { get; set; }

        [JsonProperty("endUrl")]
        public required string EndUrl { get; set; }

        [JsonProperty("eventGroupList")]
        public List<string> EventGroupList { get; set; } = [];

        [JsonProperty("includeDischarged")]
        public bool IsIncludeDischarged { get; set; } = false;

        [JsonProperty("includeOutpatient")]
        public bool IsIncludeOutpatient { get; set; } = false;

        [JsonProperty("password")]
        public required string Password { get; set; }

        [JsonProperty("username")]
        public required string Username { get; set; }

        public string? RegistrationResponse { get; set; }

        public static WebhookSubscription Create(
            string applicationName,
            string handlerDomain, 
            string handlerPort, 
            string username,
            string password,
            List<string> eventGroupList,
            bool hasRoomReservationCancellationEnabled = false,
            bool isIncludeDischarged = false,
            bool isIncludeOutpatient = false)
        {
            return new WebhookSubscription 
            { 
                ApplicationName = applicationName,
                EndUrl = $"https://{handlerDomain}:{handlerPort}/api/listener",
                Username = username,
                Password = password,
                EventGroupList = eventGroupList,
                HasRoomReservationCancellationEnabled =     hasRoomReservationCancellationEnabled,
                IsIncludeDischarged = isIncludeDischarged,
                IsIncludeOutpatient = isIncludeOutpatient
            };
        }
    }
}
