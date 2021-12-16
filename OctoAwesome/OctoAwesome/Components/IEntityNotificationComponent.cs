using OctoAwesome.Notifications;

namespace OctoAwesome.Components
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEntityNotificationComponent : IEntityComponent, INotificationSubject<SerializableNotification> { }
}