using System.Linq;
using engenious;
using engenious.Graphics;
using engenious.Helper;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.Definitions;
using OctoAwesome.UI.Components;
using OctoAwesome.UI.Languages;

namespace OctoAwesome.UI.Controls
{
    internal class DebugControl : Panel
    {
        private readonly AssetComponent _assets;
        private readonly int _bufferSize = 10;
        private readonly IDefinitionManager _definitionManager;

        private readonly Label _position, _rotation, _fps, _box, _controlInfo, _loadedChunks, _loadedTextures, _activeTool, _toolCount, _loadedInfo, _temperatureInfo, _precipitationInfo, _gravityInfo;

        private readonly float[] _frameBuffer;

        private readonly IResourceManager _resourceManager;
        private int _bufferIndex;

        private int _frameCount;
        private double _lastFps;
        private double _seconds;

        public DebugControl(BaseScreenComponent screenManager, AssetComponent assets, PlayerComponent playerComponent, IResourceManager resourceManager, IDefinitionManager definitionManager) : base(screenManager)
        {
            _frameBuffer = new float[_bufferSize];
            Player = playerComponent;
            _assets = assets;
            _resourceManager = resourceManager;
            _definitionManager = definitionManager;
            Background = new SolidColorBrush(Color.Transparent);

            //Brush for Debug Background
            var bg = new BorderBrush(Color.Black * 0.2f);
            //The left side of the Screen
            var leftView = new StackPanel(ScreenManager)
            {
                Background = bg,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            //The right Side of the Screen
            var rightView = new StackPanel(ScreenManager)
            {
                Background = bg,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };

            //Creating all Labels
            var devText = new Label(ScreenManager)
            {
                Text = OctoClient.DevelopmentVersion
            };
            leftView.Controls.Add(devText);

            _loadedChunks = new(ScreenManager);
            leftView.Controls.Add(_loadedChunks);

            _loadedTextures = new(ScreenManager);
            leftView.Controls.Add(_loadedTextures);

            _loadedInfo = new(ScreenManager);
            leftView.Controls.Add(_loadedInfo);

            _position = new(ScreenManager);
            rightView.Controls.Add(_position);

            _rotation = new(ScreenManager);
            rightView.Controls.Add(_rotation);

            _fps = new(ScreenManager);
            rightView.Controls.Add(_fps);

            _controlInfo = new(ScreenManager);
            leftView.Controls.Add(_controlInfo);

            _temperatureInfo = new(ScreenManager);
            rightView.Controls.Add(_temperatureInfo);

            _precipitationInfo = new(ScreenManager);
            rightView.Controls.Add(_precipitationInfo);

            _gravityInfo = new(ScreenManager);
            rightView.Controls.Add(_gravityInfo);

            _activeTool = new(ScreenManager);
            rightView.Controls.Add(_activeTool);

            _toolCount = new(ScreenManager);
            rightView.Controls.Add(_toolCount);

            var flyInfo = new Label(ScreenManager);
            rightView.Controls.Add(flyInfo);

            //This Label gets added to the root and is set to Bottom Left
            _box = new(ScreenManager)
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left,
                TextColor = Color.White
            };
            Controls.Add(_box);

            //Add the left & right side to the root
            Controls.Add(leftView);
            Controls.Add(rightView);

            //Label Setup - Set Settings for all Labels in one place
            foreach (var control in leftView.Controls)
            {
                control.HorizontalAlignment = HorizontalAlignment.Left;
                if (control is Label label) label.TextColor = Color.White;
            }

            foreach (var control in rightView.Controls)
            {
                control.HorizontalAlignment = HorizontalAlignment.Right;
                if (control is Label label) label.TextColor = Color.White;
            }
        }

        public PlayerComponent Player { get; set; }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (!Visible || !Enabled || !_assets.Ready)
                return;

            if (Player?.CurrentEntity == null)
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

            _frameBuffer[_bufferIndex++] = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _bufferIndex %= _bufferSize;

            //Draw Control Info
            _controlInfo.Text = OctoClient.ActiveControls + ": " + ScreenManager.ActiveScreen!.Controls.Count;

            //Draw Position
            var pos = "pos: " + Player.Position.Position;
            _position.Text = pos;

            //Draw Rotation
            var grad = Player.CurrentEntityHead.Angle / MathHelper.TwoPi * 360;
            var rot = "rot: " + (Player.CurrentEntityHead.Angle / MathHelper.TwoPi * 360 % 360).ToString("0.00") + " / " + (Player.CurrentEntityHead.Tilt / MathHelper.TwoPi * 360).ToString("0.00");
            _rotation.Text = rot;

            //Draw Fps
            var fpsString = "fps: " + (1f / _lastFps).ToString("0.00");
            _fps.Text = fpsString;

            //Draw Loaded Chunks
            _loadedChunks.Text = $"{OctoClient.LoadedChunks}: {_resourceManager.GetPlanet(Player.Position.Position.Planet).GlobalChunkCache.DirtyChunkColumn}/{_resourceManager.GetPlanet(Player.Position.Position.Planet).GlobalChunkCache.LoadedChunkColumns}";

            // Draw Loaded Textures
            _loadedTextures.Text = $"Loaded Textures: {_assets.LoadedTextures}";

            //Get Number of Loaded Items/Blocks
            _loadedInfo.Text = "" + _definitionManager.ItemDefinitions.Count() + " " + OctoClient.Items + " - " + _definitionManager.BlockDefinitions.Count() + " " + OctoClient.Blocks;

            //Additional Play Information

            //Active Tool
            if (Player.Toolbar.ActiveTool != null)
                _activeTool.Text = OctoClient.ActiveItemTool + ": " + Player.Toolbar.ActiveTool.Definition.Name + " | " + Player.Toolbar.GetSlotIndex(Player.Toolbar.ActiveTool);

            _toolCount.Text = OctoClient.ToolCount + ": " + Player.Toolbar.Tools.Count(slot => slot != null);

            ////Fly Info
            //if (Player.ActorHost.Player.FlyMode) flyInfo.Text = UI.Languages.OctoClient.FlymodeEnabled;
            //else flyInfo.Text = "";

            var planet = _resourceManager.GetPlanet(Player.Position.Position.Planet);
            // Temperature Info
            _temperatureInfo.Text = OctoClient.Temperature + ": " + planet.ClimateMap.GetTemperature(Player.Position.Position.GlobalBlockIndex);

            // Precipitation Info
            _precipitationInfo.Text = "Precipitation: " + planet.ClimateMap.GetPrecipitation(Player.Position.Position.GlobalBlockIndex);

            // Gravity Info
            _gravityInfo.Text = "Gravity" + ": " + planet.Gravity;

            //Draw Box Information
            if (Player.SelectedBox.HasValue && Player.SelectedPoint.HasValue)
            {
                var selection = "box: " + Player.SelectedBox.Value + " on " + Player.SelectedSide + " (" + Player.SelectedPoint.Value.X.ToString("0.000000") + "/" + Player.SelectedPoint.Value.Y.ToString("0.000000") + ") -> " + Player.SelectedEdge + " -> " + Player.SelectedCorner;
                _box.Text = selection;
            }
            else
            {
                _box.Text = "";
            }
        }
    }
}