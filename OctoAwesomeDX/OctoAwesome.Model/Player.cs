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

        public float Radius { get; private set; }

        public float Angle { get; private set; }

        public float Height { get; set; }

        public float Tilt { get; set; }

        public bool OnGround { get; set; }

        public PlayerState State { get; private set; }

        public IHaveInventory InteractionPartner { get; set; }

        public List<InventoryItem> InventoryItems { get; private set; }

        public Player(IInputSet input)
        {
            this.input = input;
            Position = new Coordinate(new Index3(16, 33, 16), Vector3.Zero);
            Velocity = new Vector3(0, 0, 0);
            Radius = 0.75f;
            Angle = 0f;
            Height = 3.5f;
            Mass = 100;
            InventoryItems = new List<InventoryItem>();

            InventoryItems.Add(new InventoryItem() { Name = "Apfel" });
        }

        public void Update(GameTime frameTime)
        {
            float Power = 500f;
            //float JumpPower = 10000000f;
            float JumpPower = 1500000;

            Vector3 externalPower = ((ExternalForce * ExternalForce) / (2 * Mass)) * (float)frameTime.ElapsedGameTime.TotalSeconds;

            //Input verarbeiten
            Angle += (float)frameTime.ElapsedGameTime.TotalSeconds * input.HeadX;

            Tilt += (float)frameTime.ElapsedGameTime.TotalSeconds * input.HeadY;
            Tilt = Math.Min(1.5f, Math.Max(-1.5f, Tilt));

            float lookX = (float)Math.Cos(Angle);
            float lookY = (float)Math.Sin(Angle);
            var VelocityDirection = new Vector3(lookX, 0, lookY) * input.MoveY;

            float strafeX = (float)Math.Cos(Angle + MathHelper.PiOver2);
            float strafeY = (float)Math.Sin(Angle + MathHelper.PiOver2);
            VelocityDirection += new Vector3(strafeX, 0, strafeY) * input.MoveX;

            Vector3 Friction = new Vector3(1, 0.1f, 1) * 30f;
            Vector3 powerDirection = new Vector3();

            powerDirection += ExternalForce;
            if (OnGround)
            {
                powerDirection += (Power * VelocityDirection);

                if (input.JumpTrigger)
                    powerDirection += new Vector3(VelocityDirection.X * 1000f, JumpPower, VelocityDirection.Z * 1000f);
            }

            Vector3 VelocityChange = (2.0f / Mass * (powerDirection - Friction * Velocity)) * (float)frameTime.ElapsedGameTime.TotalSeconds;

            Velocity += new Vector3(
                (float)(VelocityChange.X < 0 ? -Math.Sqrt(-VelocityChange.X) : Math.Sqrt(VelocityChange.X)),
                (float)(VelocityChange.Y < 0 ? -Math.Sqrt(-VelocityChange.Y) : Math.Sqrt(VelocityChange.Y)),
                (float)(VelocityChange.Z < 0 ? -Math.Sqrt(-VelocityChange.Z) : Math.Sqrt(VelocityChange.Z)));
        }
    }

    public enum PlayerState
    {
        WALK, IDLE
    }
}
