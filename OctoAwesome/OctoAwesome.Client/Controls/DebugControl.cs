using System.Linq;
using engenious;
using engenious.Graphics;
using engenious.Helper;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.Languages;

namespace OctoAwesome.Client.Controls
{
    internal class DebugControl : Panel
    {
        private readonly ScreenComponent _manager;

        private readonly AssetComponent _assets;
        private int _bufferIndex;
        private readonly int _bufferSize = 10;
        private readonly Label _devText;
        private readonly Label _position;
        private readonly Label _rotation;
        private readonly Label _fps;
        private readonly Label _box;
        private readonly Label _controlInfo;
        private readonly Label _loadedChunks;
        private readonly Label _loadedTextures;
        private readonly Label _activeTool;
        private readonly Label _toolCount;
        private readonly Label _loadedInfo;
        private readonly Label _flyInfo;
        private readonly Label _temperatureInfo;
        private readonly Label _precipitationInfo;
        private readonly Label _gravityInfo;
        private readonly float[] _frameBuffer;

        private int _frameCount;
        private double _lastFps;

        private readonly StackPanel _leftView;
        private readonly StackPanel _rightView;
        private double _seconds;

        public DebugControl(ScreenComponent screenManager) : base(screenManager)
        {
            _frameBuffer = new float[_bufferSize];
            Player = screenManager.Player;
            _manager = screenManager;
            _assets = screenManager.Game.Assets;

            //Brush for Debug Background
            var bg = new BorderBrush(Color.Black * 0.2f);

            //The left side of the Screen
            _leftView = new StackPanel(ScreenManager)
            {
                Background = bg,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            //The right Side of the Screen
            _rightView = new StackPanel(ScreenManager)
            {
                Background = bg,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };

            //Creating all Labels
            _devText = new Label(ScreenManager)
            {
                Text = OctoClient.DevelopmentVersion
            };
            _leftView.Controls.Add(_devText);

            _loadedChunks = new Label(ScreenManager);
            _leftView.Controls.Add(_loadedChunks);

            _loadedTextures = new Label(ScreenManager);
            _leftView.Controls.Add(_loadedTextures);

            _loadedInfo = new Label(ScreenManager);
            _leftView.Controls.Add(_loadedInfo);

            _position = new Label(ScreenManager);
            _rightView.Controls.Add(_position);

            _rotation = new Label(ScreenManager);
            _rightView.Controls.Add(_rotation);

            _fps = new Label(ScreenManager);
            _rightView.Controls.Add(_fps);

            _controlInfo = new Label(ScreenManager);
            _leftView.Controls.Add(_controlInfo);

            _temperatureInfo = new Label(ScreenManager);
            _rightView.Controls.Add(_temperatureInfo);

            _precipitationInfo = new Label(ScreenManager);
            _rightView.Controls.Add(_precipitationInfo);

            _gravityInfo = new Label(ScreenManager);
            _rightView.Controls.Add(_gravityInfo);

            _activeTool = new Label(ScreenManager);
            _rightView.Controls.Add(_activeTool);

            _toolCount = new Label(ScreenManager);
            _rightView.Controls.Add(_toolCount);

            _flyInfo = new Label(ScreenManager);
            _rightView.Controls.Add(_flyInfo);

            //This Label gets added to the root and is set to Bottom Left
            _box = new Label(ScreenManager)
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left,
                TextColor = Color.White
            };
            Controls.Add(_box);

            //Add the left & right side to the root
            Controls.Add(_leftView);
            Controls.Add(_rightView);

            //Label Setup - Set Settings for all Labels in one place
            foreach (var control in _leftView.Controls)
            {
                control.HorizontalAlignment = HorizontalAlignment.Left;
                if (control is Label) ((Label) control).TextColor = Color.White;
            }

            foreach (var control in _rightView.Controls)
            {
                control.HorizontalAlignment = HorizontalAlignment.Right;
                if (control is Label) ((Label) control).TextColor = Color.White;
            }
        }

        public PlayerComponent Player { get; set; }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (!Visible || !Enabled || !_assets.Ready)
                return;

            if (Player == null || Player.CurrentEntity == null)
                return;

            //Calculate FPS
            _frameCount++;
            _seconds += gameTime.ElapsedGameTime.TotalSeconds;
            if (_frameCount == 10)
            {
                _lastFps = _seconds / _frameCount;
                _frameCount = 0;
                _seconds = 0;
            }

            _frameBuffer[_bufferIndex++] = (float) gameTime.ElapsedGameTime.TotalSeconds;
            _bufferIndex %= _bufferSize;

            //Draw Control Info
            _controlInfo.Text = OctoClient.ActiveControls + ": " + ScreenManager.ActiveScreen.Controls.Count;

            //Draw Position
            var pos = "pos: " + Player.Position.Position;
            _position.Text = pos;

            //Draw Rotation
            var grad = Player.CurrentEntityHead.Angle / MathHelper.TwoPi * 360;
            var rot = "rot: " +
                      (Player.CurrentEntityHead.Angle / MathHelper.TwoPi * 360 % 360).ToString("0.00") + " / " +
                      (Player.CurrentEntityHead.Tilt / MathHelper.TwoPi * 360).ToString("0.00");
            _rotation.Text = rot;

            //Draw Fps
            var fpsString = "fps: " + (1f / _lastFps).ToString("0.00");
            _fps.Text = fpsString;

            //Draw Loaded Chunks
            _loadedChunks.Text =
                $"{OctoClient.LoadedChunks}: {_manager.Game.ResourceManager.GetPlanet(Player.Position.Position.Planet).GlobalChunkCache.DirtyChunkColumn}/{_manager.Game.ResourceManager.GetPlanet(Player.Position.Position.Planet).GlobalChunkCache.LoadedChunkColumns}";

            // Draw Loaded Textures
            _loadedTextures.Text = $"Loaded Textures: {_assets.LoadedTextures}";

            //Get Number of Loaded Items/Blocks
            _loadedInfo.Text = "" + _manager.Game.DefinitionManager.ItemDefinitions.Count() + " " + OctoClient.Items +
                              " - " +
                              _manager.Game.DefinitionManager.BlockDefinitions.Count() + " " + OctoClient.Blocks;

            //Additional Play Information

            //Active Tool
            if (Player.Toolbar.ActiveTool != null)
                _activeTool.Text = OctoClient.ActiveItemTool + ": " + Player.Toolbar.ActiveTool.Definition.Name + " | " +
                                  Player.Toolbar.GetSlotIndex(Player.Toolbar.ActiveTool);

            _toolCount.Text = OctoClient.ToolCount + ": " + Player.Toolbar.Tools.Count(slot => slot != null);

            ////Fly Info
            //if (Player.ActorHost.Player.FlyMode) flyInfo.Text = Languages.OctoClient.FlymodeEnabled;
            //else flyInfo.Text = "";

            var planet = _manager.Game.ResourceManager.GetPlanet(Player.Position.Position.Planet);
            // Temperature Info
            _temperatureInfo.Text = OctoClient.Temperature + ": " +
                                   planet.ClimateMap.GetTemperature(Player.Position.Position.GlobalBlockIndex);

            // Precipitation Info
            _precipitationInfo.Text = "Precipitation: " + planet.ClimateMap.GetPrecipitation(Player.Position.Position.GlobalBlockIndex);

            // Gravity Info
            _gravityInfo.Text = "Gravity" + ": " + planet.Gravity;

            //Draw Box Information
            if (Player.SelectedBox.HasValue && Player.SelectedPoint.HasValue)
            {
                var selection = "box: " +
                                Player.SelectedBox.Value + " on " +
                                Player.SelectedSide + " (" +
                                Player.SelectedPoint.Value.X.ToString("0.000000") + "/" +
                                Player.SelectedPoint.Value.Y.ToString("0.000000") + ") -> " +
                                Player.SelectedEdge + " -> " + Player.SelectedCorner;
                _box.Text = selection;
            }
            else
            {
                _box.Text = "";
            }
        }
    }
}