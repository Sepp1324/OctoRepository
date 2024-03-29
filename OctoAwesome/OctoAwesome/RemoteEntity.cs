﻿using System.IO;

namespace OctoAwesome
{
    public class RemoteEntity : Entity
    {
        public RemoteEntity()
        {
        }

        public RemoteEntity(Entity originEntity)
        {
            foreach (var component in Components)
                if (component.Sendable)
                    Components.AddComponent(component);
            Id = originEntity.Id;
        }

        public override void Serialize(BinaryWriter writer)
        {
            Components.Serialize(writer);
            base.Serialize(writer);
        }

        public override void Deserialize(BinaryReader reader)
        {
            Components.Deserialize(reader);
            base.Deserialize(reader);
        }
    }
}