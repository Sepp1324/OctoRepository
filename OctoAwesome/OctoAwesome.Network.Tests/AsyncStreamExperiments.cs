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
            var test = new OctoNetworkStream();
            var writeData = new byte[400];
            var readData = new byte[400];
            var r = new Random();

            r.NextBytes(writeData);

            test.Write(writeData, 0, writeData.Length);
            test.Read(readData, 0, readData.Length);

            Assert.IsTrue(writeData.SequenceEqual(readData));
        }

        [TestMethod]
        public async Task ReadWriteSwapAsync()
        {
            var test = new OctoNetworkStream();
            var writeData = new byte[400];
            var readData = new byte[400];
            var r = new Random();

            r.NextBytes(writeData);
            
            var readTask = new Task(() =>
            {
                int o = 0;
                while (test.Read(readData, o, 100) != 0)
                {
                    Thread.Sleep(200);
                    o += 100;
                }
            });
            test.Write(writeData, 0, writeData.Length);

            Thread.Sleep(100);
            readTask.Start();

            Assert.IsTrue(writeData.SequenceEqual(readData));
        }
    }
}
