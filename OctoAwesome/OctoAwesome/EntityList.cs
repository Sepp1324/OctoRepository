using System.Collections;
using System.Collections.Generic;
using OctoAwesome.EntityComponents;

namespace OctoAwesome
{
    /// <summary>
    /// 
    /// </summary>
    public class EntityList : IEntityList
    {
        private readonly IResourceManager _resourceManager;
        private readonly IChunkColumn _column;
        private readonly List<Entity> _entities;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        public EntityList(IChunkColumn column)
        {
            _entities = new List<Entity>();
            _column = column;
            _resourceManager = TypeContainer.Get<IResourceManager>();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count => _entities.Count;

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(Entity item) => _entities.Add(item);

        /// <summary>
        /// 
        /// </summary>
        public void Clear() => _entities.Clear();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(Entity item) => _entities.Contains(item);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(Entity[] array, int arrayIndex) => _entities.CopyTo(array, arrayIndex);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Entity> GetEnumerator() => _entities.GetEnumerator();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(Entity item) => _entities.Remove(item);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => _entities.GetEnumerator();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FailEntityChunkArgs> FailChunkEntity()
        {
            foreach (var entity in _entities)
                if (entity.Components.ContainsComponent<PositionComponent>())
                {
                    var position = entity.Components.GetComponent<PositionComponent>();

                    if (position.Position.ChunkIndex.X != _column.Index.X ||
                        position.Position.ChunkIndex.Y != _column.Index.Y)
                        yield return new FailEntityChunkArgs
                        {
                            Entity = entity,
                            CurrentChunk = new Index2(_column.Index),
                            CurrentPlanet = _column.Planet,
                            TargetChunk = new Index2(position.Position.ChunkIndex),
                            TargetPlanet = _resourceManager.GetPlanet(position.Position.Planet)
                        };
                }
        }
    }
}