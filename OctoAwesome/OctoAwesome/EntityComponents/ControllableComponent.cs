using engenious;
using OctoAwesome.Components;
using OctoAwesome.SumTypes;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// 
    /// </summary>
    public class ControllableComponent : Component, IEntityComponent
    {
        /// <summary>
        /// 
        /// </summary>
        public bool JumpInput { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Vector2 MoveInput { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool JumpActive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int JumpTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal Selection Selection { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Index3? InteractBlock { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Index3? ApplyBlock { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OrientationFlags ApplySide { get; set; }
    }
}