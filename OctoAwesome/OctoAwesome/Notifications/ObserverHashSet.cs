using OctoAwesome.Threading;
using System.Collections.Generic;

namespace OctoAwesome.Notifications
{
    public class ObserverHashSet : HashSet<INotificationObserver>
    {
        private readonly LockSemaphore _semaphore;

        public ObserverHashSet() => _semaphore = new LockSemaphore(1, 1);

<<<<<<< HEAD
        public ObserverHashSet(IEqualityComparer<INotificationObserver> comparer) :  base(comparer) => _semaphore = new LockSemaphore(1, 1);
=======
        public ObserverHashSet(IEqualityComparer<INotificationObserver> comparer) :  base(comparer)
        {
            semaphore = new LockSemaphore(1, 1);
        }
>>>>>>> feature/performance

        public ObserverHashSet(IEnumerable<INotificationObserver> collection) : base(collection) => _semaphore = new LockSemaphore(1, 1);

<<<<<<< HEAD
        public ObserverHashSet(IEnumerable<INotificationObserver> collection, IEqualityComparer<INotificationObserver> comparer) : base(collection, comparer) => _semaphore = new LockSemaphore(1, 1);

        public LockSemaphore.SemaphoreLock Wait() => _semaphore.Wait();
=======
        public ObserverHashSet(IEnumerable<INotificationObserver> collection, IEqualityComparer<INotificationObserver> comparer)
            : base(collection, comparer)
        {
            semaphore = new LockSemaphore(1, 1);
        }

        public LockSemaphore.SemaphoreLock Wait() 
            => semaphore.Wait();
>>>>>>> feature/performance
    }
}
