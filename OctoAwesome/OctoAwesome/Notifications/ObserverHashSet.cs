﻿using OctoAwesome.Threading;
using System.Collections.Generic;

namespace OctoAwesome.Notifications
{
    public class ObserverHashSet : HashSet<INotificationObserver>
    {
        private readonly LockSemaphore semaphore;

        public ObserverHashSet() : base() => semaphore = new LockSemaphore(1, 1);

        public ObserverHashSet(IEqualityComparer<INotificationObserver> comparer) :  base(comparer) => semaphore = new LockSemaphore(1, 1);

        public ObserverHashSet(IEnumerable<INotificationObserver> collection) : base(collection) => semaphore = new LockSemaphore(1, 1);

        public ObserverHashSet(IEnumerable<INotificationObserver> collection, IEqualityComparer<INotificationObserver> comparer)
            : base(collection, comparer) =>
            semaphore = new LockSemaphore(1, 1);

        public LockSemaphore.SemaphoreLock Wait() => semaphore.Wait();
    }
}
