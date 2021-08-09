using System;
using System.Collections.Generic;
using System.Linq;

namespace data.seller.RequestManager
{
    public class ChooseRequestRandomAlgorithm : ChooseRequestAlgorithm
    {
        public override TEntity Get<TEntity>(IReadOnlyCollection<TEntity> items)
        {
            var random = new Random();
            var length = items.Count;
            var index = random.Next(0, length);

            return items.ElementAt(index);
        }
    }
}
