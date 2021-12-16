﻿using engenious;
using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Services;

namespace OctoAwesome.Basics.SimulationComponents
{
    public class FunctionalBlockInteractionComponent : SimulationComponent<
        Entity,
        SimulationComponentRecord<Entity, ControllableComponent, InventoryComponent>,
        ControllableComponent,
        InventoryComponent>
    {
        private readonly BlockCollectionService service;
        private readonly Simulation simulation;


        public FunctionalBlockInteractionComponent(Simulation simulation, BlockCollectionService interactionService)
        {
            this.simulation = simulation;
            service = interactionService;
        }

        protected override void UpdateValue(GameTime gameTime,
            SimulationComponentRecord<Entity, ControllableComponent, InventoryComponent> value)
        {
            var entity = value.Value;
            var controller = value.Component1;

            controller
                .Selection?
                .Map(
                    blockInfo => { },
                    functionalBlock => InternalUpdate(controller, entity, functionalBlock),
                    entity => { }
                );
        }

        private void InternalUpdate(ControllableComponent controller, Entity entity, FunctionalBlock functionalBlock)
        {
        }
    }
}