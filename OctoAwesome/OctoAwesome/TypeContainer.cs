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

        public static object CreateObject(Type type) => throw new NotImplementedException();
        public static T CreateObject<T>() where T : class => throw new NotImplementedException();
        public static void Register(Type type, StandaloneTypeContainer.InstanceBehaviour instanceBehaviour = StandaloneTypeContainer.InstanceBehaviour.Instance) => throw new NotImplementedException();
        public static void Register(Type registrar, Type type, StandaloneTypeContainer.InstanceBehaviour instanceBehaviour = StandaloneTypeContainer.InstanceBehaviour.Instance) => throw new NotImplementedException();
        public static void Register<T>(StandaloneTypeContainer.InstanceBehaviour instanceBehaviour = StandaloneTypeContainer.InstanceBehaviour.Instance) where T : class => throw new NotImplementedException();
        public static void Register<TRegistrar, T>(StandaloneTypeContainer.InstanceBehaviour instanceBehaviour = StandaloneTypeContainer.InstanceBehaviour.Instance) where T : class => throw new NotImplementedException();
        public static void Register(Type type, object singleton) => throw new NotImplementedException();
        public static void Register(Type registrar, Type type, object singleton) => throw new NotImplementedException();
        public static void Register<T>(T singleton) where T : class => throw new NotImplementedException();
        public static void Register<TRegistrar, T>(object singleton) where T : class => throw new NotImplementedException();
        public static bool TryResolve(Type type, out object instance) => throw new NotImplementedException();
        public static bool TryResolve<T>(out T instance) where T : class => throw new NotImplementedException();
    }
}
