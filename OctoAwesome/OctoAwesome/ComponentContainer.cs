using System;
using System.Collections.Generic;
using System.IO;
using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;

namespace OctoAwesome
{
    public abstract class ComponentContainer<TComponent> : ISerializable, IIdentification, IContainsComponents,
        INotificationSubject<SerializableNotification> where TComponent : IComponent
    {
        /// <summary>
        ///     Contains only Components with notification interface implementation.
        /// </summary>
        private readonly List<INotificationSubject<SerializableNotification>> notificationComponents;

        /// <summary>
        ///     Entity die regelmäßig eine Updateevent bekommt
        /// </summary>
        public ComponentContainer()
        {
            Components = new ComponentList<TComponent>(ValidateAddComponent, ValidateRemoveComponent, OnAddComponent,
                OnRemoveComponent);
            notificationComponents = new List<INotificationSubject<SerializableNotification>>();
            Id = Guid.Empty;
        }

        /// <summary>
        ///     Contains all Components.
        /// </summary>
        public ComponentList<TComponent> Components { get; }

        /// <summary>
        ///     Reference to the active Simulation.
        /// </summary>
        public Simulation Simulation { get; internal set; }

        public bool ContainsComponent<T>()
        {
            return Components.ContainsComponent<T>();
        }

        public T GetComponent<T>()
        {
            return Components.GetComponent<T>();
        }

        /// <summary>
        ///     Id
        /// </summary>
        public Guid Id { get; internal set; }

        public virtual void OnNotification(SerializableNotification notification)
        {
        }

        public virtual void Push(SerializableNotification notification)
        {
            foreach (var component in notificationComponents)
                component?.OnNotification(notification);
        }

        /// <summary>
        ///     Serialisiert die Entität mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(Id.ToByteArray());

            Components.Serialize(writer);
        }

        /// <summary>
        ///     Deserialisiert die Entität aus dem angegebenen BinaryReader.
        /// </summary>
        /// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
        public virtual void Deserialize(BinaryReader reader)
        {
            Id = new Guid(reader.ReadBytes(16));
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

            if (component is INotificationSubject<SerializableNotification> nofiticationComponent)
                notificationComponents.Add(nofiticationComponent);
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

        public void Initialize(IResourceManager mananger)
        {
            OnInitialize(mananger);
        }

        protected virtual void OnInitialize(IResourceManager manager)
        {
        }

        public virtual void RegisterDefault()
        {
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Entity entity)
                return entity.Id == Id;

            return base.Equals(obj);
        }
    }
}