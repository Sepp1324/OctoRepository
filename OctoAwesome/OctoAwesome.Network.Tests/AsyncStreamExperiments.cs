﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OctoAwesome.Network.Tests
{
    public class AsyncStreamExperiments
    {
        [Test]
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

        [Test]
        public async Task ReadWriteSwapAsync()
        {
            var test = new OctoNetworkStream();
            var writeData = new byte[400];
            var readData = new byte[400];
            var r = new Random();

            r.NextBytes(writeData);
            var readTask = new Task(() =>
            {
                var o = 0;
                while (test.Read(readData, o, 100) != 0)
                {
                    Thread.Sleep(200);
                    o += 100;
                }
            });
            test.Write(writeData, 0, writeData.Length);

            Thread.Sleep(100);
            readTask.Start();

            //Task writeTask = new Task(() => {
            //    for (int i = 0; i < 5; i++)
            //    {
            //        Thread.Sleep(1000);
            //    }

            //});
            Assert.IsTrue(writeData.SequenceEqual(readData));
        }
    }
}