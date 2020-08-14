namespace OctoAwesome.Notifications
{
    public class EntityNotification : Notification
    {
        public ActionType Type { get; set; }

        public Entity Entity { get; set; }

        public enum ActionType
        {
            None, Add, Remove
        }
    }
}
