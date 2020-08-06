using System;

namespace OctoAwesome.Network
{
    public class OctoNetworkStream
    {
        public int Length => _writeBuffer.Length;

        private byte[] _readBuffer;
        private byte[] _writeBuffer;

        private readonly byte[] _bufferA;
        private readonly byte[] _bufferB;

        private readonly object _readLock;
        private readonly object _writeLock;

        private readonly int _writeLength;
        private readonly int _readLength;

        private int _maxReadCount;

        private int _readPosition;
        private int _writePosition;

        private bool _writingProcess;

        public OctoNetworkStream(int capacity = 1024)
        {
            _bufferA = new byte[capacity];
            _bufferB = new byte[capacity];
            _readBuffer = _bufferA;
            _writeBuffer = _bufferB;
            _readLength = capacity;
            _writeLength = capacity;
            _readPosition = 0;
            _writePosition = 0;
            _readLock = new object();
            _writeLock = new object();
        }

        public int Write(byte[] buffer, int offset, int count)
        {
            _writingProcess = true;

            SwapBuffer();

            var maxCopy = _writeLength - _writePosition;

            if (maxCopy < count)
                count = maxCopy;

            if (maxCopy < 1)
            {
                _writingProcess = false;
                return maxCopy;
            }

            lock (_writeLock)
                Buffer.BlockCopy(buffer, offset, _writeBuffer, _writePosition, count);

            _writePosition += count;

            _writingProcess = false;

            return count;
        }

        public int Write(byte data)
        {
            _writingProcess = true;

            SwapBuffer();

            if (_writeLength == _writePosition)
            {
                _writingProcess = false;
                return 0;
            }

            lock (_writeLock)
                _writeBuffer[_writePosition++] = data;

            _writingProcess = false;

            return 1;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (!_writingProcess)
                SwapBuffer();

            var maxCopy = _maxReadCount - _readPosition;

            if (maxCopy < 1)
                return maxCopy;

            if (maxCopy < count)
                count = maxCopy;

            lock (_readLock)
                Buffer.BlockCopy(_readBuffer, _readPosition, buffer, offset, count);

            _readPosition += count;

            return count;
        }

        private void SwapBuffer()
        {
            lock (_readLock)
                lock (_writeLock)
                {
                    if (_readPosition > _maxReadCount)
                        throw new IndexOutOfRangeException("ReadPositin is greater than MaxReadCount in OctoNetworkStream");
                    else if (_readPosition < _maxReadCount)
                        return;

                    var refBuf = _writeBuffer;
                    _writeBuffer = _readBuffer;
                    _readBuffer = refBuf;
                    _maxReadCount = _writePosition;
                    _writePosition = 0;
                    _readPosition = 0;
                }
        }

    }
}
