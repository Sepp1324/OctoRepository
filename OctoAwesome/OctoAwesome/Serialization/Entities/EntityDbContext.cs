using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OctoAwesome.Serialization.Entities
{
    public sealed class EntityDbContext : IDatabaseContext<GuidTag<Entity>, Entity>
    {
        private readonly EntityDefinition.EntityDefinitionContext _entityDefinitionContext;
        private readonly EntityComponentsDbContext _componentsDbContext;
        private readonly MethodInfo _getComponentMethod;
        private readonly MethodInfo _removeComponentMethod;

        public EntityDbContext(IDatabaseProvider databaseProvider, Guid universe)
        {
            var database = databaseProvider.GetDatabase<GuidTag<EntityDefinition>>(universeGuid: universe, fixedValueSize: false);
            _entityDefinitionContext = new EntityDefinition.EntityDefinitionContext(database);
            _componentsDbContext = new EntityComponentsDbContext(databaseProvider, universe);
            _getComponentMethod = typeof(EntityComponentsDbContext).GetMethod(nameof(EntityComponentsDbContext.Get), new[] { typeof(Entity) });
            _removeComponentMethod = typeof(EntityComponentsDbContext).GetMethod(nameof(EntityComponentsDbContext.Remove));
        }

        public void AddOrUpdate(Entity value)
        {
            _entityDefinitionContext.AddOrUpdate(new EntityDefinition(value));

            foreach (dynamic component in value.Components) //dynamic so tyepof<T> in get database returns correct type 
                _componentsDbContext.AddOrUpdate(component, value);
        }

        public Entity Get(GuidTag<Entity> key)
        {
            var definition = _entityDefinitionContext.Get(new GuidTag<EntityDefinition>(key.Tag));
            var entity = (Entity)Activator.CreateInstance(definition.Type);
            entity.Id = definition.Id;

            foreach (Type component in definition.Components)
            {
                var genericMethod = _getComponentMethod.MakeGenericMethod(component);
                entity.Components.AddComponent((EntityComponent)genericMethod.Invoke(_componentsDbContext, new object[] { entity }));
            }

            return entity;
        }

        public IEnumerable<Entity> GetEntitiesWithComponent<T>() where T : EntityComponent
        {
            var entities = _componentsDbContext.GetAllKeys<T>().Select(t => new GuidTag<Entity>(t.Tag));

            foreach (var entityId in entities)
                yield return Get(entityId);
        }

        public IEnumerable<GuidTag<Entity>> GetEntityIdsFromComponent<T>() where T : EntityComponent => _componentsDbContext.GetAllKeys<T>().Select(t => new GuidTag<Entity>(t.Tag));

        public IEnumerable<GuidTag<Entity>> GetAllKeys() => _entityDefinitionContext.GetAllKeys().Select(e => new GuidTag<Entity>(e.Tag));

        public void Remove(Entity value)
        {
            var definition = _entityDefinitionContext.Get(new GuidTag<EntityDefinition>(value.Id));
            _entityDefinitionContext.Remove(definition);

            foreach (var component in definition.Components)
            {
                var genericMethod = _removeComponentMethod.MakeGenericMethod(component);
                genericMethod.Invoke(_componentsDbContext, new object[] { value });
            }
        }
    }
}
