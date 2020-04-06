using Microsoft.Xna.Framework;
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
            int cellZ = (int)Player.Position.Z;

            //Geschwindigkeit modifizieren
            Player.Velocity += Player.Mass * new Vector3(0, -5f, 0) * (float)frameTime.ElapsedGameTime.TotalSeconds;

            Vector3 newPosition = Player.Position + (Player.Velocity * (float)frameTime.ElapsedGameTime.TotalSeconds);

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

            Player.OnGround = false;
            if (Player.Velocity.Y < 0)
            {
                if (newPosition.Y < 50)
                {
                    newPosition.Y = 50;
                    Player.Velocity = new Vector3(Player.Velocity.X, 0, Player.Velocity.Z);
                    Player.OnGround = true;
                }
            }
            Player.Position = newPosition;
        }
    }
}
