using Core.Resilience.Http;
using data.seller.Manager.AppGroup;
using data.seller.RequestManager;
using Ichiba.ProxyManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net.Http;
using ChooseRequestAlgorithm = data.seller.RequestManager.ChooseRequestAlgorithm;
using ChooseRequestRoundRobinAlgorithm = data.seller.RequestManager.ChooseRequestRoundRobinAlgorithm;
namespace data.seller
{
    class Program
    {
        static void Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .Build();
            ServiceCollection services = new ServiceCollection();
            services.AddTransient<HttpClient>(sp => new HttpClient());

            services.AddSingleton<IResilientHttpClientFactory, ResilientHttpClientFactory>(sp =>
            {
                //var logger = sp.GetRequiredService<ILogger<ResilientHttpClient>>();
                var retryCount = 1;
                var exceptionsAllowedBeforeBreaking = 5;

                return new ResilientHttpClientFactory(null, exceptionsAllowedBeforeBreaking, retryCount);
            });
            #region Global proxy configs

            services.AddSingleton<AppGroupClient>();
            //services.AddSingleton(configuration.GetSection("ProxyManager:AppGroup").Get<AppGroupConfig>());

            services.AddSingleton<ChooseRequestAlgorithm, ChooseRequestRoundRobinAlgorithm>();
            services.AddSingleton<HttpClient>();
            services.AddSingleton<ChooseRequestAlgorithm, ChooseRequestRoundRobinAlgorithm>();

            services.AddSingleton<IDowntime, ProxyManagerImpl>();
            services.AddSingleton<ILoadProxy, ProxyManagerImpl>();

            #endregion Global proxy configs
            services.AddSingleton<IHttpClient, ResilientHttpClient>(sp => sp.GetService<IResilientHttpClientFactory>().CreateResilientHttpClient());

            services.AddTransient<Tiki.SellerClient>();

            services.AddSingleton(configuration.GetSection("Tiki:RequestManagerConfig").Get<Tiki.TikiRequestManagerConfig>());

            services.AddSingleton<ICreateHttpClient, Tiki.CreateHttpClient>();
            services.AddSingleton<IRequestManager<Tiki.TikiRequestManagerConfig>, RequestManager<Tiki.TikiRequestManagerConfig>>();

            var serviceProvider = services.BuildServiceProvider();
            //test
            var testService = serviceProvider.GetRequiredService<Tiki.SellerClient>();
            var ax = testService.GetListSeller("https://api.tiki.vn/integration/v1/sellers/me").Result;
        }
    }
}
