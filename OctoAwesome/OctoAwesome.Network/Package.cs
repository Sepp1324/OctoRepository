using System;

namespace OctoAwesome.Network
{
    public class Package
    {
        /// <summary>
        /// Bytesize of Header
        /// </summary>
        public const int HEAD_LENGTH = 3;
        public const int SUB_HEAD_LENGTH = HEAD_LENGTH + 10;
        public const int SUB_CONTENT_HEAD_LENGTH = 9;

        public static ulong NextUid => _nextUid++;

        private static ulong _nextUid;

        public PackageType Type { get; set; }

        public ushort Command { get; set; }

        public byte[] Payload { get; set; }

        public ulong Uid { get; set; }

        private byte[] _header;

        public Package(ushort command, int size, PackageType type = 0) : this()
        {
            Type = type;
            Command = command;
            Payload = new byte[size];
        }

        public Package()
        {

        }

        public Package(byte[] data) : this(0, data.Length)
        {

        }

        public void SerializePackage(OctoNetworkStream stream)
        {
            if (Payload.Length + _header.Length > stream.Length)
            {
                SerializeSubPackages(stream);
                return;
            }
            Type = PackageType.Normal;
            WriteHead(ref _header);
            stream.Write(_header, 0, _header.Length);
            stream.Write(Payload, 0, Payload.Length);
        }

        public void SerializeSubPackages(OctoNetworkStream stream)
        {
            var firstPackage = (int)stream.Length - SUB_HEAD_LENGTH;
            var contentPackage = (int)stream.Length - SUB_CONTENT_HEAD_LENGTH;
            var count = (int)Math.Round((double)((Payload.Length - firstPackage) / contentPackage), MidpointRounding.AwayFromZero);
            var offset = firstPackage;

            Type = PackageType.Subhead;
            WriteHead(ref _header);
            _header[8] = (byte)(count >> 8);
            _header[9] = (byte)(count & 0xFF);
            stream.Write(_header, 0, _header.Length);
            stream.Write(Payload, 0, firstPackage);
            Type = PackageType.Subcontent;

            for (int i = 0; i < count - 1; i++)
            {
                WriteHead(ref _header);
                stream.Write(_header, 0, _header.Length);
                stream.Write(Payload, offset, contentPackage);
                offset += contentPackage;
            }

            WriteHead(ref _header);
            stream.Write(_header, 0, _header.Length);
            stream.Write(Payload, offset, Payload.Length - offset);
        }

        public void WriteHead(ref byte[] buffer, int offset = 0)
        {
            byte[] header, bytes;
            int index = offset;
            header = buffer;

            switch (Type)
            {
                case PackageType.Normal:
                    header = new byte[HEAD_LENGTH];
                    header[index] = (byte)Type;
                    header[index++] = (byte)(Command >> 8);
                    header[index++] = (byte)(Command & 0xFF);
                    break;
                case PackageType.Subhead:
                    header = new byte[SUB_HEAD_LENGTH];
                    header[index] = (byte)Type;
                    header[index++] = (byte)(Command >> 8);
                    header[index++] = (byte)(Command & 0xFF);

                    bytes = BitConverter.GetBytes(NextUid);
                    Array.Copy(bytes, 0, header, index++, bytes.Length);
                    break;
                case PackageType.Subcontent:
                    header = new byte[SUB_CONTENT_HEAD_LENGTH];
                    header[index] = (byte)Type;
                    bytes = BitConverter.GetBytes(Uid);
                    Array.Copy(bytes, 0, header, index++, bytes.Length);
                    break;
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