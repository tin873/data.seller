
namespace data.seller.Manager.AppGroup.Model
{
    public class ProxyDto
    {
        public string Key { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool? Status { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
        public ProxyGroupDto ProxyGroup { get; set; }
    }
}
