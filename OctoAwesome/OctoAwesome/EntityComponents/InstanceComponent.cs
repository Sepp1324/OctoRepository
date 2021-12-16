﻿using System;
using OctoAwesome.Notifications;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    ///     Base Class for all Entity Components.
    /// </summary>
    public abstract class InstanceComponent<T> : Component, INotificationSubject<SerializableNotification>
        where T : INotificationSubject<SerializableNotification>
    {
        /// <summary>
        ///     Reference to the Entity.
        /// </summary>
        public T Instance { get; private set; }

        public virtual void OnNotification(SerializableNotification notification)
        {
        }

        public virtual void Push(SerializableNotification notification)
        {
            Instance?.OnNotification(notification);
        }

        public void SetInstance(T instance)
        {
            if (Instance != null)
                throw new NotSupportedException("Can not change the " + typeof(T).Name);

            Instance = instance;
            OnSetInstance();
        }


        protected virtual void OnSetInstance()
        {
        }
    }
}