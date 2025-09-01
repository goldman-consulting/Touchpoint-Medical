using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Primitives;

using Newtonsoft.Json;

using System.Text;

using TouchpointMedical.Infrastructure;
using TouchpointMedical.Integration.PointClickCare.Configuration;
using TouchpointMedical.Logging;

namespace TouchpointMedical.Integration.PointClickCare.Infrastructure
{
    public class PointClickCareWebhookMessage : IWebhookMessageSet
    {
        [JsonProperty("messageId")]
        public string MessageId { get; init; } = Guid.NewGuid().ToString();

        [JsonProperty("messageReceived")]
        public DateTimeOffset MessageReceivedAt { get; init; } = DateTimeOffset.UtcNow;

        [JsonProperty("eventType")]
        public required string Type { get; init; } = default!;

        [JsonProperty("eventDate")]
        public DateTime EventDate { get; set; }

        [JsonProperty("orgUuid")]
        public required string OrgUuid { get; set; }

        [JsonProperty("facId")]
        public required long FacilityId { get; set; }

        [TouchpointLogMasked]
        [JsonProperty("patientId")]
        public required long ResidentId { get; set; }

        [JsonProperty("messageDate")]
        public DateTime MessageDate { get; set; }

        [JsonProperty("resourceId")]
        public required List<string> ResourceIds { get; set; }

        [JsonProperty("headers")]
        public Dictionary<string, object> Headers { get; init; } = [];

        private PointClickCareKey? _residentKey;

        [TouchpointLogMasked]
        public PointClickCareKey ResidentKey
        {
            get
            {
                _residentKey ??= PointClickCareKey.CreateResidentKey(
                    OrgUuid, FacilityId, ResidentId);

                return _residentKey;
            }
        }

        [TouchpointLogMasked]
        public string MessageKey => ResidentKey;

        public IDictionary<string, object?> Items => new Dictionary<string, object?> 
        {
            { Type, ResourceIds.FirstOrDefault() }
        };

        public enum ValidationResultType
        {
            Qualified,
            Unqualified,
            Unauthorized,
        }

        public ValidationResultType Validate(PointClickCareOptions pointClickCareOptions,
            IDictionary<string, StringValues> headerValues)
        {
            foreach (var header in headerValues)
            {
                Headers[header.Key] = header.Value;
            }

            var isValidAuthorization = Headers.Where(h => h.Key == "Authorization")
                .Select(hv =>
                {
                    string? username = default;
                    string? password = default;

                    var authValue = hv.Value.ToString();
                    try
                    {
                        if (!string.IsNullOrEmpty(authValue))
                        {
                            var encoded = authValue["BASIC ".Length..];
                            byte[] data = Convert.FromBase64String(encoded);
                            string decoded = Encoding.UTF8.GetString(data);
                            var pair = decoded.Split(':');
                            username = pair[0];
                            password = pair[1];
                        }
                    }
                    catch
                    {
                        //Invalid encoding, return false
                        return false;
                    }

                    return username is not null
                        && username.Equals(pointClickCareOptions.WebhookUsername,
                            StringComparison.OrdinalIgnoreCase)
                        && password is not null
                        && password.Equals(pointClickCareOptions.WebhookPassword,
                            StringComparison.Ordinal);

                }).FirstOrDefault();

            var result = ValidationResultType.Unqualified;
            if (!isValidAuthorization)
            {
                result = ValidationResultType.Unauthorized;
            } 
            else if (pointClickCareOptions.WebhookEventTypeList.Contains(Type))
            {
                result = ValidationResultType.Qualified;
            }

            return result;
        }

    }
}
