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
            fileStream.Seek(key.Index + Key.KEY_SIZE, SeekOrigin.Begin);
        }

        internal Key AddValue(int tag, Value value)
        {
            var key = new Key(tag, fileStream.Seek(0, SeekOrigin.End), value.Content.Length);
            //TODO: HASH
            fileStream.Write(key.GetBytes(), 0, Key.KEY_SIZE);
            fileStream.Write(value.Content, 0, value.Content.Length);
            return key;
        }
    }
}
