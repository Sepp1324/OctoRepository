using System.IO;

namespace OctoAwesome.Notifications
{
    public class EntityNotification : SerializableNotification
    {
        private Entity entity;

        public ActionType Type { get; set; }

        public Entity Entity
        {
            get => entity; set
            {
                entity = value;
                EntityId = value.Id;
            }
        }

        public SerializableNotification Notification { get; set; }

        public int EntityId { get; private set; }

        public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager = null)
        {
            Type = (ActionType)reader.ReadInt32();
            Notification.Deserialize(reader);

            if (Type == ActionType.Add)
                Entity.Deserialize(reader, definitionManager);
            else
                EntityId = reader.ReadInt32();
        }

        public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager = null)
        {
            writer.Write((int)Type);
            Notification.Serialize(writer);

            if (Type == ActionType.Add)
                Entity.Serialize(writer, definitionManager);
            else
                writer.Write(Entity.Id);
        }

        public enum ActionType
        {
            None,
            Add,
            Remove,
            Update
        }
    }
}
