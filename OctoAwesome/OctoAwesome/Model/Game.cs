﻿using OctoAwesome.Components;
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
            //Map.Items.Add(Player);
            Camera = new Camera(this, input);

            cellTypes = new Dictionary<CellType, CellTypeDefinition>();
            cellTypes.Add(CellType.Grass, new CellTypeDefinition() { CanGoto = true, VelocityFactor = 0.8f });
            cellTypes.Add(CellType.Sand, new CellTypeDefinition() { CanGoto = true, VelocityFactor = 1f });
            cellTypes.Add(CellType.Water, new CellTypeDefinition() { CanGoto = false, VelocityFactor = 0f });

            //Map Cache generieren
            Map.CellCache = new CellCache[Map.Columns, Map.Rows];

            for(int x = 0; x < Map.Columns; x++)
            {
                for(int y = 0; y < Map.Rows; y++)
                {
                    CellType cellType = Map.GetCell(x, y);

                    bool haveItems = Map.Items.Any(i => (int)i.Position.X == x && (int)i.Position.Y == y);

                    Map.CellCache[x, y] = new CellCache() { CellType = cellType, CanGoTo = cellTypes[cellType].CanGoto && !haveItems, VelocityFactor = cellTypes[cellType].VelocityFactor};
                }
            }

            //Player als Item hinzufügen
            Map.Items.Add(Player);
        }

        public void Update(TimeSpan frameTime)
        {
            Player.Update(frameTime);

            //Oberflächenbeschaffenheit ermitteln
            int cellX = (int)Player.Position.X;
            int cellY = (int)Player.Position.Y;

            CellCache cell = Map.CellCache[cellX, cellY];

            //Geschwindigkeit modifizieren
            Vector2 velocity = Player.Velocity;

            velocity *= cell.VelocityFactor;

            Vector2 newPosition = Player.Position + (velocity * (float)frameTime.TotalSeconds);

            //Block nach links (Kartenrand + nicht begehbare Zellen)
            if (velocity.X < 0)
            {
                float posLeft = newPosition.X - Player.Radius;

                cellX = (int)posLeft;
                cellY = (int)Player.Position.Y;

                cell = Map.CellCache[cellX, cellY];

                if (posLeft < 0)
                {
                    newPosition = new Vector2(cellX + Player.Radius, newPosition.Y);
                }

                if (cellX < 0 || !cell.CanGoTo)
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

                cell = Map.CellCache[cellX, cellY];

                if (posTop < 0)
                {
                    newPosition = new Vector2(newPosition.X, cellY + Player.Radius);
                }

                if (cellY < 0 || !cell.CanGoTo)
                {
                    newPosition = new Vector2(newPosition.X, cellY + 1 + Player.Radius);
                }
            }

            if(velocity.X > 0)
            {
                float posRight = newPosition.X + Player.Radius;

                cellX = (int)posRight;
                cellY = (int)Player.Position.Y;

                cell = Map.CellCache[cellX, cellY];

                if (cellX >= Map.Columns || !cell.CanGoTo)
                {
                    newPosition = new Vector2(cellX - Player.Radius, newPosition.Y);
                }
            }

            if(velocity.Y > 0)
            {
                float posBottom = newPosition.Y + Player.Radius;

                cellY = (int)posBottom;
                cellX = (int)Player.Position.X;

                cell = Map.CellCache[cellX, cellY];

                if (cellY >= Map.Rows || !cell.CanGoTo)
                {
                    newPosition = new Vector2(newPosition.X, cellY - Player.Radius);
                }
            }

            Player.Position = newPosition;

            Camera.Update(frameTime);
        }
    }
}
