using OctoAwesome.Notifications;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Runtime
{
    public class UpdateHub : IUpdateHub, IDisposable
    {
        private readonly NotificationChannelCollection _observers;

        public UpdateHub() => _observers = new NotificationChannelCollection();

        public IDisposable Subscribe(INotificationObserver observer, string channel = "none")
        {
            _observers.Add(channel, observer);
            return new NotificationSubscription(this, observer, channel);
        }

        public void Unsubscribe(INotificationObserver observer) => _observers.Remove(observer);

        public void Unsubscribe(INotificationObserver observer, string channel) => _observers.Remove(channel, observer);

        public void Push(Notification notification)
        {
            foreach (KeyValuePair<string, ObserverHashSet> observerSet in _observers)
            {
                using (observerSet.Value.Wait())
                {
                    foreach (INotificationObserver observer in observerSet.Value)
                        observer.OnNext(notification);
                }
            }
        }

        public void Push(Notification notification, string channel)
        {
            if (_observers.TryGetValue(channel, out ObserverHashSet observerSet))
            {
                using (observerSet.Wait())
                {
                    foreach (INotificationObserver observer in observerSet)
                        observer.OnNext(notification);
                }
            }

        }

        public void Dispose()
        {
            foreach (KeyValuePair<string, ObserverHashSet> observerSet in _observers)
            {
                using (observerSet.Value.Wait())
                {
                    foreach (INotificationObserver observer in observerSet.Value)
                        observer.OnCompleted();
                }
            }
            _observers.Clear();
        }
    }
}
