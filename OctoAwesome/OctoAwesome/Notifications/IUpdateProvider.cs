using System;

namespace OctoAwesome.Notifications
{
    public interface IUpdateProvider : IObservable<Notification>
    {
        void Unsubscribe(IObserver<Notification> subscriber);
    }
}
