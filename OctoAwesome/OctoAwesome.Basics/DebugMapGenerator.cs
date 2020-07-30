using System;
using System.Linq;
using System.IO;
using OctoAwesome.Basics.Definitions.Blocks;

namespace OctoAwesome.Basics
{
    public class DebugMapGenerator : IMapGenerator
    {
        public IPlanet GeneratePlanet(Guid universe, int id, int seed)
        {
            var planet = new Planet(id, universe, new Index3(4, 4, 3), seed) {Generator = this};
            return planet;
        }

        public IChunkColumn GenerateColumn(IDefinitionManager definitionManager, IPlanet planet, Index2 index)
        {
            var definitions = definitionManager.GetDefinitions().ToArray();

            IBlockDefinition sandDefinition = definitions.OfType<SandBlockDefinition>().First();
            var sandIndex = (ushort) (Array.IndexOf(definitions.ToArray(), sandDefinition) + 1);

            var result = new IChunk[planet.Size.Z];

            var column = new ChunkColumn(result, planet.Id, index);

            for (int layer = 0; layer < planet.Size.Z; layer++)
                result[layer] = new Chunk(new Index3(index.X, index.Y, layer), planet.Id);

            var part = (planet.Size.Z * Chunk.CHUNKSIZE_Z) / 4;

            for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
            {
                var heightY = (float) Math.Sin((float) (y * Math.PI) / 15f);

                for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
                {
                    var heightX = (float) Math.Sin((float) (x * Math.PI) / 18f);

                    var height = ((heightX + heightY + 2) / 4) * (2 * part);
                    for (int z = 0; z < planet.Size.Z * Chunk.CHUNKSIZE_Z; z++)
                    {
                        if (z < (int) (height + part))
                        {
                            var block = z % (Chunk.CHUNKSIZE_Z);
                            var layer = z / Chunk.CHUNKSIZE_Z;
                            result[layer].SetBlock(x, y, block, sandIndex);
                        }
                    }
                }
            }

            column.CalculateHeights();
            return column;
        }

        public IPlanet GeneratePlanet(Stream stream)
        {
            IPlanet planet = new Planet();

            using (var reader = new BinaryReader(stream))
                planet.Deserialize(reader, null);

            return planet;
        }


        public IChunkColumn GenerateColumn(Stream stream, IDefinitionManager definitionManager, int planetId,
            Index2 index)
        {
            IChunkColumn column = new ChunkColumn();
            column.Deserialize(stream, definitionManager, planetId, index);
            return column;
        }
    }
}