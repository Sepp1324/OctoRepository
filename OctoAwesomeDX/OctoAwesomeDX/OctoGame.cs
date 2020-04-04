using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OctoAwesome.Components;
using OctoAwesome.Rendering;
using OctoAwesomeDX.Components;
using System;
using System.Linq;

namespace OctoAwesomeDX
{
    public class OctoGame : Game
    {
        GraphicsDeviceManager graphics;

        CameraComponent camera;
        InputComponent input;
        RenderComponent render;
        WorldComponent world;

        Render3DComponent render3d;

        public OctoGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            input = new InputComponent(this);
            input.UpdateOrder = 1;
            Components.Add(input);

            world = new WorldComponent(this, input);
            world.UpdateOrder = 2;
            Components.Add(world);

            camera = new CameraComponent(this, world, input);
            camera.UpdateOrder = 3;
            Components.Add(camera);

            /*render = new RenderComponent(this, world, camera);
            render.DrawOrder = 1;
            Components.Add(render);*/

            render3d = new Render3DComponent(this);
            render3d.DrawOrder = 1;
            Components.Add(render3d);
        }
    }
}
