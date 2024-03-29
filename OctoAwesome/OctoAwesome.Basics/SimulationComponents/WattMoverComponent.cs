﻿using System;
using engenious;
using engenious.Helper;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Components;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Basics.SimulationComponents
{
    public class WattMoverComponent : SimulationComponent<
        Entity,
        SimulationComponentRecord<Entity, ControllableComponent, BodyPowerComponent>,
        ControllableComponent,
        BodyPowerComponent>
    {
        protected override SimulationComponentRecord<Entity, ControllableComponent, BodyPowerComponent> OnAdd(
            Entity entity)
        {
            var controllable = entity.GetComponent<ControllableComponent>();
            var bodyPower = entity.GetComponent<BodyPowerComponent>();
            return new SimulationComponentRecord<Entity, ControllableComponent, BodyPowerComponent>(entity,
                controllable, bodyPower);
        }

        protected override void UpdateValue(GameTime gameTime,
            SimulationComponentRecord<Entity, ControllableComponent, BodyPowerComponent> value)
        {
            //Move

            var e = value.Value;
            var controller = value.Component1;
            var powercomp = value.Component2;

            if (e.Components.ContainsComponent<HeadComponent>())
            {
                var head = e.Components.GetComponent<HeadComponent>();

                var lookX = (float)Math.Cos(head.Angle);
                var lookY = -(float)Math.Sin(head.Angle);
                var velocitydirection = new Vector3(lookX, lookY) * controller.MoveInput.Y;

                var stafeX = (float)Math.Cos(head.Angle + MathHelper.PiOver2);
                var stafeY = -(float)Math.Sin(head.Angle + MathHelper.PiOver2);
                velocitydirection += new Vector3(stafeX, stafeY) * controller.MoveInput.X;

                powercomp.Direction = velocitydirection;
            }
            else
            {
                powercomp.Direction = new Vector3(controller.MoveInput.X, controller.MoveInput.Y);
            }

            //Jump
            if (controller.JumpInput && !controller.JumpActive)
            {
                controller.JumpTime = powercomp.JumpTime;
                controller.JumpActive = true;
            }

            if (controller.JumpActive)
            {
                powercomp.Direction += new Vector3(0, 0, 1);
                controller.JumpTime -= gameTime.ElapsedGameTime.Milliseconds;

                if (controller.JumpTime <= 0)
                    controller.JumpActive = false;
            }
        }
    }
}