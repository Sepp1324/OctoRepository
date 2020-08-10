using System;
using System.Net.Sockets;
using System.Buffers;
using System.Net;
using System.Text;
using System.Linq;

namespace OctoAwesome.Network
{
    public class Client : BaseClient
    {
        public bool IsClient { get; set; }
        public event EventHandler<Package> PackageAvailable;

        private Package _currentPackage;
        private static int _clientReceived;

        public Client() :
            base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
        }

        public void SendPing()
        {
            var buffer = ArrayPool<byte>.Shared.Rent(4);
            Encoding.UTF8.GetBytes("PING", 0, 4, buffer, 0);
            SendAsync(buffer, 4);
        }

        public void Connect(string host, ushort port)
        {
            var address = Dns.GetHostAddresses(host).FirstOrDefault(
                a => a.AddressFamily == Socket.AddressFamily);

            Socket.BeginConnect(new IPEndPoint(address, port), OnConnected, null);
        }

        private void OnConnected(IAsyncResult ar)
        {
            Socket.EndConnect(ar);

            while (true)
            {
                if (Socket.ReceiveAsync(ReceiveArgs))
                    return; 

                Receive(ReceiveArgs);
            }
        }

        internal void SendPackage(Package package)
        {
            byte[] bytes = new byte[package.Payload.Length + Package.HEAD_LENGTH];
            package.SerializePackage(bytes);
            SendAsync(bytes, bytes.Length);
        }

        private void ClientDataAvailable(OctoNetworkEventArgs e)
        {
            byte[] bytes = new byte[e.DataCount];
            if (_currentPackage == null)
            {
                _currentPackage = new Package();
                if (e.DataCount >= Package.HEAD_LENGTH)
                {
                    e.NetworkStream.Read(bytes, 0, Package.HEAD_LENGTH);
                    _currentPackage.TryDeserializeHeader(bytes);
                    e.DataCount -= Package.HEAD_LENGTH;
                }
            }

            e.NetworkStream.Read(bytes, 0, e.DataCount);
            _currentPackage.DeserializePayload(bytes, 0, e.DataCount);

            if (_currentPackage.IsComplete)
            {
                PackageAvailable?.Invoke(this, _currentPackage);
                _currentPackage = null;
            }
        }
    }
}