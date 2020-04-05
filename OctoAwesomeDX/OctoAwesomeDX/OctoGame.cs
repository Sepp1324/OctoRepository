using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OctoAwesome.Components;
using OctoAwesomeDX.Components;
using System;
using System.Linq;

namespace OctoAwesomeDX
{
    public class OctoGame : Game
    {
        GraphicsDeviceManager graphics;

        Camera3DComponent camera3d;
        EgoCameraComponent egoCamera;
        InputComponent input;
        WorldComponent world;

        Render3DComponent render3d;

        public OctoGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Window.Title = "OctoAwesome";
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            input = new InputComponent(this);
            input.UpdateOrder = 1;
            Components.Add(input);

            world = new WorldComponent(this, input);
            world.UpdateOrder = 2;
            Components.Add(world);

            //camera3d = new Camera3DComponent(this, world);
            //camera3d.UpdateOrder = 3;
            //Components.Add(camera3d);

            egoCamera = new EgoCameraComponent(this, world);
            egoCamera.UpdateOrder = 3;
            Components.Add(egoCamera);

            render3d = new Render3DComponent(this, world, egoCamera);
            render3d.DrawOrder = 1;
            Components.Add(render3d);
        }
    }
}
