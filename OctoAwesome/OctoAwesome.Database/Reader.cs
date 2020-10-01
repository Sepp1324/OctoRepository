using System;
using System.IO;

namespace OctoAwesome.Database
{
    public sealed class Reader
    {
        internal object Read(long index, int length)
        {
            var array = new byte[length];
            fileStream.Seek(index + Key.KEY_SIZE, SeekOrigin.Begin);
            fileStream.Read(array, 0, length);
            return array;
        }
    }
}
