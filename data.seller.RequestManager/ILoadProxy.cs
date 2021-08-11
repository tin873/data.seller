
using data.seller.RequestManager;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace data.seller.RequestManager
{
    public interface ILoadProxy
    {
        Task<IReadOnlyCollection<ProxyInfo>> Execute(string appKey);
    }
}
