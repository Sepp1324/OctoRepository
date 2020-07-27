using System;
using System.IO;
using System.Threading;

namespace OctoAwesome.Network
{
    public class OctoNetworkStream
    {
        public bool CanRead => true;

        public bool CanSeek => false;

        public bool CanWrite => true;

        public long Length => _internalBuffer.LongLength;

        public int WritePosition => _writePosition;

        public int ReadPosition => _readPosition;

        private byte[] _internalBuffer;
        private int _writePosition;
        private int _readPosition;

        public OctoNetworkStream(int capacity = 1024) => _internalBuffer = new byte[capacity];

        public void Flush() => throw new NotImplementedException();

        public long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

        public void SetLength(int value) => Array.Resize(ref _internalBuffer, value);

        public int Read(byte[] buffer, int offset, int count)
        {
            var tmpCount = count;

            if (buffer.Length > offset + count)
                throw new IndexOutOfRangeException();

            if (buffer.Length - offset < count)
                tmpCount = buffer.Length - offset;

            if (_readPosition + tmpCount > _internalBuffer.Length)
            {
                tmpCount = _internalBuffer.Length - _readPosition;
                Array.Copy(_internalBuffer, _readPosition, buffer, offset, tmpCount);
                offset = tmpCount;
                tmpCount = count - tmpCount;
                _readPosition = 0;

                if(_readPosition + tmpCount > _writePosition)
                {
                    var exception = new IndexOutOfRangeException("Dont't worry, be happy d:^)");
                    exception.Data.Add("Writeposition", _writePosition);
                    exception.Data.Add("Count", count);
                    exception.Data.Add("Offset", offset);
                    throw exception;
                }
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

            int toWrite = count, bufferPosition = offset;

            if (_writePosition + toWrite > _internalBuffer.Length)
                toWrite = _internalBuffer.Length - _writePosition;

            Array.Copy(buffer, bufferPosition, _internalBuffer, _writePosition, toWrite);

            if (_writePosition + count > _internalBuffer.Length)
            {
                bufferPosition += toWrite;
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
                Array.Copy(buffer, bufferPosition, _internalBuffer, _writePosition, toWrite);
            }
            Interlocked.Exchange(ref _writePosition, _writePosition + toWrite);
        }

        public void Write(byte data)
        {
            int toWrite = 1;

            //if (_writePosition + toWrite > _readPosition)
            //{
            //    var exception = new IndexOutOfRangeException("Dont't worry, be happy d:^)"); ;
            //    exception.Data.Add("Readpos", _readPosition);
            //    exception.Data.Add("Writepos", _writePosition);
            //    throw exception;
            //}

            if (_writePosition + toWrite > _internalBuffer.Length)
                toWrite = _internalBuffer.Length - _writePosition;

            if(toWrite == 0)
                _writePosition %= _internalBuffer.Length;

            _internalBuffer[_writePosition++] = data;

           
            
            
            //Interlocked.Exchange(ref _writePosition, _writePosition + toWrite);
        }
    }
}