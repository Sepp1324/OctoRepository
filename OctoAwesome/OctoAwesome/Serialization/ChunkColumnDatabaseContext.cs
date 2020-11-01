using OctoAwesome.Database;
using System.IO;

namespace OctoAwesome.Serialization
{
    public sealed class ChunkColumnDatabaseContext : DatabaseContext<Index2Tag, ChunkColumn>
    {
        private readonly IPlanet _currentPlanet;

        public ChunkColumnDatabaseContext(Database<Index2Tag> database, IPlanet planet) : base(database)
        {
            _currentPlanet = planet;
        }

        public override void AddOrUpdate(ChunkColumn value) => Database.AddOrUpdate(new Index2Tag(value.Index), new Value(Serializer.Serialize(value)));

        public ChunkColumn Get(Index2 key) => Get(new Index2Tag(key));

        public override ChunkColumn Get(Index2Tag key)
        {
            var chunkColumn = new ChunkColumn(_currentPlanet);

            using(var memoryStream = new MemoryStream(Database.GetValue(key).Content))
            using(var binaryReader = new BinaryReader(memoryStream))
            {
                chunkColumn.Deserialize(binaryReader);
                return chunkColumn;
            }
        }

        public override void Remove(ChunkColumn value) => Database.Remove(new Index2Tag(value.Index));
    }
}

