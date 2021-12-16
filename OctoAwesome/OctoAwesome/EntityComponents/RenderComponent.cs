using System.IO;
using OctoAwesome.Components;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// 
    /// </summary>
    public class RenderComponent : Component, IEntityComponent, IFunctionalBlockComponent
    {
        /// <summary>
        /// 
        /// </summary>
        public RenderComponent() => Sendable = true;

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TextureName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float BaseZRotation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(ModelName);
            writer.Write(TextureName);
            writer.Write(BaseZRotation);
            base.Serialize(writer);
        }

        /// <summary>
        /// 
        /// </summary>
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