using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Threading
{
    /// <summary>
    /// Deadlock for Threading
    /// </summary>
    public sealed class LockSemaphore : IDisposable
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        /// <summary>
        /// Constructor for Semaphore
        /// </summary>
        /// <param name="initialCount">First value</param>
        /// <param name="maxCount">Maximal size of Semaphore</param>
        public LockSemaphore(int initialCount, int maxCount) => _semaphoreSlim = new SemaphoreSlim(initialCount, maxCount);

        /// <summary>
        /// Wait-Method for normal Threading
        /// </summary>
        /// <returns></returns>
        public SemaphoreLock Wait()
        {
            _semaphoreSlim.Wait();
            return new SemaphoreLock(this);
        }

        /// <summary>
        /// Wait-Method for Multi-Threading
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<SemaphoreLock> WaitAsync(CancellationToken token)
        {
            await _semaphoreSlim.WaitAsync(token);
            return new SemaphoreLock(this);
        }

        /// <summary>
        /// General Dispose Pattern
        /// </summary>
        public void Dispose() => _semaphoreSlim.Dispose();

        private void Release() => _semaphoreSlim.Release();

        /// <summary>
        /// Intern Semaphore for better Thread-Handling
        /// </summary>
        public readonly struct SemaphoreLock : IDisposable, IEquatable<SemaphoreLock>
        {
            /// <summary>
            /// Initial Semaphore
            /// </summary>
            public static SemaphoreLock Empty => new SemaphoreLock(null);

            private readonly LockSemaphore _internalLockSemaphore;

            /// <summary>
            /// Constructor for nested Class
            /// </summary>
            /// <param name="lockSemaphore">Deadlock</param>
            public SemaphoreLock(LockSemaphore lockSemaphore) => _internalLockSemaphore = lockSemaphore;

            /// <summary>
            /// General Dispose-Pattern
            /// </summary>
            public void Dispose() => _internalLockSemaphore?.Release();

            /// <summary>
            /// <see cref="Equals(SemaphoreLock)"/>
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj) => obj is SemaphoreLock semLock && Equals(semLock);

            /// <summary>
            /// <see cref="Equals(object)"/>
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equals(SemaphoreLock other) => EqualityComparer<LockSemaphore>.Default.Equals(_internalLockSemaphore, other._internalLockSemaphore);

            /// <summary>
            /// Unique HashCode
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode() => 37286538 + EqualityComparer<LockSemaphore>.Default.GetHashCode(_internalLockSemaphore);

            /// <summary>
            /// <see cref="Equals(object)"/>
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            public static bool operator ==(SemaphoreLock left, SemaphoreLock right) => left.Equals(right);

            /// <summary>
            /// Standard not-equal operator
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            public static bool operator !=(SemaphoreLock left, SemaphoreLock right) => !(left == right);
        }
    }
}
