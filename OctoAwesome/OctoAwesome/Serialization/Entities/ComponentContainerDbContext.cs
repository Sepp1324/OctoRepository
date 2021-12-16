using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OctoAwesome.Components;
using OctoAwesome.Database;

namespace OctoAwesome.Serialization.Entities
{
    public sealed class
        ComponentContainerDbContext<TContainer> : IDatabaseContext<GuidTag<ComponentContainer<TContainer>>,
            ComponentContainer<TContainer>> where TContainer : IComponent
    {
        private readonly ComponentContainerComponentDbContext<TContainer> componentsDbContext;

        private readonly ComponentContainerDefinition<TContainer>.ComponentContainerDefinitionContext<TContainer>
            entityDefinitionContext;

        private readonly MethodInfo getComponentMethod;
        private readonly MethodInfo removeComponentMethod;

        public ComponentContainerDbContext(IDatabaseProvider databaseProvider, Guid universe)
        {
            var database =
                databaseProvider.GetDatabase<GuidTag<ComponentContainerDefinition<TContainer>>>(universe, false);
            entityDefinitionContext =
                new ComponentContainerDefinition<TContainer>.ComponentContainerDefinitionContext<TContainer>(database);
            componentsDbContext = new ComponentContainerComponentDbContext<TContainer>(databaseProvider, universe);
            getComponentMethod = typeof(ComponentContainerComponentDbContext<TContainer>).GetMethod(
                nameof(ComponentContainerComponentDbContext<TContainer>.Get),
                new[] { typeof(ComponentContainer<TContainer>) });
            removeComponentMethod =
                typeof(ComponentContainerComponentDbContext<TContainer>).GetMethod(
                    nameof(ComponentContainerComponentDbContext<TContainer>.Remove));
        }

        public void AddOrUpdate(ComponentContainer<TContainer> value)
        {
            entityDefinitionContext.AddOrUpdate(new ComponentContainerDefinition<TContainer>(value));

            foreach (dynamic component in value.Components) //dynamic so tyepof<T> in get database returns correct type 
                componentsDbContext.AddOrUpdate(component, value);
        }

        public ComponentContainer<TContainer> Get(GuidTag<ComponentContainer<TContainer>> key)
        {
            var definition =
                entityDefinitionContext.Get(new GuidTag<ComponentContainerDefinition<TContainer>>(key.Tag));
            var entity = (ComponentContainer<TContainer>)Activator.CreateInstance(definition.Type);
            entity!.Id = definition.Id;

            foreach (var component in definition.Components)
            {
                var genericMethod = getComponentMethod.MakeGenericMethod(component);
                entity.Components.AddComponent((TContainer)genericMethod.Invoke(componentsDbContext,
                    new object[] { entity }));
            }

            return entity;
        }

        public void Remove(ComponentContainer<TContainer> value)
        {
            var definition =
                entityDefinitionContext.Get(new GuidTag<ComponentContainerDefinition<TContainer>>(value.Id));
            entityDefinitionContext.Remove(definition);

            foreach (var component in definition.Components)
            {
                var genericMethod = removeComponentMethod.MakeGenericMethod(component);
                genericMethod.Invoke(componentsDbContext, new object[] { value });
            }
        }

        public IEnumerable<ComponentContainer<TContainer>> GetComponentContainerWithComponent<T>() where T : IComponent
        {
            var entities = componentsDbContext.GetAllKeys<T>()
                .Select(t => new GuidTag<ComponentContainer<TContainer>>(t.Tag));

            foreach (var entityId in entities)
                yield return Get(entityId);
        }

        public IEnumerable<GuidTag<ComponentContainer<TContainer>>> GetComponentContainerIdsFromComponent<T>()
            where T : IComponent
        {
            return componentsDbContext.GetAllKeys<T>().Select(t => new GuidTag<ComponentContainer<TContainer>>(t.Tag));
        }

        public IEnumerable<GuidTag<ComponentContainer<TContainer>>> GetAllKeys()
        {
            return entityDefinitionContext.GetAllKeys().Select(e => new GuidTag<ComponentContainer<TContainer>>(e.Tag));
        }
    }
}