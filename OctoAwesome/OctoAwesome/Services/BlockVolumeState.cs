<<<<<<< HEAD
﻿using OctoAwesome.Definitions;
using OctoAwesome.Pooling;
using System;
=======
﻿using OctoAwesome.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
>>>>>>> feature/performance

namespace OctoAwesome.Services
{
    public sealed class BlockVolumeState : IPoolElement
    {
        public BlockInfo BlockInfo { get; protected set; }
        public IBlockDefinition BlockDefinition { get; protected set; }
        public decimal VolumeRemaining { get; internal set; }
<<<<<<< HEAD
        public DateTimeOffset ValidUntil { get; set; }

        private IPool pool;

        public void Initialize(BlockInfo info, IBlockDefinition blockDefinition, DateTimeOffset validUntil)
=======

        private IPool pool;

        public void Initialize(BlockInfo info, IBlockDefinition blockDefinition)
>>>>>>> feature/performance
        {
            BlockInfo = info;
            BlockDefinition = blockDefinition;
            VolumeRemaining = blockDefinition.VolumePerUnit;
<<<<<<< HEAD
            ValidUntil = validUntil;
        }

        public void Init(IPool pool) => this.pool = pool;

        public void Release() => pool.Push(this);

        internal bool TryReset()
        {
            if (ValidUntil >= DateTimeOffset.Now)
                return false;

            VolumeRemaining = BlockDefinition.VolumePerUnit;
            return true;
        }

        internal void RestoreTime() => ValidUntil = DateTimeOffset.Now.Add(BlockDefinition.TimeToVolumeReset);
=======
        }

        public void Init(IPool pool)
        {
            this.pool = pool;
        }

        public void Release()
        {
            pool.Push(this);
        }
>>>>>>> feature/performance
    }
}
