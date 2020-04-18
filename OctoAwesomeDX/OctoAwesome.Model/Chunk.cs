using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
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

        protected IBlock[] blocks;

        public Index3 Index { get; private set; }

        public IPlanet Planet { get; private set; }

        public int ChangeCounter { get; private set; }

        public Chunk(Index3 pos, IPlanet planet)
        {
            blocks = new IBlock[CHUNKSIZE_X * CHUNKSIZE_Y * CHUNKSIZE_Z];
            Index = pos;
            Planet = planet;
            ChangeCounter = 0;
        }

        public IBlock GetBlock(Index3 index)
        {
            return GetBlock(index.X, index.Y, index.Z);
        }

        public IBlock GetBlock(int x, int y, int z)
        {
            if (x < 0 || x >= Chunk.CHUNKSIZE_X || y < 0 || y >= Chunk.CHUNKSIZE_Y || z < 0 || z >= Chunk.CHUNKSIZE_Z)
                return null;
            return blocks[GetIndex(x, y, z)];
        }

        public void SetBlock(Index3 index, IBlock block)
        {
            SetBlock(index.X, index.Y, index.Z, block);
        }

        public void SetBlock(int x, int y, int z, IBlock block)
        {
            if (x < 0 || x >= Chunk.CHUNKSIZE_X || y < 0 || y >= Chunk.CHUNKSIZE_Y || z < 0 || z >= Chunk.CHUNKSIZE_Z)
                return;

            blocks[GetIndex(x, y, z)] = block;
            ChangeCounter++;
        }

        private int GetIndex(int x, int y, int z)
        {
            return (z * CHUNKSIZE_X * CHUNKSIZE_Y) + (y * CHUNKSIZE_X) + x;
        }

        public void Serialize(Stream stream)
        {
            using (BinaryWriter bw = new BinaryWriter(stream))
            {
                List<Type> types = new List<Type>();

                //Types sammeln
                for (int i = 0; i < blocks.Length; i++)
                {
                    if (blocks[i] != null)
                    {
                        Type t = blocks[i].GetType();

                        if (!types.Contains(blocks[i].GetType()))
                            types.Add(t);
                    }
                }

                //1. Phase: Anzahl der Counts schreiben
                bw.Write(types.Count);

                foreach (var t in types)
                {
                    bw.Write(t.FullName);
                }

                //2. Phase: Auflistung der Blocks schreiben
                for (int i = 0; i < blocks.Length; i++)
                {
                    if (blocks[i] == null)
                        bw.Write(0);
                    else
                        bw.Write(types.IndexOf(blocks[i].GetType()) + 1);
                }
            }
        }

        public void Deserialize(Stream stream, IEnumerable<IBlockDefinition> knownBlocks)
        {
            using (BinaryReader br = new BinaryReader(stream))
            {
                List<Type> types = new List<Type>();

                int typeCount = br.ReadInt32();

                for (int i = 0; i < typeCount; i++)
                {
                    string typeName = br.ReadString();

                    var blockDefinition = knownBlocks.First(d => d.GetBlockType().FullName == typeName);
                    types.Add(blockDefinition.GetBlockType());
                }

                for(int i = 0; i < blocks.Length; i++)
                {
                    int typeIndex = br.ReadInt32();

                    if(typeIndex > 0)
                    {
                        Type t = types[typeIndex - 1];
                        blocks[i] = (IBlock)Activator.CreateInstance(t);
                    }
                }
            }
        }
    }
}
