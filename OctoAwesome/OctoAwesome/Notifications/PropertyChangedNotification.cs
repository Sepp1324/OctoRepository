using System.IO;

namespace OctoAwesome.Notifications
{
    public class PropertyChangedNotification : SerializableNotification
    {
        public string Issuer { get; set; }
        public string Property { get; set; }

        public byte[] Value { get; set; }

        public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager = null)
        {
            Issuer = reader.ReadString();
            Property = reader.ReadString();
            var count = reader.ReadInt32();
            Value = reader.ReadBytes(count);
        }

        public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager = null)
        {
            writer.Write(Issuer);
            writer.Write(Property);
            writer.Write(Value.Length);
            writer.Write(Value);
        }
    }
}
