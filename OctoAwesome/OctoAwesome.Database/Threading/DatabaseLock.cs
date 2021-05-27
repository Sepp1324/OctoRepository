using System;
using System.Collections.Generic;

namespace OctoAwesome.Database.Threading
{
    public readonly struct DatabaseLock : IDisposable, IEquatable<DatabaseLock>
    {
        public static DatabaseLock Empty = default;

        public bool IsEmpty => this == default;

        private readonly DatabaseLockMonitor _lockMonitor;
        private readonly Operation _currentOperation;

        public DatabaseLock(DatabaseLockMonitor lockMonitor, Operation operation)
        {
            _lockMonitor = lockMonitor;
            _currentOperation = operation;
        }

        public void Enter() => _lockMonitor.SetLock(_currentOperation);

<<<<<<< HEAD
        public void Dispose() => _lockMonitor.ReleaseLock(_currentOperation);

        public override bool Equals(object obj) => obj is DatabaseLock @lock && Equals(@lock);
       
        public bool Equals(DatabaseLock other) => EqualityComparer<DatabaseLockMonitor>.Default.Equals(_lockMonitor, other._lockMonitor) && _currentOperation == other._currentOperation;
=======
        public void Dispose()
        {
            lockMonitor.ReleaseLock(currentOperation);
        }

        public override bool Equals(object obj) 
            => obj is DatabaseLock @lock && Equals(@lock);
        public bool Equals(DatabaseLock other) 
            => EqualityComparer<DatabaseLockMonitor>.Default.Equals(lockMonitor, other.lockMonitor) 
            && currentOperation == other.currentOperation;
>>>>>>> feature/performance

        public override int GetHashCode()
        {
            var hashCode = 1919164243;
            hashCode = hashCode * -1521134295 + EqualityComparer<DatabaseLockMonitor>.Default.GetHashCode(_lockMonitor);
            hashCode = hashCode * -1521134295 + _currentOperation.GetHashCode();
            return hashCode;
        }

<<<<<<< HEAD
        public static bool operator ==(DatabaseLock left, DatabaseLock right) => left.Equals(right);
       
        public static bool operator !=(DatabaseLock left, DatabaseLock right) => !(left == right);
=======
        public static bool operator ==(DatabaseLock left, DatabaseLock right) 
            => left.Equals(right);
        public static bool operator !=(DatabaseLock left, DatabaseLock right) 
            => !(left == right);
>>>>>>> feature/performance
    }
}
