using Newtonsoft.Json;

namespace data.seller.Tiki.Model.Request
{
    public class TikiTokenRequest
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("clientId")]
        public string ClientId { get; set; }
        [JsonProperty("clientSecret")]
        public string ClientSecret { get; set; }
        [JsonProperty("redirectUri")]
        public string RedirectUri { get; set; }
        [JsonProperty("grantType")]
        public string GrantType { get; set; }
    }
}
