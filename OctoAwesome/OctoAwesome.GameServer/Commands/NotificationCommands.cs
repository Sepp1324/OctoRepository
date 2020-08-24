using CommandManagementSystem.Attributes;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;

namespace OctoAwesome.GameServer.Commands
{
    public static class NotificationCommands
    {
        private static readonly IUpdateHub updateHub;

        static NotificationCommands() => updateHub = Program.ServerHandler.UpdateHub;

        [Command((ushort)OfficialCommand.EntityNotification)]
        public static byte[] EntityNotification(byte[] data)
        {
            var entityNotification = Serializer.Deserialize<EntityNotification>(data, null);
            updateHub.Push(entityNotification, DefaultChannels.Simulation);
            return null;
        }

        [Command((ushort)OfficialCommand.ChunkNotification)]
        public static byte[] ChunkNotification(byte[] data)
        {
            var chunkNotification = Serializer.Deserialize<ChunkNotification>(data, null);
            updateHub.Push(chunkNotification, DefaultChannels.Chunk);
            updateHub.Push(chunkNotification, DefaultChannels.Network);
            return null;
        }
    }
}
