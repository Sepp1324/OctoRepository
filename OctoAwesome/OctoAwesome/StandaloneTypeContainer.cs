using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome
{
    public sealed class StandaloneTypeContainer : ITypeContainer
    {
        private readonly Dictionary<Type, TypeInformation> _typeInformationRegister;
        private readonly Dictionary<Type, Type> _typeRegister;

        public StandaloneTypeContainer()
        {
            _typeInformationRegister = new();
            _typeRegister = new();
        }


        public void Register(Type registrar, Type type, InstanceBehaviour instanceBehaviour)
        {
            if (!_typeInformationRegister.ContainsKey(type))
                _typeInformationRegister.Add(type, new(this, type, instanceBehaviour));

            _typeRegister.Add(registrar, type);
        }

        public void Register<T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class
        {
            Register(typeof(T), typeof(T), instanceBehaviour);
        }

        public void Register<TRegistrar, T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance)
            where T : class
        {
            Register(typeof(TRegistrar), typeof(T), instanceBehaviour);
        }

        public void Register(Type registrar, Type type, object singleton)
        {
            if (!_typeInformationRegister.ContainsKey(type))
                _typeInformationRegister.Add(type, new(this, type, InstanceBehaviour.Singleton, singleton));

            _typeRegister.Add(registrar, type);
        }

        public void Register<T>(T singleton) where T : class
        {
            Register(typeof(T), typeof(T), singleton);
        }

        public void Register<TRegistrar, T>(object singleton) where T : class
        {
            Register(typeof(TRegistrar), typeof(T), singleton);
        }

        public bool TryResolve(Type type, out object instance)
        {
            instance = GetOrNull(type);
            return instance != null;
        }

        public bool TryResolve<T>(out T instance) where T : class
        {
            var result = TryResolve(typeof(T), out var obj);
            instance = (T)obj;
            return result;
        }

        public object Get(Type type) => GetOrNull(type) ?? throw new KeyNotFoundException($"Type {type} was not found in Container");

        public T Get<T>() where T : class => (T)Get(typeof(T));

        public object GetOrNull(Type type)
        {
            if (!_typeRegister.TryGetValue(type, out var searchType))
                return null;

            return _typeInformationRegister.TryGetValue(searchType, out var typeInformation)
                ? typeInformation.Instance
                : null;
        }

        public T GetOrNull<T>() where T : class => (T)GetOrNull(typeof(T));

        public object GetUnregistered(Type type) =>
            GetOrNull(type) ?? CreateObject(type) ??
            throw new InvalidOperationException($"Can not create unregistered type of {type}");

        public T GetUnregistered<T>() where T : class => (T)GetUnregistered(typeof(T));

        public object CreateObject(Type type)
        {
            var tmpList = new List<object>();

            var constructors = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length);

            foreach (var constructor in constructors)
            {
                var next = false;
                foreach (var parameter in constructor.GetParameters())
                    if (TryResolve(parameter.ParameterType, out var instance))
                    {
                        tmpList.Add(instance);
                    }
                    else if (!parameter.IsOptional)
                    {
                        tmpList.Clear();
                        next = true;
                        break;
                    }

                if (next)
                    continue;

                return constructor.Invoke(tmpList.ToArray());
            }

            if (constructors.Count() < 1)
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch
                {
                    return null;
                }

            return null;
        }

        public T CreateObject<T>() where T : class => (T)CreateObject(typeof(T));

        public void Dispose()
        {
            _typeRegister.Clear();
            _typeInformationRegister.Values.Where(t => t.Behaviour == InstanceBehaviour.Singleton && t.Instance != this)
                .Select(t => t.Instance as IDisposable).ToList().ForEach(i => i?.Dispose());

            _typeInformationRegister.Clear();
        }

        private class TypeInformation
        {
            private readonly Type _type;

            private readonly StandaloneTypeContainer _typeContainer;
            private object _singeltonInstance;

            public TypeInformation(StandaloneTypeContainer container, Type type, InstanceBehaviour instanceBehaviour,
                object instance = null)
            {
                _type = type;
                Behaviour = instanceBehaviour;
                _typeContainer = container;
                _singeltonInstance = instance;
            }

            public InstanceBehaviour Behaviour { get; }

            public object Instance => CreateObject();

            private object CreateObject()
            {
                if (Behaviour == InstanceBehaviour.Singleton && _singeltonInstance != null)
                    return _singeltonInstance;

                var obj = _typeContainer.CreateObject(_type);

                if (Behaviour == InstanceBehaviour.Singleton)
                    _singeltonInstance = obj;

                return obj;
            }
        }
    }
}