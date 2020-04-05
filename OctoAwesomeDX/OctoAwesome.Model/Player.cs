using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OctoAwesome.Model
{
    public sealed class Player : Item, IHaveInventory
    {
        private IInputSet input;

        public readonly float MAXSPEED = 10f;

        public Vector3 Position { get; set; }

        public float Radius { get; private set; }

        public float Angle { get; private set; }

        public bool OnGround { get; set; }

        public float Tilt { get; set; }

        public PlayerState State { get; private set; }

        public IHaveInventory InteractionPartner { get; set; }

        public List<InventoryItem> InventoryItems { get; private set; }

        public Player(IInputSet input)
        {
            this.input = input;
            Position = new Vector3(0, 50, 0);
            Velocity = new Vector3(0, 0, 0);
            Radius = 0.1f;
            Angle = 0f;
            Mass = 100;

            InventoryItems = new List<InventoryItem>();
            InventoryItems.Add(new InventoryItem() { Name = "Apfel" });
        }

        public void Update(GameTime frameTime)
        {
            Angle += (float)frameTime.ElapsedGameTime.TotalSeconds * input.HeadX;

            float lookX = (float)Math.Cos(Angle);
            float lookY = (float)Math.Sin(Angle);

            float strafeX = (float)Math.Cos(Angle + MathHelper.PiOver2);
            float strafeY = (float)Math.Sin(Angle + MathHelper.PiOver2);

            Vector3 force = new Vector3();

            force += new Vector3(lookX, 0, lookY) * input.MoveY;
            force += new Vector3(strafeX, 0, strafeY) * input.MoveX;
            force -= Velocity * 0.7f;
            force += new Vector3(0, -10, 0);

            Tilt += (float)frameTime.ElapsedGameTime.TotalSeconds * input.HeadY;
            Tilt = Math.Min(MathHelper.PiOver4, Math.Max(-MathHelper.PiOver4, Tilt));

            Vector3 acceleration = force / Mass;

            Velocity += acceleration * (float)frameTime.ElapsedGameTime.TotalSeconds;

            //BEwegungsberechnung
            //if (Velocity.Length() > 0)
            //{
            //    Velocity.Normalize();
            //    Velocity *= MAXSPEED;
            //    State = PlayerState.WALK;
            //}
            //else
            //{
            //    State = PlayerState.IDLE;
            //}

            ////Bewegungsrichtung
            //Velocity = new Vector2((input.Left ? -1f : 0f) + (input.Right ? 1f : 0f), (input.Up ? -1f : 0f) + (input.Down ? 1f : 0f));

            ////Bewegunsberechnung
            //if (Velocity.Length() > 0f)
            //{
            //    Velocity.Normalize();
            //   Velocity *= MAXSPEED;
            //    State = PlayerState.WALK;
            //    Angle = (float)Math.Atan2(Velocity.Y, Velocity.X);
            //}
            //else
            //{
            //    State = PlayerState.IDLE;
            //}

            //int cellX = (int)Position.X;
            //int cellY = (int)Position.Y;

            ////Umrechnung in Grad
            //float direction = (Angle * 360f) / (float)(2 * Math.PI);

            ////In positiven Bereich rechnen
            //direction += 180;

            ////Offset hinzurechnen
            //direction += 45;

            //int sector = (int)(direction / 90);

            //switch (sector)
            //{
            //    //Oben
            //    case 1: cellY -= 1; break;

            //    //rechts
            //    case 2: cellX += 1; break;

            //    //Unten
            //    case 3: cellY += 1; break;

            //    //Links
            //    case 4: cellX -= 1; break;
            //}

            ////Interaktion überprüfen
            ///*if (input.Interact && InteractionPartner == null)
            //{
            //    InteractionPartner = map.Items.
            //        Where(i => (int)i.Position.X == cellX && (int)i.Position.Y == cellY).
            //        OfType<IHaveInventory>().
            //        FirstOrDefault();
            //}

            //if (InteractionPartner != null)
            //{
            //    var partner = map.Items.
            //        Where(i => (int)i.Position.X == cellX && (int)i.Position.Y == cellY).
            //        OfType<IHaveInventory>().
            //        FirstOrDefault();

            //    if (InteractionPartner != partner)
            //        InteractionPartner = null;
            //}*/

        }
    }

    public enum PlayerState
    {
        WALK, IDLE
    }
}
