using System;
using engenious;
using OctoAwesome.Components;

namespace OctoAwesome.EntityComponents
{
    public sealed class BoxCollisionComponent : CollisionComponent, IFunctionalBlockComponent
    {
        private readonly BoundingBox[] boundingBoxes;

        public BoxCollisionComponent()
        {
        }

        public BoxCollisionComponent(BoundingBox[] boundingBoxes)
        {
            this.boundingBoxes = boundingBoxes;
        }

        public ReadOnlySpan<BoundingBox> BoundingBoxes => new(boundingBoxes);
    }
}