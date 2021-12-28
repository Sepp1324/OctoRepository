using System;
using engenious;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Basics.Entities
{
    public class WauziEntity : UpdateableEntity
    {
        public int JumpTime { get; set; }

        public override void Update(GameTime gameTime)
        {
            var body = Components.GetComponent<BodyPowerComponent>();
            var controller = Components.GetComponent<ControllableComponent>();
            controller.MoveInput = new(0.5f, 0.5f);

            if (JumpTime <= 0)
            {
                controller.JumpInput = true;
                JumpTime = 10000;
            }
            else
            {
                JumpTime -= gameTime.ElapsedGameTime.Milliseconds;
            }

            if (controller.JumpActive) controller.JumpInput = false;
        }

        public override void RegisterDefault()
        {
            var posComponent = Components.GetComponent<PositionComponent>() ?? new PositionComponent { Position = new(0, new(0, 0, 200), new(0, 0)) };

            Components.AddComponent(posComponent);
            Components.AddComponent(new GravityComponent());
            Components.AddComponent(new BodyComponent { Mass = 50f, Height = 2f, Radius = 1.5f });
            Components.AddComponent(new BodyPowerComponent { Power = 600f, JumpTime = 120 });
            Components.AddComponent(new MoveableComponent());
            Components.AddComponent(new BoxCollisionComponent(Array.Empty<BoundingBox>()));
            Components.AddComponent(new ControllableComponent());
            Components.AddComponent(new RenderComponent { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);
            Components.AddComponent(new LocalChunkCacheComponent(posComponent.Planet.GlobalChunkCache, 2, 1));
        }
    }
}