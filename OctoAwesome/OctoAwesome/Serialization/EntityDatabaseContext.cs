using OctoAwesome.Database;
using System.Collections.Generic;

namespace OctoAwesome.Serialization
{
    public sealed class EntityDatabaseContext : SerializableDatabaseContext<IdTag, Entity>
    {
        public EntityDatabaseContext(Database<IdTag> database) : base(database)
        {
        }

        public override void AddOrUpdate(Entity value)
            => InternalAddOrUpdate(new IdTag(value.Id), value);

        public IEnumerable<IdTag> GetAllKeys() => Database.Keys;

        public override void Remove(Entity value)
            => InternalRemove(new IdTag(value.Id));
    }
}
