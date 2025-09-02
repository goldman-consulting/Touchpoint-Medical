using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System.Text;

using TouchpointMedical.Configuration;
using TouchpointMedical.Http.Interfaces;

namespace TouchpointMedical.Integration
{
    public class TouchpointApiService(
        FacilitySettingsFactory facilitySettingsFactory,
        TouchpointApiTokenService tokenService,
        ILogger<TouchpointApiService> logger,
        IHttpClientFactory httpClientFactory) : 
        ApiServiceBase<TouchpointApiService>(
            httpClientFactory.CreateClient(nameof(TouchpointApiService)), 
            tokenService,
            logger,
            new HttpClientOptions())
    {
        private readonly FacilitySettingsFactory _facilitySettingsFactory = facilitySettingsFactory;

        private const string BaseUriFormat = 
            "https://{0}.touchpointmed.io/api/v1/{1}";

        private readonly ILogger<TouchpointApiService> _logger = logger;
        private static readonly HttpRequestOptionsKey<ITokenServiceInstanceKey> _tokenServiceKeyProperty = new("TokenServiceKey");


        public async Task<bool> Patient(
            string pccFacilityKey,
            string eventGroup, string eventType, object payload)
        {
            _logger.LogTrace("{@PatientPayload}", payload);

            var result = await ApiPostCall(
                pccFacilityKey, "patient", eventGroup, eventType, payload);

            return result.IsSuccess;
        }

        public async Task<bool> Medication(
            string pccFacilityKey, 
            string eventGroup, string eventType, object payload)
        {
            _logger.LogTrace("{@MedicationPayload}", payload);

            var result = await ApiPostCall(
                pccFacilityKey, "medication", eventGroup, eventType, payload);

            return result.IsSuccess;
        }

        public async Task<bool> Allergy(
            string pccFacilityKey,
            string eventGroup, string eventType, object payload)
        {
            _logger.LogTrace("{@AllergyPayload}", payload);

            var result = await ApiPostCall(
                pccFacilityKey, "allergy", eventGroup, eventType, payload);

            return result.IsSuccess;
        }

        private async Task<Result> ApiPostCall(
            string pccFacilityKey,
            string uriPath, string eventGroup, string eventType, object payload)
        {
            var facilitySettings = _facilitySettingsFactory.Get(pccFacilityKey);

            var uri = BuildCallUri(facilitySettings, uriPath, eventGroup, eventType);

            var json = JsonConvert.SerializeObject(payload);
            using var postBodyContent = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = postBodyContent
                };
                request.Options.Set(_tokenServiceKeyProperty, facilitySettings);

                var response = await HttpClient.SendAsync(request, CancellationToken.None);
                string rawResult = await response.Content.ReadAsStringAsync()!;

                _logger.LogTrace("{Uri} {RawResult}", uri, rawResult);

                var responseResult = JsonConvert.DeserializeObject<Result>(rawResult)!;

                return responseResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");

                throw;
            }
        }

        private static string BuildCallUri(
            FacilitySettings facilitySettings,
            string path, string eventGroup, string eventType)
        {
            path = $"{path.Trim('/')}?eventGroup={eventGroup}&eventType={eventType}";

            var callUri = string.Format(
                BaseUriFormat, facilitySettings.AppName, path);

            return callUri;
        }
    }
}
