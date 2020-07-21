using CommandManagementSystem;
using OctoAwesome.Network;
using System;
using System.Linq;
using System.Net;
using System.Threading;

namespace OctoAwesome.GameServer
{
    class Program
    {
        public static Server Server { get; private set; }

        private static ManualResetEvent _manualResetEvent;
        private static DefaultCommandManager<ushort, byte[], byte[]> _defaultManager;

        static void Main(string[] args)
        {
            _defaultManager = new DefaultCommandManager<ushort, byte[], byte[]>(typeof(Program).Namespace + ".Commands");
            _manualResetEvent = new ManualResetEvent(false);
            Server = new Server();
            Server.OnClientConnected += ServerOnClientConnected;
            Console.WriteLine("Server started");
            Server.Start(IPAddress.Any, 8888);
            Console.CancelKeyPress += (s, e) => _manualResetEvent.Set();
            _manualResetEvent.WaitOne();
        }

        private static void ServerOnClientConnected(object sender, ConnectedClient e)
        {
            Console.WriteLine("Client connected");

            e.OnMessageRecived += (s, args) =>
            {
                var package = new Package(args.Data.Take(args.Count).ToArray());
                _defaultManager.DispatchAsync(package.Command, package.Payload);
            };
        }
    }
}
