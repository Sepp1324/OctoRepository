using OctoAwesome.Database;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Serialization.Entities
{
    public sealed class EntityComponentsDatabaseContext
    {
        private readonly IDatabaseProvider databaseProvider;
        private readonly Guid universeGuid;

        public EntityComponentsDatabaseContext(IDatabaseProvider databaseProvider, Guid universe)
        {
            this.databaseProvider = databaseProvider;
            universeGuid = universe;
        }

        public void AddOrUpdate<T>(T value, Entity entity) where T : EntityComponent
        {
            var database = databaseProvider.GetDatabase<GuidTag<T>>(universeGuid, false);
            var tag = new GuidTag<T>(entity.Id);
            database.AddOrUpdate(tag, new Value(Serializer.Serialize(value)));
        }

        public T Get<T>(Guid id) where T : EntityComponent, new()
        {
            var database = databaseProvider.GetDatabase<GuidTag<T>>(universeGuid, false);
            var tag = new GuidTag<T>(id);
            return Serializer.Deserialize<T>(database.GetValue(tag).Content);
        }

        public T Get<T>(Entity entity) where T : EntityComponent, new()=> Get<T>(entity.Id);

        public IEnumerable<GuidTag<T>> GetAllKeys<T>() where T : EntityComponent=> databaseProvider.GetDatabase<GuidTag<T>>(universeGuid, false).Keys;

        public void Remove<T>(Entity entity) where T : EntityComponent
        {
            var database = databaseProvider.GetDatabase<GuidTag<T>>(universeGuid, false);
            var tag = new GuidTag<T>(entity.Id);
            database.Remove(tag);
        }
    }
}
