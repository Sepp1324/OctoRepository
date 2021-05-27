using OctoAwesome.EntityComponents;
using engenious;
using OctoAwesome.Services;
<<<<<<< HEAD
using OctoAwesome.Definitions;
=======
using OctoAwesome.Definitions.Items;
>>>>>>> feature/performance

namespace OctoAwesome.Basics.SimulationComponents
{
    [EntityFilter(typeof(ControllableComponent), typeof(InventoryComponent))]
    public class BlockInteractionComponent : SimulationComponent<ControllableComponent, InventoryComponent>
    {
<<<<<<< HEAD
        private readonly Simulation _simulation;
        private readonly BlockCollectionService _service;

        public BlockInteractionComponent(Simulation simulation, BlockCollectionService interactionService)
        {
            _simulation = simulation;
            _service = interactionService;
=======
        private readonly Simulation simulation;
        private readonly BlockCollectionService service;

        private readonly Hand hand;

        public BlockInteractionComponent(Simulation simulation, BlockCollectionService interactionService)
        {
            this.simulation = simulation;
            service = interactionService;
            hand = new Hand(new HandDefinition());
>>>>>>> feature/performance
        }

        protected override bool AddEntity(Entity entity) => true;

<<<<<<< HEAD
        protected override void RemoveEntity(Entity entity) { }
=======
        protected override void RemoveEntity(Entity entity)
        {

        }
>>>>>>> feature/performance

        protected override void UpdateEntity(GameTime gameTime, Entity entity, ControllableComponent controller, InventoryComponent inventory)
        {
            var toolbar = entity.Components.GetComponent<ToolBarComponent>();
            var cache = entity.Components.GetComponent<LocalChunkCacheComponent>().LocalChunkCache;

            if (controller.InteractBlock.HasValue)
            {
                var lastBlock = cache.GetBlockInfo(controller.InteractBlock.Value);

                if (!lastBlock.IsEmpty)
                {
<<<<<<< HEAD
                    IItem activeItem;

                    if (toolbar.ActiveTool.Item is IItem item)
                        activeItem = item;
                    else
                        activeItem = toolbar.HandSlot.Item as IItem;

                    var blockHitInformation = _service.Hit(lastBlock, activeItem, cache);

                    if (blockHitInformation.Valid)
                        foreach (var (quantity, definition) in blockHitInformation.List)
                        {
                            if(activeItem is IFluidInventory fluidInventory && definition is IBlockDefinition fluidBlock && fluidBlock.Material is IFluidMaterialDefinition)
                                fluidInventory.AddFluid(quantity, fluidBlock);
                            else if(definition is IInventoryable invDef) 
                                inventory.AddUnit(quantity, invDef);
                        }
=======
                    var blockHitInformation = service.Hit(lastBlock, hand, cache);

                    if (blockHitInformation.IsHitValid)
                        foreach (var definition in blockHitInformation.Definitions ?? Array.Empty<KeyValuePair<int, IDefinition>>())
                        {
                            if (definition.Value is IInventoryableDefinition invDef)
                                inventory.AddUnit(definition.Key, invDef);
                        }

>>>>>>> feature/performance
                }
                controller.InteractBlock = null;
            }

            if (toolbar != null && controller.ApplyBlock.HasValue)
            {
                if (toolbar.ActiveTool != null)
                {
                    Index3 add = new Index3();
                    switch (controller.ApplySide)
                    {
                        case OrientationFlags.SideWest: add = new Index3(-1, 0, 0); break;
                        case OrientationFlags.SideEast: add = new Index3(1, 0, 0); break;
                        case OrientationFlags.SideSouth: add = new Index3(0, -1, 0); break;
                        case OrientationFlags.SideNorth: add = new Index3(0, 1, 0); break;
                        case OrientationFlags.SideBottom: add = new Index3(0, 0, -1); break;
                        case OrientationFlags.SideTop: add = new Index3(0, 0, 1); break;
                    }

                    if (toolbar.ActiveTool.Item is IBlockDefinition definition)
                    {
<<<<<<< HEAD
                        var idx = controller.ApplyBlock.Value + add;
                        var boxes = definition.GetCollisionBoxes(cache, idx.X, idx.Y, idx.Z);

                        var intersects = false;
                        var positionComponent = entity.Components.GetComponent<PositionComponent>();
                        var bodyComponent = entity.Components.GetComponent<BodyComponent>();
=======
                        IBlockDefinition definition = toolbar.ActiveTool.Definition as IBlockDefinition;

                        Index3 idx = controller.ApplyBlock.Value + add;
                        var boxes = definition.GetCollisionBoxes(cache, idx.X, idx.Y, idx.Z);

                        bool intersects = false;
                        var positioncomponent = entity.Components.GetComponent<PositionComponent>();
                        var bodycomponent = entity.Components.GetComponent<BodyComponent>();
>>>>>>> feature/performance

                        if (positionComponent != null && bodyComponent != null)
                        {
<<<<<<< HEAD
                            const float gap = 0.01f;
                            var playerBox = new BoundingBox(
                                new Vector3(
                                    positionComponent.Position.GlobalBlockIndex.X + positionComponent.Position.BlockPosition.X - bodyComponent.Radius + gap,
                                    positionComponent.Position.GlobalBlockIndex.Y + positionComponent.Position.BlockPosition.Y - bodyComponent.Radius + gap,
                                    positionComponent.Position.GlobalBlockIndex.Z + positionComponent.Position.BlockPosition.Z + gap),
                                new Vector3(
                                    positionComponent.Position.GlobalBlockIndex.X + positionComponent.Position.BlockPosition.X + bodyComponent.Radius - gap,
                                    positionComponent.Position.GlobalBlockIndex.Y + positionComponent.Position.BlockPosition.Y + bodyComponent.Radius - gap,
                                    positionComponent.Position.GlobalBlockIndex.Z + positionComponent.Position.BlockPosition.Z + bodyComponent.Height - gap)
=======
                            float gap = 0.01f;
                            var playerBox = new BoundingBox(
                                new Vector3(
                                    positioncomponent.Position.GlobalBlockIndex.X + positioncomponent.Position.BlockPosition.X - bodycomponent.Radius + gap,
                                    positioncomponent.Position.GlobalBlockIndex.Y + positioncomponent.Position.BlockPosition.Y - bodycomponent.Radius + gap,
                                    positioncomponent.Position.GlobalBlockIndex.Z + positioncomponent.Position.BlockPosition.Z + gap),
                                new Vector3(
                                    positioncomponent.Position.GlobalBlockIndex.X + positioncomponent.Position.BlockPosition.X + bodycomponent.Radius - gap,
                                    positioncomponent.Position.GlobalBlockIndex.Y + positioncomponent.Position.BlockPosition.Y + bodycomponent.Radius - gap,
                                    positioncomponent.Position.GlobalBlockIndex.Z + positioncomponent.Position.BlockPosition.Z + bodycomponent.Height - gap)
>>>>>>> feature/performance
                                );

                            // Nicht in sich selbst reinbauen
                            foreach (var box in boxes)
                            {
                                var newBox = new BoundingBox(idx + box.Min, idx + box.Max);
                                
                                if (newBox.Min.X < playerBox.Max.X && newBox.Max.X > playerBox.Min.X &&
                                    newBox.Min.Y < playerBox.Max.Y && newBox.Max.X > playerBox.Min.Y &&
                                    newBox.Min.Z < playerBox.Max.Z && newBox.Max.X > playerBox.Min.Z)
                                    intersects = true;
                            }
                        }

                        if (!intersects)
                        {
                            if (inventory.RemoveUnit(toolbar.ActiveTool))
                            {
<<<<<<< HEAD
                                cache.SetBlock(idx, _simulation.ResourceManager.DefinitionManager.GetDefinitionIndex(definition));
                                cache.SetBlockMeta(idx, (int)controller.ApplySide);
                                
=======
                                cache.SetBlock(idx, simulation.ResourceManager.DefinitionManager.GetDefinitionIndex(definition));
                                cache.SetBlockMeta(idx, (int)controller.ApplySide);
>>>>>>> feature/performance
                                if (toolbar.ActiveTool.Amount <= 0)
                                    toolbar.RemoveSlot(toolbar.ActiveTool);
                            }
                        }
                    }
                }
                controller.ApplyBlock = null;
            }
        }
    }
}
