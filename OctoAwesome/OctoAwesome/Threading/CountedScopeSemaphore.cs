using System;
using System.Collections.Generic;
using System.Threading;

namespace OctoAwesome.Threading
{
    public class CountedScopeSemaphore : IDisposable
    {
        private readonly ManualResetEventSlim _superLock;
        private readonly ManualResetEventSlim _mainLock;

        private readonly object _lockObject;
        private readonly object _countLockObject;
        private int _counter;
        
        public CountedScopeSemaphore()
        {
            _mainLock = new ManualResetEventSlim(true);
            _superLock = new ManualResetEventSlim(true);
            _lockObject = new object();
            _countLockObject = new object();
        }

        public SuperScope Wait()
        {
            lock (_lockObject)
            {
                _mainLock.Wait();
                _superLock.Reset();
            }
            return new SuperScope(this);
        }

        public CountScope EnterScope()
        {
            lock (_lockObject)
            {
                _superLock.Wait();
                
                lock (_countLockObject)
                {
                    _counter++;
                    if (_counter > 0)
                        _mainLock.Reset();
                }
            }


            return new CountScope(this);
        }

        public void Dispose()
        {
            _superLock.Dispose();
            _mainLock.Dispose();
        }

        private void LeaveMainScope()
        {
            lock (_countLockObject)
            {
                _counter--;
                if (_counter == 0)
                    _mainLock.Set();
            }
        }

        private void LeaveSuperScope() => _superLock.Set();

        public readonly struct CountScope : IDisposable, IEquatable<CountScope>
        {
            public static CountScope Empty => new CountScope(null);

            private readonly CountedScopeSemaphore _internalSemaphore;

            public CountScope(CountedScopeSemaphore countingSemaphore) => _internalSemaphore = countingSemaphore;

            public void Dispose() => _internalSemaphore?.LeaveMainScope();

            public override bool Equals(object obj) => obj is CountScope scope && Equals(scope);
            
            public bool Equals(CountScope other) => EqualityComparer<CountedScopeSemaphore>.Default.Equals(_internalSemaphore, other._internalSemaphore);

            public override int GetHashCode() => 37286538 + EqualityComparer<CountedScopeSemaphore>.Default.GetHashCode(_internalSemaphore);

            public static bool operator ==(CountScope left, CountScope right) => left.Equals(right);
           
            public static bool operator !=(CountScope left, CountScope right) => !(left == right);
        }

        public readonly struct SuperScope : IDisposable, IEquatable<SuperScope>
        {
            public static SuperScope Empty => new SuperScope(null);

            private readonly CountedScopeSemaphore _internalSemaphore;

            public SuperScope(CountedScopeSemaphore semaphore) => _internalSemaphore = semaphore;

            public void Dispose() => _internalSemaphore?.LeaveSuperScope();

            public override bool Equals(object obj) => obj is SuperScope scope && Equals(scope);
           
            public bool Equals(SuperScope other) => EqualityComparer<CountedScopeSemaphore>.Default.Equals(_internalSemaphore, other._internalSemaphore);
            
            public override int GetHashCode() => 37296538 + EqualityComparer<CountedScopeSemaphore>.Default.GetHashCode(_internalSemaphore);

            public static bool operator ==(SuperScope left, SuperScope right) => left.Equals(right);
            
            public static bool operator !=(SuperScope left, SuperScope right) => !(left == right);
        }
    }
}
