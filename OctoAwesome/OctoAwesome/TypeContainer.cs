using System;

namespace OctoAwesome
{
    public static class TypeContainer
    {
        private static readonly ITypeContainer _instance;

        static TypeContainer()
        {
            _instance = new StandaloneTypeContainer();
            _instance.Register(_instance as StandaloneTypeContainer);
            _instance.Register<ITypeContainer, StandaloneTypeContainer>(_instance);
        }

<<<<<<< HEAD
        public static object CreateObject(Type type) => _instance.CreateObject(type);
        
        public static T CreateObject<T>() where T : class => _instance.CreateObject<T>();

        public static void Register(Type registrar, Type type, InstanceBehaviour instanceBehaviour) => _instance.Register(registrar, type, instanceBehaviour);
        
        public static void Register<T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class => _instance.Register<T>(instanceBehaviour);
        
        public static void Register<TRegistrar, T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class => _instance.Register<TRegistrar, T>(instanceBehaviour);
        
        public static void Register(Type registrar, Type type, object singleton) => _instance.Register(registrar, type, singleton); public static void Register<T>(T singelton) where T : class => _instance.Register(singelton);
        
        public static void Register<TRegistrar, T>(object singleton) where T : class => _instance.Register<TRegistrar, T>(singleton);

        public static bool TryResolve(Type type, out object resolvedInstance) => _instance.TryResolve(type, out resolvedInstance);
        
        public static bool TryResolve<T>(out T resolvedInstance) where T : class => _instance.TryResolve(out resolvedInstance);

        public static object Get(Type type) => _instance.Get(type);
        
        public static T Get<T>() where T : class => _instance.Get<T>();

        public static object GetOrNull(Type type) => _instance.GetOrNull(type);
        
        public static T GetOrNull<T>() where T : class => _instance.GetOrNull<T>();

        public static object GetUnregistered(Type type) => _instance.GetUnregistered(type);
        
        public static T GetUnregistered<T>() where T : class => _instance.GetUnregistered<T>();
=======
        public static object CreateObject(Type type)
            => instance.CreateObject(type);
        public static T CreateObject<T>() where T : class
            => instance.CreateObject<T>();

        public static void Register(Type registrar, Type type, InstanceBehaviour instanceBehaviour)
            => instance.Register(registrar, type, instanceBehaviour);
        public static void Register<T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class
            => instance.Register<T>(instanceBehaviour);
        public static void Register<TRegistrar, T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class
            => instance.Register<TRegistrar, T>(instanceBehaviour);
        public static void Register(Type registrar, Type type, object singelton)
             => instance.Register(registrar, type, singelton);
        public static void Register<T>(T singelton) where T : class
             => instance.Register(singelton);
        public static void Register<TRegistrar, T>(object singelton) where T : class
             => instance.Register<TRegistrar, T>(singelton);

        public static bool TryResolve(Type type, out object resolvedInstance)
             => instance.TryResolve(type, out resolvedInstance);
        public static bool TryResolve<T>(out T resolvedInstance) where T : class
            => instance.TryResolve(out resolvedInstance);

        public static object Get(Type type)
            => instance.Get(type);
        public static T Get<T>() where T : class
            => instance.Get<T>();

        public static object GetOrNull(Type type)
            => instance.GetOrNull(type);
        public static T GetOrNull<T>() where T : class
            => instance.GetOrNull<T>();

        public static object GetUnregistered(Type type)
            => instance.GetUnregistered(type);
        public static T GetUnregistered<T>() where T : class
            => instance.GetUnregistered<T>();

>>>>>>> feature/performance
    }
}
