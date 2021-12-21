using System;
using System.IO;
using CommandManagementSystem.Attributes;
using OctoAwesome.Network;

namespace OctoAwesome.GameServer.Commands
{
    public class ChunkCommands
    {
        [Command((ushort)OfficialCommand.LoadColumn)]
        public static byte[] LoadColumn(CommandParameter parameter)
        {
            Guid guid;
            int planetId;
            Index2 index2;

            using (var memoryStream = new MemoryStream(parameter.Data))
            using (var reader = new BinaryReader(memoryStream))
            {
                guid = new(reader.ReadBytes(16));
                planetId = reader.ReadInt32();
                index2 = new(reader.ReadInt32(), reader.ReadInt32());
            }

            var column = TypeContainer.Get<SimulationManager>().LoadColumn(planetId, index2);

            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream))
            {
                column.Serialize(writer);
                return memoryStream.ToArray();
            }
        }

        [Command((ushort)OfficialCommand.SaveColumn)]
        public static byte[] SaveColumn(CommandParameter parameter)
        {
            var chunkColumn = new ChunkColumn(null);

            using (var memoryStream = new MemoryStream(parameter.Data))
            using (var reader = new BinaryReader(memoryStream))
            {
                chunkColumn.Deserialize(reader);
            }

            TypeContainer.Get<SimulationManager>().Simulation.ResourceManager.SaveChunkColumn(chunkColumn);

            return null;
        }
    }
}