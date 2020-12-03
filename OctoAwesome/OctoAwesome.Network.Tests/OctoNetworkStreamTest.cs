using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace OctoAwesome.Network.Tests
{
    [TestClass]
    public class OctoNetworkStreamTest
    {
        private readonly OctoNetworkStream _testStream;
        private readonly Random _rand;

        public OctoNetworkStreamTest()
        {
            _testStream = new OctoNetworkStream();
            _rand = new Random();
        }

        [TestMethod]
        public void WriteTest()
        {
            var buffer = new byte[500];
            _rand.NextBytes(buffer);
            
            _testStream.Write(buffer, 0, buffer.Length);
        }

        [TestMethod]
        public void ReadTest()
        {
            var buffer = new byte[500];
            var resultTest = new byte[500];
            _rand.NextBytes(buffer);

            _testStream.Write(buffer, 0, buffer.Length);
            _testStream.Read(resultTest, 0, resultTest.Length);

            Assert.AreEqual(buffer.Length, resultTest.Length);            
            Assert.IsTrue(buffer.SequenceEqual(resultTest));
        }

        [TestMethod]
        public void RingTest()
        {
            var buffer = new byte[500];
            var resultTest = new byte[500];
            _rand.NextBytes(buffer);

            _testStream.Write(buffer, 0, buffer.Length);
            _testStream.Read(resultTest, 0, resultTest.Length);

            buffer = new byte[600];
            resultTest = new byte[600];
            _rand.NextBytes(buffer);

            _testStream.Write(buffer, 0, buffer.Length);
            _testStream.Read(resultTest, 0, resultTest.Length);

            Assert.AreEqual(buffer.Length, resultTest.Length);
            Assert.IsTrue(buffer.SequenceEqual(resultTest));
        }
    }
}
