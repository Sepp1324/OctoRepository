using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome.Runtime
{
    public sealed class DatabaseProvider
    {
        private readonly Dictionary<(Type Type, Guid Universe, int PlanetId), Database.Database> _planetDatabaseRegister;
        private readonly Dictionary<(Type Type, Guid Universe), Database.Database> _universeDatabaseRegister;
        private readonly Dictionary<Type, Database.Database> _globalDatabaseRegister;
        private readonly string _rootPath;

        public DatabaseProvider(string rootPath)
        {
            _rootPath = rootPath;
            _planetDatabaseRegister = new Dictionary<(Type Type, Guid Universe, int PlanetId), Database.Database>();
            _universeDatabaseRegister = new Dictionary<(Type Type, Guid Universe), Database.Database>();
            _globalDatabaseRegister = new Dictionary<Type, Database.Database>();
        }

        public Database<T> GetDatabase<T>() where T : ITag, new()
        {
            var key = typeof(T);

            if (_globalDatabaseRegister.TryGetValue(key, out var database))
            {
                return database as Database<T>;
            }
            else
            {
                Database<T> tmpDatabase = CreateDatabase<T>(_rootPath);
                _globalDatabaseRegister.Add(key, tmpDatabase);
                return tmpDatabase;
            }
        }

        public Database<T> GetDatabase<T>(Guid universeGuid) where T : ITag, new()
        {
            var key = (typeof(T), universeGuid);

            if (_universeDatabaseRegister.TryGetValue(key, out var database))
            {
                return database as Database<T>;
            }
            else
            {
                Database<T> tmpDatabase = CreateDatabase<T>(Path.Combine(_rootPath, universeGuid.ToString()));
                _universeDatabaseRegister.Add(key, tmpDatabase);
                return tmpDatabase;
            }
        }

        public Database<T> GetDatabase<T>(Guid universeGuid, int planetId) where T : ITag, new()
        {
            var key = (typeof(T), universeGuid, planetId);

            if (_planetDatabaseRegister.TryGetValue(key, out var database))
            {
                return database as Database<T>;
            }
            else
            {
                Database<T> tmpDatabase = CreateDatabase<T>(Path.Combine(_rootPath, universeGuid.ToString(), planetId.ToString()));
                _planetDatabaseRegister.Add(key, tmpDatabase);
                return tmpDatabase;
            }
        }

        private Database<T> CreateDatabase<T>(string path) where T : ITag, new()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var name = typeof(T).Name.Substring(0, typeof(T).Name.Length - 3);
            var keyFile = Path.Combine(path, $"{name}.keys");
            var valueFile = Path.Combine(path, $"{name}.db");
            return new Database<T>(new FileInfo(keyFile), new FileInfo(valueFile));
        }
    }
}
