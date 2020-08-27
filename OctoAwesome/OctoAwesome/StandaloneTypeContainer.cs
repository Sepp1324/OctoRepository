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


        public void Register(Type type, InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) => typeInformationRegister.Add(type, new TypeInformation(this, type, instanceBehaviour));

        public void Register(Type registrar, Type type, InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance)
        {
            if (!typeInformationRegister.ContainsKey(type))
                typeInformationRegister.Add(type, new TypeInformation(this, type, instanceBehaviour));

            typeRegister.Add(registrar, type);
        }

        public void Register<T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class => Register(typeof(T), instanceBehaviour);

        public void Register<TRegistrar, T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class => Register(typeof(TRegistrar), typeof(T), instanceBehaviour);

        public void Register(Type type, object singleton) => typeInformationRegister.Add(type, new TypeInformation(this, type, InstanceBehaviour.Singleton, singleton));

        public void Register(Type registrar, Type type, object singleton)
        {
            if (!typeInformationRegister.ContainsKey(type))
                typeInformationRegister.Add(type, new TypeInformation(this, type, InstanceBehaviour.Singleton, singleton));

            typeRegister.Add(registrar, type);
        }

        public void Register<T>(T singleton) where T : class => Register(typeof(T), singleton);

        public void Register<TRegistrar, T>(object singleton) where T : class => Register(typeof(TRegistrar), typeof(T), singleton);

        public bool TryResolve(Type type, out object instance)
        {
            if (typeInformationRegister.TryGetValue(type, out TypeInformation typeInformation))
            {
                instance = typeInformation.Instance;
                return true;
            }

            if (typeRegister.TryGetValue(type, out var searchType))
            {
                if (typeInformationRegister.TryGetValue(searchType, out typeInformation))
                {
                    instance = typeInformation.Instance;
                    return true;
                }
            }

            instance = Activator.CreateInstance(type);
            return instance == null;
        }

        public bool TryResolve<T>(out T instance) where T : class
        {
            bool result = TryResolve(typeof(T), out var obj);
            instance = (T)obj;
            return result;
        }

        public object CreateObject(Type type)
        {
            var tmpList = new List<object>();
            foreach (var constructor in type.GetConstructors().OrderByDescending(c => c.GetParameters().Length))
            {
                bool next = false;
                foreach (var parameter in constructor.GetParameters())
                {
                    if (TryResolve(parameter.ParameterType, out object instance))
                    {
                        tmpList.Add(instance);
                    }
                    else
                    {
                        tmpList.Clear();
                        next = true;
                        break;
                    }
                }

                if (next)
                    continue;

                return constructor.Invoke(type, tmpList.ToArray());
            }
            return null;
        }

        public T CreateObject<T>() where T : class => (T)CreateObject(typeof(T));

        private class TypeInformation
        {
            public InstanceBehaviour Behaviour { get; set; }
            public object Instance => CreateObject();

            private readonly StandaloneTypeContainer typeContainer;
            private readonly Type type;
            private object singeltonInstance;

            public TypeInformation(StandaloneTypeContainer container, Type type, InstanceBehaviour instanceBehaviour, object instance = null)
            {
                this.type = type;
                Behaviour = instanceBehaviour;
                typeContainer = container;
                singeltonInstance = instance;
            }

            private object CreateObject()
            {
                if (Behaviour == InstanceBehaviour.Singleton && singeltonInstance != null)
                    return singeltonInstance;

                var obj = typeContainer.CreateObject(type);

                if (Behaviour == InstanceBehaviour.Singleton)
                    singeltonInstance = obj;

                return obj;
            }
        }
    }
}
