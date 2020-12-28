using System;
using System.Threading;

namespace OctoAwesome.Database.Threading
{
    public sealed class DatabaseLockMonitor : IDisposable
    {
        private readonly ManualResetEvent _exclusiveEvent;

        private readonly ManualResetEvent _readEvent;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly ManualResetEvent _writeEvent;
        private bool _exclusiveLocks;
        private int _readLocks;

        private int _readOperations;
        private int _writeLocks;
        private int _writeOperations;

        public DatabaseLockMonitor()
        {
            _readEvent = new ManualResetEvent(true);
            _writeEvent = new ManualResetEvent(true);
            _exclusiveEvent = new ManualResetEvent(true);
            _semaphoreSlim = new SemaphoreSlim(1, 1);

            _readLocks = 0;
            _writeLocks = 0;
            _readOperations = 0;
            _writeOperations = 0;
            _exclusiveLocks = false;
        }

        public void Dispose()
        {
            _readEvent.Dispose();
            _writeEvent.Dispose();
            _exclusiveEvent.Dispose();
            _semaphoreSlim.Dispose();
        }

        public bool CheckLock(Operation operation)
        {
            _semaphoreSlim.Wait();
            
            try
            {
                if (_exclusiveLocks)
                    return false;

                if ((operation & Operation.Read) == Operation.Read)
                    return _writeLocks < 1;

                if ((operation & Operation.Write) == Operation.Write)
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
            if ((operation & Operation.Read) == Operation.Read)
                _writeEvent.WaitOne();

            if ((operation & Operation.Write) == Operation.Write)
                _readEvent.WaitOne();
        }

        internal DatabaseOperation StartOperation(Operation operation)
        {
            Wait(operation);
            _semaphoreSlim.Wait();
            
            try
            {
                if ((operation & Operation.Read) == Operation.Read)
                    ++_readOperations;

                if ((operation & Operation.Write) == Operation.Write)
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
                if ((operation & Operation.Read) == Operation.Read)
                    --_readOperations;

                if ((operation & Operation.Write) == Operation.Write)
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
                if ((operation & Operation.Exclusive) == Operation.Exclusive)
                    _exclusiveLocks = true;

                if ((operation & Operation.Read) == Operation.Read)
                    ++_readLocks;

                if ((operation & Operation.Write) == Operation.Write)
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
                if ((operation & Operation.Exclusive) == Operation.Exclusive)
                    _exclusiveLocks = false;

                if ((operation & Operation.Read) == Operation.Read)
                    --_readLocks;

                if ((operation & Operation.Write) == Operation.Write)
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
    }
}