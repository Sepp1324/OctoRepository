using System;
using System.IO;
using System.Linq;

namespace OctoAwesome
{
    public class RemoteEntity : Entity
    {
        public int RemoteID { get; set; }

        public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
        {
            writer.Write(RemoteID);

            var sendableComponents = Components.Where(component => component.Sendable);
            writer.Write(sendableComponents.Count());

            foreach (var component in sendableComponents)
            {
                writer.Write(component.GetType().FullName);
                component.Serialize(writer, definitionManager);
            }
            base.Serialize(writer, definitionManager);
        }

        public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        {
            RemoteID = reader.ReadInt32();

            var count = reader.ReadInt32();

            for(int i = 0; i < count; i++)
            {
                var name = reader.ReadString();
                var component = (Component)Activator.CreateInstance(Type.GetType(name, false));
                component.Deserialize(reader, definitionManager);
            }

            base.Deserialize(reader, definitionManager);
        }
    }
}
