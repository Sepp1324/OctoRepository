using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome
{
    public sealed class StandaloneTypeContainer : ITypeContainer
    {

        private readonly Dictionary<Type, TypeInformation> typeInformationRegister;
        private readonly Dictionary<Type, Type> typeRegister;

        public StandaloneTypeContainer()
        {
            typeInformationRegister = new Dictionary<Type, TypeInformation>();
            typeRegister = new Dictionary<Type, Type>();
        }


        public void Register(Type registrar, Type type, InstanceBehaviour instanceBehaviour)
        {
            if (!typeInformationRegister.ContainsKey(type))
                typeInformationRegister.Add(type, new TypeInformation(this, type, instanceBehaviour));

<<<<<<< HEAD
            _typeRegister.Add(registrar, type);
        }
        
        public void Register<T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class => Register(typeof(T), typeof(T), instanceBehaviour);
        
        public void Register<TRegistrar, T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class => Register(typeof(TRegistrar), typeof(T), instanceBehaviour);
        
        public void Register(Type registrar, Type type, object singleton)
        {
            if (!_typeInformationRegister.ContainsKey(type))
                _typeInformationRegister.Add(type, new TypeInformation(this, type, InstanceBehaviour.Singleton, singleton));
=======
            typeRegister.Add(registrar, type);
        }
        public void Register<T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class
            => Register(typeof(T), typeof(T), instanceBehaviour);
        public void Register<TRegistrar, T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class
            => Register(typeof(TRegistrar), typeof(T), instanceBehaviour);
        public void Register(Type registrar, Type type, object singelton)
        {
            if (!typeInformationRegister.ContainsKey(type))
                typeInformationRegister.Add(type, new TypeInformation(this, type, InstanceBehaviour.Singleton, singelton));
>>>>>>> feature/performance

            typeRegister.Add(registrar, type);
        }
<<<<<<< HEAD
        
        public void Register<T>(T singleton) where T : class => Register(typeof(T), typeof(T), singleton);
        
        public void Register<TRegistrar, T>(object singleton) where T : class => Register(typeof(TRegistrar), typeof(T), singleton);
=======
        public void Register<T>(T singelton) where T : class
            => Register(typeof(T), typeof(T), singelton);
        public void Register<TRegistrar, T>(object singelton) where T : class
            => Register(typeof(TRegistrar), typeof(T), singelton);
>>>>>>> feature/performance

        public bool TryResolve(Type type, out object instance)
        {
            instance = GetOrNull(type);
            return instance != null;
        }
<<<<<<< HEAD
        
=======
>>>>>>> feature/performance
        public bool TryResolve<T>(out T instance) where T : class
        {
            var result = TryResolve(typeof(T), out var obj);
            instance = (T)obj;
            return result;
        }

        public object Get(Type type)
            => GetOrNull(type) ?? throw new KeyNotFoundException($"Type {type} was not found in Container");

<<<<<<< HEAD
        public T Get<T>() where T : class => (T)Get(typeof(T));
=======
        public T Get<T>() where T : class
            => (T)Get(typeof(T));
>>>>>>> feature/performance

        public object GetOrNull(Type type)
        {
            if (typeRegister.TryGetValue(type, out var searchType))
            {
                if (typeInformationRegister.TryGetValue(searchType, out var typeInformation))
                    return typeInformation.Instance;
            }
            return null;
        }
<<<<<<< HEAD
        
        public T GetOrNull<T>() where T : class => (T)GetOrNull(typeof(T));

        public object GetUnregistered(Type type) => GetOrNull(type) ?? CreateObject(type) ?? throw new InvalidOperationException($"Can not create unregistered type of {type}");

        public T GetUnregistered<T>() where T : class => (T)GetUnregistered(typeof(T));
=======
        public T GetOrNull<T>() where T : class
            => (T)GetOrNull(typeof(T));

        public object GetUnregistered(Type type)
            => GetOrNull(type)
                ?? CreateObject(type)
                ?? throw new InvalidOperationException($"Can not create unregistered type of {type}");

        public T GetUnregistered<T>() where T : class
            => (T)GetUnregistered(typeof(T));
>>>>>>> feature/performance

        public object CreateObject(Type type)
        {
            var tmpList = new List<object>();

            var constructors = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length);

            foreach (var constructor in constructors)
            {
                bool next = false;
                foreach (var parameter in constructor.GetParameters())
                {
                    if (TryResolve(parameter.ParameterType, out object instance))
                    {
                        tmpList.Add(instance);
                    }
                    else if (!parameter.IsOptional)
                    {
                        tmpList.Clear();
                        next = true;
                        break;
                    }
                }

                if (next)
                    continue;

                return constructor.Invoke(tmpList.ToArray());
            }

            if (!constructors.Any())
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
<<<<<<< HEAD
        public T CreateObject<T>() where T : class => (T)CreateObject(typeof(T));
=======
        public T CreateObject<T>() where T : class
            => (T)CreateObject(typeof(T));
>>>>>>> feature/performance

        public void Dispose()
        {
            typeRegister.Clear();
            typeInformationRegister.Values
                .Where(t => t.Behaviour == InstanceBehaviour.Singleton && t.Instance != this)
                .Select(t => t.Instance as IDisposable)
                .ToList()
                .ForEach(i => i?.Dispose());

            typeInformationRegister.Clear();
        }

        private class TypeInformation
        {
<<<<<<< HEAD
            private readonly StandaloneTypeContainer _typeContainer;
            private readonly Type _type;
            private object _singletonInstance;
            
            public InstanceBehaviour Behaviour { get; private set; }
            public object Instance => CreateObject();
=======
            public InstanceBehaviour Behaviour { get; set; }
            public object Instance => CreateObject();

            private readonly StandaloneTypeContainer typeContainer;
            private readonly Type type;
            private object singeltonInstance;
>>>>>>> feature/performance

            public TypeInformation(StandaloneTypeContainer container,
                Type type, InstanceBehaviour instanceBehaviour, object instance = null)
            {
                this.type = type;
                Behaviour = instanceBehaviour;
<<<<<<< HEAD
                _typeContainer = container;
                _singletonInstance = instance;
=======
                typeContainer = container;
                singeltonInstance = instance;
>>>>>>> feature/performance
            }

            private object CreateObject()
            {
<<<<<<< HEAD
                if (Behaviour == InstanceBehaviour.Singleton && _singletonInstance != null)
                    return _singletonInstance;
=======
                if (Behaviour == InstanceBehaviour.Singleton && singeltonInstance != null)
                    return singeltonInstance;
>>>>>>> feature/performance

                var obj = typeContainer.CreateObject(type);

                if (Behaviour == InstanceBehaviour.Singleton)
<<<<<<< HEAD
                    _singletonInstance = obj;
=======
                    singeltonInstance = obj;
>>>>>>> feature/performance

                return obj;
            }
        }
    }
}
