using System;
using System.IO;

namespace OctoAwesome
{
    public interface IColumnSerializer
    {
        void Serialize(Stream stream, Guid universeId, int planetId, IChunkColumn column);

        IChunkColumn Deserialize(Stream stream, Guid universeId, int planetId, Index2 columnIndex);
    }
}
