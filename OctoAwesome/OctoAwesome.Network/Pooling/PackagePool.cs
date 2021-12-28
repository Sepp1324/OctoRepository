using System;
using System.Collections.Generic;
using OctoAwesome.Pooling;
using OctoAwesome.Threading;

namespace OctoAwesome.Network.Pooling
{
    public sealed class PackagePool : IPool<Package>
    {
        private readonly Stack<Package> _internalStack;
        private readonly LockSemaphore _semaphoreExtended;

        public PackagePool()
        {
            _internalStack = new();
            _semaphoreExtended = new(1, 1);
        }

        public Package Get()
        {
            Package obj;

            using (_semaphoreExtended.Wait())
                obj = _internalStack.Count > 0 ? _internalStack.Pop() : new();

            obj.Init(this);
            obj.UId = Package.NextUId;
            return obj;
        }

        public void Push(Package obj)
        {
            using (_semaphoreExtended.Wait())
                _internalStack.Push(obj);
        }

        public void Push(IPoolElement obj)
        {
            if (obj is Package package)
                Push(package);
            else
                throw new InvalidCastException("Can not push object from type: " + obj.GetType());
        }

        public Package GetBlank()
        {
            Package obj;

            using (_semaphoreExtended.Wait())
                obj = _internalStack.Count > 0 ? _internalStack.Pop() : new(false);

            obj.Init(this);
            return obj;
        }
    }
}