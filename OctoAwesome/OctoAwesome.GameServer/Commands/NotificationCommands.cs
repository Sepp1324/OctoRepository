using CommandManagementSystem.Attributes;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;

namespace OctoAwesome.GameServer.Commands
{
    public static class NotificationCommands
    {
        private static readonly IUpdateHub _updateHub;
        private static readonly IPool<EntityNotification> _entityNotificationPool;
        private static readonly IPool<BlockChangedNotification> _chunkNotificationPool;

        static NotificationCommands()
        {
            _updateHub = TypeContainer.Get<IUpdateHub>();
            _entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();
            _chunkNotificationPool = TypeContainer.Get<IPool<BlockChangedNotification>>();
        }

        [Command((ushort)OfficialCommand.EntityNotification)]
        public static byte[] EntityNotification(CommandParameter parameter)
        {
            var entityNotification = Serializer.DeserializePoolElement(_entityNotificationPool, parameter.Data);
            entityNotification.SenderId = parameter.ClientId;
            _updateHub.Push(entityNotification, DefaultChannels.Simulation);
            _updateHub.Push(entityNotification, DefaultChannels.Network);
            entityNotification.Release();
            return null;
        }

        [Command((ushort)OfficialCommand.ChunkNotification)]
        public static byte[] ChunkNotification(CommandParameter parameter)
        {
            var chunkNotification = Serializer.DeserializePoolElement(_chunkNotificationPool, parameter.Data);
            chunkNotification.SenderId = parameter.ClientId;
            _updateHub.Push(chunkNotification, DefaultChannels.Chunk);
            _updateHub.Push(chunkNotification, DefaultChannels.Network);
            chunkNotification.Release();

            return null;
        }
    }
}
