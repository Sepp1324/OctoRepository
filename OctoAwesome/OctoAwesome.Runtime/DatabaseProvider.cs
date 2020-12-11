using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OctoAwesome.Threading;

namespace OctoAwesome.Runtime
{
    public sealed class DatabaseProvider : IDisposable, IDatabaseProvider
    {
        private readonly string _rootPath;
        private readonly Dictionary<(Type Type, Guid Universe, int PlanetId), Database.Database> _planetDatabaseRegister;
        private readonly Dictionary<(Type Type, Guid Universe), Database.Database> _universeDatabaseRegister;
        private readonly Dictionary<Type, Database.Database> _globalDatabaseRegister;
        private readonly LockSemaphore _planetSemaphore;
        private readonly LockSemaphore _universeSemaphore;
        private readonly LockSemaphore _globalSemaphore;

        public DatabaseProvider(string rootPath)
        {
            _rootPath = rootPath;
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
                if (_globalDatabaseRegister.TryGetValue(key, out var database))
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
                    catch
                    {
                        tmpDatabase.Dispose();
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
                if (_universeDatabaseRegister.TryGetValue(key, out var database))
                {
                    return database as Database<T>;
                }
                else
                {
                    var tmpDatabase = CreateDatabase<T>(Path.Combine(_rootPath, universeGuid.ToString()), fixedValueSize);
                   
                    _universeDatabaseRegister.Add(key, tmpDatabase);
                    tmpDatabase.Open();
                    return tmpDatabase;
                }
            }
        }

        public Database<T> GetDatabase<T>(Guid universeGuid, int planetId, bool fixedValueSize) where T : ITag, new()
        {
            var key = (typeof(T), universeGuid, planetId);

            using (_planetSemaphore.Wait())
            {
                if (_planetDatabaseRegister.TryGetValue(key, out var database))
                {
                    return database as Database<T>;
                }
                else
                {
                    var tmpDatabase = CreateDatabase<T>(Path.Combine(_rootPath, universeGuid.ToString(), planetId.ToString()), fixedValueSize);
                   
                    _planetDatabaseRegister.Add(key, tmpDatabase);
                    tmpDatabase.Open();
                    return tmpDatabase;
                }
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

            _globalSemaphore.Dispose();
            _universeSemaphore.Dispose();
            _planetSemaphore.Dispose();
        }

        private Database<T> CreateDatabase<T>(string path, bool fixedValueSize, string typeName = null) where T : ITag, new()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            var type = typeof(T);
            if (typeName == null)
                typeName = type.Name;

            string name;

            foreach (var c in Path.GetInvalidFileNameChars())
                typeName = typeName.Replace(c, '\0');

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
