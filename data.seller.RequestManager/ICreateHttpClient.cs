
using data.seller.RequestManager.Model;
using System.Net.Http;

namespace data.seller.RequestManager
{
    public interface ICreateHttpClient
    {
        HttpClient Create(ProxyInfo proxyInfo);
    }
}
