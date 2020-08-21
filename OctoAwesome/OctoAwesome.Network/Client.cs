using System;
using System.Net.Sockets;
using System.Net;
using System.Linq;

namespace OctoAwesome.Network
{
    public class Client : BaseClient
    {        
        public Client() :base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
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
    }
}