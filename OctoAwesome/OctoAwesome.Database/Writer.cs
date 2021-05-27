using System;
using System.IO;

namespace OctoAwesome.Database
{
    public sealed class Writer : IDisposable
    {
        private readonly FileInfo _fileInfo;
        private FileStream _fileStream;

<<<<<<< HEAD
        public Writer(FileInfo fileInfo) => _fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));

        public Writer(string path) : this(new FileInfo(path))
        {

=======
        public Writer(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
        }
        public Writer(string path) : this(new FileInfo(path))
        {

        }

        public void Open()
        {
           fileStream =  fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
>>>>>>> feature/performance
        }

        public void Open() => _fileStream =  _fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);

<<<<<<< HEAD
        public void Close()
        {
            _fileStream.Dispose();
            _fileStream = null;
        }

        public void Write(byte[] data, int offset, int length) => _fileStream.Write(data, offset, length);
        
=======
        public void Write(byte[] data, int offset, int length)
            => fileStream.Write(data, offset, length);
>>>>>>> feature/performance
        public void Write(byte[] data, int offset, int length, long position)
        {
            _fileStream.Seek(position, SeekOrigin.Begin);
            Write(data, offset, length);
        }

        public void WriteAndFlush(byte[] data, int offset, int length)
        {
            Write(data, offset, length);
            _fileStream.Flush();
        }
        public void WriteAndFlush(byte[] data, int offset, int length, long position)
        {
            Write(data, offset, length, position);
            _fileStream.Flush();
        }

<<<<<<< HEAD
        internal long ToEnd() => _fileStream.Seek(0, SeekOrigin.End);
=======
        internal long ToEnd()
            => fileStream.Seek(0, SeekOrigin.End);
>>>>>>> feature/performance

        #region IDisposable Support
        private bool disposedValue = false;

<<<<<<< HEAD
=======
        

>>>>>>> feature/performance
        public void Dispose()
        {
            if (disposedValue)
                return;

            _fileStream?.Dispose();

            disposedValue = true;
        }
        #endregion

    }
}
