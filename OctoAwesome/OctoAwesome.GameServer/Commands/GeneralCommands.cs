using CommandManagementSystem.Attributes;
using System.IO;

namespace OctoAwesome.GameServer.Commands
{
    public static class GeneralCommands
    {
        [Command((ushort)OfficialCommands.GetUniverse)]
        public static byte[] GetUniverse(byte[] data)
        {
            var universe = Program.ServerHandler.SimulationManager.GetUniverse();
            
            using (var memoryStream = new MemoryStream())
            {                
                universe.Serialize(memoryStream);
                return memoryStream.ToArray();
            }
        }

        [Command((ushort)OfficialCommands.GetPlanet)]
        public static byte[] GetPlanet(byte[] data)
        {
            var planet = Program.ServerHandler.SimulationManager.GetPlanet(0);

            using (var memoryStream = new MemoryStream())
            {
                planet.Serialize(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
