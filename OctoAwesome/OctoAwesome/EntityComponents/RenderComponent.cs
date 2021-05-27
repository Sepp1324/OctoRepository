﻿using System.IO;

namespace OctoAwesome.EntityComponents
{
    public class RenderComponent : EntityComponent
    {
        public string Name { get; set; }
        
        public string ModelName { get; set; }
        
        public string TextureName { get; set; }

        public float BaseZRotation { get; set; }

<<<<<<< HEAD
        public RenderComponent() => Sendable = true;
=======
        public RenderComponent()
        {
            Sendable = true;
        }
>>>>>>> feature/performance

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
