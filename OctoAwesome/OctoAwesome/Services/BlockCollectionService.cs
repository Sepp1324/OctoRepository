<<<<<<< HEAD
﻿using OctoAwesome.Definitions;
using OctoAwesome.Pooling;
using System;
using System.Collections.Generic;
=======
﻿using OctoAwesome.Definitions.Items;
using OctoAwesome.Information;
using OctoAwesome.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
>>>>>>> feature/performance

namespace OctoAwesome.Services
{
    public sealed class BlockCollectionService
    {
<<<<<<< HEAD
        private readonly IPool<BlockVolumeState> _blockCollectionPool;
        private readonly IDefinitionManager _definitionManager;

        private readonly Dictionary<BlockInfo, BlockVolumeState> _blockCollectionInformations;

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="blockCollectionPool"></param>
        /// <param name="definitionManager"></param>
        public BlockCollectionService(IPool<BlockVolumeState> blockCollectionPool, IDefinitionManager definitionManager)
        {
            _blockCollectionPool = blockCollectionPool;
            _definitionManager = definitionManager;
            _blockCollectionInformations = new Dictionary<BlockInfo, BlockVolumeState>();
        }

        public (bool Valid, IReadOnlyList<(int Quantity, IDefinition Definition)> List) Hit(BlockInfo block, IItem item, ILocalChunkCache cache)
        {
            if (!_blockCollectionInformations.TryGetValue(block, out var volumeState))
            {
                var definition = _definitionManager.GetBlockDefinitionByIndex(block.Block);
                volumeState = _blockCollectionPool.Get();
                volumeState.Initialize(block, definition, DateTimeOffset.Now);
                _blockCollectionInformations.Add(block, volumeState);
            }

            volumeState.TryReset();

            var blockHitInformation = volumeState.BlockDefinition.Hit(volumeState, item);

            if (!blockHitInformation.IsHitValid)
                return (false, null);

            volumeState.VolumeRemaining -= blockHitInformation.Quantity;
            volumeState.RestoreTime();

            if (volumeState.VolumeRemaining < 1)
            {
                _blockCollectionInformations.Remove(block);
                volumeState.Release();
                cache.SetBlock(block.Position, 0);
                return (true, blockHitInformation.Definitions);
            }

            return (false, null);
=======
        private readonly IPool<BlockVolumeState> blockCollectionPool;
        private readonly IDefinitionManager definitionManager;

        private readonly Dictionary<BlockInfo, BlockVolumeState> blockCollectionInformations;

        public BlockCollectionService(IPool<BlockVolumeState> blockCollectionPool, IDefinitionManager definitionManager)
        {
            this.blockCollectionPool = blockCollectionPool;
            this.definitionManager = definitionManager;
            blockCollectionInformations = new Dictionary<BlockInfo, BlockVolumeState>();
        }

        public BlockHitInformation Hit(BlockInfo block, IItem item, ILocalChunkCache cache)
        {
            BlockVolumeState volumeState;
            if (!blockCollectionInformations.TryGetValue(block, out volumeState))
            {
                var definition = definitionManager.GetBlockDefinitionByIndex(block.Block);
                volumeState = blockCollectionPool.Get();
                volumeState.Initialize(block, definition);
                blockCollectionInformations.Add(block, volumeState);
            }

            var blockHitInformation = volumeState.BlockDefinition.Hit(volumeState, item);

            if (!blockHitInformation.IsHitValid)
                return blockHitInformation;

            item.Definition.Hit(item, volumeState.BlockDefinition, blockHitInformation);

            volumeState.VolumeRemaining -= blockHitInformation.Quantity;

            if (volumeState.VolumeRemaining < 1)
            {
                blockCollectionInformations.Remove(block);
                volumeState.Release();
                cache.SetBlock(block.Position, 0);
            }

            return blockHitInformation;
>>>>>>> feature/performance
        }
    }
}
