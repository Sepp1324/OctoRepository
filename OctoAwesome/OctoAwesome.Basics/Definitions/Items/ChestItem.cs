using System;
using engenious;
using OctoAwesome.Basics.FunctionBlocks;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.Notifications;
using OctoAwesome.Rx;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class ChestItem : Item, IDisposable
    {
        private readonly IUpdateHub _updateHub;
        private readonly Relay<Notification> _simulationRelay;
        private readonly IDisposable _simulationSource;

        public ChestItem(ChestItemDefinition definition, IMaterialDefinition materialDefinition) : base(definition, materialDefinition)
        {
            _updateHub = TypeContainer.Get<IUpdateHub>();
            _simulationSource = _updateHub.AddSource(_simulationRelay, DefaultChannels.SIMULATION);
        }

        public override int VolumePerUnit => base.VolumePerUnit;

        public override int StackLimit => base.StackLimit;

        public override int Hit(IMaterialDefinition material, BlockInfo blockInfo, decimal volumeRemaining, int volumePerHit)
        {
            //TODO: Implement Place Chest and remove this item
            var position = blockInfo.Position;
            Chest chest = new(new(0, new(position.X, position.Y, position.Z + 1), new(0.5f, 0.5f, 0.5f)));
            var notification = new FunctionalBlockNotification
            {
                Block = chest,
                Type = FunctionalBlockNotification.ActionType.Add
            };

            _simulationRelay.OnNext(notification);
            return 0;
        }

        public void Dispose()
        {
            _simulationSource?.Dispose();
        }
    }
}