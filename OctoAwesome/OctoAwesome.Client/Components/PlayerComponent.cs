using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
<<<<<<< HEAD
=======
using System.Text;
>>>>>>> feature/performance
using engenious;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Client.Components
{
    internal sealed class PlayerComponent : GameComponent
    {
        private new OctoGame Game;

        private IResourceManager resourceManager;

        #region External Input

        public Vector2 HeadInput { get; set; }

        public Vector2 MoveInput { get; set; }

        public bool InteractInput { get; set; }

        public bool ApplyInput { get; set; }

        public bool JumpInput { get; set; }

        public bool FlymodeInput { get; set; }

        public bool[] SlotInput { get; private set; } = new bool[10];

        public bool SlotLeftInput { get; set; }

        public bool SlotRightInput { get; set; }

        #endregion

        public Entity CurrentEntity { get; private set; }

        public HeadComponent CurrentEntityHead { get; private set; }

        public ControllableComponent CurrentController { get; private set; }

        public InventoryComponent Inventory { get; private set; }

        public ToolBarComponent Toolbar { get; private set; }

        public PositionComponent Position { get; private set; }

        public Index3? SelectedBox { get; set; }

        public Vector2? SelectedPoint { get; set; }

        public OrientationFlags SelectedSide { get; set; }

        public OrientationFlags SelectedEdge { get; set; }

        public OrientationFlags SelectedCorner { get; set; }

<<<<<<< HEAD
        public PlayerComponent(OctoGame game, IResourceManager resourceManager) : base(game)
        {
            _resourceManager = resourceManager;
            _game = game;
=======
        public PlayerComponent(OctoGame game, IResourceManager resourceManager)
            : base(game)
        {
            this.resourceManager = resourceManager;
            Game = game;
>>>>>>> feature/performance
        }

        public void SetEntity(Entity entity)
        {
            CurrentEntity = entity;

            if (CurrentEntity == null)
            {
                CurrentEntityHead = null;
            }
            else
            {
                // Map other Components

                CurrentController = CurrentEntity.Components.GetComponent<ControllableComponent>();
<<<<<<< HEAD
                CurrentEntityHead = CurrentEntity.Components.GetComponent<HeadComponent>() ?? new HeadComponent();
                Inventory = CurrentEntity.Components.GetComponent<InventoryComponent>() ?? new InventoryComponent();
                Toolbar = CurrentEntity.Components.GetComponent<ToolBarComponent>() ?? new ToolBarComponent();
                Position = CurrentEntity.Components.GetComponent<PositionComponent>() ?? new PositionComponent() { Position = new Coordinate(0, new Index3(0, 0, 0), new Vector3(0, 0, 0)) };
=======

                CurrentEntityHead = CurrentEntity.Components.GetComponent<HeadComponent>();
                if (CurrentEntityHead == null) CurrentEntityHead = new HeadComponent();

                Inventory = CurrentEntity.Components.GetComponent<InventoryComponent>();
                if (Inventory == null) Inventory = new InventoryComponent();

                Toolbar = CurrentEntity.Components.GetComponent<ToolBarComponent>();
                if (Toolbar == null) Toolbar = new ToolBarComponent();

                Position = CurrentEntity.Components.GetComponent<PositionComponent>();
                if (Position == null) Position = new PositionComponent() { Position = new Coordinate(0, new Index3(0, 0, 0), new Vector3(0, 0, 0)) };
>>>>>>> feature/performance
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            if (CurrentEntity == null)
                return;

            CurrentEntityHead.Angle += (float)gameTime.ElapsedGameTime.TotalSeconds * HeadInput.X;
            CurrentEntityHead.Tilt += (float)gameTime.ElapsedGameTime.TotalSeconds * HeadInput.Y;
            CurrentEntityHead.Tilt = Math.Min(1.5f, Math.Max(-1.5f, CurrentEntityHead.Tilt));
            HeadInput = Vector2.Zero;

            CurrentController.MoveInput = MoveInput;
            MoveInput = Vector2.Zero;

            CurrentController.JumpInput = JumpInput;
            JumpInput = false;

            if (InteractInput && SelectedBox.HasValue)
                CurrentController.InteractBlock = SelectedBox.Value;

<<<<<<< HEAD
=======
            InteractInput = false;
>>>>>>> feature/performance

            if (ApplyInput && SelectedBox.HasValue)
            {
                CurrentController.ApplyBlock = SelectedBox.Value;
                CurrentController.ApplySide = SelectedSide;
            }

            ApplyInput = false;

            if (Toolbar.Tools != null && Toolbar.Tools.Length > 0)
            {
<<<<<<< HEAD
                for (var i = 0; i < Math.Min(Toolbar.Tools.Length, SlotInput.Length); i++)
=======
                if (Toolbar.ActiveTool == null) Toolbar.ActiveTool = Toolbar.Tools[0];
                for (int i = 0; i < Math.Min(Toolbar.Tools.Length, SlotInput.Length); i++)
>>>>>>> feature/performance
                {
                    if (SlotInput[i])
                        Toolbar.ActiveIndex = i;
                    SlotInput[i] = false;
                }
            }

            //Index des aktiven Werkzeugs ermitteln
<<<<<<< HEAD

            if (SlotLeftInput)
                Toolbar.ActiveIndex--;
            SlotLeftInput = false;

            if (SlotRightInput)
                Toolbar.ActiveIndex++;
=======
            int activeTool = -1;
            List<int> toolIndices = new List<int>();
            if (Toolbar.Tools != null)
            {
                for (int i = 0; i < Toolbar.Tools.Length; i++)
                {
                    if (Toolbar.Tools[i] != null)
                        toolIndices.Add(i);

                    if (Toolbar.Tools[i] == Toolbar.ActiveTool)
                        activeTool = toolIndices.Count - 1;
                }
            }

            if (SlotLeftInput)
            {
                if (activeTool > -1)
                    activeTool--;
                else if (toolIndices.Count > 0)
                    activeTool = toolIndices[toolIndices.Count - 1];
            }
            SlotLeftInput = false;

            if (SlotRightInput)
            {
                if (activeTool > -1)
                    activeTool++;
                else if (toolIndices.Count > 0)
                    activeTool = toolIndices[0];
            }
>>>>>>> feature/performance
            SlotRightInput = false;
        }

        /// <summary>
        /// DEBUG METHODE: NICHT FÜR VERWENDUNG IM SPIEL!
        /// </summary>
        internal void AllBlocksDebug()
        {
            var inventory = CurrentEntity.Components.GetComponent<InventoryComponent>();
            
            if (inventory == null)
                return;

<<<<<<< HEAD
            var blockDefinitions = _resourceManager.DefinitionManager.BlockDefinitions;
            
            foreach (var blockDefinition in blockDefinitions)
                inventory.AddUnit(blockDefinition.VolumePerUnit, blockDefinition);

            var itemDefinitions = _resourceManager.DefinitionManager.ItemDefinitions;
            var wood = _resourceManager.DefinitionManager.MaterialDefinitions.FirstOrDefault(d => d.Name == "Wood");
            var stone = _resourceManager.DefinitionManager.MaterialDefinitions.FirstOrDefault(d => d.Name == "Stone");

            foreach (var itemDefinition in itemDefinitions)
            {
                var woodItem = itemDefinition.Create(wood);
                var stoneItem = itemDefinition.Create(stone);
                
                inventory.AddUnit(woodItem.VolumePerUnit, woodItem);
                inventory.AddUnit(stoneItem.VolumePerUnit, stoneItem);
            }
=======
            var blockDefinitions = resourceManager.DefinitionManager.GetBlockDefinitions();
            foreach (var blockDefinition in blockDefinitions)
                inventory.AddUnit(blockDefinition.VolumePerUnit, blockDefinition);

            var itemDefinitions = resourceManager.DefinitionManager.GetItemDefinitions();
            foreach (var itemDefinition in itemDefinitions)
                inventory.AddUnit(itemDefinition.VolumePerUnit, itemDefinition);
>>>>>>> feature/performance
        }
    }
}
