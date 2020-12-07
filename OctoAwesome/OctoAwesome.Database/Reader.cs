using System.IO;

namespace OctoAwesome.Database
{
    public sealed class Reader
    {
        private readonly FileInfo _fileInfo;

        public Reader(FileInfo fileInfo) => _fileInfo = fileInfo;

        public Reader(string path) : this(new FileInfo(path))
        {

        }

        internal byte[] Read(long index, int length)
        {
            length = length < 0 ? (int)_fileInfo.Length : length;

            var array = new byte[length];
           
            using (var fileStream = _fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fileStream.Seek(index, SeekOrigin.Begin);
                fileStream.Read(array, 0, length);
            }
            return array;
        }
    }
}
