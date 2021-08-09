using data.seller.RequestManager;
using data.seller.Tiki.Model.Request;
using data.seller.Tiki.Model.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace data.seller.Tiki
{
    public class SellerClient : BaseClient
    {
        public readonly EndPointConfig endPointConfig;
        public SellerClient(IRequestManager<TikiRequestManagerConfig> requestManager, EndPointConfig endPointConfig)
           : base(requestManager, requestManager.GetHttpClient())
        {
            this.endPointConfig = endPointConfig;
        }

        private async Task<string> ReadStringFromHttpResponseMessage(HttpResponseMessage httpResponseMessage)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var enc = Encoding.GetEncoding("UTF-8");

            using (var stream = await httpResponseMessage.Content.ReadAsStreamAsync())
            {
                using (var reader = new StreamReader(stream, enc, true) as TextReader)
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
        /// <summary>
        /// lấy token 
        /// </summary>
        /// <param name="url">đường dẫn lấy token</param>
        /// <param name="content">body</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetToken(string url)
        {
            TikiTokenRequest content = new TikiTokenRequest()
            {
                Code = "",
                ClientId = "884806377953-aoeb4oh93tb7ftfhgde7gqggbgq3el13.apps.googleusercontent.com",//usesrName
                ClientSecret = "QyqDxDH2JoUc3WKWCiMQ_vnJ",//Password
                GrantType = "",
                RedirectUri = "",
            };
            return await PostAsync(uri: url, content: content);
        }

        public async Task<IList<TikiSeller>> GetListSeller(string urlData)
        {
            var token = await GetToken("urlGetToken");
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token;
            var jsonContent = token.Content.ReadAsStringAsync();
            Token tok = JsonConvert.DeserializeObject<Token>(jsonContent.ToString());
            var resMes = await GetStringAsync(uri: urlData, authorizationToken: tok.AccessToken, cancellationToken: cancellationToken);
            var content = ReadStringFromHttpResponseMessage(resMes);
            JObject jObjectData = JObject.Parse(content.ToString());
            JArray arrObject = (JArray)jObjectData["itemList"];
            IList<TikiSeller> data = arrObject.ToObject<IList<TikiSeller>>();
            return data;
        }
    }
}
