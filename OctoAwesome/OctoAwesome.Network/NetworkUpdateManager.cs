using OctoAwesome.Logging;
using OctoAwesome.Network.Pooling;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;
using OctoAwesome.Threading;
using System;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class NetworkUpdateManager : IAsyncObserver<Package>, INotificationObserver
    {
        private readonly Client _client;
        private readonly IUpdateHub _updateHub;
        private readonly ILogger _logger;
        private readonly IDisposable _networkSubscription;
        private readonly IDisposable _clientSubscription;
        private readonly IPool<EntityNotification> _entityNotificationPool;
        private readonly IPool<ChunkNotification> _chunkNotificationPool;
        private readonly PackagePool _packagePool;

        public NetworkUpdateManager(Client client, IUpdateHub updateHub)
        {
            _client = client;
            _updateHub = updateHub;

            _logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(NetworkUpdateManager));
            _entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();
            _chunkNotificationPool = TypeContainer.Get<IPool<ChunkNotification>>();
            _packagePool = TypeContainer.Get<PackagePool>();

            _networkSubscription = updateHub.Subscribe(this, DefaultChannels.Network);
            _clientSubscription = client.Subscribe(this);
            
        }

        public Task OnNext(Package package)
        {
            switch (package.OfficialCommand)
            {
                case OfficialCommand.EntityNotification:
                    var entityNotification = Serializer.DeserializePoolElement(_entityNotificationPool, package.Payload);
                    _updateHub.Push(entityNotification, DefaultChannels.Simulation);
                    entityNotification.Release();
                    break;
                case OfficialCommand.ChunkNotification:
                    var chunkNotification = Serializer.DeserializePoolElement(_chunkNotificationPool, package.Payload);
                    _updateHub.Push(chunkNotification, DefaultChannels.Chunk);
                    chunkNotification.Release();
                    break;
                default:
                    break;
            }
            return Task.CompletedTask;
        }

        public void OnNext(Notification value)
        {
            ushort command;
            byte[] payload;

            switch (value)
            {
                case EntityNotification entityNotification:
                    command = (ushort)OfficialCommand.EntityNotification;
                    payload = Serializer.Serialize(entityNotification);
                    break;
                case ChunkNotification chunkNotification:
                    command = (ushort)OfficialCommand.ChunkNotification;
                    payload = Serializer.Serialize(chunkNotification);
                    break;
                default:
                    return;
            }

            var package = _packagePool.Get();
            package.Command = command;
            package.Payload = payload;
            _client.SendPackageAndRelase(package);
        }

        public Task OnError(Exception error)
        {
            _logger.Error(error.Message, error);
            return Task.CompletedTask;
        }

        public Task OnCompleted()
        {
            _clientSubscription.Dispose();
            return Task.CompletedTask;
        }

        void INotificationObserver.OnCompleted()
        {
            //hubSubscription.Dispose();
        }

        void INotificationObserver.OnError(Exception error) => _logger.Error(error.Message, error);
    }
}