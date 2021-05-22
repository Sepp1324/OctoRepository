using OctoAwesome.EntityComponents;
using System.Collections;
using System.Collections.Generic;

namespace OctoAwesome
{
    public class EntityList : IEntityList
    {
        private readonly List<Entity> _entities;
        private readonly IChunkColumn _column;
        private readonly IResourceManager _resourceManager;

        public EntityList(IChunkColumn column)
        {
            _entities = new List<Entity>();
            _column = column;
            _resourceManager = TypeContainer.Get<IResourceManager>();
        }

        public int Count => _entities.Count;

        public bool IsReadOnly => false;

        public void Add(Entity item) => _entities.Add(item);

        public void Clear() => _entities.Clear();

        public bool Contains(Entity item) => _entities.Contains(item);

        public void CopyTo(Entity[] array, int arrayIndex) => _entities.CopyTo(array, arrayIndex);

        public IEnumerator<Entity> GetEnumerator() => _entities.GetEnumerator();

        public bool Remove(Entity item) => _entities.Remove(item);

        IEnumerator IEnumerable.GetEnumerator() => _entities.GetEnumerator();

        public IEnumerable<FailEntityChunkArgs> FailChunkEntity()
        {
            foreach (var entity in _entities)
            {
                if (entity.Components.ContainsComponent<PositionComponent>())
                {
                    var position = entity.Components.GetComponent<PositionComponent>();

                    if (position.Position.ChunkIndex.X != _column.Index.X || position.Position.ChunkIndex.Y != _column.Index.Y)
                    {
                        yield return new FailEntityChunkArgs()
                        {
                            Entity = entity,
                            CurrentChunk = new Index2(_column.Index),
                            CurrentPlanet = _column.Planet,
                            TargetChunk = new Index2(position.Position.ChunkIndex),
                            TargetPlanet = _resourceManager.GetPlanet(position.Position.Planet),
                        };
                    }
                }
            }
        }
    }
}
