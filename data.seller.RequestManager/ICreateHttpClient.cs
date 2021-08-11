
using data.seller.RequestManager;
using System.Net.Http;

namespace data.seller.RequestManager
{
    public interface ICreateHttpClient
    {
        HttpClient Create(ProxyInfo proxyInfo);
    }
}
