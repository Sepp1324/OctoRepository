namespace OctoAwesome.Notifications
{
    public interface IUpdateHub : INotificationObservable
    {
        void Push(Notification notification, string channel);
        void Push(Notification notification);
    }
}