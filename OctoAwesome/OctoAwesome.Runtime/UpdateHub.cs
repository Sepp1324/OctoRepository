using OctoAwesome.Notifications;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Runtime
{
    public class UpdateHub : IUpdateHub, IDisposable
    {
        private readonly NotificationChannelCollection _observers;

<<<<<<< HEAD
        public UpdateHub() => _observers = new NotificationChannelCollection();
=======
        public UpdateHub()
            => observers = new NotificationChannelCollection();
>>>>>>> feature/performance

        public IDisposable Subscribe(INotificationObserver observer, string channel = "none")
        {
            _observers.Add(channel, observer);
            return new NotificationSubscription(this, observer, channel);
        }

<<<<<<< HEAD
        public void Unsubscribe(INotificationObserver observer) => _observers.Remove(observer);

        public void Unsubscribe(INotificationObserver observer, string channel) => _observers.Remove(channel, observer);

        public void Push(Notification notification)
        {
            foreach (KeyValuePair<string, ObserverHashSet> observerSet in _observers)
=======
        public void Unsubscribe(INotificationObserver observer)
            => observers.Remove(observer);

        public void Unsubscribe(INotificationObserver observer, string channel)
            => observers.Remove(channel, observer);

        public void Push(Notification notification)
        {
            foreach (KeyValuePair<string, ObserverHashSet> observerSet in observers)
>>>>>>> feature/performance
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
<<<<<<< HEAD
            if (_observers.TryGetValue(channel, out ObserverHashSet observerSet))
=======

            if (observers.TryGetValue(channel, out ObserverHashSet observerSet))
>>>>>>> feature/performance
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
<<<<<<< HEAD
            foreach (KeyValuePair<string, ObserverHashSet> observerSet in _observers)
=======

            foreach (KeyValuePair<string, ObserverHashSet> observerSet in observers)
>>>>>>> feature/performance
            {
                using (observerSet.Value.Wait())
                {
                    foreach (INotificationObserver observer in observerSet.Value)
                        observer.OnCompleted();
                }
            }
<<<<<<< HEAD
            _observers.Clear();
        }
=======
            observers.Clear();
        }

>>>>>>> feature/performance
    }
}
