using System;

namespace OctoAwesome.Notifications
{
    public interface INotificationObserver
    {
        void OnCompleted();

        void OnError(Exception error);
        
        void OnNext(Notification value);
    }
}
