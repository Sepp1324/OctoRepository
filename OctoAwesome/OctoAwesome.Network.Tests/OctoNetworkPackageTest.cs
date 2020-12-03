using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OctoAwesome.Network.Tests
{
    [TestClass]
    public class OctoNetworkPackageTest
    {
        private Package _package;
        private OctoNetworkStream _networkStream;

        [TestMethod]
        public void PackageNormal()
        {
            _package = new Package(0, 100);
            
            var packageDes = new Package(0, 100);
            var r = new Random();

            _networkStream = new OctoNetworkStream(200);
            r.NextBytes(_package.Payload);

            Assert.IsTrue(packageDes.Payload.SequenceEqual(_package.Payload));
            Assert.AreEqual(packageDes.Command, _package.Command);
        }

        [TestMethod]
        public void PackageWithSubPackages()
        {
            _package = new Package(0, 1000);
            
            var packageDes = new Package(0, 1000);
            var r = new Random();

            _networkStream = new OctoNetworkStream(100);
            r.NextBytes(_package.Payload);

            Assert.IsTrue(packageDes.Payload.SequenceEqual(_package.Payload));
            Assert.AreEqual(packageDes.Command, _package.Command);
        }

        [TestMethod]
        public void TestReadWriteStream()
        {

        }
    }
}
