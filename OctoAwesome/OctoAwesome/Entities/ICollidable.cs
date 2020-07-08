using Microsoft.Xna.Framework;

namespace OctoAwesome.Entities
{
    interface ICollidable : IBody
    {
        float Mass { get; set; }

        Vector3 ExternalForce { get; set; }

        bool OnGround { get; set; }
    }
}
