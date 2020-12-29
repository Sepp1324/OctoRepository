using CommandManagementSystem.Attributes;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;
using System;

namespace OctoAwesome.GameServer.Commands
{
    public static class NotificationCommands
    {
        private static readonly IUpdateHub _updateHub;
        private static readonly IPool<EntityNotification> _entityNotificationPool;
        private static readonly IPool<BlockChangedNotification> _blockChangedNotificationPool;
        private static readonly IPool<BlocksChangedNotification> _blocksChangedNotificationPool;

        static NotificationCommands()
        {
            _updateHub = TypeContainer.Get<IUpdateHub>();
            _entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();
            _blockChangedNotificationPool = TypeContainer.Get<IPool<BlockChangedNotification>>();
            _blocksChangedNotificationPool = TypeContainer.Get<IPool<BlocksChangedNotification>>();
        }

        [Command((ushort) OfficialCommand.EntityNotification)]
        public static byte[] EntityNotification(CommandParameter parameter)
        {
            var entityNotification = Serializer.DeserializePoolElement(_entityNotificationPool, parameter.Data);
            entityNotification.SenderId = parameter.ClientId;
            _updateHub.Push(entityNotification, DefaultChannels.Simulation);
            _updateHub.Push(entityNotification, DefaultChannels.Network);
            entityNotification.Release();
            return null;
        }

        [Command((ushort) OfficialCommand.ChunkNotification)]
        public static byte[] ChunkNotification(CommandParameter parameter)
        {
            var notificationType = (BlockNotificationType) parameter.Data[0];
            Notification chunkNotification;
            switch (notificationType)
            {
                case BlockNotificationType.BlockChanged:
                    chunkNotification = Serializer.DeserializePoolElement(_blockChangedNotificationPool, parameter.Data);
                    break;
                case BlockNotificationType.BlocksChanged:
                    chunkNotification =
                        Serializer.DeserializePoolElement(_blocksChangedNotificationPool, parameter.Data);
                    break;
                default:
                    throw new NotSupportedException($"This Type is not supported: {notificationType}");
            }

            chunkNotification.SenderId = parameter.ClientId;
            _updateHub.Push(chunkNotification, DefaultChannels.Chunk);
            _updateHub.Push(chunkNotification, DefaultChannels.Network);
            chunkNotification.Release();

            return null;
        }
    }
}