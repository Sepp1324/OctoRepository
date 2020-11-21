using OctoAwesome.Database;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Serialization.Entities
{
    public sealed class ComponentsDatabaseContext
    {
        private readonly IDatabaseProvider _databaseProvider;

        public ComponentsDatabaseContext(IDatabaseProvider databaseProvider) => _databaseProvider = databaseProvider;

        /// <summary>
        /// Fügt eine Kompenente hinzu oder updated sie
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="entity"></param>
        /// <param name="universeGuid"></param>
        /// <param name="planetId"></param>
        public void AddOrUpdate<T>(T value, Entity entity, Guid universeGuid, int planetId) where T : Component
        {
            var database = _databaseProvider.GetDatabase<ComponentTag<T>>(universeGuid, planetId);
            var tag = new ComponentTag<T>(entity.Id);
            database.AddOrUpdate(tag, new Value(Serializer.Serialize(value)));
        }

        /// <summary>
        /// Liefert eine Komponente zurück
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="universeGuid"></param>
        /// <param name="planetId"></param>
        /// <returns></returns>
        public Component Get<T>(Entity entity, Guid universeGuid, int planetId) where T : Component, new()
        {
            var database = _databaseProvider.GetDatabase<ComponentTag<T>>(universeGuid, planetId);
            var tag = new ComponentTag<T>(entity.Id);
            return Serializer.Deserialize<T>(database.GetValue(tag).Content);
        }

        /// <summary>
        /// Entfernt eine Komponente
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public void Remove<T>(Entity entity, Guid universeGuid, int planetId) where T : Component
        {
            var database = _databaseProvider.GetDatabase<ComponentTag<T>>(universeGuid, planetId);
            var tag = new ComponentTag<T>(entity.Id);
            database.Remove(tag);
        }

        private struct ComponentTag<T> : ITag, IEquatable<ComponentTag<T>>
        {
            public int Length => IdTag.Length;

            public IdTag IdTag { get; }

            public ComponentTag(int id) => IdTag = new IdTag(id);

            public void FromBytes(byte[] array, int startIndex) => IdTag.FromBytes(array, startIndex);

            public byte[] GetBytes() => IdTag.GetBytes();

            public override bool Equals(object obj) => obj is ComponentTag<T> tag && Equals(tag);

            public bool Equals(ComponentTag<T> other) => Length == other.Length && EqualityComparer<IdTag>.Default.Equals(IdTag, other.IdTag);

            public override int GetHashCode()
            {
                int hashCode = -1449734275;
                hashCode = hashCode * -1521134295 + Length.GetHashCode();
                hashCode = hashCode * -1521134295 + IdTag.GetHashCode();
                return hashCode;
            }

            public static bool operator ==(ComponentTag<T> left, ComponentTag<T> right) => left.Equals(right);

            public static bool operator !=(ComponentTag<T> left, ComponentTag<T> right) => !(left == right);
        }
    }
}
