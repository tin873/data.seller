using System.Collections.Generic;
using System.Linq;

namespace data.seller.RequestManager
{
    public class ChooseRequestRoundRobinAlgorithm : ChooseRequestAlgorithm
    {
        private readonly object lockObject = new object();
        private int currentIndex = 0;

        public override TEntity Get<TEntity>(IReadOnlyCollection<TEntity> items)
        {
            lock (lockObject)
            {
                var length = items.Count;

                if (currentIndex >= length)
                {
                    currentIndex = 0;
                }

                return items.ElementAt(currentIndex++);
            }
        }
    }
}
