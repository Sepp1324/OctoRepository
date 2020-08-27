using System;

namespace OctoAwesome
{
    public static class TypeContainer 
    {
        private static readonly ITypeContainer instance;

        static TypeContainer()
        {
            instance = new StandaloneTypeContainer();
            instance.Register(instance as StandaloneTypeContainer);
            instance.Register<ITypeContainer, StandaloneTypeContainer>(instance);
        }

        public static object CreateObject(Type type) => instance.CreateObject(type);

        public static T CreateObject<T>() where T : class => instance.CreateObject<T>();

        public static void Register(Type type, InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) => instance.Register(type, instanceBehaviour);

        public static void Register(Type registrar, Type type, InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) => instance.Register(registrar, type, instanceBehaviour);

        public static void Register<T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class => instance.Register<T>(instanceBehaviour);

        public static void Register<TRegistrar, T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class => instance.Register<TRegistrar, T>(instanceBehaviour);

        public static void Register(Type type, object singleton) => instance.Register(type, singleton);

        public static void Register(Type registrar, Type type, object singleton) => instance.Register(registrar, type, singleton);

        public static void Register<T>(T singleton) where T : class => instance.Register(singleton);

        public static void Register<TRegistrar, T>(object singleton) where T : class => instance.Register<TRegistrar, T>(singleton);

        public static bool TryResolve(Type type, out object resolvedInstance) => instance.TryResolve(type, out resolvedInstance);

        public static bool TryResolve<T>(out T resolvedInstance) where T : class => instance.TryResolve<T>(out resolvedInstance);

        public static object Get(Type type) => instance.Get(type);

        public static T Get<T>() where T : class => instance.Get<T>();
    }
}
