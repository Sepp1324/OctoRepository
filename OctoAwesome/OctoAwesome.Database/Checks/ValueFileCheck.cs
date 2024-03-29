﻿using System;
using System.IO;

namespace OctoAwesome.Database.Checks
{
    public sealed class ValueFileCheck<TTag> : ICheckable where TTag : ITag, new()
    {
        private readonly FileInfo _fileInfo;

        public ValueFileCheck(FileInfo fileInfo) => _fileInfo = fileInfo;

        public void Check()
        {
            using (var fileStream = _fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None))
            {
                var keyBuffer = new byte[Key<TTag>.KEY_SIZE];
                var length = 0;
                do
                {
                    fileStream.Read(keyBuffer, 0, keyBuffer.Length);
                    var key = Key<TTag>.FromBytes(keyBuffer, 0);

                    if (!key.Validate())
                        throw new KeyInvalidException("Key is not valid", fileStream.Position);

                    if (key.Index != fileStream.Position - Key<TTag>.KEY_SIZE)
                        throw new KeyInvalidException("Key is on the wrong Position", fileStream.Position);

                    if (key.IsEmpty)
                    {
                        var intBuffer = new byte[sizeof(int)];
                        fileStream.Read(intBuffer, 0, sizeof(int));
                        length = BitConverter.ToInt32(intBuffer, 0) - sizeof(int);
                    }
                    else
                    {
                        length = key.ValueLength;
                    }

                    fileStream.Seek(length, SeekOrigin.Current);
                } while (fileStream.Position != fileStream.Length);
            }
        }
    }
}