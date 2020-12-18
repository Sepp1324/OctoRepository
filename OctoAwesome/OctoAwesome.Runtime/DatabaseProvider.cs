﻿using OctoAwesome.Database;
using OctoAwesome.Logging;
using OctoAwesome.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Runtime
{
    public sealed class DatabaseProvider : IDisposable, IDatabaseProvider
    {
        private readonly Dictionary<Type, Database.Database> globalDatabaseRegister;
        private readonly LockSemaphore globalSemaphore;
        private readonly ILogger logger;
        private readonly Dictionary<(Type Type, Guid Universe, int PlanetId), Database.Database> planetDatabaseRegister;
        private readonly LockSemaphore planetSemaphore;
        private readonly string rootPath;
        private readonly Dictionary<(Type Type, Guid Universe), Database.Database> universeDatabaseRegister;
        private readonly LockSemaphore universeSemaphore;

        public DatabaseProvider(string rootPath, ILogger logger)
        {
            this.rootPath = rootPath;
            this.logger = (logger ?? NullLogger.Default).As(nameof(DatabaseProvider));
            planetSemaphore = new LockSemaphore(1, 1);
            universeSemaphore = new LockSemaphore(1, 1);
            globalSemaphore = new LockSemaphore(1, 1);
            planetDatabaseRegister = new Dictionary<(Type Type, Guid Universe, int PlanetId), Database.Database>();
            universeDatabaseRegister = new Dictionary<(Type Type, Guid Universe), Database.Database>();
            globalDatabaseRegister = new Dictionary<Type, Database.Database>();
        }

        public Database<T> GetDatabase<T>(bool fixedValueSize) where T : ITag, new()
        {
            var key = typeof(T);
            using (globalSemaphore.Wait())
            {
                if (globalDatabaseRegister.TryGetValue(key, out var database))
                {
                    return database as Database<T>;
                }
                else
                {
                    var tmpDatabase = CreateDatabase<T>(rootPath, fixedValueSize);

                    try
                    {
                        tmpDatabase.Open();
                    }
                    catch (Exception ex)
                    {
                        tmpDatabase.Dispose();
                        logger.Error($"Can not Open Database for global, {typeof(T).Name}", ex);
                        throw ex;
                    }

                    globalDatabaseRegister.Add(key, tmpDatabase);
                    return tmpDatabase;
                }
            }
        }

        public Database<T> GetDatabase<T>(Guid universeGuid, bool fixedValueSize) where T : ITag, new()
        {
            var key = (typeof(T), universeGuid);
            using (universeSemaphore.Wait())
            {
                if (universeDatabaseRegister.TryGetValue(key, out var database))
                {
                    return database as Database<T>;
                }
                else
                {
                    var tmpDatabase =
                        CreateDatabase<T>(Path.Combine(rootPath, universeGuid.ToString()), fixedValueSize);

                    try
                    {
                        tmpDatabase.Open();
                    }
                    catch (Exception ex)
                    {
                        tmpDatabase.Dispose();
                        logger.Error($"Can not Open Database for [{universeGuid}], {typeof(T).Name}", ex);
                        throw ex;
                    }

                    universeDatabaseRegister.Add(key, tmpDatabase);
                    return tmpDatabase;
                }
            }
        }

        public Database<T> GetDatabase<T>(Guid universeGuid, int planetId, bool fixedValueSize) where T : ITag, new()
        {
            var key = (typeof(T), universeGuid, planetId);
            using (planetSemaphore.Wait())
            {
                if (planetDatabaseRegister.TryGetValue(key, out var database))
                {
                    return database as Database<T>;
                }
                else
                {
                    var tmpDatabase =
                        CreateDatabase<T>(Path.Combine(rootPath, universeGuid.ToString(), planetId.ToString()),
                            fixedValueSize);
                    try
                    {
                        tmpDatabase.Open();
                    }
                    catch (Exception ex)
                    {
                        tmpDatabase.Dispose();
                        logger.Error($"Can not Open Database for [{universeGuid}]{planetId}, {typeof(T).Name}", ex);
                        throw ex;
                    }

                    planetDatabaseRegister.Add(key, tmpDatabase);
                    return tmpDatabase;
                }
            }
        }

        public void Dispose()
        {
            foreach (var database in planetDatabaseRegister)
                database.Value.Dispose();

            foreach (var database in universeDatabaseRegister)
                database.Value.Dispose();

            foreach (var database in globalDatabaseRegister)
                database.Value.Dispose();

            planetDatabaseRegister.Clear();
            universeDatabaseRegister.Clear();
            globalDatabaseRegister.Clear();

            planetSemaphore.Dispose();
            universeSemaphore.Dispose();
            globalSemaphore.Dispose();
        }

        private Database<T> CreateDatabase<T>(string path, bool fixedValueSize, string typeName = null)
            where T : ITag, new()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var type = typeof(T);
            if (typeName == null)
                typeName = type.Name;

            string name;

            foreach (var c in Path.GetInvalidFileNameChars())
            {
                typeName = typeName.Replace(c, '\0');
            }

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
            return new Database<T>(new FileInfo(keyFile), new FileInfo(valueFile), fixedValueSize);
        }
    }
}