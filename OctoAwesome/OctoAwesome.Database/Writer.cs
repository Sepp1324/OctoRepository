using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OctoAwesome.Database
{
    public sealed class Writer : IDisposable
    {
        private readonly FileInfo _fileInfo;
        private FileStream _fileStream;

        public Writer(FileInfo fileInfo) => _fileInfo = fileInfo;

        public Writer(string path) : this(new FileInfo(path))
        {

        }

        public void Open() => _fileStream =  _fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);

        public void Close()
        {
            _fileStream.Dispose();
            _fileStream = null;
        }

        public void Write(byte[] data, int offset, int length) => _fileStream.Write(data, offset, length);
       
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

        internal long ToEnd() => _fileStream.Seek(0, SeekOrigin.End);

        #region IDisposable Support
        private bool disposedValue = false;

        

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
