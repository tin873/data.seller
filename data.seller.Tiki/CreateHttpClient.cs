using data.seller.RequestManager;
using System;
using System.Net;
using System.Net.Http;

namespace data.seller.Tiki
{
    public class CreateHttpClient : ICreateHttpClient
    {
        private readonly TimeSpan pooledConnectionLifetime = TimeSpan.FromMinutes(1);
        public HttpClient Create(ProxyInfo proxyInfo)
        {
            if (proxyInfo is null)
            {
                return new HttpClient(new SocketsHttpHandler
                {
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                    PooledConnectionLifetime = pooledConnectionLifetime,
                    UseCookies = true,
                    CookieContainer = new CookieContainer(),
                });
            }
            // First create a proxy object
            var proxy = new WebProxy
            {
                Address = new Uri($"http://{proxyInfo.Host}:{proxyInfo.Port}"),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false
            };

            var proxyNeedToAuthen = !string.IsNullOrWhiteSpace(proxyInfo.Username)
                && !string.IsNullOrWhiteSpace(proxyInfo.Password);

            if (proxyNeedToAuthen)
            {
                var networkCredential = new NetworkCredential(userName: proxyInfo.Username,
                    password: proxyInfo.Password);

                proxy.Credentials = networkCredential;
            }

            // Now create a client handler which uses that proxy
            var httpClientHandler = new HttpClientHandler
            {
                Proxy = proxy,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };
            // Finally, create the HTTP client object
            var client = new HttpClient(handler: httpClientHandler, disposeHandler: true);

            SetDefaultHeader(client);

            return client;
        }

        private void SetDefaultHeader(HttpClient client)
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
            client.DefaultRequestHeaders.TryAddWithoutValidation("accept-encoding", "gzip, deflate, br");
        }
    }
}
