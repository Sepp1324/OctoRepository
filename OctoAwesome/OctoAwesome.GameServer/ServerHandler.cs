using CommandManagementSystem;
using NLog;
using OctoAwesome.Network;
using System;
using System.Net;

namespace OctoAwesome.GameServer
{
    public class ServerHandler
    {
        public SimulationManager SimulationManager { get; set; }

        private readonly Logger _logger;
        private readonly Server _server;
        private readonly PackageManager _packageManager;
        private readonly DefaultCommandManager<ushort, byte[], byte[]> _defaultManager;

        public ServerHandler()
        {
            _logger = LogManager.GetCurrentClassLogger();

            _server = new Server();
            SimulationManager = new SimulationManager(new Settings());
            _packageManager = new PackageManager();
            _defaultManager = new DefaultCommandManager<ushort, byte[], byte[]>(typeof(ServerHandler).Namespace + ".Commands");
        }

        public void Start()
        {
            _packageManager.PackageAvailable += PackageManagerPackageAvailable;
            _packageManager.Start();

            _server.Start(IPAddress.Any, 8888);
            _server.OnClientConnected += ServerOnClientConnected;
        }

        private void PackageManagerPackageAvailable(object sender, OctoPackageAvailableEventArgs e)
        {
            if (e.Package.Command == 0 && e.Package.Payload.Length == 0)
            {
                _logger.Debug("Received null package");
                return;
            }
            _logger.Trace("Received a new Package with ID: " + e.Package.UId);
            try
            {
                e.Package.Payload = _defaultManager.Dispatch(e.Package.Command, e.Package.Payload) ?? new byte[0];
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Dispatch failed in Command " + e.Package.Command);
                return;
            }

            _logger.Trace(e.Package.Command);
            _packageManager.SendPackage(e.Package, e.BaseClient);
        }

        private void ServerOnClientConnected(object sender, ConnectedClient e)
        {
            _logger.Debug("Hurra ein neuer Spieler");
            _packageManager.AddConnectedClient(e);
        }

    }
}
