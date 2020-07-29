 using System;

namespace OctoAwesome.Network
{
    public class Package
    {
        /// <summary>
        /// Bytesize of Header
        /// </summary>
        public const int HEAD_LENGTH = 11;
        public const int SUB_HEAD_LENGTH = HEAD_LENGTH + 10;
        public const int SUB_CONTENT_HEAD_LENGTH = 9;

        public static ulong NextUid => _nextUid++;

        private static ulong _nextUid;

        public PackageType Type { get; set; }

        public ushort Command { get; set; }

        public byte[] Payload { get; set; }

        public ulong Uid { get; set; }

        public bool IsImportantData { get; set; }

        public Package(ushort command, int size, PackageType type = 0, bool isImportantData = false) : this()
        {
            Type = type;
            Command = command;
            Payload = new byte[size];
            IsImportantData = isImportantData;
        }

        public Package()
        {

        }

        public Package(byte[] data) : this(0, data.Length)
        {

        }

        public void SerializePackage(OctoNetworkStream networkStream)
        {
            if (Payload.Length + HEAD_LENGTH > networkStream.Length)
            {
                SerializeSubPackages(networkStream);
                return;
            }
            Type = PackageType.Normal;
            WriteHead(networkStream);
            //networkStream.Write(_header, 0, _header.Length);
            networkStream.Write(Payload, 0, Payload.Length);
        }

        public void SerializeSubPackages(OctoNetworkStream networkStream)
        {
            var firstPackage = networkStream.Length - SUB_HEAD_LENGTH;
            var contentPackage = networkStream.Length - SUB_CONTENT_HEAD_LENGTH;
            var count = (int)Math.Round((double)((Payload.Length - firstPackage) / contentPackage), MidpointRounding.AwayFromZero);
            var offset = firstPackage;

            Type = PackageType.Subhead;

            WriteHead(networkStream);
            networkStream.Write(Payload, 0, firstPackage);
            Type = PackageType.Subcontent;

            for (int i = 0; i < count - 1; i++)
            {
                WriteHead(networkStream);
                //networkStream.Write(_header, 0, _header.Length);
                networkStream.Write(Payload, offset, contentPackage);
                offset += contentPackage;
            }

            WriteHead(networkStream);

            networkStream.Write(Payload, offset, Payload.Length - offset);
        }

        public void DeserializePackage(OctoNetworkStream networkStream)
        {
            ReadHead(networkStream);

            if (Type == PackageType.Subhead)
            {
                DeserializeSubPackages(networkStream);
                return;
            }
            networkStream.Read(Payload, 0, Payload.Length);
        }

        public void DeserializeSubPackages(OctoNetworkStream networkStream)
        {
            var firstPackage = networkStream.Length - SUB_HEAD_LENGTH;
            var contentPackage = networkStream.Length - SUB_CONTENT_HEAD_LENGTH;

            networkStream.Read(Payload, 0, 2);

            var count = Payload[0] << 8 | Payload[1];
            var buffer = new byte[8];

            networkStream.Read(Payload, 0, firstPackage);

            Type = PackageType.Subcontent;

            var offset = firstPackage + contentPackage;
            ulong uid;

            for (int i = 0; i < count; i++)
            {
                ReadHead(networkStream);
                networkStream.Read(buffer, 0, 8);
                uid = BitConverter.ToUInt64(buffer, 0);

                if (uid != Uid)
                    continue;

                networkStream.Read(Payload, offset, contentPackage);
                offset += contentPackage;
            }
            networkStream.Read(buffer, 0, 8);
            uid = BitConverter.ToUInt64(buffer, 0);

            networkStream.Read(Payload, offset, Payload.Length - offset);
        }

        public void WriteHead(OctoNetworkStream networkStream)
        {
            byte[] bytes;

            switch (Type)
            {
                case PackageType.Normal:
                    networkStream.Write((byte)Type);
                    networkStream.Write((byte)(Command >> 8));
                    networkStream.Write((byte)(Command & 0xF));

                    bytes = BitConverter.GetBytes(Payload.LongLength);
                    networkStream.Write(bytes, 0, bytes.Length);
                    break;
                case PackageType.Subhead:
                    networkStream.Write((byte)Type);
                    networkStream.Write((byte)(Command >> 8));
                    networkStream.Write((byte)(Command & 0xF));

                    bytes = BitConverter.GetBytes(Payload.LongLength);
                    networkStream.Write(bytes, 0, bytes.Length);
                    bytes = BitConverter.GetBytes(NextUid);
                    networkStream.Write(bytes, 0, bytes.Length);
                    break;
                case PackageType.Subcontent:
                    networkStream.Write((byte)Type);
                    bytes = BitConverter.GetBytes(Uid);
                    networkStream.Write(bytes, 0, bytes.Length);
                    break;
                case PackageType.None:
                    break;
                default:
                    break;
            }
        }

        public void ReadHead(OctoNetworkStream networkStream)
        {
            var buffer = new byte[1];
            networkStream.Read(buffer, 0, 1);
            Type = (PackageType)buffer[0];

            switch (Type)
            {
                case PackageType.Normal:
                    buffer = new byte[HEAD_LENGTH - 1];

                    networkStream.Read(buffer, 0, buffer.Length);
                    Command = (ushort)(buffer[0] << 8 | buffer[1]);
                    Payload = new byte[BitConverter.ToUInt64(buffer, 2)];
                    break;
                case PackageType.Subhead:
                    buffer = new byte[SUB_HEAD_LENGTH - 1];

                    networkStream.Read(buffer, 0, buffer.Length);
                    Command = (ushort)(buffer[0] << 8 | buffer[1]);
                    Payload = new byte[BitConverter.ToUInt64(buffer, 2)];
                    Uid = BitConverter.ToUInt64(buffer, 10);
                    break;
                case PackageType.Subcontent:
                case PackageType.None:
                    break;
                default:
                    break;
            }
        }

        public enum PackageType : byte
        {
            None,
            Normal,
            Subhead,
            Subcontent
        }
    }

}