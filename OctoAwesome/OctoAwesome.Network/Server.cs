using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace OctoAwesome.Network
{
    public class Server //TODO: Should use a base class or interface
    {
        private readonly List<ConnectedClient> _connectedClients;

        private readonly Socket _ipv4Socket;
        private readonly Socket _ipv6Socket;
        private readonly object _lockObj;

        public Server()
        {
            _ipv4Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _ipv6Socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            _connectedClients = new List<ConnectedClient>();
            _lockObj = new object();
        }

        public event EventHandler<ConnectedClient> OnClientConnected;

        public void Start(params IPEndPoint[] endpoints)
        {
            _connectedClients.Clear();

            if (endpoints.Any(x => x.AddressFamily == AddressFamily.InterNetwork))
            {
                foreach (var endpoint in endpoints.Where(e => e.AddressFamily == AddressFamily.InterNetwork))
                    _ipv4Socket.Bind(endpoint);

                _ipv4Socket.Listen(1024);
                _ipv4Socket.BeginAccept(OnClientAccepted, _ipv4Socket);
            }

            if (endpoints.Any(x => x.AddressFamily == AddressFamily.InterNetworkV6))
            {
                foreach (var endpoint in endpoints.Where(e => e.AddressFamily == AddressFamily.InterNetworkV6))
                    _ipv6Socket.Bind(endpoint);

                _ipv6Socket.Listen(1024);
                _ipv6Socket.BeginAccept(OnClientAccepted, _ipv6Socket);
            }
        }

        public void Start(string host, ushort port)
        {
            var address = Dns.GetHostAddresses(host).Where(
                a => a.AddressFamily == _ipv4Socket.AddressFamily || a.AddressFamily == _ipv6Socket.AddressFamily);

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

            lock (_lockObj)
                _connectedClients.Add(client);

            socket.BeginAccept(OnClientAccepted, socket);
        }
    }
}