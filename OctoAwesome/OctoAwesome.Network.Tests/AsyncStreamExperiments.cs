using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OctoAwesome.Network.Tests
{
    [TestClass]
    public class AsyncStreamExperiments
    {
        private class TestAsyncStream
        {
            public readonly byte[] ReadBuffer;
            public readonly byte[] WriteBuffer;

            private int _readPosition;
            private int _writePosition;

            public TestAsyncStream(int capacity = 1024)
            {
                ReadBuffer = new byte[2024];
                WriteBuffer = new byte[1024];
                _writePosition = 0;
                _readPosition = 0;
            }

            public void Write()
            {

            }

            public void Read()
            {

            }
        }
    }
}
