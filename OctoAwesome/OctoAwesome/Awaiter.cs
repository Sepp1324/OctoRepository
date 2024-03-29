﻿using System;
using System.IO;
using System.Threading;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;
using OctoAwesome.Threading;

namespace OctoAwesome
{
    public class Awaiter : IPoolElement, IDisposable
    {
        private readonly ManualResetEventSlim _manualReset;
        private readonly LockSemaphore _semaphore;
        private bool _alreadyDeserialized;
        private IPool _pool;
        private bool _isPooled;

        public Awaiter()
        {
            _manualReset = new(false);
            _semaphore = new(1, 1);
        }

        public ISerializable Serializable { get; set; }

        public bool Timeouted { get; private set; }

        public void Dispose() => _manualReset.Dispose();

        public void Init(IPool pool)
        {
            _pool = pool;
            Timeouted = false;
            _isPooled = false;
            _alreadyDeserialized = false;
            Serializable = null;
            _manualReset.Reset();
        }

        public void Release()
        {
            using (_semaphore.Wait())
            {
                if (!_manualReset.IsSet)
                    _manualReset.Set();

                _isPooled = true;

                _pool.Push(this);
            }
        }

        public ISerializable WaitOn()
        {
            if (!_alreadyDeserialized)
                Timeouted = !_manualReset.Wait(10000);

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
    }
}