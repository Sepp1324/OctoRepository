﻿using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome.Database
{
    public sealed class Defragmentation<TTag> where TTag : ITag, new()
    {
        private readonly FileInfo _keyStoreFile;
        private readonly FileInfo _valueStoreFile;

        public Defragmentation(FileInfo keyStoreFile, FileInfo valueStoreFile)
        {
            _keyStoreFile = keyStoreFile;
            _valueStoreFile = valueStoreFile;
        }

        public void StartDefragmentation()
        {
            var newValueStoreFile = new FileInfo(Path.GetTempFileName());
            var keyBuffer = new byte[Key<TTag>.KEY_SIZE];
<<<<<<< HEAD
            var keys = DefragmentValues(newValueStoreFile, keyBuffer);
=======

            IEnumerable<Key<TTag>> keys = DefragmentValues(newValueStoreFile, keyBuffer);
>>>>>>> feature/performance

            _keyStoreFile.Delete();
            WriteKeyFile(keys);

            _valueStoreFile.Delete();
            newValueStoreFile.MoveTo(_valueStoreFile.FullName);
        }

        public void RecreateKeyFile()
        {
            var keyBuffer = new byte[Key<TTag>.KEY_SIZE];
<<<<<<< HEAD
            var keys = GetKeys(keyBuffer);
=======

            IEnumerable<Key<TTag>> keys = GetKeys(keyBuffer);
>>>>>>> feature/performance

            _keyStoreFile.Delete();
            WriteKeyFile(keys);
        }

        private void WriteKeyFile(IEnumerable<Key<TTag>> keyList)
        {
<<<<<<< HEAD
            using (var newKeyStoreFile = _keyStoreFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
=======
            using (FileStream newKeyStoreFile = keyStoreFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
>>>>>>> feature/performance
            {
                foreach (Key<TTag> key in keyList)
                    newKeyStoreFile.Write(key.GetBytes(), 0, Key<TTag>.KEY_SIZE);
            }
        }

        private IEnumerable<Key<TTag>> DefragmentValues(FileInfo newValueStoreFile, byte[] keyBuffer)
        {
<<<<<<< HEAD
            using (var newValueStoreStream = newValueStoreFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            using (var currentValueStoreStream = _valueStoreFile.Open(FileMode.Open, FileAccess.Read, FileShare.None))
=======
            using (FileStream newValueStoreStream = newValueStoreFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            using (FileStream currentValueStoreStream = valueStoreFile.Open(FileMode.Open, FileAccess.Read, FileShare.None))
>>>>>>> feature/performance
            {
                do
                {
                    if (currentValueStoreStream.Read(keyBuffer, 0, keyBuffer.Length) == 0)
                        break;
                    var key = Key<TTag>.FromBytes(keyBuffer, 0);

                    if (key.IsEmpty)
                    {
                        var intBuffer = new byte[sizeof(int)];
                        if (currentValueStoreStream.Read(intBuffer, 0, sizeof(int)) == 0)
                            break;
                        var length = BitConverter.ToInt32(intBuffer, 0) - sizeof(int);
                        if (length < 0)
                            throw new DataMisalignedException();
                        currentValueStoreStream.Seek(length, SeekOrigin.Current);
                    }
                    else
                    {
                        var buffer = new byte[key.ValueLength];
                        if (currentValueStoreStream.Read(buffer, 0, buffer.Length) == 0)
                            break;
                        newValueStoreStream.Write(keyBuffer, 0, keyBuffer.Length);
                        newValueStoreStream.Write(buffer, 0, buffer.Length);
                        yield return key;
                    }
                } while (currentValueStoreStream.Position < currentValueStoreStream.Length);
            }
        }

        private IEnumerable<Key<TTag>> GetKeys(byte[] keyBuffer)
        {
<<<<<<< HEAD
            using (var fileStream = _valueStoreFile.Open(FileMode.Open, FileAccess.Read, FileShare.None))
=======
            using (FileStream fileStream = valueStoreFile.Open(FileMode.Open, FileAccess.Read, FileShare.None))
>>>>>>> feature/performance
            {
                do
                {
                    if (fileStream.Read(keyBuffer, 0, keyBuffer.Length) == 0)
                        break;
                    var key = Key<TTag>.FromBytes(keyBuffer, 0);
                    long length = 0;

                    if (key.IsEmpty)
                    {
                        var intBuffer = new byte[sizeof(int)];
                        if (fileStream.Read(intBuffer, 0, sizeof(int)) == 0)
                            break;
                        length = BitConverter.ToInt32(intBuffer, 0) - sizeof(int);
                    }
                    else
                    {
                        length = key.ValueLength;
                    }
                    if (length < 0)
                        throw new DataMisalignedException();
                    fileStream.Seek(length, SeekOrigin.Current);
                    yield return key;
                } while (fileStream.Position < fileStream.Length);
            }
        }
    }
}
