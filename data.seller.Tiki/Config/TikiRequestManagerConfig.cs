using data.seller.RequestManager;

namespace data.seller.Tiki
{
    public class TikiRequestManagerConfig : RequestManagerConfig
    {
        public string AppKey { get; set; }
        public int ReloadProxyDelayTimeSecond { get; set; } = 30;
    }
}
