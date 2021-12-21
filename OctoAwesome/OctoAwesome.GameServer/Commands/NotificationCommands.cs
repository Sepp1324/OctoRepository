using System;
using CommandManagementSystem.Attributes;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;

namespace OctoAwesome.GameServer.Commands
{
    public static class NotificationCommands
    {
        private static readonly IUpdateHub UpdateHub;
        private static readonly IPool<EntityNotification> EntityNotificationPool;
        private static readonly IPool<BlockChangedNotification> BlockChangedNotificationPool;
        private static readonly IPool<BlocksChangedNotification> BlocksChangedNotificationPool;

        static NotificationCommands()
        {
            UpdateHub = TypeContainer.Get<IUpdateHub>();
            EntityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();
            BlockChangedNotificationPool = TypeContainer.Get<IPool<BlockChangedNotification>>();
            BlocksChangedNotificationPool = TypeContainer.Get<IPool<BlocksChangedNotification>>();
        }

        [Command((ushort)OfficialCommand.EntityNotification)]
        public static byte[] EntityNotification(CommandParameter parameter)
        {
            var entityNotification = Serializer.DeserializePoolElement(EntityNotificationPool, parameter.Data);
            entityNotification.SenderId = parameter.ClientId;
            UpdateHub.Push(entityNotification, DefaultChannels.SIMULATION);
            UpdateHub.Push(entityNotification, DefaultChannels.NETWORK);
            entityNotification.Release();
            return null;
        }

        [Command((ushort)OfficialCommand.ChunkNotification)]
        public static byte[] ChunkNotification(CommandParameter parameter)
        {
            var notificationType = (BlockNotificationType)parameter.Data[0];
            Notification chunkNotification;
            switch (notificationType)
            {
                case BlockNotificationType.BlockChanged:
                    chunkNotification = Serializer.DeserializePoolElement(BlockChangedNotificationPool, parameter.Data);
                    break;
                case BlockNotificationType.BlocksChanged:
                    chunkNotification =
                        Serializer.DeserializePoolElement(BlocksChangedNotificationPool, parameter.Data);
                    break;
                default:
                    throw new NotSupportedException($"This Type is not supported: {notificationType}");
            }

            chunkNotification.SenderId = parameter.ClientId;
            UpdateHub.Push(chunkNotification, DefaultChannels.CHUNK);
            UpdateHub.Push(chunkNotification, DefaultChannels.NETWORK);
            chunkNotification.Release();

            return null;
        }
    }
}