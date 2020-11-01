using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Serialization
{
    public sealed class IdDatabaseContext<TObject> : SerializableDatabaseContext<IdTag, TObject> where TObject : ISerializable, IIdentification, new()
    {
        public IdDatabaseContext(Database<IdTag> database) : base(database)
        {

        }

        public override void AddOrUpdate(TObject value) => InternalAddOrUpdate(new IdTag(value.Id), value);

        public TObject Get(int key) => Get(new IdTag(key));

        public override void Remove(TObject value) => InternalRemove(new IdTag(value.Id));
    }
}
