using OctoAwesome.Notifications;
using System;
using System.Collections.Generic;
using System.Threading;

namespace OctoAwesome.Runtime
{
    public class UpdateHub : IUpdateHub, IDisposable
    {
        private readonly  observers;
        private readonly SemaphoreSlim observerSemaphore;

        public UpdateHub()
        {
            observers = new Dictionary<string, HashSet<INotificationObserver>>();
            observerSemaphore = new SemaphoreSlim(1, 1);
        }

        public IDisposable Subscribe(INotificationObserver observer, string channel = "none")
        {
            observerSemaphore.Wait();
            observers.Add(observer);
            observerSemaphore.Release();

            return new NotificationSubscription(this, observer, channel);
        }

        public void Unsubscribe(INotificationObserver observer, string channel)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(INotificationObserver observer)
        {
            observerSemaphore.Wait();
            observers.Remove(subscriber);
            observerSemaphore.Release();
        }

        public void Push(Notification notification)
        {
            observerSemaphore.Wait();

            foreach (var observer in observers)
                observer.OnNext(notification);

            observerSemaphore.Release();
        }

        public void Dispose()
        {
            observerSemaphore.Wait();

            foreach (var observer in observers)
                observer.OnCompleted();

            observers.Clear();
            observerSemaphore.Release();
        }
    }
}
