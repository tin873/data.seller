using System.Threading.Tasks;

namespace data.seller.RequestManager
{
    public interface IDowntime
    {
        Task Execute(string appKey, string proxyKey);
    }
}
