using CommandManagementSystem;
using OctoAwesome.Logging;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Runtime;
using OctoAwesome.Threading;
using System;
using System.Net;
using System.Threading.Tasks;

namespace OctoAwesome.GameServer
{
    public class ServerHandler : IAsyncObserver<Package>
    {
        public SimulationManager SimulationManager { get; set; }
        public IUpdateHub UpdateHub { get; private set; }

<<<<<<< HEAD
        private readonly ILogger _logger;
        private readonly Server _server;
=======
        private readonly ILogger logger;
        private readonly Server server;
>>>>>>> feature/performance
        private readonly DefaultCommandManager<ushort, CommandParameter, byte[]> defaultManager;

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

            defaultManager = new DefaultCommandManager<ushort, CommandParameter, byte[]>(typeof(ServerHandler).Namespace + ".Commands");
        }

        public void Start()
        {
            SimulationManager.Start(); //Temp
<<<<<<< HEAD
            _server.Start(new IPEndPoint(IPAddress.Any, 8888), new IPEndPoint(IPAddress.IPv6Any, 8888));
            _server.OnClientConnected += ServerOnClientConnected;
=======
            server.Start(new IPEndPoint(IPAddress.Any, 8888), new IPEndPoint(IPAddress.IPv6Any, 8888));
            server.OnClientConnected += ServerOnClientConnected;
>>>>>>> feature/performance
        }

        private void ServerOnClientConnected(object sender, ConnectedClient e)
        {
<<<<<<< HEAD
            _logger.Debug("Hurra ein neuer Spieler");
=======
            logger.Debug("Hurra ein neuer Spieler");
>>>>>>> feature/performance
            e.ServerSubscription = e.Subscribe(this);
            e.NetworkChannelSubscription = UpdateHub.Subscribe(e, DefaultChannels.Network);
        }

        public async Task OnNext(Package value)
        {
            if (value.Command == 0 && value.Payload.Length == 0)
            {
                _logger.Debug("Received null package");
                return;
            }
<<<<<<< HEAD
            _logger.Trace("Received a new Package with ID: " + value.UId);
=======
            logger.Trace("Received a new Package with ID: " + value.UId);
>>>>>>> feature/performance
            try
            {
                value.Payload = defaultManager.Dispatch(value.Command, new CommandParameter(value.BaseClient.Id, value.Payload));
            }
            catch (Exception ex)
            {
                _logger.Error("Dispatch failed in Command " + value.OfficialCommand, ex);
                return;
            }

            _logger.Trace(value.OfficialCommand);

            if (value.Payload == null)
            {
<<<<<<< HEAD
                _logger.Trace($"Payload is null, returning from Command {value.OfficialCommand} without sending return package.");
=======
                logger.Trace($"Payload is null, returning from Command {value.OfficialCommand} without sending return package.");
>>>>>>> feature/performance
                return;
            }

           await value.BaseClient.SendPackageAsync(value);
        }

        public Task OnError(Exception error)
        {
            _logger.Error(error.Message, error);
            return Task.CompletedTask;
        }
<<<<<<< HEAD

        public Task OnCompleted() => Task.CompletedTask;
=======
>>>>>>> feature/performance
    }
}
