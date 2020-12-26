using engenious.UI;
using OctoAwesome.Client.Controls;
using OctoAwesome.Client.Components;
using System;
using engenious;
using engenious.Input;
using engenious.UI.Controls;

namespace OctoAwesome.Client.Screens
{
    internal sealed class GameScreen : Screen
    {
        private const float MOUSE_SPEED = 0.2f;
        private readonly CompassControl _compass;
        private readonly CrosshairControl _crosshair;

        private readonly DebugControl _debug;
        private readonly HealthBarControl _healthbar;
        private readonly MinimapControl _minimap;
        private readonly SceneControl _scene;
        private readonly ToolbarControl _toolbar;

        public GameScreen(ScreenComponent manager) : base(manager)
        {
            DefaultMouseMode = MouseMode.Captured;

            Manager = manager;
            Padding = Border.All(0);

            _scene = new SceneControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch
            };
            Controls.Add(_scene);

            _debug = new DebugControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Visible = false
            };
            Controls.Add(_debug);

            _compass = new CompassControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = Border.All(10),
                Width = 300,
                Height = 50
            };
            Controls.Add(_compass);

            _toolbar = new ToolbarControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Bottom,
                Height = 100
            };
            Controls.Add(_toolbar);

            _minimap = new MinimapControl(manager, _scene)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Width = 128,
                Height = 128,
                Margin = Border.All(5)
            };
            Controls.Add(_minimap);

            _healthbar = new HealthBarControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Width = 240,
                Height = 78,
                Maximum = 100,
                Value = 40,
                Margin = Border.All(20, 30)
            };
            Controls.Add(_healthbar);

            _crosshair = new CrosshairControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 8,
                Height = 8
            };
            Controls.Add(_crosshair);

            Title = Languages.OctoClient.Game;

            RegisterKeyActions();
        }

        private new ScreenComponent Manager { get; set; }

        public event EventHandler OnCenterChanged
        {
            add => _scene.OnCenterChanged += value;
            remove => _scene.OnCenterChanged -= value;
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (_pressedMoveUp) Manager.Player.MoveInput += new Vector2(0f, 10f);
            if (_pressedMoveLeft) Manager.Player.MoveInput += new Vector2(-10f, 0f);
            if (_pressedMoveDown) Manager.Player.MoveInput += new Vector2(0f, -10f);
            if (_pressedMoveRight) Manager.Player.MoveInput += new Vector2(10f, 0f);
            if (_pressedHeadUp) Manager.Player.HeadInput += new Vector2(0f, 1f);
            if (_pressedHeadDown) Manager.Player.HeadInput += new Vector2(0f, -1f);
            if (_pressedHeadLeft) Manager.Player.HeadInput += new Vector2(-1f, 0f);
            if (_pressedHeadRight) Manager.Player.HeadInput += new Vector2(1f, 0f);

            HandleGamePad();

            base.OnUpdate(gameTime);
        }

        public void Unload() => _scene.Dispose();

        #region Mouse Input

        protected override void OnLeftMouseDown(MouseEventArgs args)
        {
            if (!IsActiveScreen) return;

            Manager.Player.ApplyInput = true;
            args.Handled = true;
        }

        protected override void OnRightMouseDown(MouseEventArgs args)
        {
            if (!IsActiveScreen) return;

            Manager.Player.InteractInput = true;
            args.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs args)
        {
            if (!IsActiveScreen) return;

            if (args.MouseMode == MouseMode.Captured && IsActiveScreen)
            {
                Manager.Player.HeadInput = args.GlobalPosition.ToVector2() * MOUSE_SPEED * new Vector2(1f, -1f);
                args.Handled = true;
            }
        }

        protected override void OnMouseScroll(MouseScrollEventArgs args)
        {
            if (!IsActiveScreen) return;

            Manager.Player.SlotLeftInput = args.Steps > 0;
            Manager.Player.SlotRightInput = args.Steps < 0;
            args.Handled = true;
        }

        #endregion

        #region Keyboard Input

        private bool _pressedMoveUp = false;
        private bool _pressedMoveLeft = false;
        private bool _pressedMoveDown = false;
        private bool _pressedMoveRight = false;
        private bool _pressedHeadUp = false;
        private bool _pressedHeadDown = false;
        private bool _pressedHeadLeft = false;
        private bool _pressedHeadRight = false;

        private void RegisterKeyActions()
        {
            Manager.Game.KeyMapper.AddAction("octoawesome:forward", type =>
            {
                if (!IsActiveScreen) return;

                switch (type)
                {
                    case KeyMapper.KeyType.Down:
                        _pressedMoveUp = true;
                        break;
                    case KeyMapper.KeyType.Up:
                        _pressedMoveUp = false;
                        break;
                }
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:left", type =>
            {
                if (!IsActiveScreen) return;
                switch (type)
                {
                    case KeyMapper.KeyType.Down:
                        _pressedMoveLeft = true;
                        break;
                    case KeyMapper.KeyType.Up:
                        _pressedMoveLeft = false;
                        break;
                }
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:backward", type =>
            {
                if (!IsActiveScreen) return;
                switch (type)
                {
                    case KeyMapper.KeyType.Down:
                        _pressedMoveDown = true;
                        break;
                    case KeyMapper.KeyType.Up:
                        _pressedMoveDown = false;
                        break;
                }
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:right", type =>
            {
                if (!IsActiveScreen) return;
                switch (type)
                {
                    case KeyMapper.KeyType.Down:
                        _pressedMoveRight = true;
                        break;
                    case KeyMapper.KeyType.Up:
                        _pressedMoveRight = false;
                        break;
                }
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:headup", type =>
            {
                if (!IsActiveScreen) return;
                switch (type)
                {
                    case KeyMapper.KeyType.Down:
                        _pressedHeadUp = true;
                        break;
                    case KeyMapper.KeyType.Up:
                        _pressedHeadUp = false;
                        break;
                }
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:headdown", type =>
            {
                if (!IsActiveScreen) return;
                switch (type)
                {
                    case KeyMapper.KeyType.Down:
                        _pressedHeadDown = true;
                        break;
                    case KeyMapper.KeyType.Up:
                        _pressedHeadDown = false;
                        break;
                }
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:headleft", type =>
            {
                if (!IsActiveScreen) return;
                switch (type)
                {
                    case KeyMapper.KeyType.Down:
                        _pressedHeadLeft = true;
                        break;
                    case KeyMapper.KeyType.Up:
                        _pressedHeadLeft = false;
                        break;
                }
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:headright", type =>
            {
                if (!IsActiveScreen) return;
                switch (type)
                {
                    case KeyMapper.KeyType.Down:
                        _pressedHeadRight = true;
                        break;
                    case KeyMapper.KeyType.Up:
                        _pressedHeadRight = false;
                        break;
                }
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:interact", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down) return;
                Manager.Player.InteractInput = true;
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:apply", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down) return;
                Manager.Player.ApplyInput = true;
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:flymode", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down) return;
                Manager.Player.FlymodeInput = true;
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:jump", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down) return;
                Manager.Player.JumpInput = true;
            });
            for (var i = 0; i < 10; i++)
            {
                var tmp = i; // Nicht löschen. Benötigt, um aktuellen Wert zu fangen.
                Manager.Game.KeyMapper.AddAction("octoawesome:slot" + tmp, type =>
                {
                    if (!IsActiveScreen || type != KeyMapper.KeyType.Down) return;
                    Manager.Player.SlotInput[tmp] = true;
                });
            }

            Manager.Game.KeyMapper.AddAction("octoawesome:debug.allblocks", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down) return;
                Manager.Player.AllBlocksDebug();
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:debug.control", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down) return;
                _debug.Visible = !_debug.Visible;
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:inventory", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down) return;
                Manager.NavigateToScreen(new InventoryScreen(Manager));
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:hidecontrols", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down) return;
                _compass.Visible = !_compass.Visible;
                _toolbar.Visible = !_toolbar.Visible;
                _minimap.Visible = !_minimap.Visible;
                _crosshair.Visible = !_crosshair.Visible;
                _debug.Visible = !_debug.Visible;
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:exit", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down) return;
                Manager.NavigateToScreen(new PauseScreen(Manager));
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:freemouse", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down) return;
                if (Manager.MouseMode == MouseMode.Captured)
                    Manager.FreeMouse();
                else
                    Manager.CaptureMouse();
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:teleport", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down) return;
                Manager.NavigateToScreen(new TargetScreen(Manager, (x, y) =>
                    {
                        Manager.Game.Player.Position.Position = new Coordinate(0, new Index3(x, y, 300), new Vector3());
                        Manager.NavigateBack();
                    }, Manager.Game.Player.Position.Position.GlobalBlockIndex.X,
                    Manager.Game.Player.Position.Position.GlobalBlockIndex.Y));
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:toggleWireFrame", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Up) return;
                ChunkRenderer.WIRE_FRAME = !ChunkRenderer.WIRE_FRAME;
            });
            Manager.Game.KeyMapper.AddAction("octoawesome:toggleAmbientOcclusion", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Up) return;

                ChunkRenderer.OVERRIDE_LIGHT_LEVEL = ChunkRenderer.OVERRIDE_LIGHT_LEVEL > 0f ? 0f : 1f;
            });
        }

        #endregion

        #region GamePad Input

        private bool _pressedGamepadInventory = false;
        private bool _pressedGamepadInteract = false;
        private bool _pressedGamepadApply = false;
        private bool _pressedGamepadJump = false;
        private bool _pressedGamepadFlymode = false;
        private bool _pressedGamepadSlotLeft = false;
        private bool _pressedGamepadSlotRight = false;
        private bool _disposedValue;

        private void HandleGamePad()
        {
            if (!IsActiveScreen) return;

            var succeeded = false;
            var gamePadState = new GamePadState();
            try
            {
                gamePadState = GamePad.GetState(0);
                succeeded = true;
            }
            catch (Exception)
            {
            }

            if (succeeded)
            {
                Manager.Player.MoveInput += gamePadState.ThumbSticks.Left;
                Manager.Player.HeadInput += gamePadState.ThumbSticks.Right;

                if (gamePadState.Buttons.X == ButtonState.Pressed && !_pressedGamepadInteract)
                    Manager.Player.InteractInput = true;
                _pressedGamepadInteract = gamePadState.Buttons.X == ButtonState.Pressed;

                if (gamePadState.Buttons.A == ButtonState.Pressed && !_pressedGamepadApply)
                    Manager.Player.ApplyInput = true;
                _pressedGamepadApply = gamePadState.Buttons.A == ButtonState.Pressed;

                if (gamePadState.Buttons.Y == ButtonState.Pressed && !_pressedGamepadJump)
                    Manager.Player.JumpInput = true;
                _pressedGamepadJump = gamePadState.Buttons.Y == ButtonState.Pressed;

                if (gamePadState.Buttons.LeftStick == ButtonState.Pressed && !_pressedGamepadFlymode)
                    Manager.Player.FlymodeInput = true;
                _pressedGamepadFlymode = gamePadState.Buttons.LeftStick == ButtonState.Pressed;

                if (gamePadState.Buttons.LeftShoulder == ButtonState.Pressed && !_pressedGamepadSlotLeft)
                    Manager.Player.SlotLeftInput = true;
                _pressedGamepadSlotLeft = gamePadState.Buttons.LeftShoulder == ButtonState.Pressed;

                if (gamePadState.Buttons.RightShoulder == ButtonState.Pressed && !_pressedGamepadSlotRight)
                    Manager.Player.SlotRightInput = true;
                _pressedGamepadSlotRight = gamePadState.Buttons.RightShoulder == ButtonState.Pressed;

                if (gamePadState.Buttons.Back == ButtonState.Pressed && !_pressedGamepadInventory)
                    Manager.NavigateToScreen(new InventoryScreen(Manager));
                _pressedGamepadInventory = gamePadState.Buttons.Back == ButtonState.Pressed;
            }
        }

        #endregion
    }
}