using System;
using System.Net;
using System.Threading.Tasks;
using CommandManagementSystem;
using OctoAwesome.Logging;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Threading;

namespace OctoAwesome.GameServer
{
    public class ServerHandler : IAsyncObserver<Package>
    {
        private readonly DefaultCommandManager<ushort, CommandParameter, byte[]> _defaultManager;

        private readonly ILogger _logger;
        private readonly Server _server;

        public ServerHandler()
        {
            _logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(ServerHandler));

            TypeContainer.Register<UpdateHub>(InstanceBehaviour.Singleton);
            TypeContainer.Register<IUpdateHub, UpdateHub>(InstanceBehaviour.Singleton);
            TypeContainer.Register<Server>(InstanceBehaviour.Singleton);
            TypeContainer.Register<SimulationManager>(InstanceBehaviour.Singleton);

            SimulationManager = TypeContainer.Get<SimulationManager>();
            UpdateHub = TypeContainer.Get<IUpdateHub>();
            _server = TypeContainer.Get<Server>();

            _defaultManager = new(typeof(ServerHandler).Namespace + ".Commands");
        }

        public SimulationManager SimulationManager { get; set; }

        public IUpdateHub UpdateHub { get; }

        public async Task OnNext(Package value)
        {
            if (value.Command == 0 && value.Payload.Length == 0)
            {
                _logger.Debug("Received null package");
                return;
            }

            _logger.Trace("Received a new Package with ID: " + value.UId);
            try
            {
                value.Payload = _defaultManager.Dispatch(value.Command, new(value.BaseClient.Id, value.Payload));
            }
            catch (Exception ex)
            {
                _logger.Error("Dispatch failed in Command " + value.OfficialCommand, ex);
                return;
            }

            _logger.Trace(value.OfficialCommand);

            if (value.Payload == null)
            {
                _logger.Trace(
                    $"Payload is null, returning from Command {value.OfficialCommand} without sending return package.");
                return;
            }

            await value.BaseClient.SendPackageAsync(value);
        }

        public Task OnError(Exception error)
        {
            _logger.Error(error.Message, error);
            return Task.CompletedTask;
        }

        public Task OnCompleted() => Task.CompletedTask;

        public void Start()
        {
            SimulationManager.Start(); //Temp
            _server.Start(new IPEndPoint(IPAddress.Any, 8888), new IPEndPoint(IPAddress.IPv6Any, 8888));
            _server.OnClientConnected += ServerOnClientConnected;
        }

        private void ServerOnClientConnected(object sender, ConnectedClient e)
        {
            _logger.Debug("Hurra ein neuer Spieler");
            e.ServerSubscription = e.Subscribe(this);
        }
    }
}