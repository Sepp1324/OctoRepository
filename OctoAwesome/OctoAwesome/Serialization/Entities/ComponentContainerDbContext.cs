using OctoAwesome.Components;
using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OctoAwesome.Serialization.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    public sealed class ComponentContainerDbContext<TContainer> : IDatabaseContext<GuidTag<ComponentContainer<TContainer>>, ComponentContainer<TContainer>> where TContainer : IComponent
    {
        private readonly ComponentContainerComponentDbContext<TContainer> _componentsDbContext;

        private readonly ComponentContainerDefinition<TContainer>.ComponentContainerDefinitionContext<TContainer> _entityDefinitionContext;

        private readonly MethodInfo _getComponentMethod;
        private readonly MethodInfo _removeComponentMethod;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseProvider"></param>
        /// <param name="universe"></param>
        public ComponentContainerDbContext(IDatabaseProvider databaseProvider, Guid universe)
        {
            var database = databaseProvider.GetDatabase<GuidTag<ComponentContainerDefinition<TContainer>>>(universe, false);
            _entityDefinitionContext = new ComponentContainerDefinition<TContainer>.ComponentContainerDefinitionContext<TContainer>(database);
            _componentsDbContext = new ComponentContainerComponentDbContext<TContainer>(databaseProvider, universe);
            _getComponentMethod = typeof(ComponentContainerComponentDbContext<TContainer>).GetMethod(nameof(ComponentContainerComponentDbContext<TContainer>.Get), new[] { typeof(ComponentContainer<TContainer>) });
            _removeComponentMethod = typeof(ComponentContainerComponentDbContext<TContainer>).GetMethod(nameof(ComponentContainerComponentDbContext<TContainer>.Remove));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void AddOrUpdate(ComponentContainer<TContainer> value)
        {
            _entityDefinitionContext.AddOrUpdate(new ComponentContainerDefinition<TContainer>(value));

            foreach (dynamic component in value.Components) //dynamic so tyepof<T> in get database returns correct type 
                _componentsDbContext.AddOrUpdate(component, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ComponentContainer<TContainer> Get(GuidTag<ComponentContainer<TContainer>> key)
        {
            var definition = _entityDefinitionContext.Get(new GuidTag<ComponentContainerDefinition<TContainer>>(key.Tag));
            var entity = (ComponentContainer<TContainer>)Activator.CreateInstance(definition.Type);
            entity!.Id = definition.Id;

            foreach (var component in definition.Components)
            {
                var genericMethod = _getComponentMethod.MakeGenericMethod(component);
                entity.Components.AddComponent((TContainer)genericMethod.Invoke(_componentsDbContext, new object[] { entity }));
            }

            return entity;
        }

        /// <summary>
        /// Removes ComponentContainer
        /// </summary>
        /// <param name="value">ComponentContainer</param>
        public void Remove(ComponentContainer<TContainer> value)
        {
            var definition = _entityDefinitionContext.Get(new GuidTag<ComponentContainerDefinition<TContainer>>(value.Id));
            _entityDefinitionContext.Remove(definition);

            foreach (var component in definition.Components)
            {
                var genericMethod = _removeComponentMethod.MakeGenericMethod(component);
                genericMethod.Invoke(_componentsDbContext, new object[] { value });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<ComponentContainer<TContainer>> GetComponentContainerWithComponent<T>() where T : IComponent
        {
            var entities = _componentsDbContext.GetAllKeys<T>().Select(t => new GuidTag<ComponentContainer<TContainer>>(t.Tag));

            foreach (var entityId in entities)
                yield return Get(entityId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<GuidTag<ComponentContainer<TContainer>>> GetComponentContainerIdsFromComponent<T>() where T : IComponent => _componentsDbContext.GetAllKeys<T>().Select(t => new GuidTag<ComponentContainer<TContainer>>(t.Tag));

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GuidTag<ComponentContainer<TContainer>>> GetAllKeys() => _entityDefinitionContext.GetAllKeys().Select(e => new GuidTag<ComponentContainer<TContainer>>(e.Tag));
    }
}