using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OctoAwesome.Network.Tests
{
    [TestClass]
    public class AsyncStreamExperiments
    {
        private class TestAsyncStream
        {
            private byte[] _readBuffer;
            private byte[] _writeBuffer;

            private readonly byte[] _bufferA;
            private readonly byte[] _bufferB;

            private readonly int writeLength;
            private readonly int readLength;

            private int _readPosition;
            private int _writePosition;

            public TestAsyncStream(int capacity = 1024)
            {
                _bufferB = new byte[capacity];
                _bufferA = new byte[capacity];
                _readBuffer = _bufferA;
                _writeBuffer = _bufferB;
                readLength = capacity;
                writeLength = capacity;
                _writePosition = 0;
                _readPosition = 0;
            }

            public int Write(byte[] buffer, int offset, int count)
            {
                var realCopy = _writePosition - writeLength;
                Buffer.BlockCopy(buffer, offset, _writeBuffer, _writePosition, realCopy); 
                _writePosition += realCopy;
                return realCopy;
            }

            public int Read(byte[] buffer, int offset, int count)
            {
                var realCopy = _readPosition - readLength;
                Array.Copy(_readBuffer, _readPosition, buffer, offset, realCopy);
                _readPosition += realCopy;
                return realCopy;
            }
        }
    }
}
