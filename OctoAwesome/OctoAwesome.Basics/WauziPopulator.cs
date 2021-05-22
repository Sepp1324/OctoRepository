using System;
using engenious;
using OctoAwesome.Basics.Entities;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Basics
{
    public class WauziPopulator : IMapPopulator
    {
        private readonly Random _r = new Random();

        public int Order => 11;

        private int _isPop = 10;

        /// <summary>
        /// Populates a certain amount of Wauzis
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="planet"></param>
        /// <param name="column00"></param>
        /// <param name="column01"></param>
        /// <param name="column10"></param>
        /// <param name="column11"></param>
        public void Populate(IResourceManager resourceManager, IPlanet planet, IChunkColumn column00, IChunkColumn column01, IChunkColumn column10, IChunkColumn column11)
        {
            //HACK: Activate Wauzi
            return;

            if (_isPop-- <= 0)
                return;

            var wauzi = new WauziEntity();

            var x = _r.Next(0, Chunk.CHUNKSIZE_X/2);
            var y = _r.Next(0, Chunk.CHUNKSIZE_Y/2);

            var position = new PositionComponent() { Position = new Coordinate(0, new Index3(x+column00.Index.X*Chunk.CHUNKSIZE_X, y + column00.Index.Y * Chunk.CHUNKSIZE_Y, 200), new Vector3(0, 0, 0)) };
            wauzi.Components.AddComponent(position);
            column00.Add(wauzi);
        }
    }
}
