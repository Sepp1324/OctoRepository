using System;

namespace OctoAwesome.Notifications
{
    public abstract class Notification
    {
        public virtual bool Match<T>(T filter)
        {
            return true;
        }
    }
}
