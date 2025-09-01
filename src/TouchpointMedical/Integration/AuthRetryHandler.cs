using System.Net;
using System.Net.Http.Headers;

using TouchpointMedical.Auth;
using TouchpointMedical.Http.Interfaces;

namespace TouchpointMedical.Integration
{
    public class AuthRetryHandler(ITokenService tokenService) : DelegatingHandler
    {
        private readonly ITokenService _tokenService = tokenService;
        private static readonly HttpRequestOptionsKey<ITokenServiceInstanceKey> _tokenServiceKeyProperty = new("TokenServiceKey");

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Options.TryGetValue(_tokenServiceKeyProperty, out var tokenInstanceKey);

            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer", await _tokenService.GetAccessTokenAsync(tokenInstanceKey, cancellationToken));

            try
            {
                var response = await base.SendAsync(request, cancellationToken);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Refresh token and retry
                    var newToken = await _tokenService.RefreshTokenAsync(tokenInstanceKey, cancellationToken);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);

                    // Clone request and retry (make it re-usable)
                    var clonedRequest = await CloneHttpRequestMessageAsync(request);
                    clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);

                    return await base.SendAsync(clonedRequest, cancellationToken);
                }

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Content = request.Content == null 
                    ? null 
                    : await request.Content
                        .ReadAsStreamAsync()
                        .ContinueWith(t => new StreamContent(t.Result)),
                Version = request.Version
            };

            foreach (var header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            foreach (var kvp in request.Options)
            {
                HttpRequestOptionsKey<object?> key = new(kvp.Key);

                clone.Options.Set(key, kvp.Value);
            }

            return clone;
        }
    }
}
