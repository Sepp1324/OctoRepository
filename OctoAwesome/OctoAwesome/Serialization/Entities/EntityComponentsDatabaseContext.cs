using OctoAwesome.Database;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Serialization.Entities
{
    public sealed class EntityComponentsDatabaseContext
    {
        private readonly IDatabaseProvider _databaseProvider;
        private readonly Guid _universeGuid;

        public EntityComponentsDatabaseContext(IDatabaseProvider databaseProvider, Guid universeGuid)
        {
            _databaseProvider = databaseProvider;
            _universeGuid = universeGuid;
        }

        /// <summary>
        /// Fügt eine Kompenente hinzu oder updated sie
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="entity"></param>
        /// <param name="universeGuid"></param>
        /// <param name="planetId"></param>
        public void AddOrUpdate<T>(T value, Entity entity) where T : EntityComponent
        {
            var database = _databaseProvider.GetDatabase<GuidTag<T>>(_universeGuid);
            var tag = new GuidTag<T>(entity.Id);
            database.AddOrUpdate(tag, new Value(Serializer.Serialize(value)));
        }

        /// <summary>
        /// Liefert eine Komponente zurück
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public T Get<T>(Guid entityId) where T : EntityComponent, new()
        {
            var database = _databaseProvider.GetDatabase<GuidTag<T>>(_universeGuid);
            var tag = new GuidTag<T>(entityId);
            return Serializer.Deserialize<T>(database.GetValue(tag).Content);
        }

        /// <summary>
        /// Liefert eine Komponente zurück
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Get<T>(Entity entity) where T : EntityComponent, new() => Get<T>(entity.Id);

        public IEnumerable<GuidTag<T>> GetAllKeys<T>() where T : EntityComponent => _databaseProvider.GetDatabase<GuidTag<T>>(_universeGuid).Keys;

        /// <summary>
        /// Entfernt eine Komponente
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="universeGuid"></param>
        public void Remove<T>(Entity entity) where T : EntityComponent
        {
            var database = _databaseProvider.GetDatabase<GuidTag<T>>(_universeGuid);
            var tag = new GuidTag<T>(entity.Id);
            database.Remove(tag);
        }
    }
}
