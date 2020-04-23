using Microsoft.Xna.Framework;

namespace OctoAwesome
{
    public abstract class Item
    {
        public Coordinate Position { get; set; }

        public float Mass { get; set; }

        public Vector3 Velocity { get; set; }

        public Vector3 ExternalForce { get; set; }
    }
}