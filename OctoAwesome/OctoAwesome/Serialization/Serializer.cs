using OctoAwesome.Pooling;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace OctoAwesome.Serialization
{
    public static class Serializer
    {
        public static byte[] Serialize<T>(T obj) where T : ISerializable
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                obj.Serialize(writer);
                return stream.ToArray();
            }
        }

        public static byte[] SerializeCompressed<T>(T obj) where T : ISerializable
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(memoryStream, Encoding.Default, true))
                    obj.Serialize(binaryWriter);

                using (var stream = new MemoryStream())
                {
                    memoryStream.Position = 0;

                    using (var zipStream = new GZipStream(stream, CompressionMode.Compress, true))
                        memoryStream.CopyTo(zipStream);

                    return stream.ToArray();
                }
            }
        }

        public static T Deserialize<T>(byte[] data) where T : ISerializable, new()
        {
            var obj = new T();
            InternalDeserialize(ref obj, data);
            return obj;
        }

        public static T DeserializeCompressed<T>(byte[] data) where T : ISerializable, new()
        {
            var obj = new T();
            InternalDeserializeCompressed(ref obj, data);
            return obj;
        }

        public static T DeserializePoolElement<T>(byte[] data) where T : ISerializable, IPoolElement, new()
        {
            var obj = TypeContainer.Get<IPool<T>>().Get();
            InternalDeserialize(ref obj, data);
            return obj;
        }

        public static T DeserializePoolElement<T>(IPool<T> pool, byte[] data) where T : ISerializable, IPoolElement, new()
        {
            var obj = pool.Get();
            InternalDeserialize(ref obj, data);
            return obj;
        }

        private static void InternalDeserialize<T>(ref T instance, byte[] data) where T : ISerializable
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                instance.Deserialize(reader);
            }
        }

        private static void InternalDeserializeCompressed<T>(ref T instance, byte[] data) where T : ISerializable
        {
            using (var memoryStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            using (var binaryReader = new BinaryReader(zipStream))
            {
                instance.Deserialize(binaryReader);
            }
        }
    }
}
