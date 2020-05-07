using System;
using System.IO;

namespace OctoAwesome
{
    public interface IColumnSerializer
    {
        void Serialize(Stream stream, IChunkColumn column);

        IChunkColumn Deserialize(Stream stream);
    }
}
