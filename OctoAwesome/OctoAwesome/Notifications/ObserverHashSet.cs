using OctoAwesome.Threading;
using System.Collections.Generic;

namespace OctoAwesome.Notifications
{
    public class ObserverHashSet : HashSet<INotificationObserver>
    {
        private readonly LockSemaphore _semaphore;

        public ObserverHashSet() : base() => _semaphore = new LockSemaphore(1, 1);

        public ObserverHashSet(IEqualityComparer<INotificationObserver> comparer) : base(comparer) => _semaphore = new LockSemaphore(1, 1);

        public ObserverHashSet(IEnumerable<INotificationObserver> collection) : base(collection) => _semaphore = new LockSemaphore(1, 1);

        public ObserverHashSet(IEnumerable<INotificationObserver> collection, IEqualityComparer<INotificationObserver> comparer) : base(collection, comparer) => _semaphore = new LockSemaphore(1, 1);

        public LockSemaphore.SemaphoreLock Wait() => _semaphore.Wait();
    }
}