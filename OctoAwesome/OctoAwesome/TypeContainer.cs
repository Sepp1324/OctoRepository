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

        public static object CreateObject(Type type)
        {
            return instance.CreateObject(type);
        }

        public static T CreateObject<T>() where T : class
        {
            return instance.CreateObject<T>();
        }

        public static void Register(Type registrar, Type type, InstanceBehaviour instanceBehaviour)
        {
            instance.Register(registrar, type, instanceBehaviour);
        }

        public static void Register<T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class
        {
            instance.Register<T>(instanceBehaviour);
        }

        public static void Register<TRegistrar, T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance)
            where T : class
        {
            instance.Register<TRegistrar, T>(instanceBehaviour);
        }

        public static void Register(Type registrar, Type type, object singelton)
        {
            instance.Register(registrar, type, singelton);
        }

        public static void Register<T>(T singelton) where T : class
        {
            instance.Register(singelton);
        }

        public static void Register<TRegistrar, T>(object singelton) where T : class
        {
            instance.Register<TRegistrar, T>(singelton);
        }

        public static bool TryResolve(Type type, out object resolvedInstance)
        {
            return instance.TryResolve(type, out resolvedInstance);
        }

        public static bool TryResolve<T>(out T resolvedInstance) where T : class
        {
            return instance.TryResolve(out resolvedInstance);
        }

        public static object Get(Type type)
        {
            return instance.Get(type);
        }

        public static T Get<T>() where T : class
        {
            return instance.Get<T>();
        }

        public static object GetOrNull(Type type)
        {
            return instance.GetOrNull(type);
        }

        public static T GetOrNull<T>() where T : class
        {
            return instance.GetOrNull<T>();
        }

        public static object GetUnregistered(Type type)
        {
            return instance.GetUnregistered(type);
        }

        public static T GetUnregistered<T>() where T : class
        {
            return instance.GetUnregistered<T>();
        }
    }
}