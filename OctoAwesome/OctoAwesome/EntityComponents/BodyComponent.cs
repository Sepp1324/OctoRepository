using System.IO;
using OctoAwesome.Components;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// </summary>
    public sealed class BodyComponent : Component, IEntityComponent, IFunctionalBlockComponent
    {
        /// <summary>
        /// </summary>
        public BodyComponent()
        {
            Mass = 1; //1kg
            Radius = 1;
            Height = 1;
        }

        /// <summary>
        /// </summary>
        public float Mass { get; set; }

        /// <summary>
        ///     Der Radius des Spielers in Blocks.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        ///     Die Körperhöhe des Spielers in Blocks
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// </summary>
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(Mass);
            writer.Write(Radius);
            writer.Write(Height);
        }

        /// <summary>
        /// </summary>
        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            Mass = reader.ReadSingle();
            Radius = reader.ReadSingle();
            Height = reader.ReadSingle();
        }
    }
}