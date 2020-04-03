using OctoAwesome.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Model
{
    internal sealed class Player : Item, IHaveInventory
    {
        private Input input;
        private Map map;

        public readonly float MAXSPEED = 2f;

        public Vector2 Position { get; set; }

        public Vector2 Velocity { get; set; }

        public float Radius { get; private set; }

        public float Angle { get; private set; }

        public PlayerState State { get; private set; }

        public IHaveInventory InteractionPartner { get; set; }

        public List<InventoryItem> InventoryItems { get; private set; }

        public Player(Input input, Map map)
        {
            this.input = input;
            this.map = map;
            Radius = 0.1f;
            InventoryItems = new List<InventoryItem>();

            InventoryItems.Add(new InventoryItem() { Name = "Apfel" });
        }

        public void Update(TimeSpan frameTime)
        {
            //Bewegungsrichtung
            Velocity = new Vector2((input.Left ? -1f : 0f) + (input.Right ? 1f : 0f), (input.Up ? -1f : 0f) + (input.Down ? 1f : 0f));

            //Bewegunsberechnung
            if (Velocity.Length() > 0f)
            {
                Velocity = Velocity.Normalized() * MAXSPEED;
                State = PlayerState.WALK;
                Angle = Velocity.Angle();
            }
            else
            {
                State = PlayerState.IDLE;
            }

            int cellX = (int)Position.X;
            int cellY = (int)Position.Y;

            //Umrechnung in Grad
            float direction = (Angle * 360f) / (float)(2 * Math.PI);

            //In positiven Bereich rechnen
            direction += 180;

            //Offset hinzurechnen
            direction += 45;

            int sector = (int)(direction / 90);

            switch (sector)
            {
                //Oben
                case 1: cellY -= 1; break;

                //rechts
                case 2: cellX += 1; break;

                //Unten
                case 3: cellY += 1; break;

                //Links
                case 4: cellX -= 1; break;
            }

            //Interaktion überprüfen
            if (input.Interact && InteractionPartner == null)
            {
                input.Interact = false;
                InteractionPartner = map.Items.
                    Where(i => (int)i.Position.X == cellX && (int)i.Position.Y == cellY).
                    OfType<IHaveInventory>().
                    FirstOrDefault();
            }

            if (InteractionPartner != null)
            {
                var partner = map.Items.
                    Where(i => (int)i.Position.X == cellX && (int)i.Position.Y == cellY).
                    OfType<IHaveInventory>().
                    FirstOrDefault();

                if (InteractionPartner != partner)
                    InteractionPartner = null;
            }

        }
    }

    internal enum PlayerState
    {
        WALK, IDLE
    }
}
