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
<<<<<<< HEAD
        private readonly string _rootPath;
        private readonly ILogger _logger;
        private readonly LockSemaphore _planetSemaphore;
        private readonly LockSemaphore _universeSemaphore;
        private readonly LockSemaphore _globalSemaphore;
        private readonly Dictionary<(Type Type, Guid Universe, int PlanetId), Database.Database> _planetDatabaseRegister;
        private readonly Dictionary<(Type Type, Guid Universe), Database.Database> _universeDatabaseRegister;
        private readonly Dictionary<Type, Database.Database> _globalDatabaseRegister;
=======
        private readonly string rootPath;
        private readonly ILogger logger;
        private readonly LockSemaphore planetSemaphore;
        private readonly LockSemaphore universeSemaphore;
        private readonly LockSemaphore globalSemaphore;
        private readonly Dictionary<(Type Type, Guid Universe, int PlanetId), Database.Database> planetDatabaseRegister;
        private readonly Dictionary<(Type Type, Guid Universe), Database.Database> universeDatabaseRegister;
        private readonly Dictionary<Type, Database.Database> globalDatabaseRegister;
>>>>>>> feature/performance

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
<<<<<<< HEAD
            var key = typeof(T);
            using (_globalSemaphore.Wait())
            {
                if (_globalDatabaseRegister.TryGetValue(key, out Database.Database database))
=======
            Type key = typeof(T);
            using (globalSemaphore.Wait())
            {
                if (globalDatabaseRegister.TryGetValue(key, out Database.Database database))
>>>>>>> feature/performance
                {
                    return database as Database<T>;
                }
                else
                {
<<<<<<< HEAD
                    var tmpDatabase = CreateDatabase<T>(_rootPath, fixedValueSize);
=======
                    Database<T> tmpDatabase = CreateDatabase<T>(rootPath, fixedValueSize);
>>>>>>> feature/performance
                    
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
<<<<<<< HEAD
                    _globalDatabaseRegister.Add(key, tmpDatabase);
=======
                    globalDatabaseRegister.Add(key, tmpDatabase);
>>>>>>> feature/performance
                    return tmpDatabase;
                }
            }
        }

        public Database<T> GetDatabase<T>(Guid universeGuid, bool fixedValueSize) where T : ITag, new()
        {
<<<<<<< HEAD
            var key = (typeof(T), universeGuid);
            using (_universeSemaphore.Wait())
            {
                if (_universeDatabaseRegister.TryGetValue(key, out Database.Database database))
=======
            (Type, Guid universeGuid) key = (typeof(T), universeGuid);
            using (universeSemaphore.Wait())
            {
                if (universeDatabaseRegister.TryGetValue(key, out Database.Database database))
>>>>>>> feature/performance
                {
                    return database as Database<T>;
                }
                else
                {
<<<<<<< HEAD
                    var tmpDatabase = CreateDatabase<T>(Path.Combine(_rootPath, universeGuid.ToString()), fixedValueSize);
=======
                    Database<T> tmpDatabase = CreateDatabase<T>(Path.Combine(rootPath, universeGuid.ToString()), fixedValueSize);
>>>>>>> feature/performance
                    
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
<<<<<<< HEAD
                    _universeDatabaseRegister.Add(key, tmpDatabase);
=======
                    universeDatabaseRegister.Add(key, tmpDatabase);
>>>>>>> feature/performance
                    return tmpDatabase;
                }
            }
        }

        public Database<T> GetDatabase<T>(Guid universeGuid, int planetId, bool fixedValueSize) where T : ITag, new()
        {
            (Type, Guid universeGuid, int planetId) key = (typeof(T), universeGuid, planetId);
<<<<<<< HEAD
            using (_planetSemaphore.Wait())
            {
                if (_planetDatabaseRegister.TryGetValue(key, out var database))
=======
            using (planetSemaphore.Wait())
            {
                if (planetDatabaseRegister.TryGetValue(key, out Database.Database database))
>>>>>>> feature/performance
                {
                    return database as Database<T>;
                }
                else
                {
<<<<<<< HEAD
                    var tmpDatabase = CreateDatabase<T>(Path.Combine(_rootPath, universeGuid.ToString(), planetId.ToString()), fixedValueSize);
=======
                    Database<T> tmpDatabase = CreateDatabase<T>(Path.Combine(rootPath, universeGuid.ToString(), planetId.ToString()), fixedValueSize);
>>>>>>> feature/performance
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
<<<<<<< HEAD
                    _planetDatabaseRegister.Add(key, tmpDatabase);
=======
                    planetDatabaseRegister.Add(key, tmpDatabase);
>>>>>>> feature/performance
                    return tmpDatabase;
                }
            }
        }

        public void Dispose()
        {
<<<<<<< HEAD
            foreach (var database in _planetDatabaseRegister)
                database.Value.Dispose();

            foreach (var database in _universeDatabaseRegister)
                database.Value.Dispose();

            foreach (var database in _globalDatabaseRegister)
=======
            foreach (KeyValuePair<(Type Type, Guid Universe, int PlanetId), Database.Database> database in planetDatabaseRegister)
                database.Value.Dispose();

            foreach (KeyValuePair<(Type Type, Guid Universe), Database.Database> database in universeDatabaseRegister)
                database.Value.Dispose();

            foreach (KeyValuePair<Type, Database.Database> database in globalDatabaseRegister)
>>>>>>> feature/performance
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

            Type type = typeof(T);
            if (typeName == null)
                typeName = type.Name;

            string name;

            typeName = Path.GetInvalidFileNameChars().Aggregate(typeName, (current, c) => current.Replace(c, '\0'));

            if (type.IsGenericType)
            {
                Type firstType = type.GenericTypeArguments.FirstOrDefault();

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
