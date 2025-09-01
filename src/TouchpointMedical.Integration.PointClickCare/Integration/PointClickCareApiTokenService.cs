using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Net.Http.Headers;
using System.Text;

using TouchpointMedical.Auth;
using TouchpointMedical.Http.Interfaces;
using TouchpointMedical.Integration.PointClickCare.Authorization;
using TouchpointMedical.Integration.PointClickCare.Configuration;

namespace TouchpointMedical.Integration.PointClickCare.Integration
{
    public class PointClickCareApiTokenService(
        IHttpClientFactory httpClientFactory, 
        IOptions<PointClickCareOptions> pccOptions,
        ILogger<PointClickCareApiTokenService> logger) : 
        ApiServiceBase<PointClickCareApiTokenService>(
            httpClientFactory.CreateClient(nameof(PointClickCareApiTokenService)),
            null, 
            logger, 
            pccOptions.Value.HttpClientOptions ?? new HttpClientOptions()), ITokenService
    {
        private readonly SemaphoreSlim _refreshGate = new(1, 1);
        private PointClickCareToken? _authToken = null;
        private PointClickCareOptions PointClickCareOptions => pccOptions.Value;

        public async Task<string> GetAccessTokenAsync(ITokenServiceInstanceKey? instanceKey = null, CancellationToken ct = default)
        {
            if (_authToken is null || _authToken.NeedsRefresh)
            {
                await _refreshGate.WaitAsync(ct);

                try
                {
                    var basicAuth = Convert.ToBase64String(
                        Encoding.UTF8.GetBytes(
                            $"{PointClickCareOptions.AppKey}:{PointClickCareOptions.AppKeySecret}"));

                    var callOptions = new HttpBodyContentOptions
                    {
                        Uri = PointClickCareOptions.AuthUri,
                        AuthorizationHeader = new AuthenticationHeaderValue("Basic", basicAuth),
                        PostBodyContent = "grant_type=client_credentials".AsFormContent()
                    };

                    _authToken = await PostAsync<PointClickCareToken>(callOptions);
                }
                finally
                {
                    _refreshGate.Release();
                }
            }

            return _authToken.AccessToken;
        }

        public async Task<string> RefreshTokenAsync(ITokenServiceInstanceKey? instanceKey = null, CancellationToken ct = default)
        {
            return await GetAccessTokenAsync(instanceKey, ct);
        }
    }
}
