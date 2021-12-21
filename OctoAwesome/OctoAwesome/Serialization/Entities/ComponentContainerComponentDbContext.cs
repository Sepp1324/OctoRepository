using System;
using System.Collections.Generic;
using OctoAwesome.Components;
using OctoAwesome.Database;

namespace OctoAwesome.Serialization.Entities
{
    public sealed class ComponentContainerComponentDbContext<TContainer> where TContainer : IComponent
    {
        private readonly IDatabaseProvider _databaseProvider;
        private readonly Guid _universeGuid;

        public ComponentContainerComponentDbContext(IDatabaseProvider databaseProvider, Guid universe)
        {
            _databaseProvider = databaseProvider;
            _universeGuid = universe;
        }

        public void AddOrUpdate<T>(T value, ComponentContainer<TContainer> entity) where T : IComponent
        {
            var database = _databaseProvider.GetDatabase<GuidTag<T>>(_universeGuid, false);
            var tag = new GuidTag<T>(entity.Id);

            using (database.Lock(Operation.Write))
            {
                database.AddOrUpdate(tag, new(Serializer.Serialize(value)));
            }
        }

        public T Get<T>(Guid id) where T : IComponent, new()
        {
            var database = _databaseProvider.GetDatabase<GuidTag<T>>(_universeGuid, false);
            var tag = new GuidTag<T>(id);
            return Serializer.Deserialize<T>(database.GetValue(tag).Content);
        }

        public T Get<T>(ComponentContainer<TContainer> entity) where T : IComponent, new() => Get<T>(entity.Id);

        public IEnumerable<GuidTag<T>> GetAllKeys<T>() where T : IComponent => _databaseProvider.GetDatabase<GuidTag<T>>(_universeGuid, false).Keys;

        public void Remove<T>(ComponentContainer<TContainer> entity) where T : IComponent
        {
            var database = _databaseProvider.GetDatabase<GuidTag<T>>(_universeGuid, false);
            var tag = new GuidTag<T>(entity.Id);

            using (database.Lock(Operation.Write))
            {
                database.Remove(tag);
            }
        }
    }
}