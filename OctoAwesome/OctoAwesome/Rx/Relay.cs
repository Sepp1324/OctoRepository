using System;
using System.Collections.Generic;

namespace OctoAwesome.Rx
{
    public class Relay<T> : IObservable<T>, IObserver<T>, IDisposable
    {
        private readonly List<RelaySubscription> _subscriptions;

        public Relay() => _subscriptions = new();

        public void Dispose()
        {
            foreach (var subscription in _subscriptions)
                subscription.Dispose();

            _subscriptions.Clear();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var sub = new RelaySubscription(this, observer);
            _subscriptions.Add(sub);
            return sub;
        }

        public void OnCompleted()
        {
            foreach (var subscription in _subscriptions)
                subscription?.Observer.OnCompleted();
        }

        public void OnError(Exception error)
        {
            foreach (var subscription in _subscriptions)
                subscription?.Observer.OnError(error);
        }

        public void OnNext(T value)
        {
            foreach (var subscription in _subscriptions)
                subscription?.Observer.OnNext(value);
        }

        private void Unsubscribe(RelaySubscription subscription)
        {
            _subscriptions.Remove(subscription);
        }

        private class RelaySubscription : IDisposable
        {
            private readonly Relay<T> _relay;

            public RelaySubscription(Relay<T> relay, IObserver<T> observer)
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