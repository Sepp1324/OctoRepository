using System.Collections.Generic;
using OctoAwesome.Threading;

namespace OctoAwesome.Notifications
{
    public class ObserverHashSet : HashSet<INotificationObserver>
    {
        private readonly LockSemaphore semaphore;

        public ObserverHashSet()
        {
            semaphore = new LockSemaphore(1, 1);
        }

        public ObserverHashSet(IEqualityComparer<INotificationObserver> comparer) : base(comparer)
        {
            semaphore = new LockSemaphore(1, 1);
        }

        public ObserverHashSet(IEnumerable<INotificationObserver> collection) : base(collection)
        {
            semaphore = new LockSemaphore(1, 1);
        }

        public ObserverHashSet(IEnumerable<INotificationObserver> collection,
            IEqualityComparer<INotificationObserver> comparer)
            : base(collection, comparer)
        {
            semaphore = new LockSemaphore(1, 1);
        }

        public LockSemaphore.SemaphoreLock Wait()
        {
            return semaphore.Wait();
        }
    }
}