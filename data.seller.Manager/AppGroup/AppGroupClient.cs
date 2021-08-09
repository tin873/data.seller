
using data.seller.Manager.AppGroup.Model;
using data.seller.Manager.Base.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace data.seller.Manager.AppGroup
{
    public class AppGroupConfig
    {
        public AppGroupConfig()
        {
            GetAllProxyActive = null;
            NotifyDownTimeProxy = null;
            LogProxyLoading = null;
        }

        public string BaseApi { get; set; }


        private const bool LogProxyLoadingDefault = false;
        private bool? _LogProxyLoading;
        public bool? LogProxyLoading
        {
            get => _LogProxyLoading;
            set => _LogProxyLoading = value.HasValue ? value : LogProxyLoadingDefault;
        }

        private const string GetAllProxyActivePath = "/AppGroup/GetAllProxyActive";

        private string getAllProxyActive;
        public string GetAllProxyActive
        {
            get => getAllProxyActive;
            set => getAllProxyActive = BuildFullUrl(ValueOrDefault(value, GetAllProxyActivePath));
        }

        private const string NotifyDownTimeProxyPath = "/AppGroup/NotifyDownTimeProxy";

        private string notifyDownTimeProxy;
        public string NotifyDownTimeProxy
        {
            get => notifyDownTimeProxy;
            set => notifyDownTimeProxy = BuildFullUrl(ValueOrDefault(value, NotifyDownTimeProxyPath));
        }

        private string BuildFullUrl(string path)
        {
            return $"{BaseApi}{path}";
        }

        private string ValueOrDefault(string value, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }
    }

    public class AppGroupClient
    {
        private readonly HttpClient httpClient;
        private readonly AppGroupConfig appGroupConfig;

        public AppGroupClient(HttpClient httpClient, AppGroupConfig appGroupConfig)
        {
            this.httpClient = httpClient;
            this.appGroupConfig = appGroupConfig;
        }

        public async Task<BaseEntityResponse<IReadOnlyCollection<ProxyDto>>> GetAllProxyActive(string appGroupKey)
        {
            var url = $"{appGroupConfig.GetAllProxyActive}?appGroupKey={appGroupKey}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            //requestMessage.Content = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<BaseEntityResponse<IReadOnlyCollection<ProxyDto>>>(responseContent);

            if (appGroupConfig.LogProxyLoading.HasValue && appGroupConfig.LogProxyLoading.Value)
            {
                Console.WriteLine("Proxy loading...");
                foreach (var item in result.Data)
                {
                    Console.WriteLine(item.Key);
                }
            }

            return result;
        }

        public async Task<BaseResponse> NotifyDownTimeProxy(string appGroupKey, string proxyKey)
        {
            var url = $"{appGroupConfig.NotifyDownTimeProxy}?appGroupKey={appGroupKey}&proxyKey={proxyKey}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            //requestMessage.Content = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<BaseEntityResponse<IReadOnlyCollection<ProxyDto>>>(responseContent);
        }
    }
}
