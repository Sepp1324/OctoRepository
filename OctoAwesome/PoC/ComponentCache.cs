using System;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.PoC
{
    public class ComponentCache : Cache<int, Component>
    {
        protected override Component Load(int key)
        {
            throw new NotImplementedException();
        }

        //public Component[] TryFind<T>(T key) => key switch
        //{
        //    int => Array.Empty<Component>(),
        //    _ => null
        //};
    }

    public class EntityCache : Cache<int, Entity>
    {
        protected Entity Load(Index3 key)
        {
            throw new NotImplementedException();
        }

        protected override Entity Load(int key)
        {
            throw new NotImplementedException();
        }
    }

    public class PositionComponentCache : ComponentCache
    {
        protected PositionComponent[] Find(Index3 key) => throw new NotImplementedException();

        public PositionComponent TryFindFirst<T>(T key) => key switch
        {
            Index3 => null,
            Index2 => null,
            _ => null
        };

        //public PositionComponent[] TryFind<T>(T key) => key switch
        //{
        //    Index3 => Array.Empty<PositionComponent>(),
        //    Index2 => Array.Empty<PositionComponent>(),
        //    _ => null
        //};
    }

    public class Index3PositionConverter
    {
        protected PositionComponent Convert(Index3 key)
        {
            throw new NotImplementedException();
        }

        protected PositionComponent Convert(Index2 key)
        {
            throw new NotImplementedException();
        }
    }

    public class ChunkColumnCache : Cache<Index3, ChunkColumn>
    {
        protected override ChunkColumn Load(Index3 key)
        {
            throw new NotImplementedException();
        }
    }
}

