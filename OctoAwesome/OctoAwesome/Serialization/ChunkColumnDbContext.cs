using System.IO;
using System.IO.Compression;
using OctoAwesome.Database;

namespace OctoAwesome.Serialization
{
    public sealed class ChunkColumnDbContext : DatabaseContext<Index2Tag, IChunkColumn>
    {
        private readonly IPlanet _currentPlanet;

        public ChunkColumnDbContext(Database<Index2Tag> database, IPlanet planet) : base(database)
        {
            _currentPlanet = planet;
        }

        public override void AddOrUpdate(IChunkColumn value)
        {
            using (Database.Lock(Operation.Write))
            {
                Database.AddOrUpdate(new Index2Tag(value.Index), new Value(Serializer.SerializeCompressed(value)));
            }
        }

        public IChunkColumn Get(Index2 key)
        {
            return Get(new Index2Tag(key));
        }

        public override IChunkColumn Get(Index2Tag key)
        {
            if (!Database.ContainsKey(key))
                return null;

            var chunkColumn = new ChunkColumn(_currentPlanet);
            using (var stream = new MemoryStream(Database.GetValue(key).Content))
            using (var zip = new GZipStream(stream, CompressionMode.Decompress))
            using (var buffered = new BufferedStream(zip))
            using (var reader = new BinaryReader(buffered))
            {
                chunkColumn.Deserialize(reader);
                return chunkColumn;
            }
        }

        public override void Remove(IChunkColumn value)
        {
            using (Database.Lock(Operation.Write))
            {
                Database.Remove(new Index2Tag(value.Index));
            }
        }
    }
}