using CommandManagementSystem.Attributes;
using engenious;
using OctoAwesome.EntityComponents;
using OctoAwesome.Network;
using System;
using System.IO;
using System.Text;

namespace OctoAwesome.GameServer.Commands
{
    public static class PlayerCommands
    {
        [Command((ushort)OfficialCommands.Whoami)]
        public static byte[] Whoami(byte[] data)
        {
            string playername = Encoding.UTF8.GetString(data);
            var player = new Player();

            player.Components.AddComponent(new PositionComponent { Position = new Coordinate(0, new Index3(0, 0, 0), new Vector3(0, 0, 0)) });

            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                player.Serialize(bw, Program.ServerHandler.SimulationManager.DefinitionManager);
                Console.WriteLine(playername);
                return ms.ToArray();
            }
        }
    }
}
