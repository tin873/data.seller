
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace data.seller.RequestManager
{
    public class RequestManager<TConfig> : IRequestManager<TConfig> where TConfig : RequestManagerConfig
    {
        private readonly HttpClientInfo defaultHttpClient;
        private IReadOnlyCollection<HttpClientInfo> httpClients = new List<HttpClientInfo>();
        private readonly ChooseRequestAlgorithm chooseRequestAlgorithm;
        private readonly ILoadProxy reloadProxy;
        private readonly IDowntime downtime;
        private readonly ICreateHttpClient createHttpClient;
        private readonly TConfig requestManagerConfig;

        public RequestManager(ChooseRequestAlgorithm chooseRequestAlgorithm,
            ILoadProxy reloadProxy,
            IDowntime downtime,
            ICreateHttpClient createHttpClient,
            TConfig requestManagerConfig)
        {
            this.chooseRequestAlgorithm = chooseRequestAlgorithm;
            this.chooseRequestAlgorithm = chooseRequestAlgorithm;
            this.reloadProxy = reloadProxy;
            this.downtime = downtime;
            this.createHttpClient = createHttpClient;
            this.requestManagerConfig = requestManagerConfig;

            defaultHttpClient = CreateHttpClient(null);

            AutoReloadProxy();
        }

        private HttpClientInfo Find(string proxyKey) => httpClients.Where(item => item.ProxyInfo != null && item.ProxyInfo.Key.Equals(proxyKey)).FirstOrDefault();

        public async Task Downtime(string proxyKey)
        {
            if (string.IsNullOrWhiteSpace(proxyKey))
            {
                return;
            }

            await downtime.Execute(requestManagerConfig.AppKey, proxyKey);

            var httpClientDownTime = Find(proxyKey);

            if (httpClientDownTime != null)
            {
                httpClientDownTime.UnActive();
            }
        }

        public HttpClientInfo GetHttpClient()
        {
            if (httpClients.Count == 0)
            {
                return defaultHttpClient;
            }

            return chooseRequestAlgorithm.Get(httpClients);
        }

        private HttpClientInfo CreateHttpClient(ProxyInfo proxyInfo)
        {
            var httpClient = createHttpClient.Create(proxyInfo);

            return new HttpClientInfo(httpClient, proxyInfo);
        }

        public async Task LoadProxy()
        {
            var proxies = await reloadProxy.Execute(requestManagerConfig.AppKey);
            var httpClientActives = httpClients.Where(item => proxies.Any(pItem => item.IsEquals(pItem)))
                .ToList();
            var httpClientUnActives = httpClients.Where(item => !proxies.Any(pItem => item.IsEquals(pItem)))
                .ToList();
            var httpClientNews = proxies.Where(item => !httpClients.Any(hItem => hItem.IsEquals(item)))
                .Select(item => CreateHttpClient(item));

            httpClientActives.ForEach(item => item.Active());
            httpClientUnActives.ForEach(item => item.UnActive());

            httpClients = httpClients.Concat(httpClientNews).ToList();
        }

        private void AutoReloadProxy()
        {
            try
            {
                LoadProxy().Wait();
            }
            catch
            {
            }

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(requestManagerConfig.ReloadProxyDelayTimeSecond));
                        await LoadProxy();
                    }
                    catch
                    {
                    }
                }
            });
        }
    }
}
