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
        public static ServerHandler ServerHandler { get; set; }

        private static ManualResetEvent _manualResetEvent;
        private static DefaultCommandManager<ushort, byte[], byte[]> _defaultManager;
        private static Server _server;

        static void Main(string[] args)
        {
            _defaultManager = new DefaultCommandManager<ushort, byte[], byte[]>(typeof(Program).Namespace + ".Commands");
            _manualResetEvent = new ManualResetEvent(false);
            _server = new Server();
            _server.OnClientConnected += ServerOnClientConnected;
            Console.WriteLine("Server started");
            ServerHandler = new ServerHandler(_server);
            _server.Start(IPAddress.Any, 8888);

            Console.CancelKeyPress += (s, e) => _manualResetEvent.Set();
            _manualResetEvent.WaitOne();
        }

        private static void ServerOnClientConnected(object sender, ConnectedClient e)
        {
            Console.WriteLine("Client connected");

            e.PackageReceived += (s, package) =>
            {

                package.Payload = _defaultManager.Dispatch(package.Command, package.Payload);

                Console.WriteLine(package.Command);
                if (package.Command != 12)
                    ;

                e.SendAsync(package);
            };
        }
    }
}