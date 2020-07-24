using System;
using System.IO;
using System.Threading;

namespace OctoAwesome.Network
{
    public class OctoNetworkStream
    {
        public bool CanRead => throw new NotImplementedException();

        public bool CanSeek => false;

        public bool CanWrite => throw new NotImplementedException();

        public long Length => throw new NotImplementedException();

        public long Position { get; set; }

        private byte[] _internalBuffer;
        private int _bufferIndex;
        private int _bufferSize;
        private int _writePosition;
        private int _readPosition;

        public OctoNetworkStream()
        {
            _bufferSize = 1000;
            _internalBuffer = new byte[1000];
            _bufferIndex = 0;
        }

        public void Flush() => throw new NotImplementedException();

        public long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

        public void SetLength(long value) => throw new NotImplementedException();

        public int Read(byte[] buffer, int offset, int count)
        {
            var tmpCount = count;

            if (buffer.Length - offset < count)
                tmpCount = buffer.Length - offset;

            if (_readPosition + tmpCount > _internalBuffer.Length)
            {
                var ex = new IndexOutOfRangeException("Dont't worry, Shit happens");
                ex.Data.Add("Internal Length", _internalBuffer.Length);
                ex.Data.Add("Count", tmpCount);
                ex.Data.Add("Offset", offset);
                throw ex;
            }

            Array.Copy(_internalBuffer, _readPosition, buffer, offset, tmpCount);
            Interlocked.Exchange(ref _readPosition, _readPosition + tmpCount);

            return count;
        }


        public void Write(byte[] buffer, int offset, int count)
        {
            if (buffer.Length < count + offset)
            {
                var exception = new IndexOutOfRangeException("Dont't worry, be happy d:^)");
                exception.Data.Add("Buffer Length", buffer.Length);
                exception.Data.Add("Count", count);
                exception.Data.Add("Offset", offset);
                throw exception;
            }

            int toWrite = count;
            int bufferPostion = offset;

            if (_writePosition + toWrite > _internalBuffer.Length)
                toWrite = _internalBuffer.Length - _writePosition;

            Array.Copy(buffer, bufferPostion, _internalBuffer, _writePosition, toWrite);

            if (_writePosition + toWrite > _internalBuffer.Length)
            {
                bufferPostion += toWrite;
                toWrite = count - toWrite;
                _writePosition = 0;

                if (_writePosition + toWrite > _readPosition)
                {
                    var exception = new IndexOutOfRangeException("Dont't worry, be happy d:^)");
                    exception.Data.Add("Buffer Length", buffer.Length);
                    exception.Data.Add("Count", count);
                    exception.Data.Add("Offset", offset);
                    exception.Data.Add("Readpos", _readPosition);
                    exception.Data.Add("Writepos", _writePosition);
                    throw exception;
                }
                Array.Copy(buffer, bufferPostion, _internalBuffer, _writePosition, toWrite);
            }
            Interlocked.Exchange(ref _writePosition, _writePosition + toWrite);
        }
    }
}