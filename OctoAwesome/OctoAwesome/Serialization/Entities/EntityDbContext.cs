using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OctoAwesome.Serialization.Entities
{
    public sealed class EntityDbContext : IDatabaseContext<GuidTag<Entity>, Entity>
    {
<<<<<<< HEAD
        private readonly EntityDefinition.EntityDefinitionContext _entityDefinitionContext;
        private readonly EntityComponentsDbContext _componentsDbContext;
        private readonly MethodInfo _getComponentMethod;
        private readonly MethodInfo _removeComponentMethod;
=======
        private readonly EntityDefinition.EntityDefinitionContext entityDefinitionContext;
        private readonly EntityComponentsDbContext componentsDbContext;
        private readonly MethodInfo getComponentMethod;
        private readonly MethodInfo removeComponentMethod;
>>>>>>> feature/performance

        public EntityDbContext(IDatabaseProvider databaseProvider, Guid universe)
        {
            var database = databaseProvider.GetDatabase<GuidTag<EntityDefinition>>(universeGuid: universe, fixedValueSize: false);
<<<<<<< HEAD
            _entityDefinitionContext = new EntityDefinition.EntityDefinitionContext(database);
            _componentsDbContext = new EntityComponentsDbContext(databaseProvider, universe);
            _getComponentMethod = typeof(EntityComponentsDbContext).GetMethod(nameof(EntityComponentsDbContext.Get), new[] { typeof(Entity) });
            _removeComponentMethod = typeof(EntityComponentsDbContext).GetMethod(nameof(EntityComponentsDbContext.Remove));
=======
            entityDefinitionContext = new EntityDefinition.EntityDefinitionContext(database);
            componentsDbContext = new EntityComponentsDbContext(databaseProvider, universe);
            getComponentMethod = typeof(EntityComponentsDbContext).GetMethod(nameof(EntityComponentsDbContext.Get), new[] { typeof(Entity) });
            removeComponentMethod = typeof(EntityComponentsDbContext).GetMethod(nameof(EntityComponentsDbContext.Remove));
>>>>>>> feature/performance
        }

        public void AddOrUpdate(Entity value)
        {
            _entityDefinitionContext.AddOrUpdate(new EntityDefinition(value));

            foreach (dynamic component in value.Components) //dynamic so tyepof<T> in get database returns correct type 
                _componentsDbContext.AddOrUpdate(component, value);
        }

        public Entity Get(GuidTag<Entity> key)
        {
<<<<<<< HEAD
            var definition = _entityDefinitionContext.Get(new GuidTag<EntityDefinition>(key.Tag));
=======
            EntityDefinition definition = entityDefinitionContext.Get(new GuidTag<EntityDefinition>(key.Tag));
>>>>>>> feature/performance
            var entity = (Entity)Activator.CreateInstance(definition.Type);
            entity.Id = definition.Id;

            foreach (Type component in definition.Components)
            {
<<<<<<< HEAD
                var genericMethod = _getComponentMethod.MakeGenericMethod(component);
                entity.Components.AddComponent((EntityComponent)genericMethod.Invoke(_componentsDbContext, new object[] { entity }));
=======
                MethodInfo genericMethod = getComponentMethod.MakeGenericMethod(component);
                entity.Components.AddComponent((EntityComponent)genericMethod.Invoke(componentsDbContext, new object[] { entity }));
>>>>>>> feature/performance
            }

            return entity;
        }

        public IEnumerable<Entity> GetEntitiesWithComponent<T>() where T : EntityComponent
        {
<<<<<<< HEAD
            var entities = _componentsDbContext.GetAllKeys<T>().Select(t => new GuidTag<Entity>(t.Tag));
=======
            IEnumerable<GuidTag<Entity>> entities = componentsDbContext.GetAllKeys<T>().Select(t => new GuidTag<Entity>(t.Tag));
>>>>>>> feature/performance

            foreach (GuidTag<Entity> entityId in entities)
                yield return Get(entityId);
        }

<<<<<<< HEAD
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
=======
        public IEnumerable<GuidTag<Entity>> GetEntityIdsFromComponent<T>() where T : EntityComponent
            => componentsDbContext.GetAllKeys<T>().Select(t => new GuidTag<Entity>(t.Tag));

        public IEnumerable<GuidTag<Entity>> GetAllKeys()
            => entityDefinitionContext.GetAllKeys().Select(e => new GuidTag<Entity>(e.Tag));

        public void Remove(Entity value)
        {
            EntityDefinition definition = entityDefinitionContext.Get(new GuidTag<EntityDefinition>(value.Id));
            entityDefinitionContext.Remove(definition);

            foreach (Type component in definition.Components)
            {
                MethodInfo genericMethod = removeComponentMethod.MakeGenericMethod(component);
                genericMethod.Invoke(componentsDbContext, new object[] { value });
>>>>>>> feature/performance
            }
        }
    }
}
