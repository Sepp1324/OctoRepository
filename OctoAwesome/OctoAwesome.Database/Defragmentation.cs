using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome.Database
{
    public sealed class Defragmentation<TTag> where TTag : ITag, new()
    {
        private readonly FileInfo keyStoreFile;
        private readonly FileInfo valueStoreFile;

        public Defragmentation(FileInfo keyStoreFile, FileInfo valueStoreFile)
        {
            this.keyStoreFile = keyStoreFile;
            this.valueStoreFile = valueStoreFile;
        }

        public void StartDefragmentation()
        {
            var newValueStoreFile = new FileInfo(Path.GetTempFileName());
            var keyBuffer = new byte[Key<TTag>.KEY_SIZE];

            var keys = DefragmentValues(newValueStoreFile, keyBuffer);

            keyStoreFile.Delete();

            WriteKeyFile(keys);

            valueStoreFile.Delete();
            newValueStoreFile.MoveTo(valueStoreFile.FullName);
        }

        public void RecreateKeyFile()
        {
            var keyBuffer = new byte[Key<TTag>.KEY_SIZE];

            var keys = GetKeys(keyBuffer);

            keyStoreFile.Delete();

            WriteKeyFile(keys);
        }

        private void WriteKeyFile(IEnumerable<Key<TTag>> keyList)
        {
            using (var newKeyStoreFile = keyStoreFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                foreach (var key in keyList)
                    newKeyStoreFile.Write(key.GetBytes(), 0, Key<TTag>.KEY_SIZE);
            }
        }

        private IEnumerable<Key<TTag>> DefragmentValues(FileInfo newValueStoreFile, byte[] keyBuffer)
        {
            using (var newValueStoreStream = newValueStoreFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            using (var currentValueStoreStream = valueStoreFile.Open(FileMode.Open, FileAccess.Read, FileShare.None))
            {
                do
                {
                    currentValueStoreStream.Read(keyBuffer, 0, keyBuffer.Length);

                    var key = Key<TTag>.FromBytes(keyBuffer, 0);

                    if (key.IsEmpty)
                    {
                        var intBuffer = new byte[sizeof(int)];

                        currentValueStoreStream.Read(intBuffer, 0, sizeof(int));

                        var length = BitConverter.ToInt32(intBuffer, 0) - sizeof(int);

                        currentValueStoreStream.Seek(length, SeekOrigin.Current);
                    }
                    else
                    {
                        var buffer = new byte[key.ValueLength];

                        currentValueStoreStream.Read(buffer, 0, buffer.Length);
                        newValueStoreStream.Write(keyBuffer, 0, keyBuffer.Length);
                        newValueStoreStream.Write(buffer, 0, buffer.Length);
                        yield return key;
                    }
                } while (currentValueStoreStream.Position != currentValueStoreStream.Length);
            }
        }

        private IEnumerable<Key<TTag>> GetKeys(byte[] keyBuffer)
        {
            using (var fileStream = valueStoreFile.Open(FileMode.Open, FileAccess.Read, FileShare.None))
            {
                do
                {
                    fileStream.Read(keyBuffer, 0, keyBuffer.Length);

                    var key = Key<TTag>.FromBytes(keyBuffer, 0);
                    long length;

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
                    yield return key;
                } while (fileStream.Position != fileStream.Length);
            }
        }
    }
}
