﻿using System.IO;
using CommandManagementSystem.Attributes;
using OctoAwesome.Network;

namespace OctoAwesome.GameServer.Commands
{
    public static class GeneralCommands
    {
        [Command((ushort)OfficialCommand.GetUniverse)]
        public static byte[] GetUniverse(CommandParameter parameter)
        {
            var universe = TypeContainer.Get<SimulationManager>().GetUniverse();

            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);
            universe.Serialize(writer);
            return memoryStream.ToArray();
        }

        [Command((ushort)OfficialCommand.GetPlanet)]
        public static byte[] GetPlanet(CommandParameter parameter)
        {
            var planet = TypeContainer.Get<SimulationManager>().GetPlanet(0);

            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);
            planet.Serialize(writer);
            return memoryStream.ToArray();
        }
    }
}