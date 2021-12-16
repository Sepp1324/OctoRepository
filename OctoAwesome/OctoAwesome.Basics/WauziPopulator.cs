using System;
using engenious;
using OctoAwesome.Basics.Entities;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Basics
{
    /// <summary>
    /// Populator for Wauzis
    /// </summary>
    public class WauziPopulator : IMapPopulator
    {
        private int _ispop = 10;

        private readonly Random r = new();

        /// <summary>
        /// 
        /// </summary>
        public int Order => 11;

        /// <summary>
        /// Spawns Wauzis
        /// </summary>
        /// <param name="resourceManager"><see cref="IResourceManager"/></param>
        /// <param name="planet">Current <see cref="Planet"/></param>
        /// <param name="column00"></param>
        /// <param name="column01"></param>
        /// <param name="column10"></param>
        /// <param name="column11"></param>
        public void Populate(IResourceManager resourceManager, IPlanet planet, IChunkColumn column00, IChunkColumn column01, IChunkColumn column10, IChunkColumn column11)
        {
            //HACK: Activate Wauzi
            //return;

            if (_ispop-- <= 0)
                return;

            var wauzi = new WauziEntity();

            var x = r.Next(0, Chunk.CHUNKSIZE_X / 2);
            var y = r.Next(0, Chunk.CHUNKSIZE_Y / 2);

            var position = new PositionComponent
            {
                Position = new Coordinate(0, new Index3(x + column00.Index.X * Chunk.CHUNKSIZE_X, y + column00.Index.Y * Chunk.CHUNKSIZE_Y, 200), new Vector3(0, 0))
            };
            wauzi.Components.AddComponent(position);
            column00.Add(wauzi);
        }
    }
}