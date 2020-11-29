using System;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public sealed class SemaphoreExtended : IDisposable
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        public SemaphoreExtended(int initialCount, int maxCount) => _semaphoreSlim = new SemaphoreSlim(initialCount, maxCount);

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

        public struct SemaphoreLock : IDisposable
        {
            public static SemaphoreLock Empty => new SemaphoreLock(null);

            private readonly SemaphoreExtended _internalSemaphore;

            public SemaphoreLock(SemaphoreExtended semaphoreExtended) => _internalSemaphore = semaphoreExtended;

            public void Dispose() => _internalSemaphore?.Release();
        }
    }
}
