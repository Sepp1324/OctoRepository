using System;

namespace OctoAwesome.Notifications
{
    public sealed class NotificationSubscription : IDisposable
    {
        private IUpdateProvider updateProvider;
        private IUpdateSubscribe subscriber;

        public NotificationSubscription(IUpdateProvider updateProvider, IUpdateSubscribe subscriber)
        {
            this.subscriber = subscriber;
            this.updateProvider = updateProvider; 
        }

        public void Dispose()
        {
            updateProvider.Unsubscribe(subscriber);

            updateProvider = null;
            subscriber = null;
        }
    }
}
