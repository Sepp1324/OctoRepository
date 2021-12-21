using System;
using OctoAwesome.Notifications;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    ///     Base Class for all Entity Components.
    /// </summary>
    public abstract class InstanceComponent<T> : Component, INotificationSubject<SerializableNotification> where T : INotificationSubject<SerializableNotification>
    {
        /// <summary>
        ///     Reference to the Entity.
        /// </summary>
        public T Instance { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notification"></param>
        public virtual void OnNotification(SerializableNotification notification) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notification"></param>
        public virtual void Push(SerializableNotification notification) => Instance?.OnNotification(notification);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <exception cref="NotSupportedException"></exception>
        public void SetInstance(T instance)
        {
            if (Instance != null)
                throw new NotSupportedException("Can not change the " + typeof(T).Name);

            Instance = instance;
            OnSetInstance();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnSetInstance() { }
    }
}