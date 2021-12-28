using System;
using OctoAwesome.Pooling;

namespace OctoAwesome.Network
{
    public sealed class Package : IPoolElement
    {
        /// <summary>
        /// Byte-Size of the Header
        /// </summary>
        public const int HEAD_LENGTH = sizeof(ushort) + sizeof(int) + sizeof(uint);

        private static volatile uint _nextUid;

        private int _internalOffset;
        private IPool _pool;

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

        public Package(byte[] data) : this(0, data.Length) { }

        public static uint NextUId => _nextUid++;

        public BaseClient BaseClient { get; set; }

        public OfficialCommand OfficialCommand => (OfficialCommand)Command;

        public ushort Command { get; set; }

        public byte[] Payload { get; set; }

        public uint UId { get; set; }

        public bool IsComplete => _internalOffset == Payload.Length;

        public int PayloadRest => Payload.Length - _internalOffset;

        public void Init(IPool pool)
        {
            Payload = Array.Empty<byte>();
            _pool = pool;
        }

        public void Release()
        {
            BaseClient = default;
            Command = default;
            Payload = default;
            UId = default;
            _internalOffset = default;

            _pool.Push(this);
        }

        public bool TryDeserializeHeader(byte[] buffer, int offset)
        {
            if (buffer.Length < HEAD_LENGTH)
                return false;

            Command = (ushort)((buffer[offset] << 8) | buffer[offset + 1]);
            Payload = new byte[BitConverter.ToInt32(buffer, offset + 2)];
            UId = BitConverter.ToUInt32(buffer, offset + 6);
            _internalOffset = 0;
            return true;
        }

        public int DeserializePayload(byte[] buffer, int offset, int count)
        {
            if (_internalOffset + count > Payload.Length)
                count = PayloadRest;

            Buffer.BlockCopy(buffer, offset, Payload, _internalOffset, count);
            _internalOffset += count;

            return count;
        }

        public void DeserializePackage(byte[] buffer, int offset)
        {
            TryDeserializeHeader(buffer, offset);
            Buffer.BlockCopy(buffer, offset + HEAD_LENGTH, Payload, 0, Payload.Length);
            _internalOffset = Payload.Length;
        }

        public int SerializePackage(byte[] buffer, int offset)
        {
            buffer[offset] = (byte)(Command >> 8);
            buffer[offset + 1] = (byte)(Command & 0xFF);
            var bytes = BitConverter.GetBytes(Payload.Length);
            Buffer.BlockCopy(bytes, 0, buffer, offset + 2, 4);
            bytes = BitConverter.GetBytes(UId);
            Buffer.BlockCopy(bytes, 0, buffer, offset + 6, 4);
            Buffer.BlockCopy(Payload, 0, buffer, offset + HEAD_LENGTH, Payload.Length);
            return Payload.Length + HEAD_LENGTH;
        }
    }
}