using System;
using System.IO;

namespace OctoAwesome.Database
{
    public sealed class Writer : IDisposable
    {
        private readonly FileInfo _fileInfo;
        private FileStream _fileStream;

        public Writer(FileInfo fileInfo) => this._fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));

        public Writer(string path) : this(new FileInfo(path))
        {
        }

        public void Open() => _fileStream = _fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);

        public void Close()
        {
            _fileStream.Dispose();
            _fileStream = null;
        }

        public void Write(ReadOnlySpan<byte> data) => _fileStream.Write(data);

        public void Write(ReadOnlySpan<byte> data, long position)
        {
            _fileStream.Seek(position, SeekOrigin.Begin);
            Write(data);
        }

        public void WriteAndFlush(ReadOnlySpan<byte> data)
        {
            Write(data);
            _fileStream.Flush();
        }

        public void WriteAndFlush(ReadOnlySpan<byte> data, long position)
        {
            Write(data, position);
            _fileStream.Flush();
        }

        public void Write(ReadOnlySpan<byte> data, int offset, int length)
        {
            _fileStream.Write(data[offset..(offset + length)]);
        }

        public void Write(ReadOnlySpan<byte> data, int offset, int length, long position)
        {
            _fileStream.Seek(position, SeekOrigin.Begin);
            Write(data[offset..(offset + length)]);
        }

        public void WriteAndFlush(ReadOnlySpan<byte> data, int offset, int length)
        {
            Write(data[offset..(offset + length)]);
            _fileStream.Flush();
        }

        public void WriteAndFlush(ReadOnlySpan<byte> data, int offset, int length, long position)
        {
            Write(data[offset..(offset + length)], position);
            _fileStream.Flush();
        }

        internal long ToEnd() => _fileStream.Seek(0, SeekOrigin.End);

        #region IDisposable Support

        private bool _disposedValue;


        public void Dispose()
        {
            if (_disposedValue)
                return;

            _fileStream?.Dispose();

            _disposedValue = true;
        }

        #endregion
    }
}