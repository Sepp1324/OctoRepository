using OctoAwesome.Pooling;

namespace OctoAwesome.Basics.Information
{
    public abstract class BlockInteractionInformation : IPoolElement
    {
        public BlockInfo BlockInfo { get; }

        private IPool _pool;

        public virtual void Init(IPool pool) => _pool = pool;

        public virtual void Release() => _pool.Push(this);
    }
}