using OctoAwesome.EntityComponents;
using System.Collections;
using System.Collections.Generic;

namespace OctoAwesome
{
    public class EntityList : IEntityList
    {
<<<<<<< HEAD
        private readonly List<Entity> _entities;
        private readonly IChunkColumn _column;
        private readonly IResourceManager _resourceManager;
=======
        private List<Entity> entities;
        private IChunkColumn column;
        private readonly IResourceManager resourceManager;
>>>>>>> feature/performance

        public EntityList(IChunkColumn column)
        {
            _entities = new List<Entity>();
            _column = column;
            _resourceManager = TypeContainer.Get<IResourceManager>();
        }

        public int Count => _entities.Count;

        public bool IsReadOnly => false;

        public void Add(Entity item) => _entities.Add(item);

<<<<<<< HEAD
        public void Clear() => _entities.Clear();

        public bool Contains(Entity item) => _entities.Contains(item);

        public void CopyTo(Entity[] array, int arrayIndex) => _entities.CopyTo(array, arrayIndex);

        public IEnumerator<Entity> GetEnumerator() => _entities.GetEnumerator();
=======
        public void Clear() => entities.Clear();

        public bool Contains(Entity item) => entities.Contains(item);

        public void CopyTo(Entity[] array, int arrayIndex) => entities.CopyTo(array, arrayIndex);

        public IEnumerator<Entity> GetEnumerator() => entities.GetEnumerator();
>>>>>>> feature/performance

        public bool Remove(Entity item) => _entities.Remove(item);

<<<<<<< HEAD
        IEnumerator IEnumerable.GetEnumerator() => _entities.GetEnumerator();
=======
        IEnumerator IEnumerable.GetEnumerator() => entities.GetEnumerator();
>>>>>>> feature/performance

        public IEnumerable<FailEntityChunkArgs> FailChunkEntity()
        {
            foreach (var entity in _entities)
            {
                if (entity.Components.ContainsComponent<PositionComponent>())
                {
                    var position = entity.Components.GetComponent<PositionComponent>();

<<<<<<< HEAD
                    if (position.Position.ChunkIndex.X != _column.Index.X || position.Position.ChunkIndex.Y != _column.Index.Y)
=======
                    if (position.Position.ChunkIndex.X != column.Index.X || position.Position.ChunkIndex.Y != column.Index.Y)
>>>>>>> feature/performance
                    {
                        yield return new FailEntityChunkArgs()
                        {
                            Entity = entity,
                            CurrentChunk = new Index2(_column.Index),
                            CurrentPlanet = _column.Planet,
                            TargetChunk = new Index2(position.Position.ChunkIndex),
<<<<<<< HEAD
                            TargetPlanet = _resourceManager.GetPlanet(position.Position.Planet),
=======
                            TargetPlanet = resourceManager.GetPlanet(position.Position.Planet),
>>>>>>> feature/performance
                        };
                    }
                }
            }
        }
    }
}
