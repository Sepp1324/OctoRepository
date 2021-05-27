using System;
using System.Collections.Generic;
using System.Threading;

namespace OctoAwesome.Threading
{
    public class CountedScopeSemaphore : IDisposable
    {
<<<<<<< HEAD
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
=======
        private readonly ManualResetEventSlim superLock;
        private readonly ManualResetEventSlim mainLock;

        private readonly object lockObject;
        private readonly object countLockObject;
        private int counter;
        public CountedScopeSemaphore()
        {
            mainLock = new ManualResetEventSlim(true);
            superLock = new ManualResetEventSlim(true);
            lockObject = new object();
            countLockObject = new object();
>>>>>>> feature/performance
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
<<<<<<< HEAD
            _superLock.Dispose();
            _mainLock.Dispose();
=======
            superLock.Dispose();
            mainLock.Dispose();
>>>>>>> feature/performance
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

<<<<<<< HEAD
            public override bool Equals(object obj) => obj is CountScope scope && Equals(scope);
            
            public bool Equals(CountScope other) => EqualityComparer<CountedScopeSemaphore>.Default.Equals(_internalSemaphore, other._internalSemaphore);

            public override int GetHashCode() => 37286538 + EqualityComparer<CountedScopeSemaphore>.Default.GetHashCode(_internalSemaphore);

            public static bool operator ==(CountScope left, CountScope right) => left.Equals(right);
           
            public static bool operator !=(CountScope left, CountScope right) => !(left == right);
=======
            public override bool Equals(object obj)
                => obj is CountScope scope
                  && Equals(scope);
            public bool Equals(CountScope other)
                => EqualityComparer<CountedScopeSemaphore>.Default.Equals(internalSemaphore, other.internalSemaphore);

            public override int GetHashCode()
                => 37286538 + EqualityComparer<CountedScopeSemaphore>.Default.GetHashCode(internalSemaphore);

            public static bool operator ==(CountScope left, CountScope right)
                => left.Equals(right);
            public static bool operator !=(CountScope left, CountScope right)
                => !(left == right);
>>>>>>> feature/performance
        }

        public readonly struct SuperScope : IDisposable, IEquatable<SuperScope>
        {
            public static SuperScope Empty => new SuperScope(null);

<<<<<<< HEAD
            private readonly CountedScopeSemaphore _internalSemaphore;

            public SuperScope(CountedScopeSemaphore semaphore) => _internalSemaphore = semaphore;

            public void Dispose() => _internalSemaphore?.LeaveSuperScope();

            public override bool Equals(object obj) => obj is SuperScope scope && Equals(scope);
           
            public bool Equals(SuperScope other) => EqualityComparer<CountedScopeSemaphore>.Default.Equals(_internalSemaphore, other._internalSemaphore);
            
            public override int GetHashCode() => 37296538 + EqualityComparer<CountedScopeSemaphore>.Default.GetHashCode(_internalSemaphore);

            public static bool operator ==(SuperScope left, SuperScope right) => left.Equals(right);
            
=======
            private readonly CountedScopeSemaphore internalSemaphore;

            public SuperScope(CountedScopeSemaphore semaphore)
            {
                internalSemaphore = semaphore;
            }

            public void Dispose()
            {
                internalSemaphore?.LeaveSuperScope();
            }

            public override bool Equals(object obj) => obj is SuperScope scope && Equals(scope);
            public bool Equals(SuperScope other)
                => EqualityComparer<CountedScopeSemaphore>.Default.Equals(internalSemaphore, other.internalSemaphore);
            public override int GetHashCode()
                => 37296538 + EqualityComparer<CountedScopeSemaphore>.Default.GetHashCode(internalSemaphore);

            public static bool operator ==(SuperScope left, SuperScope right) => left.Equals(right);
>>>>>>> feature/performance
            public static bool operator !=(SuperScope left, SuperScope right) => !(left == right);
        }
    }
}
