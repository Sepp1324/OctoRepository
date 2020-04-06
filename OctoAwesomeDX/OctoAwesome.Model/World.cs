using Microsoft.Xna.Framework;
using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Model
{
    public sealed class World
    {
        public Player Player { get; private set; }

        public Chunk Chunk { get; private set; }

        public World(IInputSet input)
        {
            Chunk = new Chunk();
            Player = new Player(input);
        }

        public void Update(GameTime frameTime)
        {
            Player.Update(frameTime);

            //Geschwindigkeit modifizieren
            Player.Velocity += Player.Mass * new Vector3(0, -5f, 0) * (float)frameTime.ElapsedGameTime.TotalSeconds;

            Vector3 move = Player.Velocity * (float)frameTime.ElapsedGameTime.TotalSeconds;

            //Zellenindizies
            int cellX = (int)(Player.Position.X + move.X);
            int cellY = (int)(Player.Position.Y + move.Y);
            int cellZ = (int)(Player.Position.Z + move.Z);

            BoundingBox playerBox = new BoundingBox(
                new Vector3(Player.Position.X + move.X - Player.Radius, Player.Position.Y + move.Y, Player.Position.Z + move.Z - Player.Radius),
                new Vector3(Player.Position.X + move.X + Player.Radius, Player.Position.Y + move.Y + 4f, Player.Position.Z + move.Z + Player.Radius));

            int range = 1;
            Player.OnGround = false;

            for (int z = cellZ - range; z < cellZ + range; z++)
            {
                for (int y = cellY - range; y < cellY + range; y++)
                {
                    for (int x = cellX - range; x < cellX + range; x++)
                    {
                        if (x < 0 || x >= Chunk.CHUNKSIZE_X || y < 0 || y >= Chunk.CHUNKSIZE_Y || z < 0 || z >= Chunk.CHUNKSIZE_Z)
                            continue;

                        IBlock block = Chunk.Blocks[x, y, z];

                        if (block == null) continue;

                        BoundingBox[] boxes = block.GetCollisionBoxes();

                        foreach (var box in boxes)
                        {
                            BoundingBox transformedBox = new BoundingBox(box.Min + new Vector3(x, y, z), box.Max + new Vector3(x, y, z));

                            //(1) Kollisions-Check
                            bool collisionX = (transformedBox.Min.X < playerBox.Max.X && transformedBox.Max.X > playerBox.Min.X);
                            bool collisionY = (transformedBox.Min.Y < playerBox.Max.Y && transformedBox.Max.Y > playerBox.Min.Y);
                            bool collisionZ = (transformedBox.Min.Z < playerBox.Max.Z && transformedBox.Max.Z > playerBox.Min.Z);

                            float gap = 0.01f;

                            if (collisionX && collisionY && collisionZ)
                            {
                                //(2) Kollisions-Zeitpunkt bestimmen
                                float nx = 1f;

                                if (move.X > 0)
                                {
                                    float diff = playerBox.Max.X - transformedBox.Min.X;

                                    if (diff < move.X) nx = 1f - (diff / move.X) - gap;
                                }
                                else if (move.X < 0)
                                {
                                    float diff = transformedBox.Max.X - playerBox.Min.X;

                                    if (diff < -move.X) nx = 1f - (diff / -move.X) - gap;
                                }

                                float ny = 1f;

                                if (move.Y > 0)
                                {
                                    float diff = playerBox.Max.Y - transformedBox.Min.Y;

                                    if (diff < move.Y) ny = 1f - (diff / move.Y) - gap;
                                }
                                else if (move.Y < 0)
                                {
                                    float diff = transformedBox.Max.Y - playerBox.Min.Y;

                                    if (diff < -move.Y)
                                    {
                                        ny = 1f - (diff / -move.Y) - gap;
                                        Player.Velocity = new Vector3(Player.Velocity.X, 0, Player.Velocity.Z);
                                        Player.OnGround = true;
                                    }
                                }

                                float nz = 1f;

                                if (move.Z > 0)
                                {
                                    float diff = playerBox.Max.Z - transformedBox.Min.Z;

                                    if (diff < move.Z) nz = 1f - (diff / move.Z) - gap;
                                }
                                else if (move.Z < 0)
                                {
                                    float diff = transformedBox.Max.Z - playerBox.Min.Z;

                                    if (diff < -move.Z) nz = 1f - (diff / -move.Z) - gap;
                                }

                                //(3) Kollisionsauflösung
                                if (nx < ny)
                                {
                                    if (nx < nz) move = new Vector3(move.X * nx, move.Y, move.Z);
                                    else move = new Vector3(move.X, move.Y, move.Z * nz);
                                }
                                else
                                {
                                    if (ny < nz) move = new Vector3(move.X, move.Y * ny, move.Z);
                                    else move = new Vector3(move.X, move.Y, move.Z * nz);
                                }
                            }
                        }
                    }
                }
            }

            Player.Position += move;
        }
    }
}
