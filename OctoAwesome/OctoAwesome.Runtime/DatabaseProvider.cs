using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OctoAwesome.Database;
using OctoAwesome.Logging;
using OctoAwesome.Threading;

namespace OctoAwesome.Runtime
{
    public sealed class DatabaseProvider : IDisposable, IDatabaseProvider
    {
        private readonly Dictionary<Type, Database.Database> _globalDatabaseRegister;
        private readonly LockSemaphore _globalSemaphore;
        private readonly ILogger _logger;
        private readonly Dictionary<(Type Type, Guid Universe, int PlanetId), Database.Database> _planetDatabaseRegister;
        private readonly LockSemaphore _planetSemaphore;
        private readonly string _rootPath;
        private readonly Dictionary<(Type Type, Guid Universe), Database.Database> _universeDatabaseRegister;
        private readonly LockSemaphore _universeSemaphore;

        public DatabaseProvider(string rootPath, ILogger logger)
        {
            _rootPath = rootPath;
            _logger = (logger ?? NullLogger.Default).As(nameof(DatabaseProvider));
            _planetSemaphore = new(1, 1);
            _universeSemaphore = new(1, 1);
            _globalSemaphore = new(1, 1);
            _planetDatabaseRegister = new();
            _universeDatabaseRegister = new();
            _globalDatabaseRegister = new();
        }

        public Database<T> GetDatabase<T>(bool fixedValueSize) where T : ITag, new()
        {
            var key = typeof(T);
            using (_globalSemaphore.Wait())
            {
                if (_globalDatabaseRegister.TryGetValue(key, out var database))
                    return database as Database<T>;

                var tmpDatabase = CreateDatabase<T>(_rootPath, fixedValueSize);

                try
                {
                    tmpDatabase.Open();
                }
                catch (Exception ex)
                {
                    tmpDatabase.Dispose();
                    _logger.Error($"Can not Open Database for global, {typeof(T).Name}", ex);
                    throw;
                }

                _globalDatabaseRegister.Add(key, tmpDatabase);
                return tmpDatabase;
            }
        }

        public Database<T> GetDatabase<T>(Guid universeGuid, bool fixedValueSize) where T : ITag, new()
        {
            (Type, Guid universeGuid) key = (typeof(T), universeGuid);
            using (_universeSemaphore.Wait())
            {
                if (_universeDatabaseRegister.TryGetValue(key, out var database))
                    return database as Database<T>;

                var tmpDatabase = CreateDatabase<T>(Path.Combine(_rootPath, universeGuid.ToString()), fixedValueSize);

                try
                {
                    tmpDatabase.Open();
                }
                catch (Exception ex)
                {
                    tmpDatabase.Dispose();
                    _logger.Error($"Can not Open Database for [{universeGuid}], {typeof(T).Name}", ex);
                    throw;
                }

                _universeDatabaseRegister.Add(key, tmpDatabase);
                return tmpDatabase;
            }
        }

        public Database<T> GetDatabase<T>(Guid universeGuid, int planetId, bool fixedValueSize) where T : ITag, new()
        {
            (Type, Guid universeGuid, int planetId) key = (typeof(T), universeGuid, planetId);
            using (_planetSemaphore.Wait())
            {
                if (_planetDatabaseRegister.TryGetValue(key, out var database))
                    return database as Database<T>;

                var tmpDatabase =
                    CreateDatabase<T>(Path.Combine(_rootPath, universeGuid.ToString(), planetId.ToString()),
                        fixedValueSize);
                try
                {
                    tmpDatabase.Open();
                }
                catch (Exception ex)
                {
                    tmpDatabase.Dispose();
                    _logger.Error($"Can not Open Database for [{universeGuid}]{planetId}, {typeof(T).Name}", ex);
                    throw;
                }

                _planetDatabaseRegister.Add(key, tmpDatabase);
                return tmpDatabase;
            }
        }

        public void Dispose()
        {
            foreach (var database in _planetDatabaseRegister)
                database.Value.Dispose();

            foreach (var database in _universeDatabaseRegister)
                database.Value.Dispose();

            foreach (var database in _globalDatabaseRegister)
                database.Value.Dispose();

            _planetDatabaseRegister.Clear();
            _universeDatabaseRegister.Clear();
            _globalDatabaseRegister.Clear();

            _planetSemaphore.Dispose();
            _universeSemaphore.Dispose();
            _globalSemaphore.Dispose();
        }

        private Database<T> CreateDatabase<T>(string path, bool fixedValueSize, string typeName = null) where T : ITag, new()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var type = typeof(T);
            typeName ??= type.Name;

            string name;

            foreach (var c in Path.GetInvalidFileNameChars()) typeName = typeName.Replace(c, '\0');

            if (type.IsGenericType)
            {
                var firstType = type.GenericTypeArguments.FirstOrDefault();

                if (firstType != default)
                    name = $"{typeName}_{firstType.Name}";
                else
                    name = typeName;
            }
            else
            {
                name = typeName;
            }

            var keyFile = Path.Combine(path, $"{name}.keys");
            var valueFile = Path.Combine(path, $"{name}.db");
            return new(new(keyFile), new(valueFile), fixedValueSize);
        }
    }
}