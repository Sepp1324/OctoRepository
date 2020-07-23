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

        public bool Zipped { get; set; }

        public int Size => Payload.Length;

        private MemoryStream _memoryStream;
        private GZipStream _gzipStream;

        public Package(ushort command, int size, byte type = 0)
        {
            Type = type;
            Command = command;
            Payload = new byte[size];

            Zipped = size > 2000;

            _memoryStream = new MemoryStream();
            _gzipStream = new GZipStream(_memoryStream, CompressionMode.Compress);
        }
        public Package(byte[] data) : this(0, data.Length)
        {
            Write(data, 0, data.Length);
        }

        private int _writePosition;

        public int Write(byte[] buffer, int offset, int count)
        {
            int written = 0;

            if (_writePosition == 0)
            {
                if (count < HEAD_LENGTH)
                    return 0;

                Type = buffer[offset];
                Command = (ushort)(buffer[offset + 1] << 8 | buffer[offset + 2]);
                int size = (ushort)(buffer[offset + 3] << 24 | buffer[offset + 4] << 16 | buffer[offset + 5] << 8 | buffer[offset + 6]);
                Payload = new byte[size];
                Zipped = buffer[offset + 7] == 1;

                if (Zipped)
                {
                    _memoryStream.Position = 0;
                    _gzipStream = new GZipStream(_memoryStream, CompressionMode.Decompress, true);
                }
                written += HEAD_LENGTH;
                _writePosition += HEAD_LENGTH;
                offset += HEAD_LENGTH;
                count -= HEAD_LENGTH;
            }

            var toRead = Payload.Length - _writePosition - HEAD_LENGTH;

            if (Zipped)
            {
                _memoryStream.Write(buffer, offset, count);
                written += count;
                int read = _gzipStream.Read(Payload, _writePosition - HEAD_LENGTH, toRead);
                _writePosition += read;
            }
            else
            {
                toRead = Math.Min(count, toRead);
                Array.Copy(buffer, offset, Payload, _writePosition - HEAD_LENGTH, toRead);
                _writePosition += toRead;
                written += toRead;
            }
            return written;
        }

        private int _readPosition;

        public int Read(byte[] buffer, int offset, int count)
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

                buffer[offset++] = (byte)(Zipped ? 1 : 0);

                _readPosition += HEAD_LENGTH;
                count -= HEAD_LENGTH;
            }

            if (Zipped)
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
