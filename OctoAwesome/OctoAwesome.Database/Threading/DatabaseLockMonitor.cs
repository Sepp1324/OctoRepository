using System.Threading;

namespace OctoAwesome.Database.Threading
{
    internal sealed class DatabaseLockMonitor
    {
        private int _readLock;
        private int _writeLock;
        private int _exclusiveLock;

        private readonly ManualResetEvent _readEvent;
        private readonly ManualResetEvent _writeEvent;

        public DatabaseLockMonitor()
        {
            _readEvent = new ManualResetEvent(true);
            _writeEvent = new ManualResetEvent(true);

            _readLock = 0;
            _writeLock = 0;
            _exclusiveLock = 0;
        }

        public bool CheckLock(Operation operation)
        {
            if (_exclusiveLock > 0)
                return false;

            if (operation.HasFlag(Operation.Read))
                return _writeLock < 1;

            if (operation.HasFlag(Operation.Write))
                return _readLock < 1;

            return true;
        }

        public void Wait(Operation operation)
        {
            if (operation.HasFlag(Operation.Read))
                _writeEvent.WaitOne();

            if (operation.HasFlag(Operation.Write))
                _readEvent.WaitOne();
        }

        public void SetLock(Operation operation)
        {
            if (operation.HasFlag(Operation.Exclusive))
                Interlocked.Increment(ref _exclusiveLock);

            if (operation.HasFlag(Operation.Read))
                Interlocked.Increment(ref _readLock);

            if (operation.HasFlag(Operation.Write))
                Interlocked.Increment(ref _writeLock);
        }

        public void ReleaseLock(Operation operation)
        {
            if (operation.HasFlag(Operation.Exclusive))
                Interlocked.Decrement(ref _exclusiveLock);

            if (operation.HasFlag(Operation.Read))
                Interlocked.Decrement(ref _readLock);

            if (operation.HasFlag(Operation.Write))
                Interlocked.Decrement(ref _writeLock);
        }
    }
}
