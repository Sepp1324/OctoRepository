using System;
using System.Collections.Generic;
using OctoAwesome.Threading;

namespace OctoAwesome.Rx
{
    public class ConcurrentRelay<T> : IObservable<T>, IObserver<T>, IDisposable
    {
        private readonly LockSemaphore _lockSemaphore;
        private readonly List<RelaySubscription> _subscriptions;

        public ConcurrentRelay()
        {
            _lockSemaphore = new(1, 1);
            _subscriptions = new();
        }

        public void Dispose()
        {
            foreach (var subscription in _subscriptions)
                subscription.Dispose();

            _subscriptions.Clear();
            _lockSemaphore.Dispose();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var sub = new RelaySubscription(this, observer);

            using var scope = _lockSemaphore.Wait();
            _subscriptions.Add(sub);

            return sub;
        }

        public void OnCompleted()
        {
            using var scope = _lockSemaphore.Wait();
            foreach (var subscription in _subscriptions)
                subscription?.Observer.OnCompleted();
        }

        public void OnError(Exception error)
        {
            using var scope = _lockSemaphore.Wait();
            foreach (var subscription in _subscriptions)
                subscription?.Observer.OnError(error);
        }

        public void OnNext(T value)
        {
            using var scope = _lockSemaphore.Wait();
            foreach (var subscription in _subscriptions)
                subscription?.Observer.OnNext(value);
        }

        private void Unsubscribe(RelaySubscription subscription)
        {
            using var scope = _lockSemaphore.Wait();
            _subscriptions.Remove(subscription);
        }

        private class RelaySubscription : IDisposable
        {
            private readonly ConcurrentRelay<T> _relay;

            public RelaySubscription(ConcurrentRelay<T> relay, IObserver<T> observer)
            {
                _relay = relay;

                Observer = observer;
            }

            public IObserver<T> Observer { get; }

            public void Dispose()
            {
                _relay.Unsubscribe(this);
            }
        }
    }
}