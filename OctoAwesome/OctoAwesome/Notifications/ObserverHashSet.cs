using System.Collections.Generic;

namespace OctoAwesome.Notifications
{
    public class ObserverHashSet : HashSet<INotificationObserver>
    {
        private readonly SemaphoreExtended _semaphore;

        public ObserverHashSet() : base() => _semaphore = new SemaphoreExtended(1, 1);

        public ObserverHashSet(IEqualityComparer<INotificationObserver> comparer) : base(comparer) => _semaphore = new SemaphoreExtended(1, 1);

        public ObserverHashSet(IEnumerable<INotificationObserver> collection) : base(collection) => _semaphore = new SemaphoreExtended(1, 1);

        public ObserverHashSet(IEnumerable<INotificationObserver> collection, IEqualityComparer<INotificationObserver> comparer)
            : base(collection, comparer) => _semaphore = new SemaphoreExtended(1, 1);

        public SemaphoreExtended.SemaphoreLock Wait() => _semaphore.Wait();
    }
}
