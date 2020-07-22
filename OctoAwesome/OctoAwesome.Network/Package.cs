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
        public const int HEAD_LENGTH = 4;

        public byte Type { get; set; }

        public ushort Command { get; set; }

        public byte[] Payload { get; set; }

        public int Size => Payload.Length;

        private MemoryStream _memoryStream;
        private GZipStream _gzipStream;

        public Package(ushort command, int size, byte type = 0)
        {
            Type = type;
            Command = command;
            Payload = new byte[size];

            _memoryStream = new MemoryStream();
            _gzipStream = new GZipStream(_memoryStream, CompressionMode.Compress);
        }
        public Package(byte[] data) : this(0, data.Length)
        {
            Write(data);
        }

        private int _writePosition;

        public int Write(byte[] buffer, int offset, int count)
        {
            if (_writePosition == 0)
            {
                if (count < HEAD_LENGTH)
                    return 0;

                Type = buffer[offset];
                Command = (ushort)(buffer[offset + 1] << 8 | buffer[offset + 2]);
                int size = (ushort)(buffer[offset + 3] << 24 | buffer[offset + 4] << 16 | buffer[offset + 5] << 8 | buffer[offset + 6]);
                Payload = new byte[size];

                if(buffer[offset + 7] == 1)
                {
                    _gzipStream = new GZipStream(_memoryStream, CompressionMode.Decompress);
                }

                _writePosition += HEAD_LENGTH;
                offset += HEAD_LENGTH;
            }

            
            { 
                using (var memoryStream = new MemoryStream(buffer, HEAD_LENGTH, buffer.Length - HEAD_LENGTH))
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    using (var targetStream = new MemoryStream())
                    {
                        gzipStream.CopyTo(targetStream);
                        Payload = targetStream.ToArray();
                    }
                }
            }
            else
            {
                Array.Copy(buffer, HEAD_LENGTH, Payload, 0, buffer.Length - HEAD_LENGTH);
            }

        }

        private int _readPosition;

        public int Read(byte[] buffer, int offset, int count, bool zip = false)
        {
            int oldPosition = _readPosition;
            if (_readPosition == 0)
            {
                if (count < HEAD_LENGTH)
                    return 0;

                buffer[offset++] = Type;
                buffer[offset++] = (byte)(Command >> 8);
                buffer[offset++] = (byte)(Command & 0xFF);

                buffer[offset++] = (byte)(Size >> 24);
                buffer[offset++] = (byte)(Size >> 16);
                buffer[offset++] = (byte)(Size >> 8);
                buffer[offset++] = (byte)(Size & 0xFF);

                buffer[offset++] = (byte)(zip ? 1 : 0);

                _readPosition += HEAD_LENGTH;
                count -= HEAD_LENGTH;
            }

            if (zip)
            {
                if (_memoryStream.Length == 0)
                {
                    using (var gzipStream = new GZipStream(_memoryStream, CompressionMode.Compress, true))
                    {
                        gzipStream.Write(Payload, 0, Payload.Length);
                    }
                    _memoryStream.Position = 0;
                }
                _readPosition += _memoryStream.Read(buffer, offset, count);
            }
            else
            {
                var toCopy = Math.Min(count, Payload.Length - (_readPosition - HEAD_LENGTH));
                if (toCopy > 0)
                {
                    Array.Copy(Payload, _readPosition - HEAD_LENGTH, buffer, offset, toCopy);
                    _readPosition += toCopy;
                }
            }
            return _readPosition - oldPosition;
        }

        public void Dispose()
        {
            _memoryStream.Flush();
            _memoryStream.Dispose();
        }
    }
}
