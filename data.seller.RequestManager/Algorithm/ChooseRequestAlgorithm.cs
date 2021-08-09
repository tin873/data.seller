using System.Collections.Generic;

namespace data.seller.RequestManager
{
    public abstract class ChooseRequestAlgorithm
    {
        public abstract TEntity Get<TEntity>(IReadOnlyCollection<TEntity> items);
    }
}
