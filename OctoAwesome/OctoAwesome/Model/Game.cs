using OctoAwesome.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Model
{
    internal sealed class Game
    {
        private Input input;

        private Dictionary<CellType, CellTypeDefinition> cellTypes;

        public Camera Camera { get; private set; }

        public Vector2 PlaygroundSize
        {
            get
            {
                return new Vector2(Map.Columns, Map.Rows);
            }
        }

        public Player Player { get; private set; }

        public Map Map { get; private set; }

        public Game(Input input)
        {
            //Map = Map.Generate(20, 20, CellType.Grass);
            Map = Map.Load("Assets/testMap.map");
            Player = new Player(input, Map);
            Map.Items.Add(Player);
            Camera = new Camera(this, input);

            cellTypes = new Dictionary<CellType, CellTypeDefinition>();
            cellTypes.Add(CellType.Grass, new CellTypeDefinition() { CanGoto = true, VelocityFactor = 0.8f });
            cellTypes.Add(CellType.Sand, new CellTypeDefinition() { CanGoto = true, VelocityFactor = 1f });
            cellTypes.Add(CellType.Water, new CellTypeDefinition() { CanGoto = false, VelocityFactor = 0f });
        }

        public void Update(TimeSpan frameTime)
        {
            Player.Update(frameTime);

            //Oberflächenbeschaffenheit ermitteln
            int cellX = (int)Player.Position.X;
            int cellY = (int)Player.Position.Y;

            CellType cellType = Map.GetCell(cellX, cellY);

            //Geschwindigkeit modifizieren
            Vector2 velocity = Player.Velocity;
            var cellTypeDefinition = cellTypes[cellType];

            velocity *= cellTypeDefinition.VelocityFactor;

            Vector2 newPosition = Player.Position + (velocity * (float)frameTime.TotalSeconds);

            //Block nach links (Kartenrand + nicht begehbare Zellen)
            if (velocity.X < 0)
            {
                float posLeft = newPosition.X - Player.Radius;

                cellX = (int)posLeft;
                cellY = (int)Player.Position.Y;

                if(posLeft < 0)
                {
                    newPosition = new Vector2(cellX + Player.Radius, newPosition.Y);
                }

                if (cellX < 0 || !cellTypes[Map.GetCell(cellX, cellY)].CanGoto)
                {
                    newPosition = new Vector2((cellX + 1) + Player.Radius, newPosition.Y);
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
                    newPosition = new Vector2(newPosition.X, cellY + Player.Radius);
                }

                if (cellY < 0 || !cellTypes[Map.GetCell(cellX, cellY)].CanGoto)
                {
                    newPosition = new Vector2(newPosition.X, cellY + 1 + Player.Radius);
                }
            }

            if(velocity.X > 0)
            {
                float posRight = newPosition.X + Player.Radius;

                cellX = (int)posRight;
                cellY = (int)Player.Position.Y;

                if (cellX >= Map.Columns || !cellTypes[Map.GetCell(cellX, cellY)].CanGoto)
                {
                    newPosition = new Vector2(cellX - Player.Radius, newPosition.Y);
                }
            }

            if(velocity.Y > 0)
            {
                float posBottom = newPosition.Y + Player.Radius;

                cellY = (int)posBottom;
                cellX = (int)Player.Position.X;

                if (cellY >= Map.Rows || !cellTypes[Map.GetCell(cellX, cellY)].CanGoto)
                {
                    newPosition = new Vector2(newPosition.X, cellY - Player.Radius);
                }
            }

            Player.Position = newPosition;

            Camera.Update(frameTime);
        }
    }
}
