using System;
using System.IO;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;

namespace OctoAwesome.Notifications
{
    public sealed class EntityNotification : SerializableNotification
    {
        public enum ActionType
        {
            None,
            Add,
            Remove,
            Update,
            Request
        }

        private readonly IPool<PropertyChangedNotification> _propertyChangedNotificationPool;

        private Entity _entity;

        public EntityNotification() => _propertyChangedNotificationPool = TypeContainer.Get<IPool<PropertyChangedNotification>>();

        public EntityNotification(Guid id) : this() => EntityId = id;

        public ActionType Type { get; set; }
        public Guid EntityId { get; set; }

        public Entity Entity
        {
            get => _entity;
            set
            {
                _entity = value;
                EntityId = value?.Id ?? default;
            }
        }

        public PropertyChangedNotification Notification { get; set; }

        public override void Deserialize(BinaryReader reader)
        {
            Type = (ActionType)reader.ReadInt32();


            if (Type == ActionType.Add)
                Entity = Serializer.Deserialize<RemoteEntity>(reader.ReadBytes(reader.ReadInt32()));
            else
                EntityId = new(reader.ReadBytes(16));

            var isNotification = reader.ReadBoolean();

            if (isNotification)
                Notification = Serializer.DeserializePoolElement(_propertyChangedNotificationPool, reader.ReadBytes(reader.ReadInt32()));
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write((int)Type);

            if (Type == ActionType.Add)
            {
                var bytes = Serializer.Serialize(Entity);
                writer.Write(bytes.Length);
                writer.Write(bytes);
            }
            else
            {
                writer.Write(EntityId.ToByteArray());
            }

            var subNotification = Notification != null;
            writer.Write(subNotification);

            if (subNotification)
            {
                var bytes = Serializer.Serialize(Notification);
                writer.Write(bytes.Length);
                writer.Write(bytes);
            }
        }

        protected override void OnRelease()
        {
            Notification?.Release();

            Type = default;
            Entity = default;
            Notification = default;

            base.OnRelease();
        }
    }
}