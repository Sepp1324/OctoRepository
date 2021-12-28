using engenious;
using engenious.UI;
using OctoAwesome.Basics.EntityComponents.UIComponents;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Basics.FunctionBlocks
{
    public class Chest : FunctionalBlock
    {
        private readonly AnimationComponent _animationComponent;
        private readonly TransferUIComponent _transferUiComponent;

        public Chest(Coordinate position)
        {
            var inventoryComponent = new InventoryComponent();
            _animationComponent = new();
            _transferUiComponent = new(inventoryComponent);
            _transferUiComponent.Closed += TransferUiComponentClosed;
            Components.AddComponent(inventoryComponent);
            Components.AddComponent(new PositionComponent
            {
                Position = position
            });

            Components.AddComponent(new BodyComponent { Height = 0.4f, Radius = 0.2f });
            Components.AddComponent(new BoxCollisionComponent(new[] { new BoundingBox(new(0, 0), new(1, 1, 1)) }));
            Components.AddComponent(new RenderComponent { Name = "Chest", ModelName = "chest", TextureName = "texchestmodel", BaseZRotation = -90 }, true);
            Components.AddComponent(_transferUiComponent, true);
            Components.AddComponent(_animationComponent);
        }

        private void TransferUiComponentClosed(object sender, NavigationEventArgs e)
        {
            _animationComponent.AnimationSpeed = -60f;
        }

        protected override void OnInteract(GameTime gameTime, Entity entity)
        {
            if (entity is not Player p) 
                return;

            _transferUiComponent.Show(p);
            _animationComponent.CurrentTime = 0f;
            _animationComponent.AnimationSpeed = 60f;
        }
    }
}