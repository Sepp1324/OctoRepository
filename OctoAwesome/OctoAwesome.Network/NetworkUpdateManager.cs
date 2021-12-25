﻿using System;
using System.Threading.Tasks;
using OctoAwesome.Logging;
using OctoAwesome.Network.Pooling;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;
using OctoAwesome.Threading;
using OctoAwesome.Rx;

namespace OctoAwesome.Network
{
    public class NetworkUpdateManager : IAsyncObserver<Package>, IDisposable
    {
        private readonly IPool<BlockChangedNotification> _blockChangedNotificationPool;
        private readonly IPool<BlocksChangedNotification> _blocksChangedNotificationPool;
        private readonly IPool<EntityNotification> _entityNotificationPool;

        private readonly Client _client;

        private readonly IDisposable _clientSubscription;
        private readonly IDisposable _hubSubscription;
        private readonly IDisposable _simulationSource;
        private readonly IDisposable _chunkSource;
        
        private readonly ILogger _logger;
        private readonly PackagePool _packagePool;

        private readonly Relay<Notification> _simulationRelay;
        private readonly Relay<Notification> _chunkRelay;

        public NetworkUpdateManager(Client client, IUpdateHub updateHub)
        {
            _client = client;

            _logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(NetworkUpdateManager));
            _entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();
            _blockChangedNotificationPool = TypeContainer.Get<IPool<BlockChangedNotification>>();
            _blocksChangedNotificationPool = TypeContainer.Get<IPool<BlocksChangedNotification>>();
            _packagePool = TypeContainer.Get<PackagePool>();

            _simulationRelay = new();
            _chunkRelay = new();

            _hubSubscription = updateHub.ListenOn(DefaultChannels.NETWORK).Subscribe(OnNext, error => _logger.Error(error.Message, error));
            _simulationSource = updateHub.AddSource(_simulationRelay, DefaultChannels.SIMULATION);
            _chunkSource = updateHub.AddSource(_chunkRelay, DefaultChannels.CHUNK);
            _clientSubscription = client.Subscribe(this);
        }

        public Task OnNext(Package package)
        {
            switch (package.OfficialCommand)
            {
                case OfficialCommand.EntityNotification:
                    var entityNotification = Serializer.DeserializePoolElement(_entityNotificationPool, package.Payload);
                    _simulationRelay.OnNext(entityNotification);
                    entityNotification.Release();
                    break;
                case OfficialCommand.ChunkNotification:
                    var notificationType = (BlockNotificationType)package.Payload[0];
                    Notification chunkNotification = notificationType switch
                    {
                        BlockNotificationType.BlockChanged => Serializer.DeserializePoolElement(_blockChangedNotificationPool, package.Payload),
                        BlockNotificationType.BlocksChanged => Serializer.DeserializePoolElement(_blocksChangedNotificationPool, package.Payload),
                        _ => throw new NotSupportedException($"This Type is not supported: {notificationType}")
                    };

                    _chunkRelay.OnNext(chunkNotification);
                    chunkNotification.Release();
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
                case BlockChangedNotification chunkNotification:
                    command = (ushort)OfficialCommand.ChunkNotification;
                    payload = Serializer.Serialize(chunkNotification);
                    break;
                default:
                    return;
            }

            var package = _packagePool.Get();
            package.Command = command;
            package.Payload = payload;
            _client.SendPackageAndRelease(package);
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

        public void Dispose()
        {
            _hubSubscription?.Dispose();
            _simulationSource?.Dispose();
            _chunkSource?.Dispose();
            _simulationRelay?.Dispose();
            _chunkRelay?.Dispose();
        }
    }
}