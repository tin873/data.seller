
using data.seller.Manager.AppGroup;
using data.seller.RequestManager;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ichiba.ProxyManager
{
    public class ProxyManagerImpl : IDowntime, ILoadProxy
    {
        private readonly AppGroupClient appGroupClient;

        public ProxyManagerImpl(AppGroupClient appGroupClient)
        {
            this.appGroupClient = appGroupClient;
        }

        public async Task Execute(string appKey, string proxyKey)
        {
            var response = await appGroupClient.NotifyDownTimeProxy(appKey, proxyKey);
        }

        public async Task<IReadOnlyCollection<ProxyInfo>> Execute(string appKey)
        {
            var response = await appGroupClient.GetAllProxyActive(appKey);
            return response.Data.Select(item => new ProxyInfo()
            {
                Key = item.Key,
                Host = item.Host,
                Port = item.Port,
                Username = item.UserName,
                Password = item.Password,
                Status = item.Status,
                Group = item.Group,
            }).ToList();
        }
    }
}
