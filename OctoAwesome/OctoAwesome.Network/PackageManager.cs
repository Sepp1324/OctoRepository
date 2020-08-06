using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace OctoAwesome.Network
{
    public class PackageManager
    {
        public List<BaseClient> ConnectedClients { get; set; }
        private Dictionary<BaseClient, Package> _packages;
        public event EventHandler<OctoPackageAvailableEventArgs> PackageAvailable;
        private byte[] _receiveBuffer;


        public PackageManager()
        {
            _packages = new Dictionary<BaseClient, Package>();
            ConnectedClients = new List<BaseClient>();
            //_receiveBuffer = new byte[0];
        }

        public void AddConnectedClient(BaseClient client) => client.DataAvailable += ClientDataAvailable;

        public void SendPackage(Package package, BaseClient client)
        {
            byte[] bytes = new byte[package.Payload.Length + Package.HEAD_LENGTH];
            package.SerializePackage(bytes);
            client.SendAsync(bytes, bytes.Length);
        }

        private void ClientDataAvailable(object sender, OctoNetworkEventArgs e)
        {
            var baseClient = (BaseClient)sender;

            byte[] bytes = new byte[e.DataCount];

            if (!_packages.TryGetValue(baseClient, out Package package))
            {
                package = new Package();
                _packages.Add(baseClient, package);

                //if (e.DataCount >= Package.HEAD_LENGTH)
                //{
                int current = 0;


                current = e.NetworkStream.Read(bytes, current, Package.HEAD_LENGTH - current);

                if (current != Package.HEAD_LENGTH)
                    Console.WriteLine($"Package wos not complete; Bytes received: {current}");

                package.TryDeserializeHeader(bytes);
                e.DataCount -= Package.HEAD_LENGTH;
                //}
                //else
                //{
                //    _receiveBuffer = new byte[e.DataCount];
                //    e.NetworkStream.Read(_receiveBuffer, 0, e.DataCount);
                //    _packages.Remove(baseClient);
                //    return;
                //}
            }

            e.NetworkStream.Read(bytes, 0, e.DataCount);
            var count = package.DeserializePayload(bytes, 0, e.DataCount);

            if (package.IsComplete)
            {
                _packages.Remove(baseClient);

                PackageAvailable?.Invoke(this, new OctoPackageAvailableEventArgs { BaseClient = baseClient, Package = package });

                if (e.DataCount - count > 0)
                    ClientDataAvailable(sender, new OctoNetworkEventArgs() { DataCount = e.DataCount - count, NetworkStream = e.NetworkStream });
            }
        }
    }
}
