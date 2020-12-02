using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Threading
{
    public sealed class LockedSemaphore : IDisposable
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        public LockedSemaphore(int initialCount, int maxCount) => _semaphoreSlim = new SemaphoreSlim(initialCount, maxCount);

        public SemaphoreLock Wait()
        {
            _semaphoreSlim.Wait();
            return new SemaphoreLock(this);
        }

        public async Task<SemaphoreLock> WaitAsync(CancellationToken token)
        {
            await _semaphoreSlim.WaitAsync(token);
            return new SemaphoreLock(this);
        }
              
        public void Dispose() => _semaphoreSlim.Dispose();

        private void Release() => _semaphoreSlim.Release();

        public readonly struct SemaphoreLock : IDisposable, IEquatable<SemaphoreLock>
        {
            public static SemaphoreLock Empty => new SemaphoreLock(null);

            private readonly LockedSemaphore _internalLockedSemaphore;

            public SemaphoreLock(LockedSemaphore lockedSemaphore) => _internalLockedSemaphore = lockedSemaphore;

            public void Dispose() => _internalLockedSemaphore?.Release();

            public override bool Equals(object obj) => obj is SemaphoreLock semLock && Equals(semLock);

            public bool Equals(SemaphoreLock other) => EqualityComparer<LockedSemaphore>.Default.Equals(_internalLockedSemaphore, other._internalLockedSemaphore);

            public override int GetHashCode() => 37286538 + EqualityComparer<LockedSemaphore>.Default.GetHashCode(_internalLockedSemaphore);

            public static bool operator ==(SemaphoreLock left, SemaphoreLock right) => left.Equals(right);

            public static bool operator !=(SemaphoreLock left, SemaphoreLock right) => !(left == right);
        }
    }
}
