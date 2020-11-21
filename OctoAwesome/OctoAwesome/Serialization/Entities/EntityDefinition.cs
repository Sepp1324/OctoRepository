using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome.Serialization.Entities
{
    public sealed class EntityDefinition : ISerializable
    {
        /// <summary>
        /// Type des Entities
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Id des Entities
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Anzahl der Komponenten
        /// </summary>
        public int ComponentsCount { get; set; }

        /// <summary>
        /// Liste von Komponenten
        /// </summary>
        public IEnumerable<Type> Components { get; set; }

        public EntityDefinition()
        {

        }

        public EntityDefinition(Entity entity)
        {
            Type = entity.GetType();
            Id = entity.Id;
            var tmpComponents = entity.Components.ToList();
            ComponentsCount = tmpComponents.Count;
            Components = tmpComponents.Select(c => c.GetType());
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Type.AssemblyQualifiedName);
            writer.Write(Id);
            writer.Write(ComponentsCount);

            foreach (var component in Components)
                writer.Write(component.AssemblyQualifiedName);
        }

        public void Deserialize(BinaryReader reader)
        {
            Type = Type.GetType(reader.ReadString());
            Id = reader.ReadInt32();
            ComponentsCount = reader.ReadInt32();

            var list = new List<Type>();

            for (int i = 0; i < ComponentsCount; i++)
                list.Add(Type.GetType(reader.ReadString()));

            Components = list;
        }

        public sealed class EntityDefinitionContext : SerializableDatabaseContext<IdTag<EntityDefinition>, EntityDefinition>
        {
            public EntityDefinitionContext(Database<IdTag<EntityDefinition>> database) : base(database)
            {

            }

            public IEnumerable<IdTag<EntityDefinition>> GetAllKeys() => Database.Keys;

            public override void AddOrUpdate(EntityDefinition value) => InternalAddOrUpdate(new IdTag<EntityDefinition>(value.Id), value);

            public override void Remove(EntityDefinition value) => InternalRemove(new IdTag<EntityDefinition>(value.Id));
        }
    }
}
