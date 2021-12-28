using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using OctoAwesome.Database.Checks;
using OctoAwesome.Database.Threading;

namespace OctoAwesome.Database
{
    public abstract class Database : IDisposable
    {
        protected Database(Type tagType) => TagType = tagType;

        public Type TagType { get; }
        public abstract void Dispose();

        public abstract void Open();
        public abstract void Close();

        /// <summary>
        ///     Locks this Database for the specific operation
        /// </summary>
        /// <param name="mode">Indicates witch operation is currently performed</param>
        /// <returns>A new database lock</returns>
        public abstract DatabaseLock Lock(Operation mode);
    }

    public sealed class Database<TTag> : Database where TTag : ITag, new()
    {
        private readonly DatabaseLockMonitor _databaseLockMonitor;
        private readonly SemaphoreSlim _dbLockSemaphore;
        private readonly Defragmentation<TTag> _defragmentation;
        private readonly FileInfo _keyFile;
        private readonly KeyStore<TTag> _keyStore;
        private readonly FileInfo _valueFile;
        private readonly ValueStore _valueStore;
        private readonly Action _checkFunc;

        private readonly Action _startDefragFunc;

        public Database(FileInfo keyFile, FileInfo valueFile, bool fixedValueLength) : base(typeof(TTag))
        {
            _dbLockSemaphore = new(1, 1);
            _databaseLockMonitor = new();
            _keyStore = new(new(keyFile), new(keyFile));
            _valueStore = new(new(valueFile), new(valueFile), fixedValueLength);
            _defragmentation = new(keyFile, valueFile);
            var fileCheck = new ValueFileCheck<TTag>(valueFile);
            _keyFile = keyFile;
            _valueFile = valueFile;
            Threshold = 1000;
            _startDefragFunc = _defragmentation.StartDefragmentation;
            _checkFunc = fileCheck.Check;
        }

        public Database(FileInfo keyFile, FileInfo valueFile) : this(keyFile, valueFile, false)
        {
        }

        public bool FixedValueLength => _valueStore.FixedValueLength;

        public IReadOnlyList<TTag> Keys
        {
            get
            {
                using (_databaseLockMonitor.StartOperation(Operation.Read))
                {
                    return _keyStore.Tags;
                }
            }
        }

        public bool IsOpen { get; private set; }

        /// <summary>
        ///     This Threshold handels the auto defragmenation.
        ///     If the Database have more Empty Values than this Threshold the <see cref="Defragmentation" /> is executed.
        ///     Use -1 to deactivate the deframentation for this Database.
        ///     Default Value is 1000.
        /// </summary>
        public int Threshold { get; set; }

        public override void Open()
        {
            IsOpen = true;

            if (_valueFile.Exists && _valueFile.Length > 0 && (!_keyFile.Exists || _keyFile.Length == 0))
                _defragmentation.RecreateKeyFile();

            try
            {
                _keyStore.Open();
            }
            catch (Exception ex)
                when (ex is KeyInvalidException || ex is ArgumentException)
            {
                _keyStore.Close();
                _defragmentation.RecreateKeyFile();
                _keyStore.Open();
            }

            _valueStore.Open();

            if (Threshold >= 0 && _keyStore.EmptyKeys >= Threshold)
                Defragmentation();
        }

        public override void Close()
        {
            IsOpen = false;
            _keyStore.Close();
            _valueStore.Close();
        }

        public void Validate() => ExecuteOperationOnKeyValueStore(_checkFunc);

        public void Defragmentation() => ExecuteOperationOnKeyValueStore(_startDefragFunc);

        public Value GetValue(TTag tag)
        {
            using (_databaseLockMonitor.StartOperation(Operation.Read))
            {
                var key = _keyStore.GetKey(tag);
                return _valueStore.GetValue(key);
            }
        }

        public void AddOrUpdate(TTag tag, Value value)
        {
            using (_databaseLockMonitor.StartOperation(Operation.Write))
            {
                var contains = _keyStore.Contains(tag);
                if (contains)
                {
                    var key = _keyStore.GetKey(tag);

                    if (FixedValueLength)
                        _valueStore.Update(key, value);
                    else
                        _valueStore.Remove(key);
                }

                var newKey = _valueStore.AddValue(tag, value);

                if (contains)
                    _keyStore.Update(newKey);
                else
                    _keyStore.Add(newKey);
            }
        }

        public bool ContainsKey(TTag tag)
        {
            using (_databaseLockMonitor.StartOperation(Operation.Read))
            {
                return _keyStore.Contains(tag);
            }
        }

        public void Remove(TTag tag)
        {
            using (_databaseLockMonitor.StartOperation(Operation.Write))
            {
                _keyStore.Remove(tag, out var key);
                _valueStore.Remove(key);
            }
        }

        public override DatabaseLock Lock(Operation mode)
        {
            //Read -> Blocks Write && Other read is ok
            //Exclusive -> Blocks every other operation

            //Write -> Blocks Read && Other write is ok
            //Exclusive -> Blocks every other operation
            _dbLockSemaphore.Wait();
            try
            {
                if (!_databaseLockMonitor.CheckLock(mode)) _databaseLockMonitor.Wait(mode);

                var dbLock = new DatabaseLock(_databaseLockMonitor, mode);
                dbLock.Enter();
                return dbLock;
            }
            finally
            {
                _dbLockSemaphore.Release();
            }
        }

        public override void Dispose()
        {
            _keyStore.Dispose();
            _valueStore.Dispose();

            _databaseLockMonitor.Dispose();
            _dbLockSemaphore.Dispose();
        }

        private void ExecuteOperationOnKeyValueStore(Action action)
        {
            if (IsOpen)
            {
                _keyStore.Close();
                _valueStore.Close();
            }

            action();

            if (!IsOpen) 
                return;

            _keyStore.Open();
            _valueStore.Open();
        }
    }
}