using System;
using System.Collections.Generic;
using OctoAwesome.Definitions;
using OctoAwesome.Pooling;

namespace OctoAwesome.Services
{
    public sealed class BlockCollectionService
    {
        private readonly Dictionary<BlockInfo, BlockVolumeState> _blockCollectionInformations;
        private readonly IPool<BlockVolumeState> _blockCollectionPool;
        private readonly IDefinitionManager _definitionManager;

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
        }
    }
}