using System;
using OctoAwesome.Notifications;

namespace OctoAwesome.Runtime
{
    public class UpdateHub : IUpdateHub, IDisposable
    {
        private readonly NotificationChannelCollection _observers;

        public UpdateHub() => _observers = new();

        public void Dispose()
        {
            foreach (var observerSet in _observers)
                using (observerSet.Value.Wait())
                {
                    foreach (var observer in observerSet.Value)
                        observer.OnCompleted();
                }

            _observers.Clear();
        }

        public IDisposable Subscribe(INotificationObserver observer, string channel = "none")
        {
            _observers.Add(channel, observer);
            return new NotificationSubscription(this, observer, channel);
        }

        public void Unsubscribe(INotificationObserver observer) => _observers.Remove(observer);

        public void Unsubscribe(INotificationObserver observer, string channel) => _observers.Remove(channel, observer);

        public void Push(Notification notification)
        {
            foreach (var observerSet in _observers)
                using (observerSet.Value.Wait())
                {
                    foreach (var observer in observerSet.Value)
                        observer.OnNext(notification);
                }
        }

        public void Push(Notification notification, string channel)
        {
            if (!_observers.TryGetValue(channel, out var observerSet)) 
                return;

            using (observerSet.Wait())
            {
                foreach (var observer in observerSet)
                    observer.OnNext(notification);
            }
        }
    }
}