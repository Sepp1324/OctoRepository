using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace OctoAwesome.Network
{
    public class Server //TODO: Should use a base class or interface
    {
        public event EventHandler<ConnectedClient> OnClientConnected;

        private readonly Socket ipV4Socket;
        private readonly Socket ipV6Socket;
        private readonly List<ConnectedClient> connectedClients;
        private readonly object lockObj;

        public Server()
        {
            ipV4Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipV6Socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            connectedClients = new List<ConnectedClient>();
            lockObj = new object();
        }

        public void Start(params IPEndPoint[] endpoints)
        {
            connectedClients.Clear();

            if (endpoints.Any(x => x.AddressFamily == AddressFamily.InterNetwork))
            {
                foreach (var endpoint in endpoints.Where(e => e.AddressFamily == AddressFamily.InterNetwork))
                    ipV4Socket.Bind(endpoint);

                ipV4Socket.Listen(1024);
                ipV4Socket.BeginAccept(OnClientAccepted, ipV4Socket);
            }
            if (endpoints.Any(x => x.AddressFamily == AddressFamily.InterNetworkV6))
            {
                foreach (var endpoint in endpoints.Where(e => e.AddressFamily == AddressFamily.InterNetworkV6))
                    ipV6Socket.Bind(endpoint);

                ipV6Socket.Listen(1024);
                ipV6Socket.BeginAccept(OnClientAccepted, ipV6Socket);
            }
        }

        public void Start(string host, ushort port)
        {
            var address = Dns.GetHostAddresses(host).Where(a => a.AddressFamily == ipV4Socket.AddressFamily || a.AddressFamily == ipV6Socket.AddressFamily);
            Start(address.Select(a => new IPEndPoint(a, port)).ToArray());
        }

        private void OnClientAccepted(IAsyncResult ar)
        {
            var socket = ar.AsyncState as Socket;
            var tmpSocket = socket.EndAccept(ar);

            tmpSocket.NoDelay = true;

            var client = new ConnectedClient(tmpSocket);
            client.Start();

            OnClientConnected?.Invoke(this, client);

            lock (lockObj)
                connectedClients.Add(client);
            socket.BeginAccept(OnClientAccepted, socket);
        }
    }
}
