using System;
using System.IO;

namespace OctoAwesome.Database
{
    public sealed class Reader
    {
        private readonly FileInfo _fileInfo;

<<<<<<< HEAD
        public Reader(FileInfo fileInfo) => _fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));

=======
        public Reader(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
        }
>>>>>>> feature/performance
        public Reader(string path) : this(new FileInfo(path))
        {

        }

        internal byte[] Read(long index, int length)
        {
            if (length < 0)
            {
<<<<<<< HEAD
                _fileInfo.Refresh();
                length = _fileInfo.Exists ? (int)_fileInfo.Length : length;
=======
                fileInfo.Refresh();
                length = fileInfo.Exists ? (int)fileInfo.Length : length;
>>>>>>> feature/performance
            }

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
