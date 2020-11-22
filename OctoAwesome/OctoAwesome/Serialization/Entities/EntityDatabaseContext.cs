using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OctoAwesome.Serialization.Entities
{
    public sealed class EntityDatabaseContext : IDatabaseContext<IdTag<Entity>, Entity>
    {
        private readonly EntityDefinition.EntityDefinitionContext _entityDefinitionContext;
        private readonly EntityComponentsDatabaseContext _componentsDatabaseContext;
        private readonly MethodInfo _getComponentMethod;
        private readonly MethodInfo _removeComponentMethod;

        public EntityDatabaseContext(IDatabaseProvider databaseProvider, Guid universeGuid)
        {
            var database = databaseProvider.GetDatabase<IdTag<EntityDefinition>>(universeGuid);

            _entityDefinitionContext = new EntityDefinition.EntityDefinitionContext(database);
            _componentsDatabaseContext = new EntityComponentsDatabaseContext(databaseProvider, universeGuid);
            _getComponentMethod = typeof(EntityComponentsDatabaseContext).GetMethod(nameof(EntityComponentsDatabaseContext.Get));
            _removeComponentMethod = typeof(EntityComponentsDatabaseContext).GetMethod(nameof(EntityComponentsDatabaseContext.Remove));
        }

        public void AddOrUpdate(Entity entity)
        {
            _entityDefinitionContext.AddOrUpdate(new EntityDefinition(entity));

            foreach (var component in entity.Components)
                _componentsDatabaseContext.AddOrUpdate(component, entity);
        }

        public Entity Get(IdTag<Entity> key)
        {
            var definition = _entityDefinitionContext.Get(new IdTag<EntityDefinition>(key.Tag));
            var entity = (Entity)Activator.CreateInstance(definition.Type);

            entity.Id = definition.Id;

            foreach (var component in definition.Components)
            {
                var genericMethod = _getComponentMethod.MakeGenericMethod(component);

                entity.Components.AddComponent((EntityComponent)genericMethod.Invoke(_componentsDatabaseContext, new object[] { entity }));
            }
            return entity;
        }

        public IEnumerable<Entity> GetEntitiesWithComponents<T>() where T : EntityComponent
        {
            var entities = _componentsDatabaseContext.GetAllKeys<T>().Select(t => new IdTag<Entity>(t.Tag));

            foreach (var entityId in entities)
                yield return Get(entityId);
        }

        public IEnumerable<IdTag<Entity>> GetAllKeys() => _entityDefinitionContext.GetAllKeys().Select(e => new IdTag<Entity>(e.Tag));

        public void Remove(Entity entity)
        {
            var definition = _entityDefinitionContext.Get(new IdTag<EntityDefinition>(entity.Id));

            _entityDefinitionContext.Remove(definition);

            foreach (var component in definition.Components)
            {
                var genericMethod = _removeComponentMethod.MakeGenericMethod(component);

                genericMethod.Invoke(_componentsDatabaseContext, new object[] { entity });
            }
        }
    }
}
