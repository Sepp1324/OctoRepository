using CommandManagementSystem.Attributes;
using System;
using System.Text;

namespace OctoAwesome.GameServer.Commands
{
    public static class PlayerCommands
    {
        [Command((ushort)10)]
        public static byte[] Whoami(byte[] data)
        {
            string playerName = Encoding.UTF8.GetString(data);
            Console.WriteLine(playerName);
            return new byte[0];
        }
    }
}
