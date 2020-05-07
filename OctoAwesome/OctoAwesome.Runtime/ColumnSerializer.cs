using System;
using System.IO;

namespace OctoAwesome.Runtime
{
    public sealed class ColumnSerializer : IColumnSerializer
    {
        public IChunkColumn Deserialize(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Serialize(Stream stream, IChunkColumn column)
        {
            throw new NotImplementedException();
        }
    }
}
