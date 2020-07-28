using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OctoAwesome.Network.Tests
{
    [TestClass]
    public class AsyncStreamExperiments
    {
        [TestMethod]
        public void ReadWriteSwap()
        {
            TestAsyncStream test = new TestAsyncStream();
            byte[] _writeData = new byte[400];
            byte[] _readData = new byte[400];
            Random random = new Random();

            random.NextBytes(_writeData);

            test.Write(_writeData, 0, _writeData.Length);
            test.Read(_readData, 0, _readData.Length);

            Assert.IsTrue(_writeData.SequenceEqual(_readData));
        }

        [TestMethod]
        public async Task ReadWriteSwapAsync()
        {
            TestAsyncStream test = new TestAsyncStream();
            byte[] _writeData = new byte[400];
            byte[] _readData = new byte[400];
            Random random = new Random();

            random.NextBytes(_writeData);

            Task _readTask = new Task(() =>
            {
                int o = 0;

                while (test.Read(_readData, o, 100) != 0)
                {
                    Thread.Sleep(200);
                    o += 100;
                }
            });
            _readTask.Start();

            Thread.Sleep(1000);

            test.Write(_writeData, 0, _writeData.Length);
            Assert.IsTrue(_writeData.SequenceEqual(_readData));
        }

        private class TestAsyncStream
        {
            private byte[] _readBuffer;
            private byte[] _writeBuffer;

            private readonly byte[] _bufferA;
            private readonly byte[] _bufferB;

            private readonly byte[] _readLock;
            private readonly byte[] _writeLock;

            private readonly int _writeLength;
            private readonly int _readLength;

            private int _maxReadCount;

            private int _readPosition;
            private int _writePosition;

            private bool _writingProcess;

            public TestAsyncStream(int capacity = 1024)
            {
                _bufferB = new byte[capacity];
                _bufferA = new byte[capacity];
                _readBuffer = _bufferA;
                _writeBuffer = _bufferB;
                _readLength = capacity;
                _writeLength = capacity;
                _writePosition = 0;
                _readPosition = 0;
            }

            public int Write(byte[] buffer, int offset, int count)
            {
                _writingProcess = true;

                lock (_writeLock)
                    SwapBuffer();

                var maxCopy = _writeLength - _writePosition;

                if (maxCopy < count)
                    count = maxCopy;

                lock (_writeLock)
                    Buffer.BlockCopy(buffer, offset, _writeBuffer, _writePosition, count);

                _writePosition += count;
                _writingProcess = false;
                return count;
            }

            public int Read(byte[] buffer, int offset, int count)
            {
                if (!_writingProcess)
                    SwapBuffer();

                var maxCopy = _maxReadCount - _readPosition;

                if (maxCopy < count)
                    count = maxCopy;

                lock (_readLock)
                    Array.Copy(_readBuffer, _readPosition, buffer, offset, count);

                _readPosition += count;
                return count;
            }

            private void SwapBuffer()
            {
                lock (_readBuffer)
                    lock (_writeBuffer)
                    {
                        if (_maxReadCount != _readPosition)
                            return;

                        _writingProcess = true;


                        var refBuffer = _writeBuffer;
                        _writeBuffer = _readBuffer;
                        _readBuffer = refBuffer;
                        _maxReadCount = _writePosition;
                        _writePosition = 0;
                        _readPosition = 0;

                    }
            }
        }
    }
}
