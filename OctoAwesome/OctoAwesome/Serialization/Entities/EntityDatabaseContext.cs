using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OctoAwesome.Serialization.Entities
{
    public sealed class EntityDatabaseContext : IDatabaseContext<GuidTag<Entity>, Entity>
    {
        private readonly EntityDefinition.EntityDefinitionContext _entityDefinitionContext;
        private readonly EntityComponentsDatabaseContext _componentsDbContext;
        private readonly MethodInfo _getComponentMethod;
        private readonly MethodInfo _removeComponentMethod;

        public EntityDatabaseContext(IDatabaseProvider databaseProvider, Guid universe)
        {
            var database = databaseProvider.GetDatabase<GuidTag<EntityDefinition>>(universeGuid: universe, fixedValueSize: false);
            _entityDefinitionContext = new EntityDefinition.EntityDefinitionContext(database: database);
            _componentsDbContext = new EntityComponentsDatabaseContext(databaseProvider: databaseProvider, universe: universe);
            _getComponentMethod = typeof(EntityComponentsDatabaseContext).GetMethod(nameof(EntityComponentsDatabaseContext.Get), new[] { typeof(Entity) });
            _removeComponentMethod = typeof(EntityComponentsDatabaseContext).GetMethod(nameof(EntityComponentsDatabaseContext.Remove));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void AddOrUpdate(Entity value)
        {
            _entityDefinitionContext.AddOrUpdate(value: new EntityDefinition(value));

            foreach (dynamic component in value.Components) //dynamic so tyepof<T> in get database returns correct type 
                _componentsDbContext.AddOrUpdate(component, value);
        }

        public Entity Get(GuidTag<Entity> key)
        {
            var definition = _entityDefinitionContext.Get(new GuidTag<EntityDefinition>(key.Tag));
            var entity = (Entity)Activator.CreateInstance(definition.Type);
            entity.Id = definition.Id;

            foreach (var component in definition.Components)
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
