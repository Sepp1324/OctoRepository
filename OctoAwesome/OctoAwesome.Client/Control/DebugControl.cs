﻿using System.Linq;
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
        private readonly AssetComponent assets;
        private readonly int buffersize = 10;
        private readonly IDefinitionManager definitionManager;

        private readonly Label devText,
            position,
            rotation,
            fps,
            box,
            controlInfo,
            loadedChunks,
            loadedTextures,
            activeTool,
            toolCount,
            loadedInfo,
            flyInfo,
            temperatureInfo,
            precipitationInfo,
            gravityInfo;

        private readonly float[] framebuffer;

        private readonly StackPanel leftView, rightView;
        private readonly IResourceManager resourceManager;
        private int bufferindex;

        private int framecount;
        private double lastfps;
        private double seconds;

        public DebugControl(BaseScreenComponent screenManager, AssetComponent assets, PlayerComponent playerComponent,
            IResourceManager resourceManager, IDefinitionManager definitionManager)
            : base(screenManager)
        {
            framebuffer = new float[buffersize];
            Player = playerComponent;
            this.assets = assets;
            this.resourceManager = resourceManager;
            this.definitionManager = definitionManager;
            Background = new SolidColorBrush(Color.Transparent);

            //Brush for Debug Background
            var bg = new BorderBrush(Color.Black * 0.2f);
            //The left side of the Screen
            leftView = new StackPanel(ScreenManager)
            {
                Background = bg,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            //The right Side of the Screen
            rightView = new StackPanel(ScreenManager)
            {
                Background = bg,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };

            //Creating all Labels
            devText = new Label(ScreenManager)
            {
                Text = OctoClient.DevelopmentVersion
            };
            leftView.Controls.Add(devText);

            loadedChunks = new Label(ScreenManager);
            leftView.Controls.Add(loadedChunks);

            loadedTextures = new Label(ScreenManager);
            leftView.Controls.Add(loadedTextures);

            loadedInfo = new Label(ScreenManager);
            leftView.Controls.Add(loadedInfo);

            position = new Label(ScreenManager);
            rightView.Controls.Add(position);

            rotation = new Label(ScreenManager);
            rightView.Controls.Add(rotation);

            fps = new Label(ScreenManager);
            rightView.Controls.Add(fps);

            controlInfo = new Label(ScreenManager);
            leftView.Controls.Add(controlInfo);

            temperatureInfo = new Label(ScreenManager);
            rightView.Controls.Add(temperatureInfo);

            precipitationInfo = new Label(ScreenManager);
            rightView.Controls.Add(precipitationInfo);

            gravityInfo = new Label(ScreenManager);
            rightView.Controls.Add(gravityInfo);

            activeTool = new Label(ScreenManager);
            rightView.Controls.Add(activeTool);

            toolCount = new Label(ScreenManager);
            rightView.Controls.Add(toolCount);

            flyInfo = new Label(ScreenManager);
            rightView.Controls.Add(flyInfo);

            //This Label gets added to the root and is set to Bottom Left
            box = new Label(ScreenManager)
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left,
                TextColor = Color.White
            };
            Controls.Add(box);

            //Add the left & right side to the root
            Controls.Add(leftView);
            Controls.Add(rightView);

            //Label Setup - Set Settings for all Labels in one place
            foreach (var control in leftView.Controls)
            {
                control.HorizontalAlignment = HorizontalAlignment.Left;
                if (control is Label) ((Label)control).TextColor = Color.White;
            }

            foreach (var control in rightView.Controls)
            {
                control.HorizontalAlignment = HorizontalAlignment.Right;
                if (control is Label) ((Label)control).TextColor = Color.White;
            }
        }

        public PlayerComponent Player { get; set; }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (!Visible || !Enabled || !assets.Ready)
                return;

            if (Player == null || Player.CurrentEntity == null)
                return;

            //Calculate FPS
            framecount++;
            seconds += gameTime.ElapsedGameTime.TotalSeconds;
            if (framecount == 10)
            {
                lastfps = seconds / framecount;
                framecount = 0;
                seconds = 0;
            }

            framebuffer[bufferindex++] = (float)gameTime.ElapsedGameTime.TotalSeconds;
            bufferindex %= buffersize;

            //Draw Control Info
            controlInfo.Text = OctoClient.ActiveControls + ": " + ScreenManager.ActiveScreen.Controls.Count;

            //Draw Position
            var pos = "pos: " + Player.Position.Position;
            position.Text = pos;

            //Draw Rotation
            var grad = Player.CurrentEntityHead.Angle / MathHelper.TwoPi * 360;
            var rot = "rot: " +
                      (Player.CurrentEntityHead.Angle / MathHelper.TwoPi * 360 % 360).ToString("0.00") + " / " +
                      (Player.CurrentEntityHead.Tilt / MathHelper.TwoPi * 360).ToString("0.00");
            rotation.Text = rot;

            //Draw Fps
            var fpsString = "fps: " + (1f / lastfps).ToString("0.00");
            fps.Text = fpsString;

            //Draw Loaded Chunks
            loadedChunks.Text = string.Format("{0}: {1}/{2}",
                OctoClient.LoadedChunks,
                resourceManager.GetPlanet(Player.Position.Position.Planet).GlobalChunkCache.DirtyChunkColumn,
                resourceManager.GetPlanet(Player.Position.Position.Planet).GlobalChunkCache.LoadedChunkColumns);

            // Draw Loaded Textures
            loadedTextures.Text = string.Format("Loaded Textures: {0}",
                assets.LoadedTextures);

            //Get Number of Loaded Items/Blocks
            loadedInfo.Text = "" + definitionManager.ItemDefinitions.Count() + " " + OctoClient.Items + " - " +
                              definitionManager.BlockDefinitions.Count() + " " + OctoClient.Blocks;

            //Additional Play Information

            //Active Tool
            if (Player.Toolbar.ActiveTool != null)
                activeTool.Text = OctoClient.ActiveItemTool + ": " + Player.Toolbar.ActiveTool.Definition.Name + " | " +
                                  Player.Toolbar.GetSlotIndex(Player.Toolbar.ActiveTool);

            toolCount.Text = OctoClient.ToolCount + ": " + Player.Toolbar.Tools.Count(slot => slot != null);

            ////Fly Info
            //if (Player.ActorHost.Player.FlyMode) flyInfo.Text = UI.Languages.OctoClient.FlymodeEnabled;
            //else flyInfo.Text = "";

            var planet = resourceManager.GetPlanet(Player.Position.Position.Planet);
            // Temperature Info
            temperatureInfo.Text = OctoClient.Temperature + ": " +
                                   planet.ClimateMap.GetTemperature(Player.Position.Position.GlobalBlockIndex);

            // Precipitation Info
            precipitationInfo.Text = "Precipitation: " +
                                     planet.ClimateMap.GetPrecipitation(Player.Position.Position.GlobalBlockIndex);

            // Gravity Info
            gravityInfo.Text = "Gravity" + ": " + planet.Gravity;

            //Draw Box Information
            if (Player.SelectedBox.HasValue && Player.SelectedPoint.HasValue)
            {
                var selection = "box: " +
                                Player.SelectedBox.Value + " on " +
                                Player.SelectedSide + " (" +
                                Player.SelectedPoint.Value.X.ToString("0.000000") + "/" +
                                Player.SelectedPoint.Value.Y.ToString("0.000000") + ") -> " +
                                Player.SelectedEdge + " -> " + Player.SelectedCorner;
                box.Text = selection;
            }
            else
            {
                box.Text = "";
            }
        }
    }
}