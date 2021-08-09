
using data.seller.RequestManager.Model;
using System.Threading.Tasks;

namespace data.seller.RequestManager
{
    public interface RequestManagerConfig
    {
        string AppKey { get; set; }
        int ReloadProxyDelayTimeSecond { get; set; }
    }

    public interface IRequestManager<TConfig> where TConfig : RequestManagerConfig
    {
        Task LoadProxy();
        Task Downtime(string proxyKey);
        HttpClientInfo GetHttpClient();
    }
}
