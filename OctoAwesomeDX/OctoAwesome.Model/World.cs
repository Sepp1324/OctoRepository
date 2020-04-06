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

            //Oberflächenbeschaffenheit ermitteln
            int cellX = (int)Player.Position.X;
            int cellY = (int)Player.Position.Y;
            int cellZ = (int)Player.Position.Z;

            //Geschwindigkeit modifizieren
            Player.Velocity += Player.Mass * new Vector3(0, -5f, 0) * (float)frameTime.ElapsedGameTime.TotalSeconds;

            Vector3 newPosition = Player.Position + (Player.Velocity * (float)frameTime.ElapsedGameTime.TotalSeconds);

            BoundingBox playerBox = new BoundingBox(
                new Vector3(newPosition.X - Player.Radius, newPosition.Y, newPosition.Z - Player.Radius),
                new Vector3(newPosition.X + Player.Radius, newPosition.Y + 4f, newPosition.Z + Player.Radius));

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
                            BoundingBox boxX = new BoundingBox(box.Min + new Vector3(x, y, z), box.Max + new Vector3(x, y, z));

                            if (playerBox.Intersects(boxX))
                            {
                                newPosition.Y = boxX.Max.Y;
                                Player.Velocity = new Vector3(Player.Velocity.X, 0, Player.Velocity.Z);
                                Player.OnGround = true;

                                playerBox = new BoundingBox(
                                     new Vector3(newPosition.X - Player.Radius, newPosition.Y, newPosition.Z - Player.Radius),
                                     new Vector3(newPosition.X + Player.Radius, newPosition.Y + 4f, newPosition.Z + Player.Radius));
                            }
                            //if(playerBox.Min.X < box.Min.X && playerBox.Max.X > box.Min.X ||
                            //    playerBox.Min.X < box.Max.X && playerBox.Max.X > box.Min.X)
                        }
                    }
                }
            }

            //Block nach links (Kartenrand + nicht begehbare Zellen)
            //if (velocity.X < 0)
            //{
            //    float posLeft = newPosition.X - Player.Radius;

            //    cellX = (int)posLeft;
            //    cellZ = (int)Player.Position.Z;

            //    if (posLeft < 0)
            //    {
            //        newPosition = new Vector3(cellX + Player.Radius, newPosition.Y, newPosition.Z);
            //    }

            //    if (cellX < 0)
            //    {
            //        newPosition = new Vector3((cellX + 1) + Player.Radius, newPosition.Y, newPosition.Z);
            //    }
            //}

            ////Block nach oben (Kartenrand + nicht begehbare Zellen)
            //if(velocity.Z < 0)
            //{
            //    float posTop = newPosition.Z - Player.Radius;

            //    cellZ = (int)posTop;
            //    cellX = (int)Player.Position.X;

            //    if (posTop < 0)
            //    {
            //        newPosition = new Vector3(newPosition.X, newPosition.Y, cellZ + Player.Radius);
            //    }

            //    if (cellZ < 0)
            //    {
            //        newPosition = new Vector3(newPosition.X, newPosition.Y, cellZ + 1 + Player.Radius);
            //    }
            //}

            //if(velocity.X > 0)
            //{
            //    float posRight = newPosition.X + Player.Radius;

            //    cellX = (int)posRight;
            //    cellZ = (int)Player.Position.Z;

            //    if (cellX >= Chunk.CHUNKSIZE_X)
            //    {
            //        newPosition = new Vector3(cellX - Player.Radius, newPosition.Y, newPosition.Z);
            //    }
            //}

            //if(velocity.Z > 0)
            //{
            //    float posBottom = newPosition.Z + Player.Radius;

            //    cellZ = (int)posBottom;
            //    cellX = (int)Player.Position.X;

            //    if (cellZ >= Chunk.CHUNKSIZE_Z)
            //    {
            //        newPosition = new Vector3(newPosition.X, newPosition.Y, cellZ - Player.Radius);
            //    }
            //}

            //Player.OnGround = false;
            //if (Player.Velocity.Y < 0)
            //{
            //    if (newPosition.Y < 50)
            //    {

            //    }
            //}
            Player.Position = newPosition;
        }
    }
}
