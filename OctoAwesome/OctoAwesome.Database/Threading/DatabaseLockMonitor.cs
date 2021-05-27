using System;
using System.Threading;

namespace OctoAwesome.Database.Threading
{
    public sealed class DatabaseLockMonitor : IDisposable
    {
<<<<<<< HEAD
        private int _readLocks;
        private int _writeLocks;
        private bool _exclusiveLocks;

        private int _readOperations;
        private int _writeOperations;

        private readonly ManualResetEvent _readEvent;
        private readonly ManualResetEvent _writeEvent;
        private readonly ManualResetEvent _exclusiveEvent;
        private readonly SemaphoreSlim _semaphoreSlim;
=======
        private int readLocks;
        private int writeLocks;
        private bool exclusiveLocks;

        private int readOperations;
        private int writeOperations;
>>>>>>> feature/performance

        private readonly ManualResetEvent readEvent;
        private readonly ManualResetEvent writeEvent;
        private readonly ManualResetEvent exclusiveEvent;
        private readonly SemaphoreSlim semaphoreSlim;

        public DatabaseLockMonitor()
        {
<<<<<<< HEAD
            _readEvent = new ManualResetEvent(true);
            _writeEvent = new ManualResetEvent(true);
            _exclusiveEvent = new ManualResetEvent(true);
            _semaphoreSlim = new SemaphoreSlim(1, 1);

            _readLocks = 0;
            _writeLocks = 0;
            _readOperations = 0;
            _writeOperations = 0;
            _exclusiveLocks = false;
=======
            readEvent = new ManualResetEvent(true);
            writeEvent = new ManualResetEvent(true);
            exclusiveEvent = new ManualResetEvent(true);
            semaphoreSlim = new SemaphoreSlim(1, 1);

            readLocks = 0;
            writeLocks = 0;
            readOperations = 0;
            writeOperations = 0;
            exclusiveLocks = false;
>>>>>>> feature/performance
        }

        public bool CheckLock(Operation operation)
        {
            _semaphoreSlim.Wait();
            try
            {
                if (_exclusiveLocks)
                    return false;

                if (operation.HasFlag(Operation.Read))
                    return _writeLocks < 1;

                if (operation.HasFlag(Operation.Write))
                    return _readLocks < 1;
            }
            finally
            {
                _semaphoreSlim.Release();
            }

            return true;
        }

        public void Wait(Operation operation)
        {
            //if (operation.HasFlag(Operation.Exclusive))
            //    exclusiveEvent.WaitOne();

            if (operation.HasFlag(Operation.Read))
                _writeEvent.WaitOne();

            if (operation.HasFlag(Operation.Write))
<<<<<<< HEAD
                _readEvent.WaitOne();
=======
                readEvent.WaitOne();
>>>>>>> feature/performance

        }

        internal DatabaseOperation StartOperation(Operation operation)
        {
            Wait(operation);
            _semaphoreSlim.Wait();
            try
            {
                if (operation.HasFlag(Operation.Read))
                    ++_readOperations;

                if (operation.HasFlag(Operation.Write))
                    ++_writeOperations;

                return new DatabaseOperation(this, operation);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        internal void StopOperation(Operation operation)
        {
            _semaphoreSlim.Wait();
            try
            {
                if (operation.HasFlag(Operation.Read))
                    --_readOperations;

                if (operation.HasFlag(Operation.Write))
                    --_writeOperations;

                if (_readLocks == 0 && _readOperations == 0)
                    _writeEvent.Set();

                if (_writeLocks == 0 && _writeOperations == 0)
                    _readEvent.Set();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public void SetLock(Operation operation)
        {
            _semaphoreSlim.Wait();
            try
            {
                if (operation.HasFlag(Operation.Exclusive))
                    _exclusiveLocks = true;

                if (operation.HasFlag(Operation.Read))
                    ++_readLocks;

                if (operation.HasFlag(Operation.Write))
                    ++_writeLocks;

                if (_exclusiveLocks)
                {
                    _exclusiveEvent.Reset();
                    return;
                }

                if (_readLocks > 0)
                    _readEvent.Reset();

                if (_writeLocks > 0)
                    _writeEvent.Reset();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public void ReleaseLock(Operation operation)
        {
            _semaphoreSlim.Wait();
            try
            {
                if (operation.HasFlag(Operation.Exclusive))
                    _exclusiveLocks = false;

                if (operation.HasFlag(Operation.Read))
                    --_readLocks;

                if (operation.HasFlag(Operation.Write))
                    --_writeLocks;

                if (_readLocks == 0 && _readOperations == 0)
                    _readEvent.Set();

                if (_writeLocks == 0 && _writeOperations == 0)
                    _writeEvent.Set();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public void Dispose()
        {
<<<<<<< HEAD
            _readEvent.Dispose();
            _writeEvent.Dispose();
            _exclusiveEvent.Dispose();
            _semaphoreSlim.Dispose();
=======
            readEvent.Dispose();
            writeEvent.Dispose();
            exclusiveEvent.Dispose();
            semaphoreSlim.Dispose();
>>>>>>> feature/performance
        }
    }
}
