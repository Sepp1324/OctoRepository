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
        private readonly Vector3[] CollisionOrder = new[] 
        {
            new Vector3(0, -5, 0),
            new Vector3(0, -4, 0),
            new Vector3(0, -3, 0),
            new Vector3(0, -2, 0),

            //Block direkt unter dem Player
            new Vector3(0, -1, 0),

            //Blöcke am Boden um den Player herum
            new Vector3(-1, -1, -1), new Vector3(1, -1, -1), new Vector3(-1, -1, 1), new Vector3(1, -1, 1),
            new Vector3(-1, -1, 0), new Vector3(1, -1, 0), new Vector3(0, -1, -1), new Vector3(0, -1, 1), //TEST

            //Kollision mit der Decke
            new Vector3(0, 6, 0),
            new Vector3(-1, 6, -1), new Vector3(1, 6, -1), new Vector3(-1, 6, 1), new Vector3(1, 6, 1),
            new Vector3(-1, 6, 0), new Vector3(1, 6, 0), new Vector3(0, 6, -1), new Vector3(0, 6, 1),
            new Vector3(0, 5, 0),
            new Vector3(-1, 5, -1), new Vector3(1, 5, -1), new Vector3(-1, 5, 1), new Vector3(1, 5, 1),
            new Vector3(-1, 5, 0), new Vector3(1, 5, 0), new Vector3(0, 5, -1), new Vector3(0, 5, 1),
            new Vector3(0, 4, 0),
            new Vector3(-1, 4, -1), new Vector3(1, 4, -1), new Vector3(-1, 4, 1), new Vector3(1, 4, 1),
            new Vector3(-1, 4, 0), new Vector3(1, 4, 0), new Vector3(0, 4, -1), new Vector3(0, 4, 1),
            new Vector3(0, 3, 0),
            new Vector3(-1, 3, -1), new Vector3(1, 3, -1), new Vector3(-1, 3, 1), new Vector3(1, 3, 1),
            new Vector3(-1, 3, 0), new Vector3(1, 3, 0), new Vector3(0, 3, -1), new Vector3(0, 3, 1),
            new Vector3(0, 2, 0),
            new Vector3(-1, 2, -1), new Vector3(1, 2, -1), new Vector3(-1, 2, 1), new Vector3(1, 2, 1),
            new Vector3(-1, 2, 0), new Vector3(1, 2, 0), new Vector3(0, 2, -1), new Vector3(0, 2, 1),
            new Vector3(0, 1, 0),
            new Vector3(-1, 1, -1), new Vector3(1, 1, -1), new Vector3(-1, 1, 1), new Vector3(1, 1, 1),
            new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(0, 1, -1), new Vector3(0, 1, 1),
            new Vector3(0, 0, 0),
            new Vector3(-1, 0, -1), new Vector3(1, 0, -1), new Vector3(-1, 0, 1), new Vector3(1, 0, 1),
            new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, -1), new Vector3(0, 0, 1)
        };

        public Player Player { get; private set; }

        public Chunk Chunk { get; private set; }

        public World(IInputSet input)
        {
            Chunk = new Chunk();
            Player = new Player(input);
        }

        public void Update(GameTime frameTime)
        {
            Player.ExternalForce = new Vector3(0, -20f, 0) * Player.Mass;

            Player.Update(frameTime);

            Vector3 move = Player.Velocity * (float)frameTime.ElapsedGameTime.TotalSeconds;

            //Zellenindizies
            //int cellX = (int)(Player.Position.X + move.X);
            //int cellY = (int)(Player.Position.Y + move.Y);
            //int cellZ = (int)(Player.Position.Z + move.Z);

            BoundingBox playerBox = new BoundingBox(
                new Vector3(Player.Position.X + move.X - Player.Radius, Player.Position.Y + move.Y, Player.Position.Z + move.Z - Player.Radius),
                new Vector3(Player.Position.X + move.X + Player.Radius, Player.Position.Y + move.Y + 4f, Player.Position.Z + move.Z + Player.Radius));

            Player.OnGround = false;

            foreach (var collisionBox in CollisionOrder)
            {
                int x = (int)(collisionBox.X + Player.Position.X + move.X);
                int y = (int)(collisionBox.Y + Player.Position.Y + move.Y);
                int z = (int)(collisionBox.Z + Player.Position.Z + move.Z);


                if (x < 0 || x >= Chunk.CHUNKSIZE_X || y < 0 || y >= Chunk.CHUNKSIZE_Y || z < 0 || z >= Chunk.CHUNKSIZE_Z)
                    continue;

                IBlock block = Chunk.Blocks[x, y, z];

                if (block == null) continue;

                BoundingBox[] boxes = block.GetCollisionBoxes();

                foreach (var box in boxes)
                {
                    BoundingBox transformedBox = new BoundingBox(box.Min + new Vector3(x, y, z), box.Max + new Vector3(x, y, z));

                    //(1) Kollisions-Check
                    bool collisionX = (transformedBox.Min.X <= playerBox.Max.X && transformedBox.Max.X >= playerBox.Min.X);
                    bool collisionY = (transformedBox.Min.Y <= playerBox.Max.Y && transformedBox.Max.Y >= playerBox.Min.Y);
                    bool collisionZ = (transformedBox.Min.Z <= playerBox.Max.Z && transformedBox.Max.Z >= playerBox.Min.Z);

                    float gap = 0.001f;

                    if (collisionX && collisionY && collisionZ)
                    {
                        //(2) Kollisions-Zeitpunkt bestimmen
                        float max = 0f;
                        float nx = 1f;

                        Vector3 correctedMove = move;
                        Vector3 correctedVelocity = Player.Velocity;

                        bool correctedOnGround = false;

                        if (move.X > 0)
                        {
                            float diff = playerBox.Max.X - transformedBox.Min.X;

                            if (diff < move.X)
                            {
                                nx = 1f - (diff / move.X);

                                if (nx > max)
                                {
                                    max = nx;
                                    correctedMove = new Vector3((move.X * nx) - gap, move.Y, move.Z);
                                    correctedVelocity = new Vector3(0, Player.Velocity.Y, Player.Velocity.Z);
                                    correctedOnGround = false;
                                }
                            }
                        }
                        else if (move.X < 0)
                        {
                            float diff = transformedBox.Max.X - playerBox.Min.X;

                            if (diff < -move.X)
                            {
                                nx = 1f - (diff / -move.X);

                                if (nx > max)
                                {
                                    max = nx;
                                    correctedMove = new Vector3((move.X * nx) + gap, move.Y, move.Z);
                                    correctedVelocity = new Vector3(0, Player.Velocity.Y, Player.Velocity.Z);
                                    correctedOnGround = false;
                                }
                            }
                        }

                        float ny = 1f;

                        if (move.Y > 0)
                        {
                            float diff = playerBox.Max.Y - transformedBox.Min.Y;

                            if (diff < move.Y)
                            {
                                ny = 1f - (diff / move.Y);

                                if (ny > max)
                                {
                                    max = ny;
                                    correctedMove = new Vector3(move.X, (move.Y * ny) - gap, move.Z);
                                    correctedVelocity = new Vector3(Player.Velocity.X, 0, Player.Velocity.Z);
                                    correctedOnGround = false;
                                }
                            }
                        }
                        else if (move.Y < 0)
                        {
                            float diff = transformedBox.Max.Y - playerBox.Min.Y;

                            if (diff < -move.Y)
                            {
                                ny = 1f - (diff / -move.Y);

                                if (ny > max)
                                {
                                    max = ny;
                                    correctedMove = new Vector3(move.X, (move.Y * ny) + gap, move.Z);
                                    correctedVelocity = new Vector3(Player.Velocity.X, 0, Player.Velocity.Z);
                                    correctedOnGround = true;
                                }
                            }
                        }

                        float nz = 1f;

                        if (move.Z > 0)
                        {
                            float diff = playerBox.Max.Z - transformedBox.Min.Z;

                            if (diff < move.Z)
                            {
                                nz = 1f - (diff / move.Z);

                                if (nz > max)
                                {
                                    max = nz;
                                    correctedMove = new Vector3(move.X, move.Y, (move.Z * nz) - gap);
                                    correctedVelocity = new Vector3(Player.Velocity.X, Player.Velocity.Y, 0);
                                    correctedOnGround = false;
                                }
                            }
                        }
                        else if (move.Z < 0)
                        {
                            float diff = transformedBox.Max.Z - playerBox.Min.Z;

                            if (diff < -move.Z)
                            {
                                nz = 1f - (diff / -move.Z);

                                if (nz > max)
                                {
                                    max = nz;
                                    correctedMove = new Vector3(move.X, move.Y, (move.Z * nz) + gap);
                                    correctedVelocity = new Vector3(Player.Velocity.X, Player.Velocity.Y, 0);
                                    correctedOnGround = false;
                                }
                            }
                        }
                        move = correctedMove;
                        Player.Velocity = correctedVelocity;
                        Player.OnGround = correctedOnGround;

                        //(3) Kollisionsauflösung
                        //if (nx < ny)
                        //{
                        //    if (nx < nz) move = new Vector3(move.X * nx, move.Y, move.Z);
                        //    else move = new Vector3(move.X, move.Y, move.Z * nz);
                        //}
                        //else
                        //{
                        //    if (ny < nz) move = new Vector3(move.X, move.Y * ny, move.Z);
                        //    else move = new Vector3(move.X, move.Y, move.Z * nz);
                        //}
                    }
                }
            }

            Player.Position += move;
        }

        public void DeleteBlock(int x, int y, int z)
        {
            Chunk.Blocks[x, y, z] = null;
        }

        public void PutBlock(int x, int y, int z)
        {
            Chunk.Blocks[x, y, z] = new GrassBlock();
        }
    }
}
