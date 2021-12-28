using engenious;
using OctoAwesome.Components;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using OctoAwesome.Services;

namespace OctoAwesome.Basics.SimulationComponents
{
    public class BlockInteractionComponent : SimulationComponent<Entity, SimulationComponentRecord<Entity, ControllableComponent, InventoryComponent>, ControllableComponent, InventoryComponent>
    {
        private readonly BlockCollectionService _service;
        private readonly Simulation _simulation;

        public BlockInteractionComponent(Simulation simulation, BlockCollectionService interactionService)
        {
            _simulation = simulation;
            _service = interactionService;
        }

        protected override void UpdateValue(GameTime gameTime, SimulationComponentRecord<Entity, ControllableComponent, InventoryComponent> value)
        {
            var entity = value.Value;
            var controller = value.Component1;
            var inventory = value.Component2;

            var toolbar = entity.Components.GetComponent<ToolBarComponent>();
            var cache = entity.Components.GetComponent<LocalChunkCacheComponent>().LocalChunkCache;

            controller.Selection?.Visit(blockInfo => InteractWith(blockInfo, inventory, toolbar, cache), functionalBlock => functionalBlock.Interact(gameTime, entity), entity => { });

            if (toolbar == null || !controller.ApplyBlock.HasValue) 
                return;

            if (toolbar.ActiveTool != null)
            {
                var add = new Index3();
                switch (controller.ApplySide)
                {
                    case OrientationFlags.SideWest:
                        add = new(-1, 0, 0);
                        break;
                    case OrientationFlags.SideEast:
                        add = new(1, 0, 0);
                        break;
                    case OrientationFlags.SideSouth:
                        add = new(0, -1, 0);
                        break;
                    case OrientationFlags.SideNorth:
                        add = new(0, 1, 0);
                        break;
                    case OrientationFlags.SideBottom:
                        add = new(0, 0, -1);
                        break;
                    case OrientationFlags.SideTop:
                        add = new(0, 0, 1);
                        break;
                }

                if (toolbar.ActiveTool.Item is IBlockDefinition definition)
                {
                    var idx = controller.ApplyBlock.Value + add;
                    var boxes = definition.GetCollisionBoxes(cache, idx.X, idx.Y, idx.Z);

                    var intersects = false;
                    var positionComponent = entity.Components.GetComponent<PositionComponent>();
                    var bodyComponent = entity.Components.GetComponent<BodyComponent>();

                    if (positionComponent != null && bodyComponent != null)
                    {
                        var gap = 0.01f;
                        var playerBox = new BoundingBox(new(positionComponent.Position.GlobalBlockIndex.X + positionComponent.Position.BlockPosition.X - bodyComponent.Radius + gap, positionComponent.Position.GlobalBlockIndex.Y + positionComponent.Position.BlockPosition.Y - bodyComponent.Radius + gap, positionComponent.Position.GlobalBlockIndex.Z + positionComponent.Position.BlockPosition.Z + gap), new(positionComponent.Position.GlobalBlockIndex.X + positionComponent.Position.BlockPosition.X + bodyComponent.Radius - gap, positionComponent.Position.GlobalBlockIndex.Y + positionComponent.Position.BlockPosition.Y + bodyComponent.Radius - gap, positionComponent.Position.GlobalBlockIndex.Z + positionComponent.Position.BlockPosition.Z + bodyComponent.Height - gap));

                        // Nicht in sich selbst reinbauen
                        for (var i = 0; i < boxes.Length; i++)
                        {
                            var box = boxes[i];
                            var newBox = new BoundingBox(idx + box.Min, idx + box.Max);
                            if (newBox.Min.X < playerBox.Max.X && newBox.Max.X > playerBox.Min.X &&
                                newBox.Min.Y < playerBox.Max.Y && newBox.Max.X > playerBox.Min.Y &&
                                newBox.Min.Z < playerBox.Max.Z && newBox.Max.X > playerBox.Min.Z)
                                intersects = true;
                        }
                    }

                    if (!intersects)
                        if (inventory.RemoveUnit(toolbar.ActiveTool))
                        {
                            cache.SetBlock(idx, _simulation.ResourceManager.DefinitionManager.GetDefinitionIndex(definition));
                            cache.SetBlockMeta(idx, (int)controller.ApplySide);

                            if (toolbar.ActiveTool.Amount <= 0)
                                toolbar.RemoveSlot(toolbar.ActiveTool);
                        }
                }
            }

            controller.ApplyBlock = null;
        }

        private void InteractWith(BlockInfo lastBlock, InventoryComponent inventory, ToolBarComponent toolbar, ILocalChunkCache cache)
        {
            if (lastBlock.IsEmpty || lastBlock.Block == 0) 
                return;

            IItem activeItem;
            if (toolbar.ActiveTool.Item is IItem item)
                activeItem = item;
            else
                activeItem = toolbar.HandSlot.Item as IItem;

            var blockHitInformation = _service.Hit(lastBlock, activeItem, cache);

            if (!blockHitInformation.Valid) 
                return;

            foreach (var (Quantity, Definition) in blockHitInformation.List)
                if (activeItem is IFluidInventory fluidInventory
                    && Definition is IBlockDefinition { Material: IFluidMaterialDefinition } fluidBlock)
                    fluidInventory.AddFluid(Quantity, fluidBlock);
                else if (Definition is IInventoryable invDef) inventory.AddUnit(Quantity, invDef);
        }
    }
}