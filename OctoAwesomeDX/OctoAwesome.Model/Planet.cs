﻿using Microsoft.Xna.Framework;
using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    internal class Planet : IPlanet
    {
        private readonly int CACHELIMIT = 10000;

        private IMapGenerator generator;

        private IChunk[, ,] chunks;

        private Dictionary<Index3, int> lastAccess = new Dictionary<Index3, int>();

        private int accessCounter = 0;

        public int Id { get; private set; }

        public int Seed { get; private set; }

        public Index3 Size { get; private set; }
        
        public IChunkPersistence ChunkPersistence { get; set; }

        public Planet(Index3 size, IMapGenerator generator, int seed)
        {
            this.Id = 0;
            this.generator = generator;
            Size = size;
            Seed = seed;

            chunks = new Chunk[Size.X, Size.Y, Size.Z];
        }

        public IChunk GetChunk(Index3 index)
        {
            if (index.X < 0 || index.X >= Size.X || 
                index.Y < 0 || index.Y >= Size.Y || 
                index.Z < 0 || index.Z >= Size.Z) 
                return null;

            if (chunks[index.X, index.Y, index.Z] == null)
            {
                //Load from Disk
                IChunk first = ChunkPersistence.Load(Id, index);

                if (first != null)
                {
                    for (int z = 0; z < this.Size.Z; z++)
                    {
                        chunks[index.X, index.Y, z] = ChunkPersistence.Load(Id, new Index3(index.X, index.Y, z));
                        lastAccess.Add(new Index3(index.X, index.Y, z), accessCounter++);
                    }
                }
                else
                {
                    IChunk[] result = generator.GenerateChunk(this, new Index2(index.X, index.Y));

                    for (int layer = 0; layer < this.Size.Z; layer++)
                    {
                        chunks[index.X, index.Y, layer] = result[layer];
                        lastAccess.Add(new Index3(index.X, index.Y, layer), accessCounter++);
                    }
                }

                //Cache regulieren
                while(lastAccess.Count > CACHELIMIT)
                {
                    Index3 oldest = lastAccess.OrderBy(a => a.Value).Select(a => a.Key).First();
                    var chunk = chunks[oldest.X, oldest.Y, oldest.Z];
                    ChunkPersistence.Save(chunk, Id);
                    chunks[oldest.X, oldest.Y, oldest.Z] = null; //TODO: Pooling
                    lastAccess.Remove(oldest);
                }
            }
            else
            {
                lastAccess[index] = accessCounter++;
            }

            return chunks[index.X, index.Y, index.Z];
        }

        public IBlock GetBlock(Index3 index)
        {
            index.NormalizeXY(new Index2(Size.X * Chunk.CHUNKSIZE_X, Size.Y * Chunk.CHUNKSIZE_Y));
            Coordinate coordinate = new Coordinate(0, index, Vector3.Zero);
            
            //Betroffenen Chunk ermitteln
            IChunk chunk = GetChunk(coordinate.ChunkIndex);
            if (chunk == null) return null;

            return chunk.GetBlock(coordinate.LocalBlockIndex);
        }

        public void SetBlock(Index3 index, IBlock block, TimeSpan time)
        {
            index.NormalizeXYZ(new Index3(Size.X * Chunk.CHUNKSIZE_X, Size.Y * Chunk.CHUNKSIZE_Y, Size.Z * Chunk.CHUNKSIZE_Z));
            Coordinate coordinate = new Coordinate(0, index, Vector3.Zero);
            IChunk chunk = GetChunk(coordinate.ChunkIndex);
            chunk.SetBlock(coordinate.LocalBlockIndex, block);
        }

        public void Save()
        {
            for(int z = 0; z < Size.Z; z++)
            {
                for(int y = 0; y < Size.Y; y++)
                {
                    for(int x = 0; x < Size.X; x++)
                    {
                        if (chunks[x, y, z] != null)
                            ChunkPersistence.Save(chunks[x, y, z], Id);
                    }
                }
            }
        }
    }
}
