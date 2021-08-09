using Core.Common;
using data.seller.RequestManager;
using data.seller.RequestManager.Model;
using data.seller.Tiki.Model.Request;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace data.seller.Tiki
{
    public abstract class BaseClient
    {
        private readonly IRequestManager<TikiRequestManagerConfig> _requestManager;
        protected HttpClientInfo httpClientInfo;
        protected HttpClient client => httpClientInfo.HttpClient;

        public BaseClient(IRequestManager<TikiRequestManagerConfig> requestManager,
            HttpClientInfo httpClientInfo)
        {
            _requestManager = requestManager;
            this.httpClientInfo = httpClientInfo;
        }
        public async Task<HttpResponseMessage> GetStringAsync(string uri,
           string authorizationToken = null,
           string authorizationMethod = "Bearer",
           Action<HttpRequestMessage> configHttpRequestMessage = null,
           CancellationToken cancellationToken = default)
        {

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }

            ExecconfigHttpRequestMessage(configHttpRequestMessage, requestMessage); //Action<HttpRequestMessage> configHttpRequestMessage = null,

            return await client.SendAsync(requestMessage, cancellationToken);

            //var response = await GetAsync(uri, authorizationToken, authorizationMethod);

            //return await response.Content.ReadAsStringAsync();
        }
        protected virtual void ExecconfigHttpRequestMessage(Action<HttpRequestMessage> configHttpRequestMessage,
            HttpRequestMessage requestMessage)
        {
            if (configHttpRequestMessage == null
                || requestMessage == null)
            {
                return;
            }

            configHttpRequestMessage.Invoke(requestMessage);
        }
        protected virtual async Task<TResponse> Get<TResponse>(string uri,
           string authorizationToken = null)
        {
            var response = await GetStringAsync(uri,
                authorizationToken);
            var data = Serialize.JsonDeserializeObject<TResponse>(response.Content.ReadAsStringAsync().ToString());

            return data;
        }
        private async Task<HttpResponseMessage> DoPostPutAsync(HttpMethod method, string uri, TikiTokenRequest item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
            {
                throw new ArgumentException("Value must be either post or put.", nameof(method));
            }

            // a new StringContent must be created for each retry
            // as it is disposed after each call

            var requestMessage = new HttpRequestMessage(method, uri);

            requestMessage.Content = new StringContent(Serialize.JsonSerializeObject(item), System.Text.Encoding.UTF8, "application/json");

            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }

            if (requestId != null)
            {
                requestMessage.Headers.Add("x-requestid", requestId);
            }

            var response = await client.SendAsync(requestMessage);

            // raise exception if HttpResponseCode 500
            // needed for circuit breaker to track fails

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }

            return response;
        }
        public async Task<HttpResponseMessage> PostAsync(string uri, TikiTokenRequest content)
        {
            return await DoPostPutAsync(HttpMethod.Post, uri, content);
        }

    }
}
