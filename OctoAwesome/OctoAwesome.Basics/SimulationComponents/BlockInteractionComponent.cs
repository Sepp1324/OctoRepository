using OctoAwesome.EntityComponents;
using engenious;
using OctoAwesome.Services;
using OctoAwesome.Definitions.Items;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.SimulationComponents
{
    [EntityFilter(typeof(ControllableComponent), typeof(InventoryComponent))]
    public class BlockInteractionComponent : SimulationComponent<ControllableComponent, InventoryComponent>
    {
        private readonly Simulation _simulation;
        private readonly BlockCollectionService _service;
        private readonly Hand _hand;

        public BlockInteractionComponent(Simulation simulation, BlockCollectionService interactionService)
        {
            _simulation = simulation;
            _service = interactionService;
            _hand = new Hand(new HandDefinition());
        }

        protected override bool AddEntity(Entity entity) => true;

        protected override void RemoveEntity(Entity entity)
        {

        }

        protected override void UpdateEntity(GameTime gameTime, Entity entity, ControllableComponent controller, InventoryComponent inventory)
        {
            var toolbar = entity.Components.GetComponent<ToolBarComponent>();
            var cache = entity.Components.GetComponent<LocalChunkCacheComponent>().LocalChunkCache;

            if (controller.InteractBlock.HasValue)
            {
                var lastBlock = cache.GetBlockInfo(controller.InteractBlock.Value);

                if (!lastBlock.IsEmpty)
                {
                    if (toolbar.ActiveTool is null || !(toolbar.ActiveTool.Item is IItem item))
                        item = _hand;
                    
                    var blockHitInformation = _service.Hit(lastBlock, item, cache);

                    if (blockHitInformation.Valid)
                        foreach (var (Quantity, Definition) in blockHitInformation.List)
                        {
                            if (Definition is IInventoryable invDef)
                                inventory.AddUnit(Quantity, invDef);
                        }

                }
                controller.InteractBlock = null;
            }

            if (toolbar != null && controller.ApplyBlock.HasValue)
            {
                if (toolbar.ActiveTool != null)
                {
                    var add = new Index3();
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
                        var idx = controller.ApplyBlock.Value + add;
                        var boxes = definition.GetCollisionBoxes(cache, idx.X, idx.Y, idx.Z);

                        var intersects = false;
                        var positionComponent = entity.Components.GetComponent<PositionComponent>();
                        var bodyComponent = entity.Components.GetComponent<BodyComponent>();

                        if (positionComponent != null && bodyComponent != null)
                        {
                            var gap = 0.01f;
                            var playerBox = new BoundingBox(
                                new Vector3(
                                    positionComponent.Position.GlobalBlockIndex.X + positionComponent.Position.BlockPosition.X - bodyComponent.Radius + gap,
                                    positionComponent.Position.GlobalBlockIndex.Y + positionComponent.Position.BlockPosition.Y - bodyComponent.Radius + gap,
                                    positionComponent.Position.GlobalBlockIndex.Z + positionComponent.Position.BlockPosition.Z + gap),
                                new Vector3(
                                    positionComponent.Position.GlobalBlockIndex.X + positionComponent.Position.BlockPosition.X + bodyComponent.Radius - gap,
                                    positionComponent.Position.GlobalBlockIndex.Y + positionComponent.Position.BlockPosition.Y + bodyComponent.Radius - gap,
                                    positionComponent.Position.GlobalBlockIndex.Z + positionComponent.Position.BlockPosition.Z + bodyComponent.Height - gap)
                                );

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
                        {
                            if (inventory.RemoveUnit(toolbar.ActiveTool))
                            {
                                cache.SetBlock(idx, _simulation.ResourceManager.DefinitionManager.GetDefinitionIndex(definition));
                                cache.SetBlockMeta(idx, (int)controller.ApplySide);
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
