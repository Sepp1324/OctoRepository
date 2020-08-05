using System;
using System.Net;
using System.Threading;
using CommandManagementSystem;
using NLog;
using NLog.Config;
using NLog.Targets;
using OctoAwesome.Network;

namespace OctoAwesome.GameServer
{
    internal class Program
    {
        public static ServerHandler ServerHandler { get; set; }

        private static ManualResetEvent _manualResetEvent;
        private static Logger _logger;
        private static DefaultCommandManager<ushort, byte[], byte[]> _defaultManager;
        private static Server _server;
        private static PackageManager _packageManager;

        private static void Main(string[] args)
        {
            var config = new LoggingConfiguration();

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, new ColoredConsoleTarget("octoawesome.logconsole"));
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, new FileTarget("octoawesome.logfile") { FileName = "server.log" });

            LogManager.Configuration = config;
            _logger = LogManager.GetCurrentClassLogger(typeof(Program));

            _defaultManager = new DefaultCommandManager<ushort, byte[], byte[]>(typeof(Program).Namespace + ".Commands");
            _manualResetEvent = new ManualResetEvent(false);
            _server = new Server();
            _packageManager = new PackageManager();
            _packageManager.PackageAvailable += PackageManagerPackageAvailable;
            _server.OnClientConnected += ServerOnClientConnected;

            _logger.Info("Server started");
            ServerHandler = new ServerHandler(_server);
            _server.Start(IPAddress.Any, 8888);

            Console.CancelKeyPress += (s, e) => _manualResetEvent.Set();
            _manualResetEvent.WaitOne();
        }

        private static void PackageManagerPackageAvailable(object sender, OctoPackageAvailableEventArgs e)
        {
            if(e.Package.Command == 0 && e.Package.Payload.Length == 0)
            {
                _logger.Debug("Received null package");
                return;
            }
            _logger.Trace("Received new Package with ID: " + e.Package.Command);

            try
            {
                e.Package.Payload = _defaultManager.Dispatch(e.Package.Command, e.Package.Payload) ?? new byte[0];
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Dispatch failed in command " + e.Package.Command);
                return;
            }
            _logger.Trace(e.Package.Command);
            _packageManager.SendPackage(e.Package, e.BaseClient);
        }

        private static void ServerOnClientConnected(object sender, ConnectedClient e)
        {
            _logger.Debug("Client connected");
            _packageManager.AddConnectedClient(e);
        }
    }
}
