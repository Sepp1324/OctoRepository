using OctoAwesome.Pooling;
using System;
using System.Collections.Generic;
using OctoAwesome.Threading;

namespace OctoAwesome.Network.Pooling
{
    public sealed class PackagePool : IPool<Package>
    {
        private readonly Stack<Package> _internalStack;
        private readonly LockSemaphore _lockSemaphore;

        public PackagePool()
        {
            _internalStack = new Stack<Package>();
            _lockSemaphore = new LockSemaphore(1, 1);
        }

        public Package Get()
        {
            Package obj;

            using (_lockSemaphore.Wait())
            {
                obj = _internalStack.Count > 0 ? _internalStack.Pop() : new Package();
            }

            obj.Init(this);
            obj.UId = Package.NextUId;
            return obj;
        }

        public Package GetBlank()
        {
            Package obj;

            using (_lockSemaphore.Wait())
            {
                obj = _internalStack.Count > 0 ? _internalStack.Pop() : new Package(false);
            }

            obj.Init(this);
            return obj;
        }

        public void Push(Package obj)
        {
            using (_lockSemaphore.Wait())
                _internalStack.Push(obj);
        }

        public void Push(IPoolElement obj)
        {
            if (obj is Package package)
                Push(package);
            else
                throw new InvalidCastException("Can not push object from type: " + obj.GetType());
        }
    }
}
