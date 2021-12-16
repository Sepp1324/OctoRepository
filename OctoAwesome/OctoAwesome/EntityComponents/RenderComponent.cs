using System.IO;
using OctoAwesome.Components;

namespace OctoAwesome.EntityComponents
{
    public class RenderComponent : Component, IEntityComponent, IFunctionalBlockComponent
    {
        public RenderComponent()
        {
            Sendable = true;
        }

        public string Name { get; set; }
        public string ModelName { get; set; }
        public string TextureName { get; set; }

        public float BaseZRotation { get; set; }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(ModelName);
            writer.Write(TextureName);
            writer.Write(BaseZRotation);
            base.Serialize(writer);
        }

        public override void Deserialize(BinaryReader reader)
        {
            Name = reader.ReadString();
            ModelName = reader.ReadString();
            TextureName = reader.ReadString();
            BaseZRotation = reader.ReadSingle();
            base.Deserialize(reader);
        }
    }
}