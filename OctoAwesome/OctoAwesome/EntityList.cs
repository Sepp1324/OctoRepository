using System.Collections;
using System.Collections.Generic;
using OctoAwesome.EntityComponents;

namespace OctoAwesome
{
    public class EntityList : IEntityList
    {
        private readonly IChunkColumn column;
        private readonly List<Entity> entities;
        private readonly IResourceManager resourceManager;

        public EntityList(IChunkColumn column)
        {
            entities = new List<Entity>();
            this.column = column;
            resourceManager = TypeContainer.Get<IResourceManager>();
        }

        public int Count => entities.Count;

        public bool IsReadOnly => false;

        public void Add(Entity item)
        {
            entities.Add(item);
        }

        public void Clear()
        {
            entities.Clear();
        }

        public bool Contains(Entity item)
        {
            return entities.Contains(item);
        }

        public void CopyTo(Entity[] array, int arrayIndex)
        {
            entities.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return entities.GetEnumerator();
        }

        public bool Remove(Entity item)
        {
            return entities.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return entities.GetEnumerator();
        }

        public IEnumerable<FailEntityChunkArgs> FailChunkEntity()
        {
            foreach (var entity in entities)
                if (entity.Components.ContainsComponent<PositionComponent>())
                {
                    var position = entity.Components.GetComponent<PositionComponent>();

                    if (position.Position.ChunkIndex.X != column.Index.X || position.Position.ChunkIndex.Y != column.Index.Y)
                        yield return new FailEntityChunkArgs()
                        {
                            Entity = entity,
                            CurrentChunk = new Index2(column.Index),
                            CurrentPlanet = column.Planet,
                            TargetChunk = new Index2(position.Position.ChunkIndex),
                            TargetPlanet = resourceManager.GetPlanet(position.Position.Planet)
                        };
                }
        }
    }
}