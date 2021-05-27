using System;
using System.Collections.Generic;
using engenious;
using engenious.Input;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.Controls;
using OctoAwesome.Client.Languages;
using OctoAwesome.Common;
using OctoAwesome.Definitions;
using OctoAwesome.Notifications;
using OctoAwesome.Runtime;

namespace OctoAwesome.Client
{
    /// <summary>
    ///     This is the main type for your game
    /// </summary>
    internal class OctoGame : Game
    {
        private readonly ITypeContainer typeContainer;

        public OctoGame()
        {
            //graphics = new GraphicsDeviceManager(this);
            //graphics.PreferredBackBufferWidth = 1080;
            //graphics.PreferredBackBufferHeight = 720;

            //Content.RootDirectory = "Content";

            Title = "OctoAwesome";
            IsMouseVisible = true;

            //TODO: REVIEW THIS ERROR
            //Icon = Properties.Resources.octoawesome;

            typeContainer = TypeContainer.Get<ITypeContainer>();
            Register(typeContainer);

            //Window.AllowUserResizing = true;
            Settings = TypeContainer.Get<Settings>();

            ExtensionLoader = TypeContainer.Get<ExtensionLoader>();
            ExtensionLoader.LoadExtensions();

            Service = TypeContainer.Get<GameService>();
            //TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 15);

            var width = Settings.Get("Width", 1080);
            var height = Settings.Get("Height", 720);
            Window.ClientSize = new Size(width, height);

            Window.Fullscreen = Settings.Get("EnableFullscreen", false);

            if (Settings.KeyExists("Viewrange"))
            {
                var viewrange = Settings.Get<int>("Viewrange");

                if (viewrange < 1)
                    throw new NotSupportedException("Viewrange in app.config darf nicht kleiner 1 sein");

                SceneControl.VIEWRANGE = viewrange;
            }

            Assets = new AssetComponent(this);
            Components.Add(Assets);


            Screen = new ScreenComponent(this);
            Screen.UpdateOrder = 1;
            Screen.DrawOrder = 1;
            Components.Add(Screen);


            KeyMapper = new KeyMapper(Screen, Settings);


            #region GameComponents

            DefinitionManager = TypeContainer.Get<DefinitionManager>();

            //var persistenceManager = new DiskPersistenceManager(ExtensionLoader, DefinitionManager, Settings);
            //ResourceManager = new ResourceManager(ExtensionLoader, DefinitionManager, Settings, persistenceManager);
            ResourceManager = TypeContainer.Get<ContainerResourceManager>();


            Player = new PlayerComponent(this, ResourceManager);
            Player.UpdateOrder = 2;
            Components.Add(Player);

            Simulation = new Components.SimulationComponent(this,
                ExtensionLoader, ResourceManager);

            Entity = new Components.EntityComponent(this, Simulation);
            Entity.UpdateOrder = 2;
            Components.Add(Entity);

            Camera = new CameraComponent(this);
            Camera.UpdateOrder = 3;
            Components.Add(Camera);

            Simulation.UpdateOrder = 4;
            Components.Add(Simulation);

            #endregion GameComponents

            /*Resize += (s, e) =>
            {
                //if (Window.ClientBounds.Height == graphics.PreferredBackBufferHeight &&
                //   Window.ClientBounds.Width == graphics.PreferredBackBufferWidth)
                //    return;

                //graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                //graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                //graphics.ApplyChanges();
            };*/
            SetKeyBindings();
        }

        //GraphicsDeviceManager graphics;

        public CameraComponent Camera { get; }

        public PlayerComponent Player { get; }

        public Components.SimulationComponent Simulation { get; }

        public GameService Service { get; }

        public ScreenComponent Screen { get; }

        public KeyMapper KeyMapper { get; }

        public AssetComponent Assets { get; }

        public Settings Settings { get; }

        public IDefinitionManager DefinitionManager { get; }

        public IResourceManager ResourceManager { get; }

        public ExtensionLoader ExtensionLoader { get; }

        public Components.EntityComponent Entity { get; }

        private static void Register(ITypeContainer typeContainer)
        {
            typeContainer.Register<Settings>(InstanceBehaviour.Singleton);
            typeContainer.Register<ISettings, Settings>(InstanceBehaviour.Singleton);
            typeContainer.Register<ExtensionLoader>(InstanceBehaviour.Singleton);
            typeContainer.Register<IExtensionLoader, ExtensionLoader>(InstanceBehaviour.Singleton);
            typeContainer.Register<IExtensionResolver, ExtensionLoader>(InstanceBehaviour.Singleton);
            typeContainer.Register<DefinitionManager>(InstanceBehaviour.Singleton);
            typeContainer.Register<IDefinitionManager, DefinitionManager>(InstanceBehaviour.Singleton);
            typeContainer.Register<ContainerResourceManager>(InstanceBehaviour.Singleton);
            typeContainer.Register<IResourceManager, ContainerResourceManager>(InstanceBehaviour.Singleton);
            typeContainer.Register<GameService>(InstanceBehaviour.Singleton);
            typeContainer.Register<IGameService, GameService>(InstanceBehaviour.Singleton);
            typeContainer.Register<UpdateHub>(InstanceBehaviour.Singleton);
            typeContainer.Register<IUpdateHub, UpdateHub>(InstanceBehaviour.Singleton);
        }

        private void SetKeyBindings()
        {
            KeyMapper.RegisterBinding("octoawesome:forward", OctoKeys.forward);
            KeyMapper.RegisterBinding("octoawesome:left", OctoKeys.left);
            KeyMapper.RegisterBinding("octoawesome:backward", OctoKeys.backward);
            KeyMapper.RegisterBinding("octoawesome:right", OctoKeys.right);
            KeyMapper.RegisterBinding("octoawesome:headup", OctoKeys.headup);
            KeyMapper.RegisterBinding("octoawesome:headdown", OctoKeys.headdown);
            KeyMapper.RegisterBinding("octoawesome:headleft", OctoKeys.headleft);
            KeyMapper.RegisterBinding("octoawesome:headright", OctoKeys.headright);
            KeyMapper.RegisterBinding("octoawesome:interact", OctoKeys.interact);
            KeyMapper.RegisterBinding("octoawesome:apply", OctoKeys.apply);
            KeyMapper.RegisterBinding("octoawesome:flymode", OctoKeys.flymode);
            KeyMapper.RegisterBinding("octoawesome:jump", OctoKeys.jump);
            for (var i = 0; i < 10; i++)
                KeyMapper.RegisterBinding("octoawesome:slot" + i, OctoKeys.ResourceManager.GetString("slot" + i));
            KeyMapper.RegisterBinding("octoawesome:debug.allblocks", OctoKeys.debug_allblocks);
            KeyMapper.RegisterBinding("octoawesome:debug.control", OctoKeys.debug_control);
            KeyMapper.RegisterBinding("octoawesome:inventory", OctoKeys.inventory);
            KeyMapper.RegisterBinding("octoawesome:hidecontrols", OctoKeys.hidecontrols);
            KeyMapper.RegisterBinding("octoawesome:exit", OctoKeys.exit);
            KeyMapper.RegisterBinding("octoawesome:freemouse", OctoKeys.freemouse);
            KeyMapper.RegisterBinding("octoawesome:fullscreen", OctoKeys.fullscreen);
            KeyMapper.RegisterBinding("octoawesome:teleport", OctoKeys.teleport);
            KeyMapper.RegisterBinding("octoawesome:toggleAmbientOcclusion", OctoKeys.ambientOcclusion);
            KeyMapper.RegisterBinding("octoawesome:toggleWireFrame", OctoKeys.wireFrame);

            var standardKeys = new Dictionary<string, Keys>
            {
                {"octoawesome:forward", Keys.W},
                {"octoawesome:left", Keys.A},
                {"octoawesome:backward", Keys.S},
                {"octoawesome:right", Keys.D},
                {"octoawesome:headup", Keys.Up},
                {"octoawesome:headdown", Keys.Down},
                {"octoawesome:headleft", Keys.Left},
                {"octoawesome:headright", Keys.Right},
                {"octoawesome:interact", Keys.E},
                {"octoawesome:apply", Keys.Q},
                {"octoawesome:flymode", Keys.ScrollLock},
                {"octoawesome:jump", Keys.Space},
                {"octoawesome:slot0", Keys.D1},
                {"octoawesome:slot1", Keys.D2},
                {"octoawesome:slot2", Keys.D3},
                {"octoawesome:slot3", Keys.D4},
                {"octoawesome:slot4", Keys.D5},
                {"octoawesome:slot5", Keys.D6},
                {"octoawesome:slot6", Keys.D7},
                {"octoawesome:slot7", Keys.D8},
                {"octoawesome:slot8", Keys.D9},
                {"octoawesome:slot9", Keys.D0},
                {"octoawesome:debug.allblocks", Keys.L},
                {"octoawesome:debug.control", Keys.F10},
                {"octoawesome:inventory", Keys.I},
                {"octoawesome:hidecontrols", Keys.F9},
                {"octoawesome:exit", Keys.Escape},
                {"octoawesome:freemouse", Keys.F12},
                {"octoawesome:fullscreen", Keys.F11},
                {"octoawesome:teleport", Keys.T},
                {"octoawesome:toggleAmbientOcclusion", Keys.O},
                {"octoawesome:toggleWireFrame", Keys.J}
            };

            KeyMapper.LoadFromConfig(standardKeys);

            KeyMapper.AddAction("octoawesome:fullscreen", type =>
            {
                if (type == KeyMapper.KeyType.Down) Window.Fullscreen = !Window.Fullscreen;
            });
        }

        protected override void OnExiting(EventArgs args)
        {
            Player.SetEntity(null);
            Simulation.ExitGame();
        }


        public override void Dispose()
        {
            base.Dispose();
        }
    }
}