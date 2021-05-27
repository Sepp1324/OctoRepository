using OctoAwesome.Database;
using System;

namespace OctoAwesome.Serialization
{
<<<<<<< HEAD
    public sealed class IdDatabaseContext<TObject> : SerializableDatabaseContext<GuidTag<int>, TObject> where TObject : ISerializable, IIdentification, new()
=======
    public sealed class IdDatabaseContext<TObject> : SerializableDatabaseContext<GuidTag<int>, TObject> 
        where TObject : ISerializable, IIdentification, new()
>>>>>>> feature/performance
    {
        public IdDatabaseContext(Database<GuidTag<int>> database) : base(database) { }

<<<<<<< HEAD
        public override void AddOrUpdate(TObject value) => InternalAddOrUpdate(new GuidTag<int>(value.Id), value);

        public TObject Get(Guid key) => Get(new GuidTag<int>(key));

        public override void Remove(TObject value) => InternalRemove(new GuidTag<int>(value.Id));
=======
        public override void AddOrUpdate(TObject value) 
            => InternalAddOrUpdate(new GuidTag<int>(value.Id), value);

        public TObject Get(Guid key) 
            => Get(new GuidTag<int>(key));

        public override void Remove(TObject value) 
           => InternalRemove(new GuidTag<int>(value.Id));
>>>>>>> feature/performance
    }
}
