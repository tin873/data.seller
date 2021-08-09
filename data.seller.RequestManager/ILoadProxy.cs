
using data.seller.RequestManager.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace data.seller.RequestManager
{
    public interface ILoadProxy
    {
        Task<IReadOnlyCollection<ProxyInfo>> Execute(string appKey);
    }
}
