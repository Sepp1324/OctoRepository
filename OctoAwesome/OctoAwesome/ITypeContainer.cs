﻿using System;

namespace OctoAwesome
{
    public interface ITypeContainer : IDisposable
    {
        object CreateObject(Type type);
        T CreateObject<T>() where T : class;

        void Register(Type registrar, Type type, InstanceBehaviour instanceBehaviour);
        void Register<T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class;
        void Register<TRegistrar, T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class;
        void Register(Type registrar, Type type, object singleton);
        void Register<T>(T singleton) where T : class;
        void Register<TRegistrar, T>(object singleton) where T : class;

        bool TryResolve(Type type, out object instance);
        bool TryResolve<T>(out T instance) where T : class;

        object Get(Type type);
        T Get<T>() where T : class;

        object GetUnregistered(Type type);
        T GetUnregistered<T>() where T : class;

        object GetOrNull(Type type);
        T GetOrNull<T>() where T : class;
    }
}