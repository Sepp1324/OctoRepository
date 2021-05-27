using System;
using OctoAwesome.Definitions;
using OctoAwesome.Pooling;

namespace OctoAwesome.Services
{
    public sealed class BlockVolumeState : IPoolElement
    {
        private IPool _pool;

        public BlockInfo BlockInfo { get; protected set; }

        public IBlockDefinition BlockDefinition { get; protected set; }

        public decimal VolumeRemaining { get; internal set; }

        public DateTimeOffset ValidUntil { get; set; }

        public void Init(IPool pool)
        {
            _pool = pool;
        }

        public void Release()
        {
            _pool.Push(this);
        }

        public void Initialize(BlockInfo info, IBlockDefinition blockDefinition, DateTimeOffset validUntil)
        {
            BlockInfo = info;
            BlockDefinition = blockDefinition;
            VolumeRemaining = blockDefinition.VolumePerUnit;
            ValidUntil = validUntil;
        }

        internal bool TryReset()
        {
            if (ValidUntil >= DateTimeOffset.Now)
                return false;

            VolumeRemaining = BlockDefinition.VolumePerUnit;
            return true;
        }

        internal void RestoreTime()
        {
            ValidUntil = DateTimeOffset.Now.Add(BlockDefinition.TimeToVolumeReset);
        }
    }
}