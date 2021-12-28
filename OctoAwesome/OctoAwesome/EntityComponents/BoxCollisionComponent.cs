using System;
using engenious;
using OctoAwesome.Components;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// </summary>
    public sealed class BoxCollisionComponent : CollisionComponent, IFunctionalBlockComponent
    {
        private readonly BoundingBox[] _boundingBoxes;

        /// <summary>
        /// </summary>
        public BoxCollisionComponent()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="boundingBoxes"></param>
        public BoxCollisionComponent(BoundingBox[] boundingBoxes) => _boundingBoxes = boundingBoxes;

        /// <summary>
        /// </summary>
        public ReadOnlySpan<BoundingBox> BoundingBoxes => new(_boundingBoxes);
    }
}