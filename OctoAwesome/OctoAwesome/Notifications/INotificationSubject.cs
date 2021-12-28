namespace OctoAwesome.Notifications
{
    public interface INotificationSubject<TNotification> where TNotification : Notification
    {
        void OnNotification(TNotification notification);

        void Push(TNotification notification);
    }
}