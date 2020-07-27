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
            _package = new Package(0, 1000);
            Package packageDes = new Package(0, 1000);

            Random random = new Random();
            random.NextBytes(_package.Payload);

            _networkStream = new OctoNetworkStream(100);

            _package.SerializePackage(_networkStream);
            packageDes.DeserializePackage(_networkStream);

            Assert.IsTrue(packageDes.Payload.SequenceEqual(_package.Payload));
            Assert.AreEqual(packageDes.Command, _package.Command);
            Assert.AreEqual(packageDes.Uid, _package.Uid);
            Assert.AreEqual(packageDes.Type, _package.Type);
        }
    }
}
