using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Model
{
    public class Chunk : IChunk
    {
        public const int CHUNKSIZE_X = 32;
        public const int CHUNKSIZE_Y = 32;
        public const int CHUNKSIZE_Z = 32;

        private IBlock[, ,] blocks;

        public Index3 Index { get; private set; }

        public TimeSpan LastChange { get; private set; }

        public Chunk(Index3 pos)
        {
            blocks = new IBlock[CHUNKSIZE_X, CHUNKSIZE_Y, CHUNKSIZE_Z];
            Index = pos;

            for(int z = 0; z < CHUNKSIZE_Z; z++)
            {
                for(int y = 0; y < CHUNKSIZE_Y; y++)
                {
                    float heightY = (float)Math.Sin((float)(y * Math.PI) / 16f);
                    for(int x = 0; x < CHUNKSIZE_X; x++)
                    {
                        float heightX = (float)Math.Sin((float)(x * Math.PI) / 16f);
                        float height = (heightX + heightY) * 2;

                        if (z < (int)(16 + height))
                        {
                            //if (x % 2 == 0 || y % 2 == 0)
                            blocks[x, y, z] = new GrassBlock();
                            //else
                            //    Blocks[x, y, z] = new SandBlock();
                        }
                    }
                }
            }
        }

        public IBlock GetBlock(Index3 index)
        {

            if (index.X < 0 || index.X >= Chunk.CHUNKSIZE_X || index.Y < 0 || index.Y >= Chunk.CHUNKSIZE_Y || index.Z < 0 || index.Z >= Chunk.CHUNKSIZE_Z)
                throw new IndexOutOfRangeException();

            return blocks[index.X, index.Y, index.Z];
        }

        //public IBlock GetBlock(int x, int y, int z)
        //{

        //}

        public void SetBlock(Index3 index, IBlock block, TimeSpan time)
        {
            blocks[index.X, index.Y, index.Z] = block;
            LastChange = time;
        }

        //public void SetBlock(int x, int y, int z, IBlock block, TimeSpan time)
        //{

        //}
    }
}
