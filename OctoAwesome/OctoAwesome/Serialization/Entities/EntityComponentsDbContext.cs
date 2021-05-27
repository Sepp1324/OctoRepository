﻿using OctoAwesome.Database;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Serialization.Entities
{
    public sealed class EntityComponentsDbContext
    {
        private readonly IDatabaseProvider _databaseProvider;
        private readonly Guid _universeGuid;

        public EntityComponentsDbContext(IDatabaseProvider databaseProvider, Guid universe)
        {
            _databaseProvider = databaseProvider;
            _universeGuid = universe;
        }

        public void AddOrUpdate<T>(T value, Entity entity) where T : EntityComponent
        {
<<<<<<< HEAD
            var database = _databaseProvider.GetDatabase<GuidTag<T>>(_universeGuid, false);
=======
            Database<GuidTag<T>> database = databaseProvider.GetDatabase<GuidTag<T>>(universeGuid, false);
>>>>>>> feature/performance
            var tag = new GuidTag<T>(entity.Id);
            
            using (database.Lock(Operation.Write))
                database.AddOrUpdate(tag, new Value(Serializer.Serialize(value)));
        }

        public T Get<T>(Guid id) where T : EntityComponent, new()
        {
<<<<<<< HEAD
            var database = _databaseProvider.GetDatabase<GuidTag<T>>(_universeGuid, false);
=======
            Database<GuidTag<T>> database = databaseProvider.GetDatabase<GuidTag<T>>(universeGuid, false);
>>>>>>> feature/performance
            var tag = new GuidTag<T>(id);
            
            return Serializer.Deserialize<T>(database.GetValue(tag).Content);
        }
<<<<<<< HEAD
        public T Get<T>(Entity entity) where T : EntityComponent, new() => Get<T>(entity.Id);

        public IEnumerable<GuidTag<T>> GetAllKeys<T>() where T : EntityComponent => _databaseProvider.GetDatabase<GuidTag<T>>(_universeGuid, false).Keys;

        public void Remove<T>(Entity entity) where T : EntityComponent
        {
            var database = _databaseProvider.GetDatabase<GuidTag<T>>(_universeGuid, false);
=======
        public T Get<T>(Entity entity) where T : EntityComponent, new()
            => Get<T>(entity.Id);

        public IEnumerable<GuidTag<T>> GetAllKeys<T>() where T : EntityComponent
            => databaseProvider.GetDatabase<GuidTag<T>>(universeGuid, false).Keys;

        public void Remove<T>(Entity entity) where T : EntityComponent
        {
            Database<GuidTag<T>> database = databaseProvider.GetDatabase<GuidTag<T>>(universeGuid, false);
>>>>>>> feature/performance
            var tag = new GuidTag<T>(entity.Id);
            
            using (database.Lock(Operation.Write))
                database.Remove(tag);
        }

    }
}
