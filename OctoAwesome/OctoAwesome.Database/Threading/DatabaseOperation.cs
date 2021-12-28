using System;
using System.Collections.Generic;

namespace OctoAwesome.Database.Threading
{
    internal readonly struct DatabaseOperation : IDisposable, IEquatable<DatabaseOperation>
    {
        public static DatabaseOperation Empty = default;

        public bool IsEmpty => this == default;

        private readonly DatabaseLockMonitor _lockMonitor;
        private readonly Operation _currentOperation;

        public DatabaseOperation(DatabaseLockMonitor lockMonitor, Operation operation)
        {
            _lockMonitor = lockMonitor;
            _currentOperation = operation;
        }

        public void Dispose() => _lockMonitor.StopOperation(_currentOperation);

        public override bool Equals(object obj) => obj is DatabaseOperation @lock && Equals(@lock);

        public bool Equals(DatabaseOperation other) => EqualityComparer<DatabaseLockMonitor>.Default.Equals(_lockMonitor, other._lockMonitor) && _currentOperation == other._currentOperation;

        public override int GetHashCode()
        {
            var hashCode = 1919164243;
            hashCode = hashCode * -1521134295 + EqualityComparer<DatabaseLockMonitor>.Default.GetHashCode(_lockMonitor);
            hashCode = hashCode * -1521134295 + _currentOperation.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(DatabaseOperation left, DatabaseOperation right) => left.Equals(right);

        public static bool operator !=(DatabaseOperation left, DatabaseOperation right) => !(left == right);
    }
}