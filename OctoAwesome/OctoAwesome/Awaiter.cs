using OctoAwesome.Pooling;
using OctoAwesome.Serialization;
using System;
using System.IO;
using System.Threading;

namespace OctoAwesome
{
    public class Awaiter : IPoolElement, IDisposable
    {
        public ISerializable Serializable { get; set; }
        public bool Timeouted { get; private set; }

        private readonly ManualResetEventSlim _manualReset;
        private readonly SemaphoreExtended _semaphore;
        private bool _alreadyDeserialized;
        private IPool _pool;

        public Awaiter()
        {
            _manualReset = new ManualResetEventSlim(false);
            _semaphore = new SemaphoreExtended(1, 1);
        }

        public ISerializable WaitOn()
        {
            if (!_alreadyDeserialized)
                Timeouted = !_manualReset.Wait(3000);

            return Serializable;
        }

        public void WaitOnAndRelease()
        {
            WaitOn();
            Release();
        }

        public void SetResult(ISerializable serializable)
        {
            using (_semaphore.Wait())
            {
                Serializable = serializable;
                _manualReset.Set();
                _alreadyDeserialized = true;
            }
        }

        public bool TrySetResult(byte[] bytes)
        {
            using (_semaphore.Wait())
            {
                if (Timeouted)
                    return false;

                if (Serializable == null)
                    throw new ArgumentNullException(nameof(Serializable));

                using (var stream = new MemoryStream(bytes))
                using (var reader = new BinaryReader(stream))
                {
                    Serializable.Deserialize(reader);
                }
                _manualReset.Set();
                return _alreadyDeserialized = true;
            }
        }

        public void Init(IPool pool)
        {
            _pool = pool;
            _manualReset.Reset();
        }

        public void Release()
        {
            using (_semaphore.Wait())
            {
                if (!_manualReset.IsSet)
                    _manualReset.Set();

                _alreadyDeserialized = false;
                Timeouted = false;
                Serializable = null;

                _pool.Push(this);
            }
        }

        public void Dispose() => _manualReset.Dispose();
    }
}
