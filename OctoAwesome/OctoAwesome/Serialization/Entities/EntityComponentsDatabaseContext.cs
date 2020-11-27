using OctoAwesome.Database;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Serialization.Entities
{
    public sealed class EntityComponentsDatabaseContext
    {
        private readonly IDatabaseProvider _databaseProvider;
        private readonly Guid _universeGuid;

        public EntityComponentsDatabaseContext(IDatabaseProvider databaseProvider, Guid universe)
        {
            _databaseProvider = databaseProvider;
            _universeGuid = universe;
        }

        public void AddOrUpdate<T>(T value, Entity entity) where T : EntityComponent
        {
            var database = _databaseProvider.GetDatabase<GuidTag<T>>(_universeGuid, false);
            var tag = new GuidTag<T>(entity.Id);
            database.AddOrUpdate(tag, new Value(Serializer.Serialize(value)));
        }

        public T Get<T>(Guid id) where T : EntityComponent, new()
        {
            var database = _databaseProvider.GetDatabase<GuidTag<T>>(_universeGuid, false);
            var tag = new GuidTag<T>(id);
            return Serializer.Deserialize<T>(database.GetValue(tag).Content);
        }

        public T Get<T>(Entity entity) where T : EntityComponent, new()=> Get<T>(entity.Id);

        public IEnumerable<GuidTag<T>> GetAllKeys<T>() where T : EntityComponent=> _databaseProvider.GetDatabase<GuidTag<T>>(_universeGuid, false).Keys;

        public void Remove<T>(Entity entity) where T : EntityComponent
        {
            var database = _databaseProvider.GetDatabase<GuidTag<T>>(_universeGuid, false);
            var tag = new GuidTag<T>(entity.Id);
            database.Remove(tag);
        }
    }
}
