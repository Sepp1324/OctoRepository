using System.IO;
using System.Threading;

namespace OctoAwesome
{
    public class Awaiter
    {
        public ISerializable Serializable { get; set; }

        private readonly ManualResetEventSlim _manualResetEventSlim;
        private bool _alreadyDeserialized;

        public Awaiter()
        {
            _manualResetEventSlim = new ManualResetEventSlim(false);
        }

        public ISerializable WaitOn()
        {
            if (!_alreadyDeserialized)
                _manualResetEventSlim.Wait();
            return Serializable;
        }

        public void SetResult(ISerializable serializable)
        {
            Serializable = serializable;
            _manualResetEventSlim.Set();
            _alreadyDeserialized = true;
        }

        public void SetResult(byte[] bytes, IDefinitionManager definitionManager)
        {
            using (var stream = new MemoryStream(bytes))
            using (var reader = new BinaryReader(stream))
            {
                Serializable.Deserialize(reader, definitionManager);
            }
            _manualResetEventSlim.Set();
            _alreadyDeserialized = true;
        }
    }
}