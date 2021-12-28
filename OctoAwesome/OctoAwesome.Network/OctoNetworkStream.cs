using System;

namespace OctoAwesome.Network
{
    public class OctoNetworkStream
    {
        private readonly object _readLock;

        private readonly int _writeLength;
        private readonly object _writeLock;

        private int _maxReadCount;

        private byte[] _readBuffer;

        private int _readPosition;
        private byte[] _writeBuffer;
        private int _writePosition;

        private bool _writingProcess;

        public OctoNetworkStream(int capacity = 1024)
        {
            var bufferA = new byte[capacity];
            var bufferB = new byte[capacity];
            _readBuffer = bufferA;
            _writeBuffer = bufferB;
            _writeLength = capacity;
            _readPosition = 0;
            _writePosition = 0;
            _readLock = new();
            _writeLock = new();
        }

        public int Length => _writeBuffer.Length;

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
            {
                Buffer.BlockCopy(buffer, offset, _writeBuffer, _writePosition, count);
            }

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
            {
                Buffer.BlockCopy(_readBuffer, _readPosition, buffer, offset, count);
            }

            _readPosition += count;

            return count;
        }

        public int DataAvailable(int count)
        {
            if (!_writingProcess)
                SwapBuffer();

            var maxCopy = _maxReadCount - _readPosition;

            if (maxCopy < 1)
                return maxCopy;

            if (maxCopy < count)
                count = maxCopy;

            return count;
        }

        private void SwapBuffer()
        {
            lock (_readLock)
            lock (_writeLock)
            {
                if (_readPosition > _maxReadCount)
                    throw new IndexOutOfRangeException("ReadPositin is greater than MaxReadCount in OctoNetworkStream");
                if (_readPosition < _maxReadCount)
                    return;

                (_writeBuffer, _readBuffer) = (_readBuffer, _writeBuffer);
                _maxReadCount = _writePosition;
                _writePosition = 0;
                _readPosition = 0;
            }
        }
    }
}