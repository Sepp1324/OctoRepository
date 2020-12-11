using OctoAwesome.Database.Checks;
using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome.Database
{
    public abstract class Database : IDisposable
    {
        public Type TagType { get; }

        protected Database(Type tagType) => TagType = tagType;

        public abstract void Open();

        public abstract void Close();

        public abstract void Dispose();
    }

    public sealed class Database<TTag> : Database where TTag : ITag, new()
    {
        public bool FixedValueLength => _valueStore.FixedValueLength;

        public IEnumerable<TTag> Keys => _keyStore.Tags;

        public bool IsOpen { get; private set; }

        /// <summary>
        /// This Threshold handels the auto defragmenation. 
        /// If the Database have more Empty Values than this Threshold the <see cref="Defragmentation"/> is executed.
        /// Use -1 to deactivate the deframentation for this Database.
        /// Default Value is 1000.
        /// </summary>
        public int Threshold { get; set; }

        private readonly KeyStore<TTag> _keyStore;
        private readonly ValueStore _valueStore;
        private readonly Defragmentation<TTag> _defragmentation;
        private readonly ValueFileCheck<TTag> _fileCheck;
        private readonly FileInfo _keyFile;
        private readonly FileInfo _valueFile;

        public Database(FileInfo keyFile, FileInfo valueFile, bool fixedValueLength) : base(typeof(TTag))
        {
            _keyStore = new KeyStore<TTag>(new Writer(keyFile), new Reader(keyFile));
            _valueStore = new ValueStore(new Writer(valueFile), new Reader(valueFile), fixedValueLength);
            _defragmentation = new Defragmentation<TTag>(keyFile, valueFile);
            _fileCheck = new ValueFileCheck<TTag>(valueFile);
            _keyFile = keyFile;
            _valueFile = valueFile;
            Threshold = 1000;
        }

        public Database(FileInfo keyFile, FileInfo valueFile) : this(keyFile, valueFile, false)
        {
        }

        public override void Open()
        {
            IsOpen = true;

            if (_valueFile.Exists &&_valueFile.Length > 0 && (!_keyFile.Exists || _keyFile.Length == 0))
                _defragmentation.RecreateKeyFile();

            try
            {
                _keyStore.Open();
            }
            catch (Exception ex) when (ex is KeyInvalidException || ex is ArgumentException)
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

        public void Validate() => ExecuteOperationOnKeyValueStore(_fileCheck.Check);

        public void Defragmentation() => ExecuteOperationOnKeyValueStore(_defragmentation.StartDefragmentation);

        public Value GetValue(TTag tag)
        {
            var key = _keyStore.GetKey(tag);
            return _valueStore.GetValue(key);
        }

        public void AddOrUpdate(TTag tag, Value value)
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

        public bool ContainsKey(TTag tag) => _keyStore.Contains(tag);

        public void Remove(TTag tag)
        {
            _keyStore.Remove(tag, out var key);
            _valueStore.Remove(key);
        }

        public override void Dispose()
        {
            _keyStore.Dispose();
            _valueStore.Dispose();
        }

        private void ExecuteOperationOnKeyValueStore(Action action)
        {
            if (IsOpen)
            {
                _keyStore.Close();
                _valueStore.Close();
            }

            action();

            if (IsOpen)
            {
                _keyStore.Open();
                _valueStore.Open();
            }
        }
    }
}
