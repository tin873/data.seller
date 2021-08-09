using System.Net.Http;

namespace data.seller.RequestManager.Model
{
    public class HttpClientInfo
    {
        public ProxyInfo ProxyInfo { get; }
        public HttpClient HttpClient { get; }
        public bool IsActive { get; private set; } = true;

        public HttpClientInfo(HttpClient httpClient,
            ProxyInfo proxyInfo)
        {
            HttpClient = httpClient;
            ProxyInfo = proxyInfo;
        }

        public void Active() => IsActive = true;
        public void UnActive() => IsActive = false;

        public bool IsEquals(ProxyInfo proxyInfo)
        {
            if (proxyInfo is null || ProxyInfo is null)
            {
                return false;
            }

            return proxyInfo.Key == ProxyInfo.Key
                && proxyInfo.Host == ProxyInfo.Host
                && proxyInfo.Port == ProxyInfo.Port
                && proxyInfo.Username == ProxyInfo.Username
                && proxyInfo.Password == ProxyInfo.Password;
        }
    }
}
