﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OctoAwesome.Model;
using OctoAwesomeDX.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Components
{
    internal sealed class Render3DComponent : DrawableGameComponent
    {
        private WorldComponent world;
        private Camera3DComponent camera;

        private BasicEffect effect;

        private Texture2D grass;
        private Texture2D sand;
        private Texture2D water;
        private Texture2D tree;
        private Texture2D sprite;
        private Texture2D box;

        private VertexBuffer vb;
        private IndexBuffer ib;

        private int vertexCount;
        private int indexCount;

        public Render3DComponent(Game game, WorldComponent world, Camera3DComponent camera)
            : base(game)
        {
            this.world = world;
            this.camera = camera;
        }

        protected override void LoadContent()
        {
            //int width = world.World.Map.CellCache.GetLength(0);
            //int height = world.World.Map.CellCache.GetLength(1);

            //vertexCount = width * height * 4;
            //indexCount = width * height * 6;

            //VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[vertexCount];
            //short[] index = new short[indexCount];

            //for (int z = 0; z < height; z++)
            //{
            //    for (int x = 0; x < width; x++)
            //    {
            //        int vertexOffset = (((z * width) + x) * 4);
            //        int indexOffset = (((z * width) + x) * 6);

            //        vertices[vertexOffset + 0] = new VertexPositionNormalTexture(new Vector3(x, 0, z), Vector3.Up, new Vector2(0, 0));
            //        vertices[vertexOffset + 1] = new VertexPositionNormalTexture(new Vector3(x + 1, 0, z), Vector3.Up, new Vector2(1, 0));
            //        vertices[vertexOffset + 2] = new VertexPositionNormalTexture(new Vector3(x, 0, z + 1), Vector3.Up, new Vector2(0, 1));
            //        vertices[vertexOffset + 3] = new VertexPositionNormalTexture(new Vector3(x + 1, 0, z + 1), Vector3.Up, new Vector2(1, 1));

            //        index[indexOffset + 0] = (short)(vertexOffset + 0);
            //        index[indexOffset + 1] = (short)(vertexOffset + 1);
            //        index[indexOffset + 2] = (short)(vertexOffset + 3);
            //        index[indexOffset + 3] = (short)(vertexOffset + 0);
            //        index[indexOffset + 4] = (short)(vertexOffset + 3);
            //        index[indexOffset + 5] = (short)(vertexOffset + 2);
            //    }
            //}

            vertexCount = Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y * Chunk.CHUNKSIZE_Z * 24;
            indexCount = Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y * Chunk.CHUNKSIZE_Z * 36;

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[vertexCount];
            short[] index = new short[indexCount];

            for (int z = 0; z < Chunk.CHUNKSIZE_Z; z++)
            {
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
                    {
                        int offset = x + (y * Chunk.CHUNKSIZE_X) + (z * Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y);
                        int vertexOffset = offset * 24;
                        int indexOffset = offset * 36;

                        //Oben
                        vertices[vertexOffset + 0] = new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 0), Vector3.Up, new Vector2(0, 0));
                        vertices[vertexOffset + 1] = new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 0), Vector3.Up, new Vector2(1, 0));
                        vertices[vertexOffset + 2] = new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 1), Vector3.Up, new Vector2(0, 1));
                        vertices[vertexOffset + 3] = new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 1), Vector3.Up, new Vector2(1, 1));
                        index[indexOffset + 0] = (short)(vertexOffset + 0);
                        index[indexOffset + 1] = (short)(vertexOffset + 1);
                        index[indexOffset + 2] = (short)(vertexOffset + 3);
                        index[indexOffset + 3] = (short)(vertexOffset + 0);
                        index[indexOffset + 4] = (short)(vertexOffset + 3);
                        index[indexOffset + 5] = (short)(vertexOffset + 2);

                        //Links
                        vertices[vertexOffset + 4] = new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 0), Vector3.Left, new Vector2(0, 0));
                        vertices[vertexOffset + 5] = new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 1), Vector3.Left, new Vector2(1, 0));
                        vertices[vertexOffset + 6] = new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 0), Vector3.Left, new Vector2(0, 1));
                        vertices[vertexOffset + 7] = new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 1), Vector3.Left, new Vector2(1, 1));
                        index[indexOffset + 6] = (short)(vertexOffset + 4);
                        index[indexOffset + 7] = (short)(vertexOffset + 5);
                        index[indexOffset + 8] = (short)(vertexOffset + 7);
                        index[indexOffset + 9] = (short)(vertexOffset + 4);
                        index[indexOffset + 10] = (short)(vertexOffset + 7);
                        index[indexOffset + 11] = (short)(vertexOffset + 6);

                        //Vorne
                        vertices[vertexOffset + 8] = new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 1), Vector3.Forward, new Vector2(0, 0));
                        vertices[vertexOffset + 9] = new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 1), Vector3.Forward, new Vector2(1, 0));
                        vertices[vertexOffset + 10] = new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 1), Vector3.Forward, new Vector2(0, 1));
                        vertices[vertexOffset + 11] = new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 1), Vector3.Forward, new Vector2(1, 1));
                        index[indexOffset + 12] = (short)(vertexOffset + 8);
                        index[indexOffset + 13] = (short)(vertexOffset + 9);
                        index[indexOffset + 14] = (short)(vertexOffset + 11);
                        index[indexOffset + 15] = (short)(vertexOffset + 8);
                        index[indexOffset + 16] = (short)(vertexOffset + 11);
                        index[indexOffset + 17] = (short)(vertexOffset + 10);

                        //Rechts
                        vertices[vertexOffset + 12] = new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 1), Vector3.Right, new Vector2(0, 0));
                        vertices[vertexOffset + 13] = new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 0), Vector3.Right, new Vector2(1, 0));
                        vertices[vertexOffset + 14] = new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 1), Vector3.Right, new Vector2(0, 1));
                        vertices[vertexOffset + 15] = new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 0), Vector3.Right, new Vector2(1, 1));
                        index[indexOffset + 18] = (short)(vertexOffset + 12);
                        index[indexOffset + 19] = (short)(vertexOffset + 13);
                        index[indexOffset + 20] = (short)(vertexOffset + 15);
                        index[indexOffset + 21] = (short)(vertexOffset + 12);
                        index[indexOffset + 22] = (short)(vertexOffset + 15);
                        index[indexOffset + 23] = (short)(vertexOffset + 14);

                        //Hinten
                        vertices[vertexOffset + 16] = new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 0), Vector3.Backward, new Vector2(0, 0));
                        vertices[vertexOffset + 17] = new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 0), Vector3.Backward, new Vector2(1, 0));
                        vertices[vertexOffset + 18] = new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 0), Vector3.Backward, new Vector2(0, 1));
                        vertices[vertexOffset + 19] = new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 0), Vector3.Backward, new Vector2(1, 1));
                        index[indexOffset + 24] = (short)(vertexOffset + 16);
                        index[indexOffset + 25] = (short)(vertexOffset + 17);
                        index[indexOffset + 26] = (short)(vertexOffset + 19);
                        index[indexOffset + 27] = (short)(vertexOffset + 16);
                        index[indexOffset + 28] = (short)(vertexOffset + 19);
                        index[indexOffset + 29] = (short)(vertexOffset + 18);

                        //Unten
                        vertices[vertexOffset + 20] = new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 1), Vector3.Down, new Vector2(0, 0));
                        vertices[vertexOffset + 21] = new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 1), Vector3.Down, new Vector2(1, 0));
                        vertices[vertexOffset + 22] = new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 0), Vector3.Down, new Vector2(0, 1));
                        vertices[vertexOffset + 23] = new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 0), Vector3.Down, new Vector2(1, 1));
                        index[indexOffset + 30] = (short)(vertexOffset + 20);
                        index[indexOffset + 31] = (short)(vertexOffset + 21);
                        index[indexOffset + 32] = (short)(vertexOffset + 23);
                        index[indexOffset + 33] = (short)(vertexOffset + 20);
                        index[indexOffset + 34] = (short)(vertexOffset + 23);
                        index[indexOffset + 35] = (short)(vertexOffset + 22);
                    }
                }
            }

            vb = new VertexBuffer(GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, vertexCount, BufferUsage.WriteOnly);
            vb.SetData<VertexPositionNormalTexture>(vertices);

            ib = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, indexCount, BufferUsage.WriteOnly);
            ib.SetData<short>(index);

            grass = Game.Content.Load<Texture2D>("Textures/grass_center");
            sand = Game.Content.Load<Texture2D>("Textures/sand_center");
            tree = Game.Content.Load<Texture2D>("Textures/tree");
            sprite = Game.Content.Load<Texture2D>("Textures/sprite");
            water = Game.Content.Load<Texture2D>("Textures/water_center");
            box = Game.Content.Load<Texture2D>("Textures/box");

            effect = new BasicEffect(GraphicsDevice);

            effect.World = Matrix.Identity;
            effect.Projection = camera.Projection;
            effect.TextureEnabled = true;

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            effect.World = Matrix.Identity;
            effect.View = camera.View;

            GraphicsDevice.SetVertexBuffer(vb);
            GraphicsDevice.Indices = ib;

            int width = world.World.Map.CellCache.GetLength(0);
            int height = world.World.Map.CellCache.GetLength(1);

            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    Model.CellCache cell = world.World.Map.CellCache[x, z];

                    switch (cell.CellType)
                    {
                        case Model.CellType.Grass:
                            effect.Texture = grass;
                            break;

                        case Model.CellType.Sand:
                            effect.Texture = sand;
                            break;
                        case Model.CellType.Water:
                            effect.Texture = water;
                            break;
                    }

                    int indexOffset = ((z * width) + x) * 6;

                    foreach (var pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, indexOffset, 2);
                    }
                }
            }

            foreach (var item in world.World.Map.Items.OrderBy(t => t.Position.Y))
            {
                if (item is Model.TreeItem)
                {
                    effect.Texture = tree;

                    VertexPositionNormalTexture[] treeVertices = new VertexPositionNormalTexture[]
                    {
                        new VertexPositionNormalTexture(new Vector3(-0.5f, 2, 0), Vector3.Backward, new Vector2(0, 0)),
                        new VertexPositionNormalTexture(new Vector3(0.5f, 2, 0), Vector3.Backward, new Vector2(1, 0)),
                        new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), Vector3.Backward, new Vector2(1, 1)),
                        new VertexPositionNormalTexture(new Vector3(-0.5f, 2, 0), Vector3.Backward, new Vector2(0, 0)),
                        new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), Vector3.Backward, new Vector2(1, 1)),
                        new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0), Vector3.Backward, new Vector2(0, 1))
                    };

                    Matrix billboard = Matrix.Invert(camera.View);
                    billboard.Translation = new Vector3(item.Position.X, 0, item.Position.Y);

                    effect.World = billboard;

                    foreach (var pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, treeVertices, 0, 2);
                    }
                }

                if (item is Model.BoxItem)
                {
                    effect.Texture = box;

                    VertexPositionNormalTexture[] boxVertices = new VertexPositionNormalTexture[]
                    {
                        new VertexPositionNormalTexture(new Vector3(-0.5f, 1, 0), Vector3.Backward, new Vector2(0, 0)),
                        new VertexPositionNormalTexture(new Vector3(0.5f, 1, 0), Vector3.Backward, new Vector2(1, 0)),
                        new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), Vector3.Backward, new Vector2(1, 1)),
                        new VertexPositionNormalTexture(new Vector3(-0.5f, 1, 0), Vector3.Backward, new Vector2(0, 0)),
                        new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), Vector3.Backward, new Vector2(1, 1)),
                        new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0), Vector3.Backward, new Vector2(0, 1))
                    };

                    Matrix billboard = Matrix.Invert(camera.View);
                    billboard.Translation = new Vector3(item.Position.X, 0, item.Position.Y);

                    effect.World = billboard;

                    foreach (var pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, boxVertices, 0, 2);
                    }
                }

                if (item is Model.Player)
                {
                    effect.Texture = sprite;

                    float spriteWidth = 1f / 9;
                    float spriteHeight = 1f / 8;

                    int frame = (int)(gameTime.TotalGameTime.TotalMilliseconds / 250) % 4;

                    float offsetx = 0;

                    if (world.World.Player.State == Model.PlayerState.WALK)
                    {

                        switch (frame)
                        {
                            case 0: offsetx = 0; break;
                            case 1: offsetx = spriteWidth; break;
                            case 2: offsetx = 2 * spriteWidth; break;
                            case 3: offsetx = spriteWidth; break;
                        }
                    }
                    else
                    {
                        offsetx = spriteWidth;
                    }

                    //Umrechnung in Grad
                    float direction = (world.World.Player.Angle * 360f) / (float)(2 * Math.PI);

                    //in positiven BEreich
                    direction += 180;

                    //offset
                    direction += 45;

                    int sector = (int)(direction / 90);

                    float offsety = 0;

                    switch (sector)
                    {
                        case 1: offsety = 3 * spriteHeight; break;
                        case 2: offsety = 2 * spriteHeight; break;
                        case 3: offsety = 0 * spriteHeight; break;
                        case 4: offsety = 1 * spriteHeight; break;
                    }

                    VertexPositionNormalTexture[] spriteVertices = new VertexPositionNormalTexture[]
                    {
                        new VertexPositionNormalTexture(new Vector3(-0.5f, 1, 0), Vector3.Backward, new Vector2(offsetx, offsety)),
                        new VertexPositionNormalTexture(new Vector3(0.5f, 1, 0), Vector3.Backward, new Vector2(offsetx + spriteWidth, offsety)),
                        new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), Vector3.Backward, new Vector2(offsetx + spriteWidth, offsety + spriteHeight)),
                        new VertexPositionNormalTexture(new Vector3(-0.5f, 1, 0), Vector3.Backward, new Vector2(offsetx, offsety)),
                        new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), Vector3.Backward, new Vector2(offsetx + spriteWidth, offsety + spriteHeight)),
                        new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0), Vector3.Backward, new Vector2(offsetx, offsety + spriteHeight))
                    };

                    Matrix billboard = Matrix.Invert(camera.View);
                    billboard.Translation = new Vector3(item.Position.X, 0, item.Position.Y);

                    effect.World = billboard;


                    foreach (var pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, spriteVertices, 0, 2);
                    }
                }
            }
        }
    }
}
