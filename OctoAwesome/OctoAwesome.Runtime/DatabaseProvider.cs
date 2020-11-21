using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome.Runtime
{
    public sealed class DatabaseProvider
    {
        private readonly Dictionary<(Type Type, Guid Universe, int PlanetId), Database.Database> _databaseRegister;
        private readonly string _rootPath;

        public DatabaseProvider(string rootPath)
        {
            _rootPath = rootPath;
            _databaseRegister = new Dictionary<(Type Type, Guid Universe, int PlanetId), Database.Database>();
        }

        public Database<T> GetDatabase<T>(Guid universeGuid, int planetId) where T : ITag, new()
        {
            var key = (typeof(T), universeGuid, planetId);

            if (_databaseRegister.TryGetValue(key, out var database))
            {
                return database as Database<T>;
            } 
            else
            {
                var name = typeof(T).Name.Substring(0, typeof(T).Name.Length - 3);

                string path = Path.Combine(_rootPath, universeGuid.ToString(), planetId.ToString());
                Directory.CreateDirectory(path);

                string keyFile = Path.Combine(path, $"{name}.keys");
                var valueFile = Path.Combine(path, $"{name}.db");
             
                var tmpDatabase = new Database<T>(new FileInfo(keyFile), new FileInfo(valueFile));
                _databaseRegister.Add(key, tmpDatabase);
                return tmpDatabase;
            }
        }
    }
}
