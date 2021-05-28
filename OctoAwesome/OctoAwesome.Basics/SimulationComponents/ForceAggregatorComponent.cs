using System.Collections.Generic;
using System.Linq;
using engenious;
using OctoAwesome.Basics.EntityComponents;

namespace OctoAwesome.Basics.SimulationComponents
{
    [EntityFilter(typeof(ForceComponent), typeof(MoveableComponent))]
    public sealed class ForceAggregatorComponent : SimulationComponent
    {
        private readonly List<ForcedEntity> _forcedEntities = new();

        protected override bool AddEntity(Entity entity)
        {
            var forcedEntity = new ForcedEntity
            {
                Entity = entity,
                Moveable = entity.Components.GetComponent<MoveableComponent>(),
                Forces = entity.Components.OfType<ForceComponent>().ToArray()
            };

            _forcedEntities.Add(forcedEntity);
            return true;
        }

        protected override void RemoveEntity(Entity entity)
        {
            var forcedEntity = _forcedEntities.FirstOrDefault(e => e.Entity == entity);
            if (forcedEntity != null)
                _forcedEntities.Remove(forcedEntity);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in _forcedEntities)
                entity.Moveable.ExternalForces =
                    entity.Forces.Aggregate(Vector3.Zero, (s, f) => s + f.Force);
        }

        private class ForcedEntity
        {
            public Entity Entity { get; set; }

            public MoveableComponent Moveable { get; set; }

            public ForceComponent[] Forces { get; set; }
        }
    }
}