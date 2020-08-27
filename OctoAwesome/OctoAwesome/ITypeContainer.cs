﻿using System;

namespace OctoAwesome
{
    public interface ITypeContainer
    {
        object CreateObject(Type type);
        T CreateObject<T>() where T : class;
        void Register(Type type, StandaloneTypeContainer.InstanceBehaviour instanceBehaviour = StandaloneTypeContainer.InstanceBehaviour.Instance);
        void Register(Type registrar, Type type, StandaloneTypeContainer.InstanceBehaviour instanceBehaviour = StandaloneTypeContainer.InstanceBehaviour.Instance);
        void Register<T>(StandaloneTypeContainer.InstanceBehaviour instanceBehaviour = StandaloneTypeContainer.InstanceBehaviour.Instance) where T : class;
        void Register<TRegistrar, T>(StandaloneTypeContainer.InstanceBehaviour instanceBehaviour = StandaloneTypeContainer.InstanceBehaviour.Instance) where T : class;
        void Register(Type type, object singleton);
        void Register(Type registrar, Type type, object singleton);
        void Register<T>(T singleton) where T : class;
        void Register<TRegistrar, T>(object singleton) where T : class;
        bool TryResolve(Type type, out object instance);
        bool TryResolve<T>(out T instance) where T : class;
    }
}