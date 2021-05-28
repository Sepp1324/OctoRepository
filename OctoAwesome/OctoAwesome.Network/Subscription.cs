using System;
using OctoAwesome.Threading;

namespace OctoAwesome.Network
{
    public class Subscription<T> : IDisposable
    {
        public Subscription(IAsyncObservable<T> observable, IAsyncObserver<T> observer)
        {
            Observable = observable;
            Observer = observer;
        }

        private IAsyncObservable<T> Observable { get; }

        private IAsyncObserver<T> Observer { get; }

        public void Dispose()
        {
        }
    }
}