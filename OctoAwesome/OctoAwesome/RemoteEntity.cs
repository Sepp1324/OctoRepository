using System.IO;

namespace OctoAwesome
{
    public class RemoteEntity : Entity
    {
        public int RemoteID { get; set; }

        public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
        {
            writer.Write(RemoteID);
            base.Serialize(writer, definitionManager);
        }

        public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        {
            RemoteID = reader.ReadInt32();
            base.Deserialize(reader, definitionManager);
        }
    }
}
