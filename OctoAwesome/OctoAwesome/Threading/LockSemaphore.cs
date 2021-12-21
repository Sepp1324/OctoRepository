using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Threading
{
    public sealed class LockSemaphore : IDisposable
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        public LockSemaphore(int initialCount, int maxCount) => _semaphoreSlim = new(initialCount, maxCount);

        public void Dispose() => _semaphoreSlim.Dispose();

        public SemaphoreLock Wait()
        {
            _semaphoreSlim.Wait();
            return new(this);
        }

        public async Task<SemaphoreLock> WaitAsync(CancellationToken token)
        {
            await _semaphoreSlim.WaitAsync(token);
            return new(this);
        }

        private void Release() => _semaphoreSlim.Release();

        public readonly struct SemaphoreLock : IDisposable, IEquatable<SemaphoreLock>
        {
            public static SemaphoreLock Empty => new(null);

            private readonly LockSemaphore _internalSemaphore;

            public SemaphoreLock(LockSemaphore semaphoreExtended) => _internalSemaphore = semaphoreExtended;

            public void Dispose() => _internalSemaphore?.Release();

            public override bool Equals(object obj) => obj is SemaphoreLock @lock && Equals(@lock);

            public bool Equals(SemaphoreLock other) => EqualityComparer<LockSemaphore>.Default.Equals(_internalSemaphore, other._internalSemaphore);

            public override int GetHashCode() => 37286538 + EqualityComparer<LockSemaphore>.Default.GetHashCode(_internalSemaphore);

            public static bool operator ==(SemaphoreLock left, SemaphoreLock right) => left.Equals(right);

            public static bool operator !=(SemaphoreLock left, SemaphoreLock right) => !(left == right);
        }
    }
}