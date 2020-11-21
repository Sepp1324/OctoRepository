﻿using OctoAwesome.Pooling;
using OctoAwesome.Serialization;
using System.IO;

namespace OctoAwesome.Notifications
{
    public sealed class EntityNotification : SerializableNotification
    {
        public ActionType Type { get; set; }

        public int EntityId { get; set; }

        public Entity Entity
        {
            get => _entity; set
            {
                _entity = value;
                EntityId = value?.Id ?? default;
            }
        }

        public PropertyChangedNotification Notification { get; set; }

        private Entity _entity;
        private readonly IPool<PropertyChangedNotification> _propertyChangedNotificationPool;

        public EntityNotification() => _propertyChangedNotificationPool = TypeContainer.Get<IPool<PropertyChangedNotification>>();

        public EntityNotification(int id) : this() => EntityId = id;

        public override void Deserialize(BinaryReader reader)
        {
            Type = (ActionType)reader.ReadInt32();


            if (Type == ActionType.Add)
                Entity = Serializer.Deserialize<RemoteEntity>(reader.ReadBytes(reader.ReadInt32()));
            else
                EntityId = reader.ReadInt32();

            var isNotification = reader.ReadBoolean();
            if (isNotification)
                Notification = Serializer.DeserializePoolElement(
                    _propertyChangedNotificationPool, reader.ReadBytes(reader.ReadInt32()));
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
                writer.Write(EntityId);
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

        public enum ActionType
        {
            None,
            Add,
            Remove,
            Update,
            Request
        }
    }
}
