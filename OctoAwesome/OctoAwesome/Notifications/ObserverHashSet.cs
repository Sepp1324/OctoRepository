using System.Collections.Generic;
using OctoAwesome.Threading;

namespace OctoAwesome.Notifications
{
    public class ObserverHashSet : HashSet<INotificationObserver>
    {
        private readonly LockSemaphore _lockSemaphore;

        public ObserverHashSet() : base() => _lockSemaphore = new LockSemaphore(1, 1);

        public ObserverHashSet(IEqualityComparer<INotificationObserver> comparer) :  base(comparer) => _lockSemaphore = new LockSemaphore(1, 1);

        public ObserverHashSet(IEnumerable<INotificationObserver> collection) : base(collection) => _lockSemaphore = new LockSemaphore(1, 1);

        public ObserverHashSet(IEnumerable<INotificationObserver> collection, IEqualityComparer<INotificationObserver> comparer) : base(collection, comparer) => _lockSemaphore = new LockSemaphore(1, 1);

        public LockSemaphore.SemaphoreLock Wait() => _lockSemaphore.Wait();
    }
}
