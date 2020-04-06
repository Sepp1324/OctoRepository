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
            int cellY = (int)Player.Position.Y;

            //Geschwindigkeit modifizieren
            Vector3 velocity = Player.Velocity;

            //velocity *= cell.VelocityFactor;

            Vector3 newPosition = Player.Position + (velocity * (float)frameTime.ElapsedGameTime.TotalSeconds);

            //Block nach links (Kartenrand + nicht begehbare Zellen)
            if (velocity.X < 0)
            {
                float posLeft = newPosition.X - Player.Radius;

                cellX = (int)posLeft;
                cellY = (int)Player.Position.Y;

                if (posLeft < 0)
                {
                    newPosition = new Vector3(cellX + Player.Radius, 0, newPosition.Y);
                }

                if (cellX < 0)
                {
                    newPosition = new Vector3((cellX + 1) + Player.Radius, 0, newPosition.Y);
                }
            }

            //Block nach oben (Kartenrand + nicht begehbare Zellen)
            if(velocity.Y < 0)
            {
                float posTop = newPosition.Y - Player.Radius;

                cellY = (int)posTop;
                cellX = (int)Player.Position.X;

                if (posTop < 0)
                {
                    newPosition = new Vector3(newPosition.X, 0, cellY + Player.Radius);
                }

                if (cellY < 0)
                {
                    newPosition = new Vector3(newPosition.X, 0, cellY + 1 + Player.Radius);
                }
            }

            if(velocity.X > 0)
            {
                float posRight = newPosition.X + Player.Radius;

                cellX = (int)posRight;
                cellY = (int)Player.Position.Y;

                if (cellX >= Chunk.CHUNKSIZE_X)
                {
                    newPosition = new Vector3(cellX - Player.Radius, 0, newPosition.Y);
                }
            }

            if(velocity.Y > 0)
            {
                float posBottom = newPosition.Y + Player.Radius;

                cellY = (int)posBottom;
                cellX = (int)Player.Position.X;

                if (cellY >= Chunk.CHUNKSIZE_Y)
                {
                    newPosition = new Vector3(newPosition.X, 0, cellY - Player.Radius);
                }
            }

            Player.Position = newPosition;
        }
    }
}
