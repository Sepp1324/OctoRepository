using System;
using OctoAwesome.Notifications;

namespace OctoAwesome.Runtime
{
    public class UpdateHub : IUpdateHub, IDisposable
    {
        private readonly NotificationChannelCollection observers;

        public UpdateHub()
        {
            observers = new NotificationChannelCollection();
        }

        public void Dispose()
        {
            foreach (var observerSet in observers)
                using (observerSet.Value.Wait())
                {
                    foreach (var observer in observerSet.Value)
                        observer.OnCompleted();
                }

            observers.Clear();
        }

        public IDisposable Subscribe(INotificationObserver observer, string channel = "none")
        {
            observers.Add(channel, observer);
            return new NotificationSubscription(this, observer, channel);
        }

        public void Unsubscribe(INotificationObserver observer)
        {
            observers.Remove(observer);
        }

        public void Unsubscribe(INotificationObserver observer, string channel)
        {
            observers.Remove(channel, observer);
        }

        public void Push(Notification notification)
        {
            foreach (var observerSet in observers)
                using (observerSet.Value.Wait())
                {
                    foreach (var observer in observerSet.Value)
                        observer.OnNext(notification);
                }
        }

        public void Push(Notification notification, string channel)
        {
            if (observers.TryGetValue(channel, out var observerSet))
                using (observerSet.Wait())
                {
                    foreach (var observer in observerSet)
                        observer.OnNext(notification);
                }
        }
    }
}