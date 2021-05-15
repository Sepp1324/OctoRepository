using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// ExtensionLoader
    /// </summary>
    public sealed class ExtensionLoader : IExtensionLoader, IExtensionResolver
    {
        private const string SETTINGSKEY = "DisabledExtensions";

        private readonly List<Type> _entities;
        private readonly Dictionary<Type, List<Action<Entity>>> _entityExtender;
        private readonly List<Action<Simulation>> _simulationExtender;
        private readonly List<IMapGenerator> _mapGenerators;
        private readonly List<IMapPopulator> _mapPopulators;

        /// <summary>
        /// List of Loaded Extensions
        /// </summary>
        public List<IExtension> LoadedExtensions { get; private set; }

        /// <summary>
        /// List of active Extensions
        /// </summary>
        public List<IExtension> ActiveExtensions { get; private set; }

        private readonly Dictionary<Type, List<Type>> _definitionsLookup;

        private readonly ISettings _settings;
        private readonly ITypeContainer _typeContainer;
        private readonly ITypeContainer _definitionTypeContainer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings">Current Gamesettings</param>
        public ExtensionLoader(ITypeContainer typeContainer, ISettings settings)
        {
            _settings = settings;
            _typeContainer = typeContainer;
            _definitionTypeContainer = new StandaloneTypeContainer();
            _definitionsLookup = new Dictionary<Type, List<Type>>();
            _entities = new List<Type>();
            _entityExtender = new Dictionary<Type, List<Action<Entity>>>();
            _simulationExtender = new List<Action<Simulation>>();
            _mapGenerators = new List<IMapGenerator>();
            _mapPopulators = new List<IMapPopulator>();
            LoadedExtensions = new List<IExtension>();
            ActiveExtensions = new List<IExtension>();

        }

        /// <summary>
        /// Load all Plugins
        /// </summary>
        public void LoadExtensions()
        {
            var assemblies = new List<Assembly>();
            var tempAssembly = Assembly.GetEntryAssembly();

            if (tempAssembly == null)
                tempAssembly = Assembly.GetAssembly(GetType());

            var dir = new DirectoryInfo(Path.GetDirectoryName(tempAssembly.Location));
            assemblies.AddRange(LoadAssemblies(dir));

            var plugins = new DirectoryInfo(Path.Combine(dir.FullName, "plugins"));
            if (plugins.Exists)
                assemblies.AddRange(LoadAssemblies(plugins));

            var disabledExtensions = _settings.KeyExists(SETTINGSKEY) ? _settings.GetArray<string>(SETTINGSKEY) : new string[0];

            var result = new List<Type>();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes().Where(t => typeof(IExtension).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var type in types)
                {
                    try
                    {
                        var extension = (IExtension)Activator.CreateInstance(type);
                        extension.Register(_typeContainer);
                        extension.Register(this, _typeContainer);

                        if (disabledExtensions.Contains(type.FullName))
                            LoadedExtensions.Add(extension);
                        else
                            ActiveExtensions.Add(extension);
                    }
                    catch (Exception ex)
                    {
                        // TODO: Logging
                    }
                }
            }
        }

        private IEnumerable<Assembly> LoadAssemblies(DirectoryInfo directory)
        {
            var assemblies = new List<Assembly>();
            foreach (var file in directory.GetFiles("*.dll"))
            {
                try
                {
                    var assembly = Assembly.LoadFile(file.FullName);
                    assemblies.Add(assembly);
                }
                catch (Exception)
                {
                    // TODO: Error Handling
                }
            }
            return assemblies;
        }

        /// <summary>
        /// Activate the Extenisons
        /// </summary>
        /// <param name="disabledExtensions">List of Extensions</param>
        public void ApplyExtensions(IList<IExtension> disabledExtensions)
        {
            var types = disabledExtensions.Select(e => e.GetType().FullName).ToArray();
            _settings.Set(SETTINGSKEY, types);
        }

        #region Loader Methods

        /// <summary>
        /// Registers a new Definition.
        /// </summary>
        /// <param name="definition">Definition Instance</param>
        public void RegisterDefinition(Type definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            var interfaceTypes = definition.GetInterfaces();

            foreach (var interfaceType in interfaceTypes)
            {
                if (_definitionsLookup.TryGetValue(interfaceType, out var typeList))
                {
                    typeList.Add(definition);
                }
                else
                {
                    _definitionsLookup.Add(interfaceType, new List<Type> { definition });
                }
            }

            _definitionTypeContainer.Register(definition, definition, InstanceBehaviour.Singleton);
        }

        /// <summary>
        /// Removes an existing Definition Type.
        /// </summary>
        /// <typeparam name="T">Definition Type</typeparam>
        public void RemoveDefinition<T>() where T : IDefinition => throw new NotSupportedException("Currently not supported by TypeContainer");

        /// <summary>
        /// Registers a new Entity.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        public void RegisterEntity<T>() where T : Entity
        {
            var type = typeof(T);
            if (_entities.Contains(type))
                throw new ArgumentException("Already registered");

            _entities.Add(type);
        }

        /// <summary>
        /// Adds a new Extender for the given Entity Type.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="extenderDelegate">Extender Delegate</param>
        public void RegisterEntityExtender<T>(Action<Entity> extenderDelegate) where T : Entity
        {
            var type = typeof(T);
            if (!_entityExtender.TryGetValue(type, out var list))
            {
                list = new List<Action<Entity>>();
                _entityExtender.Add(type, list);
            }
            list.Add(extenderDelegate);
        }

        public void RegisterDefaultEntityExtender<T>() where T : Entity => RegisterEntityExtender<T>((e) => e.RegisterDefault());

        /// <summary>
        /// Adds a new Extender for the simulation.
        /// </summary>
        /// <param name="extenderDelegate"></param>
        public void RegisterSimulationExtender(Action<Simulation> extenderDelegate) => _simulationExtender.Add(extenderDelegate);

        /// <summary>
        /// Adds a new Map Generator.
        /// </summary>
        public void RegisterMapGenerator(IMapGenerator generator) => _mapGenerators.Add(generator);

        public void RegisterMapPopulator(IMapPopulator populator) => _mapPopulators.Add(populator);

        /// <summary>
        /// Removes an existing Entity Type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveEntity<T>() where T : Entity => _entities.Remove(typeof(T));

        /// <summary>
        /// Removes an existing Map Generator.
        /// </summary>
        /// <typeparam name="T">Map Generator Type</typeparam>
        public void RemoveMapGenerator<T>(T item) where T : IMapGenerator => _mapGenerators.Remove(item);

        public void RemoveMapPopulator<T>(T item) where T : IMapPopulator => _mapPopulators.Remove(item);

        #endregion

        #region Resolver Methods

        /// <summary>
        /// Extend a Simulation
        /// </summary>
        /// <param name="simulation">Simulation</param>
        public void ExtendSimulation(Simulation simulation)
        {
            foreach (var extender in _simulationExtender)
                extender(simulation);
        }

        /// <summary>
        /// Extend a Entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void ExtendEntity(Entity entity)
        {
            var stack = new List<Type>();
            var t = entity.GetType();
            stack.Add(t);
            do
            {
                t = t.BaseType;
                stack.Add(t);
            }
            while (t != typeof(Entity));
            stack.Reverse();

            foreach (var type in stack)
            {
                if (!_entityExtender.TryGetValue(type, out var list))
                    continue;

                foreach (var item in list)
                    item(entity);
            }
        }

        /// <summary>
        /// Return a List of Definitions
        /// </summary>
        /// <typeparam name="T">Definitiontype</typeparam>
        /// <returns>List</returns>
        public IEnumerable<T> GetDefinitions<T>() where T : class, IDefinition
        {
            if (_definitionsLookup.TryGetValue(typeof(T), out var definitionTypes))
            {
                foreach (var type in definitionTypes)
                    yield return (T)_definitionTypeContainer.Get(type);
            }
        }

        /// <summary>
        /// Return a List of MapGenerators
        /// </summary>
        /// <returns>List of Generators</returns>
        public IEnumerable<IMapGenerator> GetMapGenerator() => _mapGenerators;

        /// <summary>
        /// Return a List of Populators
        /// </summary>
        /// <returns>List of Populators</returns>
        public IEnumerable<IMapPopulator> GetMapPopulator() => _mapPopulators;

        #endregion
    }
}
