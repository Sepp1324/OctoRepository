using System.Buffers;
using System.IO;

namespace OctoAwesome.Database
{
    internal class ValueStore
    {
        private readonly FileStream fileStream;

        public ValueStore(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

        public Value GetValue(Key key)
        {
            var byteArray = new byte[key.Length];
            fileStream.Seek(key.Index + Key.KEY_SIZE, SeekOrigin.Begin);
            fileStream.Read(byteArray, 0, key.Length);
            return new Value(byteArray);
        }

        internal Key AddValue<TTag>(TTag tag, Value value) where TTag : ITagable
        {
            var key = new Key(tag.Tag, fileStream.Seek(0, SeekOrigin.End), value.Content.Length);
            //TODO: HASH
            fileStream.Write(key.GetBytes(), 0, Key.KEY_SIZE);
            fileStream.Write(value.Content, 0, value.Content.Length);
            return key;
        }
    }
}
