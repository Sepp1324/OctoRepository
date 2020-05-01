using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome.Runtime
{
    public class ChunkSerializer : IChunkSerializer
    {
        public void Serialize(Stream stream, IChunk chunk)
        {
            using (BinaryWriter bw = new BinaryWriter(stream))
            {
                List<Type> types = new List<Type>();

                // Types sammeln
                for (int i = 0; i < _blocks.Length; i++)
                {
                    if (_blocks[i] != null)
                    {
                        Type t = _blocks[i].GetType();
                        if (!types.Contains(t))
                            types.Add(t);
                    }
                }

                // Schreibe Phase 1
                bw.Write(types.Count);

                // Im Falle eines Luft-Chunks...
                if (types.Count == 0)
                    return;

                foreach (var t in types)
                {
                    bw.Write(t.FullName);
                }

                // Schreibe Phase 2
                for (int i = 0; i < _blocks.Length; i++)
                {
                    if (_blocks[i] == null)
                        bw.Write(0);
                    else
                    {
                        bw.Write(types.IndexOf(_blocks[i].GetType()) + 1);

                        //TODO: Rework for Meta-Infos
                        //bw.Write((byte)_blocks[i].Orientation);
                    }
                }
            }
        }

        public IChunk Deserialize(Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream))
            {
                List<Type> types = new List<Type>();
                int typecount = br.ReadInt32();

                // Im Falle eines Luftchunks
                if (typecount == 0)
                    return;

                for (int i = 0; i < typecount; i++)
                {
                    string typeName = br.ReadString();
                    var blockDefinition = knownBlocks.First(d => d.GetBlockType().FullName == typeName);
                    types.Add(blockDefinition.GetBlockType());
                }

                for (int i = 0; i < _blocks.Length; i++)
                {
                    int typeIndex = br.ReadInt32();

                    if (typeIndex > 0)
                    {
                        OrientationFlags orientation = (OrientationFlags)br.ReadByte();
                        Type t = types[typeIndex - 1];
                        //_blocks[i].Orientation = orientation;
                    }
                }
            }
        }
    }
}
