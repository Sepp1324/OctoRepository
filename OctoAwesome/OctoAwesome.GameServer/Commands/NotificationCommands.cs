using CommandManagementSystem.Attributes;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;

namespace OctoAwesome.GameServer.Commands
{
    public static class NotificationCommands
    {
        private static readonly IUpdateHub updateHub;

        static NotificationCommands()
        {
            updateHub = Program.ServerHandler.UpdateHub;
        }

        [Command((ushort)OfficialCommand.EntityNotification)]
        public static byte[] EntityNotification(CommandParameter parameter)
        {
            var entityNotification = Serializer.Deserialize<EntityNotification>(parameter.Data);
            entityNotification.SenderId = parameter.ClientId;
            updateHub.Push(entityNotification, DefaultChannels.Simulation);
            updateHub.Push(entityNotification, DefaultChannels.Network);
            return null;
        }

        [Command((ushort)OfficialCommand.ChunkNotification)]
        public static byte[] ChunkNotification(CommandParameter parameter)
        {
            var chunkNotification = Serializer.Deserialize<ChunkNotification>(parameter.Data);
            chunkNotification.SenderId = parameter.ClientId;
            updateHub.Push(chunkNotification, DefaultChannels.Chunk);
            updateHub.Push(chunkNotification, DefaultChannels.Network);
            return null;
        }
    }
}
