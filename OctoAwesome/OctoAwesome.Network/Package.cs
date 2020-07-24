using System;
using System.IO;
using System.IO.Compression;

namespace OctoAwesome.Network
{
    public class Package : IDisposable
    {
        /// <summary>
        /// Bytesize of Header
        /// </summary>
        public const int HEAD_LENGTH = 8;

        public byte Type { get; set; }

        public ushort Command { get; set; }

        public byte[] Payload { get; set; }

        public bool Zipped { get; set; }

        private int _compressedSize;

        private MemoryStream _memoryStream;
        private GZipStream _gzipStream;

        private int _readPosition;
        private int _writePosition;

        public Package(ushort command, int size, byte type = 0) : this()
        {
            Type = type;
            Command = command;
            Payload = new byte[size];

            Zipped = size > 2000;
        }
        public Package()
        {

            _memoryStream = new MemoryStream();
        }
        public Package(byte[] data) : this(0, data.Length)
        {
            Write(data, 0, data.Length);
        }

        public void Dispose()
        {
            _memoryStream.Flush();
            _memoryStream.Dispose();
        }
    }
}