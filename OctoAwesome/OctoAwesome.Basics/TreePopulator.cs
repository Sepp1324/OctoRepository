using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Basics
{
    public class TreePopulator : MapPopulator
    {
        private IEnumerable<ITreeDefinition> _treeDefinitions = null;

        public TreePopulator() => Order = 10;

        private static IChunkColumn getColumn(IChunkColumn column00, IChunkColumn column10, IChunkColumn column01, IChunkColumn column11, int x, int y)
        {
            IChunkColumn column;

            if (x >= Chunk.CHUNKSIZE_X && y >= Chunk.CHUNKSIZE_Y)
                column = column11;
            else if (x < Chunk.CHUNKSIZE_X && y >= Chunk.CHUNKSIZE_Y)
                column = column01;
            else if (x >= Chunk.CHUNKSIZE_X && y < Chunk.CHUNKSIZE_Y)
                column = column10;
            else
                column = column00;

            return column;
        }

        public override void Populate(IResourceManager resourceManager, IPlanet planet, IChunkColumn column00,
            IChunkColumn column10, IChunkColumn column01, IChunkColumn column11)
        {
            // Tree Definitions initialisieren
            if (_treeDefinitions == null)
            {
                _treeDefinitions = resourceManager.DefinitionManager.GetDefinitions<ITreeDefinition>().OrderBy(d => d.Order).ToArray();

                foreach (var treeDefinition in _treeDefinitions)
                    treeDefinition.Init(resourceManager.DefinitionManager);
            }

            var salt = (column00.Index.X & 0xffff) + ((column00.Index.Y & 0xffff) << 16);
            var random = new Random(planet.Seed + salt);
            var sample = new Index3(column00.Index.X * Chunk.CHUNKSIZE_X, column00.Index.Y * Chunk.CHUNKSIZE_Y, column00.Heights[0, 0]);
            //var wasChanged = false;

            foreach (var treeDefinition in _treeDefinitions)
            {
                var density = treeDefinition.GetDensity(planet, sample);

                if (density <= 0)
                    continue;

                for (var i = 0; i < density; i++)
                {
                    var x = random.Next(Chunk.CHUNKSIZE_X / 2, Chunk.CHUNKSIZE_X * 3 / 2);
                    var y = random.Next(Chunk.CHUNKSIZE_Y / 2, Chunk.CHUNKSIZE_Y * 3 / 2);
                    var z = LocalBuilder.GetSurfaceHeight(column00, column10, column01, column11, x, y);
                    var blockTmp = planet.ClimateMap.GetTemperature(new Index3(column00.Index.X * Chunk.CHUNKSIZE_X, column00.Index.Y * Chunk.CHUNKSIZE_X, z));

                    if (blockTmp > treeDefinition.MaxTemperature || blockTmp < treeDefinition.MinTemperature)
                        continue;

                    var builder = new LocalBuilder(x, y, z + 1, column00, column10, column01, column11);
                    treeDefinition.PlantTree(planet, new Index3(x, y, z), builder, random.Next(int.MaxValue));
                    //wasChanged = true;
                }
            }

            //if (wasChanged)
            //{
            //    resourceManager.SaveChunkColumn(column00);
            //    resourceManager.SaveChunkColumn(column10);
            //    resourceManager.SaveChunkColumn(column01);
            //    resourceManager.SaveChunkColumn(column11);
            //}
        }
    }
}
