using System;
using System.Collections.Generic;
using System.Threading;
// ReSharper disable NonAtomicCompoundOperator

namespace OctoAwesome.Threading
{
    public class CountedScopeSemaphore
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        private volatile int _counter;

        public CountedScopeSemaphore(int initialCount)
        {
            _semaphoreSlim = new SemaphoreSlim(1, 1);
            _counter = initialCount;
        }

        public void Wait()
        {
            if (_counter > 0)
                _semaphoreSlim.Wait();
        }

        public CountScope EnterScope()
        {
            _counter++;
            return new CountScope(this);
        }

        public void Dispose() => _semaphoreSlim.Dispose();

        private void LeaveScope()
        {
            _counter--;

            if (_counter == 0 && _semaphoreSlim.CurrentCount < 1)
                _semaphoreSlim.Release();
        }

        public readonly struct CountScope : IDisposable, IEquatable<CountScope>
        {
            public static CountScope Empty => new CountScope(null);

            private readonly CountedScopeSemaphore _internalCountedScopeSemaphore;

            public CountScope(CountedScopeSemaphore countedScopeSemaphore) => _internalCountedScopeSemaphore = countedScopeSemaphore;

            public void Dispose() => _internalCountedScopeSemaphore?.LeaveScope();

            public override bool Equals(object obj) => obj is CountScope scope && Equals(scope);

            public bool Equals(CountScope other) => EqualityComparer<CountedScopeSemaphore>.Default.Equals(_internalCountedScopeSemaphore, other._internalCountedScopeSemaphore);

            public override int GetHashCode() => 37286538 + EqualityComparer<CountedScopeSemaphore>.Default.GetHashCode(_internalCountedScopeSemaphore);

            public static bool operator ==(CountScope left, CountScope right) => left.Equals(right);

            public static bool operator !=(CountScope left, CountScope right) => !(left == right);
        }
    }
}

