﻿using System;

namespace OctoAwesome.Network
{
    public class Package
    {
        /// <summary>
        /// Bytesize of Header
        /// </summary>
        public const int HEAD_LENGTH = sizeof(ushort) + sizeof(int) + sizeof(uint);

        public static uint NextUId => _nextUid++;

        public ushort Command { get; set; }

        public byte[] Payload { get; set; }

        public uint UId { get; set; }

        public bool IsComplete => _internalOffset == Payload.Length;

        private static uint _nextUid;

        private int _internalOffset;

        public Package(ushort command, int size) : this()
        {
            Command = command;
            Payload = new byte[size];
        }

        public Package() : this(true) { }

        public Package(bool setUid)
        {
            if (setUid)
                UId = NextUId;
        }

        public Package(byte[] data) : this(0, data.Length)
        {
        }

        public bool TryDeserializeHeader(byte[] buffer)
        {
            if (buffer.Length < HEAD_LENGTH)
                return false;

            Command = (ushort)((buffer[0] << 8) | buffer[1]);
            Payload = new byte[BitConverter.ToInt32(buffer, 2)];
            UId = BitConverter.ToUInt32(buffer, 6);
            _internalOffset = 0;
            return true;
        }

        public int DeserializePayload(byte[] buffer, int offset, int count)
        {
            if (_internalOffset + count > Payload.Length)
                count = Payload.Length - _internalOffset;

            Buffer.BlockCopy(buffer, offset, Payload, _internalOffset, count);
            _internalOffset += count;

            return count;
        }
        public void DeserializePackage(byte[] buffer)
        {
            TryDeserializeHeader(buffer);
            Buffer.BlockCopy(buffer, HEAD_LENGTH, Payload, 0, Payload.Length);
            _internalOffset = Payload.Length;
        }

        public int SerializePackage(byte[] buffer)
        {
            buffer[0] = (byte)(Command >> 8);
            buffer[1] = (byte)(Command & 0xFF);
            var bytes = BitConverter.GetBytes(Payload.Length);
            Buffer.BlockCopy(bytes, 0, buffer, 2, 4);
            bytes = BitConverter.GetBytes(UId);
            Buffer.BlockCopy(bytes, 0, buffer, 6, 4);
            Buffer.BlockCopy(Payload, 0, buffer, HEAD_LENGTH, Payload.Length); //Payload.Serialize();
            return Payload.Length + HEAD_LENGTH;
        }
        
    }
}
