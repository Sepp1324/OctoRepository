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

        private readonly ILogger logger;
        private readonly Server server;
        private readonly DefaultCommandManager<ushort, CommandParameter, byte[]> defaultManager;

        public ServerHandler()
        {
            logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(ServerHandler));

            TypeContainer.Register<UpdateHub>(InstanceBehaviour.Singleton);
            TypeContainer.Register<IUpdateHub, UpdateHub>(InstanceBehaviour.Singleton);
            TypeContainer.Register<Server>(InstanceBehaviour.Singleton);
            TypeContainer.Register<SimulationManager>(InstanceBehaviour.Singleton);

            SimulationManager = TypeContainer.Get<SimulationManager>();
            UpdateHub = TypeContainer.Get<IUpdateHub>();
            server = TypeContainer.Get<Server>();

            defaultManager = new DefaultCommandManager<ushort, CommandParameter, byte[]>(typeof(ServerHandler).Namespace + ".Commands");
        }

        public void Start()
        {
            SimulationManager.Start(); //Temp
            server.Start(new IPEndPoint(IPAddress.Any, 8888), new IPEndPoint(IPAddress.IPv6Any, 8888));
            server.OnClientConnected += ServerOnClientConnected;
        }

        private void ServerOnClientConnected(object sender, ConnectedClient e)
        {
            logger.Debug("Hurra ein neuer Spieler");
            e.ServerSubscription = e.Subscribe(this);
            e.NetworkChannelSubscription = UpdateHub.Subscribe(e, DefaultChannels.Network);
        }

        public Task OnNext(Package value)
        {
            if (value.Command == 0 && value.Payload.Length == 0)
            {
                logger.Debug("Received null package");
                return Task.CompletedTask;
            }
            logger.Trace("Received a new Package with ID: " + value.UId);
            try
            {
                value.Payload = defaultManager.Dispatch(value.Command, new CommandParameter(value.BaseClient.Id, value.Payload));
            }
            catch (Exception ex)
            {
                logger.Error("Dispatch failed in Command " + value.OfficialCommand, ex);
                return Task.CompletedTask;
            }

            logger.Trace(value.OfficialCommand);

            if (value.Payload == null)
            {
                logger.Trace($"Payload is null, returning from Command {value.OfficialCommand} without sending return package.");
                return Task.CompletedTask;
            }
            value.BaseClient.SendPackage(value);
            return Task.CompletedTask;
        }

        public Task OnError(Exception error)
        {
            logger.Error(error.Message, error);
            return Task.CompletedTask;
        }

        public Task OnCompleted() => Task.CompletedTask;
    }
}
