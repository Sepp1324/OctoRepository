using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Threading
{
    /// <summary>
    /// Deadlock for Threading
    /// </summary>
    public sealed class LockedSemaphore : IDisposable
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        /// <summary>
        /// Constructor for Semaphore
        /// </summary>
        /// <param name="initialCount">First value</param>
        /// <param name="maxCount">Maximal size of Semaphore</param>
        public LockedSemaphore(int initialCount, int maxCount) => _semaphoreSlim = new SemaphoreSlim(initialCount, maxCount);

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

            private readonly LockedSemaphore _internalLockedSemaphore;

            /// <summary>
            /// Constructor for nested Class
            /// </summary>
            /// <param name="lockedSemaphore">Deadlock</param>
            public SemaphoreLock(LockedSemaphore lockedSemaphore) => _internalLockedSemaphore = lockedSemaphore;

            /// <summary>
            /// General Dispose-Pattern
            /// </summary>
            public void Dispose() => _internalLockedSemaphore?.Release();

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
            public bool Equals(SemaphoreLock other) => EqualityComparer<LockedSemaphore>.Default.Equals(_internalLockedSemaphore, other._internalLockedSemaphore);

            /// <summary>
            /// Unique HashCode
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode() => 37286538 + EqualityComparer<LockedSemaphore>.Default.GetHashCode(_internalLockedSemaphore);

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
