﻿using OctoAwesome.Notifications;
using System;

namespace OctoAwesome
{
    /// <summary>
    /// Base Class for all Entity Components.
    /// </summary>
    public abstract class EntityComponent : Component
    {
        /// <summary>
        /// Reference to the Entity.
        /// </summary>
        public Entity Entity { get; private set; }

        public EntityComponent()
        {
        }

        public void SetEntity(Entity entity)
        {
            if (Entity != null)
                throw new NotSupportedException("Can not change the Entity");

            Entity = entity;
            OnSetEntity();
        }

        public virtual void OnUpdate(SerializableNotification notification)
        {

        }

        protected virtual void OnSetEntity()
        {
<<<<<<< HEAD
=======

        }
>>>>>>> feature/performance

        }
<<<<<<< HEAD

        protected virtual void Update(SerializableNotification notification) => Entity?.OnUpdate(notification);
=======
        
>>>>>>> feature/performance
    }
}
