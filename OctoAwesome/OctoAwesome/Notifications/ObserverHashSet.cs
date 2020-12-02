using System.Collections.Generic;
using OctoAwesome.Threading;

namespace OctoAwesome.Notifications
{
    public class ObserverHashSet : HashSet<INotificationObserver>
    {
        private readonly LockedSemaphore _lockedSemaphore;

        public ObserverHashSet() : base() => _lockedSemaphore = new LockedSemaphore(1, 1);

        public ObserverHashSet(IEqualityComparer<INotificationObserver> comparer) :  base(comparer) => _lockedSemaphore = new LockedSemaphore(1, 1);

        public ObserverHashSet(IEnumerable<INotificationObserver> collection) : base(collection) => _lockedSemaphore = new LockedSemaphore(1, 1);

        public ObserverHashSet(IEnumerable<INotificationObserver> collection, IEqualityComparer<INotificationObserver> comparer)
            : base(collection, comparer) =>
            _lockedSemaphore = new LockedSemaphore(1, 1);

        public LockedSemaphore.SemaphoreLock Wait() 
            => _lockedSemaphore.Wait();
    }
}
