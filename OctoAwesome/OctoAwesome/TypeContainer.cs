using System;

namespace OctoAwesome
{
    public static class TypeContainer
    {
        private static readonly ITypeContainer Instance;

        static TypeContainer()
        {
            Instance = new StandaloneTypeContainer();
            Instance.Register(Instance as StandaloneTypeContainer);
            Instance.Register<ITypeContainer, StandaloneTypeContainer>(Instance);
        }

        public static object CreateObject(Type type) => Instance.CreateObject(type);

        public static T CreateObject<T>() where T : class => Instance.CreateObject<T>();

        public static void Register(Type registrar, Type type, InstanceBehaviour instanceBehaviour) => Instance.Register(registrar, type, instanceBehaviour);

        public static void Register<T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class => Instance.Register<T>(instanceBehaviour);

        public static void Register<TRegistrar, T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class => Instance.Register<TRegistrar, T>(instanceBehaviour);

        public static void Register(Type registrar, Type type, object singleton) => Instance.Register(registrar, type, singleton);

        public static void Register<T>(T singleton) where T : class => Instance.Register(singleton);

        public static void Register<TRegistrar, T>(object singleton) where T : class => Instance.Register<TRegistrar, T>(singleton);

        public static bool TryResolve(Type type, out object resolvedInstance) => Instance.TryResolve(type, out resolvedInstance);

        public static bool TryResolve<T>(out T resolvedInstance) where T : class => Instance.TryResolve(out resolvedInstance);

        public static object Get(Type type) => Instance.Get(type);

        public static T Get<T>() where T : class => Instance.Get<T>();

        public static object GetOrNull(Type type) => Instance.GetOrNull(type);

        public static T GetOrNull<T>() where T : class => Instance.GetOrNull<T>();

        public static object GetUnregistered(Type type) => Instance.GetUnregistered(type);

        public static T GetUnregistered<T>() where T : class => Instance.GetUnregistered<T>();
    }
}