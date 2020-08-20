using OctoAwesome.Serialization;
using System.IO;

namespace OctoAwesome.Notifications
{
    public abstract class SerializableNotification : Notification, ISerializable
    {
        public abstract void Deserialize(BinaryReader reader, IDefinitionManager definitionManager = null);

        public abstract void Serialize(BinaryWriter writer, IDefinitionManager definitionManager = null);
    }
}
