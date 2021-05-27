using System;
using System.IO;
using System.Linq;
using OctoAwesome.Basics.Definitions.Blocks;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics
{
    public class DebugMapGenerator : IMapGenerator
    {
        public IPlanet GeneratePlanet(Guid universe, int id, int seed)
        {
            var planet = new Planet(id, universe, new Index3(5, 5, 4), seed);
            planet.Generator = this;
            return planet;
        }

        public IChunkColumn GenerateColumn(IDefinitionManager definitionManager, IPlanet planet, Index2 index)
        {
            var definitions = definitionManager.Definitions.ToArray();

            IBlockDefinition sandDefinition = definitions.OfType<SandBlockDefinition>().First();
            var sandIndex = (ushort) (Array.IndexOf(definitions.ToArray(), sandDefinition) + 1);

            var result = new IChunk[planet.Size.Z];

            var column = new ChunkColumn(result, planet, index);


            for (var layer = 0; layer < planet.Size.Z; layer++)
                result[layer] = new Chunk(new Index3(index.X, index.Y, layer), planet);

            var part = (planet.Size.Z * Chunk.CHUNKSIZE_Z) / 4;

            for (var y = 0; y < Chunk.CHUNKSIZE_Y; y++)
            {
                var heightY = (float) Math.Sin((float) (y * Math.PI) / 15f);
                for (var x = 0; x < Chunk.CHUNKSIZE_X; x++)
                {
                    var heightX = (float) Math.Sin((float) (x * Math.PI) / 18f);

                    var height = ((heightX + heightY + 2) / 4) * (2 * part);
                    for (var z = 0; z < planet.Size.Z * Chunk.CHUNKSIZE_Z; z++)
                        if (z < (int) (height + part))
                        {
                            var block = z % (Chunk.CHUNKSIZE_Z);
                            var layer = z / Chunk.CHUNKSIZE_Z;
                            result[layer].SetBlock(x, y, block, sandIndex);
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
            {
                planet.Deserialize(reader);
            }

            return planet;
        }


        public IChunkColumn GenerateColumn(Stream stream, IPlanet planet, Index2 index)
        {
            IChunkColumn column = new ChunkColumn(planet);
            using (var reader = new BinaryReader(stream))
            {
                column.Deserialize(reader);
            }

            return column;
        }
    }
}