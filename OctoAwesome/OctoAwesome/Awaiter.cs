using OctoAwesome.Pooling;
using OctoAwesome.Serialization;
using System;
using System.IO;
using System.Threading;
using OctoAwesome.Threading;

namespace OctoAwesome
{
    public class Awaiter : IPoolElement, IDisposable
    {
        /// <summary>
        /// Indicates whether a Component is serializable or not 
        /// </summary>
        public ISerializable Serializable { get; set; }

        /// <summary>
        /// Indicates whether a Component is timeouted or not
        /// </summary>
        public bool Timeouted { get; private set; }

        private readonly ManualResetEventSlim _manualReset;
        private readonly LockSemaphore _lockSemaphore;
        private bool _alreadyDeserialized;
        private IPool _pool;

        /// <summary>
        /// Constructor for Awaiter
        /// </summary>
        public Awaiter()
        {
            _manualReset = new ManualResetEventSlim(false);
            _lockSemaphore = new LockSemaphore(1, 1);
        }

        /// <summary>
        /// Waits until Serialization is finished
        /// </summary>
        /// <returns></returns>
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
            using (_lockSemaphore.Wait())
            {
                Serializable = serializable;
                _manualReset.Set();
                _alreadyDeserialized = true;
            }
        }

        public bool TrySetResult(byte[] bytes)
        {
            using (_lockSemaphore.Wait())
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

        /// <summary>
        /// Init for an Awaiter
        /// </summary>
        /// <param name="pool"></param>
        public void Init(IPool pool)
        {
            _pool = pool;
            _manualReset.Reset();
        }

        public void Release()
        {
            using (_lockSemaphore.Wait())
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
