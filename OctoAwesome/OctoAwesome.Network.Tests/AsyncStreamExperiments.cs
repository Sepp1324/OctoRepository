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
            OctoNetworkStream test = new OctoNetworkStream();
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
            OctoNetworkStream test = new OctoNetworkStream();
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
            test.Write(_writeData, 0, _writeData.Length);

            Thread.Sleep(1000);
            _readTask.Start();

            Assert.IsTrue(_writeData.SequenceEqual(_readData));
        }

        
    }
}
