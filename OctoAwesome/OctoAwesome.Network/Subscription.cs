using OctoAwesome.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class Subscription<T> : IDisposable
    {
        public Subscription(IAsyncObservable<T> observable, IAsyncObserver<T> observer)
        {
            Observable = observable;
            Observer = observer;
        }

        public IAsyncObservable<T> Observable { get; }
        public IAsyncObserver<T> Observer { get; }

        public void Dispose()
        {
        }
    }
}