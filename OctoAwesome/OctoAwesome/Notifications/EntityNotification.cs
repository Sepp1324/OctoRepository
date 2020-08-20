using System.IO;

namespace OctoAwesome.Notifications
{
    public class EntityNotification : SerializableNotification
    {
        public ActionType Type { get; set; }
        public Entity Entity { get; set; }

        public SerializableNotification Notification { get; set; }

        public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager = null)
        {
            Type = (ActionType)reader.ReadInt32();
            Notification.Deserialize(reader);
            Entity.Deserialize(reader, definitionManager);
        }

        public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager = null)
        {
            writer.Write((int)Type);
            Notification.Serialize(writer);
            Entity.Serialize(writer, definitionManager);
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
