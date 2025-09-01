using Microsoft.Extensions.Logging;

using TouchpointMedical.Auth;
using TouchpointMedical.Configuration;
using TouchpointMedical.Http.Interfaces;
using TouchpointMedical.Models;

namespace TouchpointMedical.Integration
{
    public class TouchpointApiTokenService(
        ILogger<TouchpointApiTokenService> logger, 
        IHttpClientFactory httpClientFactory) : 
        ApiServiceBase<TouchpointApiTokenService>(
            httpClientFactory.CreateClient(nameof(TouchpointApiTokenService)), 
            null,
            logger,
            new HttpClientOptions()), ITokenService
    {
        private const string BaseUriFormat = 
            "https://{0}.touchpointmed.io/api/v1/{1}";

        private readonly Dictionary<string, SemaphoreSlim> _semaphoreMap = [];
        private readonly Dictionary<string, TouchpointToken> _authTokenMap = [];

        public async Task<string> GetAccessTokenAsync(ITokenServiceInstanceKey? instanceKeyValue = null, CancellationToken ct = default)
        {
            FacilitySettings facilitySettings = (FacilitySettings)instanceKeyValue!;

            var instanceToken = _authTokenMap.TryGetValue(
                facilitySettings.Name, out TouchpointToken? tokenValue) ? tokenValue : default;

            if (instanceToken is null || instanceToken.NeedsRefresh)
            {
                Logger.LogDebug("Getting new access token: {TouchpointInstance} {Reason}", 
                    instanceKeyValue, instanceToken is null ? "New Token" : "Refresh");

                if (!_semaphoreMap.TryGetValue(facilitySettings.InstanceKey, out SemaphoreSlim? semaphore))
                {
                    semaphore = new SemaphoreSlim(1, 1);

                    _semaphoreMap.Add(facilitySettings.InstanceKey, semaphore);
                }

                await semaphore.WaitAsync(ct);

                try
                {
                    var json = new TouchpointSiteApp
                    {
                        ClientId = facilitySettings.AppKey,
                        ClientSecret = facilitySettings.AppKeySecret
                    };

                    var callOptions = new HttpBodyContentOptions
                    {
                        Uri = BuildAuthorizationUri(facilitySettings),
                        AuthorizationHeader = null,
                        PostBodyContent = json.AsJsonContent()
                    };

                    tokenValue = await PostAsync<TouchpointToken>(callOptions);

                    Logger.LogDebug("{Instance} {@TouchpointToken}", instanceKeyValue, tokenValue);

                    _authTokenMap[facilitySettings.InstanceKey] = tokenValue;                   
                }
                finally
                {
                    semaphore.Release();
                }
            }

            return tokenValue!.AccessToken;
        }

        public async Task<string> RefreshTokenAsync(ITokenServiceInstanceKey? instanceKey = null, CancellationToken ct = default)
        {
            return await GetAccessTokenAsync(instanceKey, ct);
        }

        private static string BuildAuthorizationUri(FacilitySettings facilitySettings)
        {
            var baseUri = string.Format(
                BaseUriFormat, facilitySettings.AppName, "auth/token");

            return $"{baseUri}";
        }
    }
}
