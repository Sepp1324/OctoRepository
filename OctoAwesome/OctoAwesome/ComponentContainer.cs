using System;
using System.Collections.Generic;
using System.IO;
using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;

namespace OctoAwesome
{
    /// <summary>
    ///     Container for Components
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    public abstract class ComponentContainer<TComponent> : ISerializable, IIdentification, IContainsComponents,
        INotificationSubject<SerializableNotification> where TComponent : IComponent
    {
        /// <summary>
        ///     Contains Components with an Implementation of <see cref="INotificationSubject{TNotification}" />
        /// </summary>
        private readonly List<INotificationSubject<SerializableNotification>> _notificationComponents;

        /// <summary>
        ///     Entities with periodic Update-Events
        /// </summary>
        protected ComponentContainer()
        {
            Components = new(ValidateAddComponent, ValidateRemoveComponent, OnAddComponent, OnRemoveComponent);
            _notificationComponents = new();
            Id = Guid.Empty;
        }

        /// <summary>
        ///     Contains all Components
        /// </summary>
        public ComponentList<TComponent> Components { get; }

        /// <summary>
        ///     Reference to the active Simulation
        /// </summary>
        public Simulation Simulation { get; internal set; }

        public bool ContainsComponent<T>() => Components.ContainsComponent<T>();

        public T GetComponent<T>() => Components.GetComponent<T>();

        /// <summary>
        ///     Id
        /// </summary>
        public Guid Id { get; internal set; }

        public virtual void OnNotification(SerializableNotification notification)
        {
        }

        public virtual void Push(SerializableNotification notification)
        {
            foreach (var component in _notificationComponents)
                component?.OnNotification(notification);
        }

        /// <summary>
        ///     Serializes an Entity with the given <see cref="BinaryWriter" />
        /// </summary>
        /// <param name="writer">Given <see cref="BinaryWriter" /></param>
        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(Id.ToByteArray());

            Components.Serialize(writer);
        }

        /// <summary>
        ///     Deserializes an Entity with the given <see cref="BinaryReader" />
        /// </summary>
        /// <param name="reader">Given <see cref="BinaryReader" /></param>
        public virtual void Deserialize(BinaryReader reader)
        {
            Id = new(reader.ReadBytes(16));
            Components.Deserialize(reader);
        }

        protected void OnRemoveComponent(TComponent component)
        {
        }

        protected virtual void OnAddComponent(TComponent component)
        {
            if (component is InstanceComponent<INotificationSubject<SerializableNotification>> instanceComponent)
                instanceComponent.SetInstance(this);

            //HACK: Remove PositionComponent Dependency
            if (component is LocalChunkCacheComponent cacheComponent)
            {
                if (cacheComponent.LocalChunkCache != null)
                    return;

                var positionComponent = Components.GetComponent<PositionComponent>();

                if (positionComponent == null)
                    return;

                cacheComponent.LocalChunkCache = new LocalChunkCache(positionComponent.Planet.GlobalChunkCache, 4, 2);
            }

            if (component is INotificationSubject<SerializableNotification> notificationComponent)
                _notificationComponents.Add(notificationComponent);
        }

        protected virtual void ValidateAddComponent(TComponent component)
        {
            if (Simulation is not null)
                throw new NotSupportedException("Can't add components during simulation");
        }

        protected virtual void ValidateRemoveComponent(TComponent component)
        {
            if (Simulation is not null)
                throw new NotSupportedException("Can't remove components during simulation");
        }

        public void Initialize(IResourceManager manager)
        {
            OnInitialize(manager);
        }

        protected virtual void OnInitialize(IResourceManager manager)
        {
        }

        public virtual void RegisterDefault()
        {
        }

        public override int GetHashCode() => Id.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is Entity entity)
                return entity.Id == Id;

            return base.Equals(obj);
        }
    }
}