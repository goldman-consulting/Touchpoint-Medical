using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System.Runtime.CompilerServices;

using TouchpointMedical.Auth;
using TouchpointMedical.Logging;
using TouchpointMedical.Models;

namespace TouchpointMedical.Integration
{
    public abstract class ApiServiceBase<TLogger>(
        HttpClient httpClient, 
        ITokenService? tokenService, 
        ILogger<TLogger> logger,
        HttpClientOptions? httpClientOptions)
    {
        protected HttpClient HttpClient => httpClient;
        protected ITokenService? TokenService => tokenService;
        protected ILogger<TLogger> Logger => logger;
        protected HttpClientOptions Options => httpClientOptions ?? new HttpClientOptions();

        //private string? _token = null;
        //protected virtual async Task EnsureAuthorizationHeaderAsync(object? tokenKey)
        //{
        //    if (TokenService is not null)
        //    {
        //        string token = await TokenService.GetAccessTokenAsync(tokenKey, CancellationToken.None);

        //        if (!token.Equals(_token))
        //        {
        //            _token = token;

        //            HttpClient.DefaultRequestHeaders.Authorization =
        //                new AuthenticationHeaderValue("Bearer", token);
        //        }
        //    }
        //}

        protected virtual async Task<T> GetSingleAsync<T>(string uri, [CallerMemberName] string? caller = null)
        {
            //await EnsureAuthorizationHeaderAsync(null);

            LogRequestCaller(uri, HttpClient.DefaultRequestHeaders, null, caller);

            var response = await HttpClient.GetAsync(uri);

            string rawResult = await response.Content.ReadAsStringAsync()!;

            var singleResult = JsonConvert.DeserializeObject<T>(rawResult)!;

            LogResponseCaller(uri, response.Headers, singleResult, caller);

            return singleResult;
        }

        protected virtual async Task<List<T>> GetPagedAsync<T>(
            string uri, bool getFirstPageOnly = false, [CallerMemberName] string? caller = null)
        {
            //await EnsureAuthorizationHeaderAsync(null);

            List<T> data = [];

            int page = 1;
            bool hasMore;
            do
            {
                var pageUri = getFirstPageOnly ? uri : $"{uri}&page={page++}";

                LogRequestCaller(pageUri, HttpClient.DefaultRequestHeaders, null, caller);

                var response = await HttpClient.GetAsync(pageUri);

                string rawResult = await response.Content.ReadAsStringAsync()!;

                var pagedResult = JsonConvert.DeserializeObject<PagedResult<T>>(rawResult)!;

                LogResponseCaller(pageUri, response.Headers, pagedResult, caller);

                foreach (T resultItem in pagedResult.Data)
                {
                    data.Add(resultItem);
                }

                hasMore = !getFirstPageOnly && pagedResult.Paging.HasMore;

            } while (hasMore);

            return data;
        }

        protected async Task<T> PostAsync<T>(HttpBodyContentOptions callOptions, [CallerMemberName] string? caller = null)
        {
            LogRequestCaller(callOptions.Uri, HttpClient.DefaultRequestHeaders, callOptions.PostBodyContent, caller);

            HttpResponseMessage response;
            if (callOptions.AuthorizationHeader is null)
            {
                //await EnsureAuthorizationHeaderAsync(null);

                response = await HttpClient.PostAsync(
                    callOptions.Uri, callOptions.PostBodyContent);
            }
            else
            {
                var request = new HttpRequestMessage(HttpMethod.Post, callOptions.Uri)
                {
                    Content = callOptions.PostBodyContent
                };

                request.Headers.Authorization = callOptions.AuthorizationHeader;

                response = await HttpClient.SendAsync(request);
            }

            string rawResult = await response.Content.ReadAsStringAsync()!;

            var postResponseResult = JsonConvert.DeserializeObject<T>(rawResult)!;

            LogResponseCaller(callOptions.Uri, response.Headers, postResponseResult, caller);

            return postResponseResult;
        }

        protected async Task<T> PostAsync<T>(string uri, HttpContent postBodyContent, [CallerMemberName] string? caller = null)
        {
            return await PostAsync<T>(new HttpBodyContentOptions { Uri = uri, PostBodyContent = postBodyContent }, caller);

            //HttpClient.DefaultRequestHeaders.Authorization
            //    = new AuthenticationHeaderValue("Bearer", await GetAccessToken());

            //LogRequestCaller(uri, HttpClient.DefaultRequestHeaders, caller);

            //var response = await HttpClient.PostAsync(
            //    uri, postBodyContent);

            //string rawResult = await response.Content.ReadAsStringAsync()!;

            //var postResponseResult = JsonConvert.DeserializeObject<T>(rawResult)!;

            //LogResponseCaller(uri, response.Headers, postResponseResult, caller);

            //return postResponseResult;
        }

        protected void LogRequest(
            string uri,
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers,
            HttpContent? bodyContent = null
            ) => LogRequestCaller(uri, headers, bodyContent);

        protected void LogRequestCaller(
            string uri,
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers,
            HttpContent? bodyContent = null,
            [CallerMemberName] string? caller = null)
        {
            if (Options.CallLogging.HasAllFlags(WebApiCallLoggingType.Request | WebApiCallLoggingType.WithHeaders))
            {
                if (bodyContent is null)
                {
                    Logger.LogDebug("{Message} {Uri} {@RequestHeaders}", caller, uri, headers);
                }
                else
                {
                    Logger.LogDebug("{Message} {Uri} {@RequestHeaders} {@BodyContent}", caller, uri, headers, bodyContent);
                }

            }
            else
            {
                if (bodyContent is null)
                {
                    Logger.LogDebug("{Message} {Uri}", caller, uri);
                }
                else
                {
                    Logger.LogDebug("{Message} {Uri} {@BodyContent}", caller, uri, bodyContent);
                }

            }
        }

        protected void LogResponse(
            string uri,
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers,
            object result) =>LogResponseCaller(uri, headers, result);

        protected void LogResponseCaller(
            string uri, 
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers, 
            object result, [CallerMemberName] string? caller = null)
        {
            if (Options.CallLogging.HasAllFlags(WebApiCallLoggingType.Response | WebApiCallLoggingType.WithHeaders)) 
            {
                Logger.LogDebug("{Message} {Uri} {@ResponseHeaders} {@Result}", caller, uri, headers, result);
            }
            else
            {
                Logger.LogDebug("{Message} {Uri} {@Result}", caller, uri, result);
            }
        }
    }
}
