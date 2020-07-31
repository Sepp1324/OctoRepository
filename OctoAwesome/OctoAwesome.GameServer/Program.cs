using System;
using System.Net;
using System.Threading;
using CommandManagementSystem;
using OctoAwesome.Network;

namespace OctoAwesome.GameServer
{
    internal class Program
    {
        public static ServerHandler ServerHandler { get; set; }

        private static ManualResetEvent _manualResetEvent;
        private static DefaultCommandManager<ushort, byte[], byte[]> _defaultManager;
        private static Server _server;
        private static PackageManager _packageManager;

        private static void Main(string[] args)
        {
            _defaultManager = new DefaultCommandManager<ushort, byte[], byte[]>(typeof(Program).Namespace + ".Commands");
            _manualResetEvent = new ManualResetEvent(false);
            _server = new Server();
            _packageManager = new PackageManager();
            _packageManager.PackageAvailable += PackageManagerPackageAvailable;
            _server.OnClientConnected += ServerOnClientConnected;
            Console.WriteLine("Server started");
            ServerHandler = new ServerHandler(_server);
            _server.Start(IPAddress.Any, 8888);

            Console.CancelKeyPress += (s, e) => _manualResetEvent.Set();
            _manualResetEvent.WaitOne();
        }

        private static void PackageManagerPackageAvailable(object sender, OctoPackageAvailableEventArgs e)
        {
            e.Package.Payload = _defaultManager.Dispatch(e.Package.Command, e.Package.Payload);
            Console.WriteLine(e.Package.Command);
            _packageManager.SendPackage(e.Package, e.BaseClient);
        }

        private static void ServerOnClientConnected(object sender, ConnectedClient e)
        {
            Console.WriteLine("Client connected");
            _packageManager.AddConnectedClient(e);
        }
    }
}
