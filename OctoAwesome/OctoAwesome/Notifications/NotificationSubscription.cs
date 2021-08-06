using System;

namespace OctoAwesome.Notifications
{
    public sealed class NotificationSubscription : IDisposable
    {
        private readonly string channel;
        private INotificationObservable observable;
        private INotificationObserver observer;

        public NotificationSubscription(INotificationObservable observable, INotificationObserver observer,
            string channel)
        {
            this.observer = observer;
            this.observable = observable;
            this.channel = channel;
        }

        public void Dispose()
        {
            observer?.OnCompleted();
            observable?.Unsubscribe(observer, channel);
            observable = null;
            observer = null;
        }
    }
}