using OctoAwesome.Database;
using OctoAwesome.Logging;
using OctoAwesome.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome.Runtime
{
    public sealed class DatabaseProvider : IDisposable, IDatabaseProvider
    {
        private readonly string _rootPath;
        private readonly ILogger _logger;
        private readonly LockSemaphore _planetSemaphore;
        private readonly LockSemaphore _universeSemaphore;
        private readonly LockSemaphore _globalSemaphore;
        private readonly Dictionary<(Type Type, Guid Universe, int PlanetId), Database.Database> _planetDatabaseRegister;
        private readonly Dictionary<(Type Type, Guid Universe), Database.Database> _universeDatabaseRegister;
        private readonly Dictionary<Type, Database.Database> _globalDatabaseRegister;

        public DatabaseProvider(string rootPath, ILogger logger)
        {
            _rootPath = rootPath;
            _logger = (logger ?? NullLogger.Default).As(nameof(DatabaseProvider));
            _planetSemaphore = new LockSemaphore(1, 1);
            _universeSemaphore = new LockSemaphore(1, 1);
            _globalSemaphore = new LockSemaphore(1, 1);
            _planetDatabaseRegister = new Dictionary<(Type Type, Guid Universe, int PlanetId), Database.Database>();
            _universeDatabaseRegister = new Dictionary<(Type Type, Guid Universe), Database.Database>();
            _globalDatabaseRegister = new Dictionary<Type, Database.Database>();
        }

        public Database<T> GetDatabase<T>(bool fixedValueSize) where T : ITag, new()
        {
            var key = typeof(T);
            using (_globalSemaphore.Wait())
            {
                if (_globalDatabaseRegister.TryGetValue(key, out Database.Database database))
                {
                    return database as Database<T>;
                }
                else
                {
                    var tmpDatabase = CreateDatabase<T>(_rootPath, fixedValueSize);
                    
                    try
                    {
                        tmpDatabase.Open();
                    }
                    catch (Exception ex)
                    {
                        tmpDatabase.Dispose();
                        _logger.Error($"Can not Open Database for global, {typeof(T).Name}", ex);
                        throw ex;
                    }
                    _globalDatabaseRegister.Add(key, tmpDatabase);
                    return tmpDatabase;
                }
            }
        }

        public Database<T> GetDatabase<T>(Guid universeGuid, bool fixedValueSize) where T : ITag, new()
        {
            var key = (typeof(T), universeGuid);
            using (_universeSemaphore.Wait())
            {
                if (_universeDatabaseRegister.TryGetValue(key, out Database.Database database))
                {
                    return database as Database<T>;
                }
                else
                {
                    var tmpDatabase = CreateDatabase<T>(Path.Combine(_rootPath, universeGuid.ToString()), fixedValueSize);
                    
                    try
                    {
                        tmpDatabase.Open();
                    }
                    catch (Exception ex)
                    {
                        tmpDatabase.Dispose();
                        _logger.Error($"Can not Open Database for [{universeGuid}], {typeof(T).Name}", ex);
                        throw ex;
                    }
                    _universeDatabaseRegister.Add(key, tmpDatabase);
                    return tmpDatabase;
                }
            }
        }

        public Database<T> GetDatabase<T>(Guid universeGuid, int planetId, bool fixedValueSize) where T : ITag, new()
        {
            (Type, Guid universeGuid, int planetId) key = (typeof(T), universeGuid, planetId);
            using (_planetSemaphore.Wait())
            {
                if (_planetDatabaseRegister.TryGetValue(key, out var database))
                {
                    return database as Database<T>;
                }
                else
                {
                    var tmpDatabase = CreateDatabase<T>(Path.Combine(_rootPath, universeGuid.ToString(), planetId.ToString()), fixedValueSize);
                    try
                    {
                        tmpDatabase.Open();
                    }
                    catch(Exception ex)
                    {
                        tmpDatabase.Dispose();
                        _logger.Error($"Can not Open Database for [{universeGuid}]{planetId}, {typeof(T).Name}", ex);
                        throw ex;
                    }
                    _planetDatabaseRegister.Add(key, tmpDatabase);
                    return tmpDatabase;
                }
            }
        }

        public void Dispose()
        {
            foreach (KeyValuePair<(Type Type, Guid Universe, int PlanetId), Database.Database> database in _planetDatabaseRegister)
                database.Value.Dispose();

            foreach (KeyValuePair<(Type Type, Guid Universe), Database.Database> database in _universeDatabaseRegister)
                database.Value.Dispose();

            foreach (KeyValuePair<Type, Database.Database> database in _globalDatabaseRegister)
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
            if (typeName == null)
                typeName = type.Name;

            string name;

            typeName = Path.GetInvalidFileNameChars().Aggregate(typeName, (current, c) => current.Replace(c, '\0'));

            if (type.IsGenericType)
            {
                var firstType = type.GenericTypeArguments.FirstOrDefault();

                name = firstType != default ? $"{typeName}_{firstType.Name}" : typeName;
            }
            else
            {
                name = typeName;
            }

            var keyFile = Path.Combine(path, $"{name}.keys");
            var valueFile = Path.Combine(path, $"{name}.db");
            return new Database<T>(new FileInfo(keyFile), new FileInfo(valueFile), fixedValueSize);
        }
    }
}
