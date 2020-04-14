using Microsoft.Xna.Framework;
using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public class Planet : IPlanet
    {
        private Chunk[,,] chunks;

        public Index3 Size { get; private set; }

        public int SizeX { get; private set; }

        public int SizeY { get; private set; }

        public int SizeZ { get; private set; }

        public Planet(Index3 size)
        {
            Size = size;

            chunks = new Chunk[Size.X, Size.Y, Size.Z];

            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    for (int z = 0; z < Size.Z; z++)
                    {
                        chunks[x, y, z] = new Chunk(new Index3(x, y, z));
                    }
                }
            }
        }

        public IChunk GetChunk(Index3 pos)
        {
            return GetChunk(pos.X, pos.Y, pos.Z);
        }

        public IChunk GetChunk(int x, int y, int z)
        {
            if (chunks[x, y, z] == null)
            {
                //TODO: Load from Disk
            }

            return chunks[x, y, z];
        }

        public IBlock GetBlock(Index3 pos)
        {
            Coordinate coordinate = new Coordinate(0, pos, Vector3.Zero);
            IChunk chunk = GetChunk(coordinate.AsChunk());

            return chunk.GetBlock(coordinate.AsLocalBlock());
        }

        public void SetBlock(Index3 pos, IBlock block, TimeSpan time)
        {
            Coordinate coordinate = new Coordinate(0, pos, Vector3.Zero);
            IChunk chunk = GetChunk(coordinate.AsChunk());
            chunk.SetBlock(coordinate.AsLocalBlock(), block, time);
        }
    }
}
