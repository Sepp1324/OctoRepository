using OctoAwesome.Database;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Serialization.Entities
{
    public sealed class EntityDatabaseContext : DatabaseContext<IdTag, Entity>
    {
        public EntityDatabaseContext(Database<IdTag> database) : base(database)
        {
        }

        public override void AddOrUpdate(Entity value) => InternalAddOrUpdate(new IdTag(value.Id), value);

        public override Entity Get(IdTag key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IdTag> GetAllKeys() => Database.Keys;

        public override void Remove(Entity value) => InternalRemove(new IdTag(value.Id));
    }
}
