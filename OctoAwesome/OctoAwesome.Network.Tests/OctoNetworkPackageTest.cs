using System;
using System.Linq;
using NUnit.Framework;

namespace OctoAwesome.Network.Tests
{
    [TestOf(typeof(Package))]
    public class OctoNetworkPackageTest
    {
        private OctoNetworkStream networkStream;
        private Package package;

        [Test]
        public void PackageNormal()
        {
            package = new Package(0, 100);
            var packageDes = new Package(0, 100);

            var r = new Random();

            networkStream = new OctoNetworkStream(200);
            r.NextBytes(package.Payload);

            //package.SerializePackage(networkStream);

            //packageDes.DeserializePackage(networkStream);

            Assert.IsTrue(packageDes.Payload.SequenceEqual(package.Payload));
            Assert.AreEqual(packageDes.Command, package.Command);
        }

        [Test]
        public void PackageWithSubPackages()
        {
            package = new Package(0, 1000);
            var packageDes = new Package(0, 1000);

            var r = new Random();

            networkStream = new OctoNetworkStream(100);
            r.NextBytes(package.Payload);

            //package.SerializePackage(networkStream);

            //packageDes.DeserializePackage(networkStream);

            Assert.IsTrue(packageDes.Payload.SequenceEqual(package.Payload));
            Assert.AreEqual(packageDes.Command, package.Command);
        }

        [Test]
        public void TestReadWriteStream()
        {
        }
    }
}