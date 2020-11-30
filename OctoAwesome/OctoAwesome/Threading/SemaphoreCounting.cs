using System;
using System.Collections.Generic;
using System.Threading;
// ReSharper disable NonAtomicCompoundOperator

namespace OctoAwesome.Threading
{
    public class SemaphoreCounting
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        private volatile int _counter;

        public SemaphoreCounting(int initialCount)
        {
            _semaphoreSlim = new SemaphoreSlim(1, 1);
            _counter = initialCount;
        }

        public void Wait()
        {
            _semaphoreSlim.Wait();
        }

        public CountScope EnterScope()
        {
            _counter++;
            return new CountScope(this);
        }

        public void Dispose() => _semaphoreSlim.Dispose();

        private void Release() => _semaphoreSlim.Release();

        private void LeaveScope()
        {
            _counter--;

            if(_counter == 0)
                Release();
        }

        /// <inheritdoc />
        public readonly struct CountScope : IDisposable, IEquatable<CountScope>
        {
            public static CountScope Empty => new CountScope(null);

            private readonly SemaphoreCounting _internalSemaphore;

            public CountScope(SemaphoreCounting semaphoreCounting) => _internalSemaphore = semaphoreCounting;

            public void Dispose() => _internalSemaphore?.LeaveScope();

            public override bool Equals(object obj) => obj is CountScope scope && Equals(scope);

            public bool Equals(CountScope other) => EqualityComparer<SemaphoreCounting>.Default.Equals(_internalSemaphore, other._internalSemaphore);

            public override int GetHashCode() => 37286538 + EqualityComparer<SemaphoreCounting>.Default.GetHashCode(_internalSemaphore);

            public static bool operator ==(CountScope left, CountScope right) => left.Equals(right);

            public static bool operator !=(CountScope left, CountScope right) => !(left == right);
        }
    }
}

