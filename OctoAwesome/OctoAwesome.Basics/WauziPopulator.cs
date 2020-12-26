using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using engenious;
using OctoAwesome.Basics.Entities;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Basics
{
    public class WauziPopulator : IMapPopulator
    {
        int ispop = 10;

        readonly Random r = new Random();

        public int Order => 11;

        public void Populate(IResourceManager resourcemanager, IPlanet planet, IChunkColumn column00,
            IChunkColumn column01, IChunkColumn column10, IChunkColumn column11)
        {
            //HACK: Activate Wauzi
            return;

            if (ispop-- <= 0)
                return;

            var wauzi = new WauziEntity();

            var x = r.Next(0, Chunk.CHUNKSIZE_X / 2);
            var y = r.Next(0, Chunk.CHUNKSIZE_Y / 2);

            var position = new PositionComponent()
            {
                Position = new Coordinate(0,
                    new Index3(x + column00.Index.X * Chunk.CHUNKSIZE_X, y + column00.Index.Y * Chunk.CHUNKSIZE_Y, 200),
                    new Vector3(0, 0, 0))
            };
            wauzi.Components.AddComponent(position);
            column00.Add(wauzi);
        }
    }
}